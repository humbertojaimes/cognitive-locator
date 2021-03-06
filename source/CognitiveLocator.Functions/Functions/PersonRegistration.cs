using CognitiveLocator.Domain;
using CognitiveLocator.Functions.Client;
using CognitiveLocator.Functions.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveLocator.Functions
{
    public static class PersonRegistration
    {
        private static FaceClient client_face = new FaceClient();
        private static DocumentClient client_document = new DocumentClient(new Uri(Settings.DocumentDB), Settings.DocumentDBAuthKey);

        [FunctionName(nameof(PersonRegistration))]
        public static async Task Run(
            [BlobTrigger("images/{name}.{extension}")]CloudBlockBlob blobImage, string name, string extension, TraceWriter log)
        {

            var tc = new TelemetryClient();
            tc.InstrumentationKey = Settings.APPINSIGHTS_INSTRUMENTATIONKEY;

            Person p = new Person();
            string notificationMessage = "Error";
            bool hasError = true; ;
            string json = string.Empty;
            string deviceId = string.Empty;
            var collection = await client_document.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(Settings.DatabaseId), new DocumentCollection { Id = Settings.PersonCollectionId }, new RequestOptions { OfferThroughput = 1000  });

            //get json file from storage
            CloudBlockBlob blobJson = await StorageHelper.GetBlockBlob($"{name}.json", Settings.AzureWebJobsStorage, "metadata", false);

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    blobJson.DownloadToStream(memoryStream);
                    json = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                    p = JsonConvert.DeserializeObject<Person>(json);
                }

                 deviceId = p.ReportedByDeviceId;
                await NotificationsHelper.AddToRequest(deviceId, name);

                //validate record has not been processed before
                var query = client_document.CreateDocumentQuery<Person>(collection.Resource.SelfLink, new SqlQuerySpec()
                {
                    QueryText = $"SELECT * FROM Person p WHERE p.picture = '{name}.{extension}'"
                });
                int count = query.ToList().Count;
                if (count > 0)
                    return;
                log.Info(blobImage.Uri.AbsoluteUri);
                //determine if image has a face
                List<JObject> list = await client_face.DetectFaces(blobImage.Uri.AbsoluteUri);

                //validate image extension 
                if (blobImage.Properties.ContentType != "image/jpeg")
                {
                    log.Info($"no valid content type for: {name}.{extension}");
                    await blobImage.DeleteAsync();
                    await blobJson.DeleteAsync();
                    notificationMessage = "Formato de fotograf�a incorrecto";
                    hasError = true;
                    await NotificationsHelper.SendNotification(hasError, notificationMessage, name, deviceId);
                    return;
                }

                //if image has no faces
                if (list.Count == 0)
                {
                    log.Info($"there are no faces in the image: {name}.{extension}");
                    await blobImage.DeleteAsync();
                    await blobJson.DeleteAsync();
                    notificationMessage = $"La fotograf�a de {p.Name} {p.Lastname} no contiene rostros";
                    hasError = true;
                    await NotificationsHelper.SendNotification(hasError, notificationMessage, name, deviceId);
                    return;
                }

                //if image has more than one face
                if (list.Count > 1)
                {
                    log.Info($"multiple faces detected in the image: {name}.{extension}");
                    await blobImage.DeleteAsync();
                    await blobJson.DeleteAsync();
                    notificationMessage = $"La fotograf�a de {p.Name} {p.Lastname} contiene mas de un rostro";
                    hasError = true;
                    await NotificationsHelper.SendNotification(hasError, notificationMessage, name, deviceId);
                    return;
                }



                //register person in Face API
                CreatePerson resultCreatePerson = await client_face.AddPersonToGroup(p.Name + " " + p.Lastname);
                AddPersonFace resultPersonFace = await client_face.AddPersonFace(blobImage.Uri.AbsoluteUri, resultCreatePerson.personId);
                AddFaceToList resultFaceToList = await client_face.AddFaceToList(blobImage.Uri.AbsoluteUri);

                //Add custom settings
                p.IsActive = 1;
                p.IsFound = 0;
                p.FaceAPIFaceId = resultFaceToList.persistedFaceId;
                p.FaceAPIPersonId = resultCreatePerson.personId;
                p.Picture = $"{name}.jpg";

                await client_document.CreateDatabaseIfNotExistsAsync(new Database { Id = Settings.DatabaseId });
                var result = await client_document.CreateDocumentAsync(collection.Resource.SelfLink, p);
                var document = result.Resource;
            }
            catch (Exception ex)
            {
             
                tc.TrackException(ex);
                await blobImage.DeleteAsync();
                await blobJson.DeleteAsync();
                log.Info($"Error in file: {name}.{extension} - {ex.Message} {ex.StackTrace}");
                notificationMessage = $"Ocurri� un error durante el registro de {p.Name} {p.Lastname}";
                hasError = true;
                await NotificationsHelper.SendNotification(hasError, notificationMessage, name, deviceId);
                return;
            }

            await blobJson.DeleteAsync();
            log.Info("person registered successfully");
            notificationMessage = $"El registro de {p.Name} {p.Lastname} se realizo correctamente";
            hasError = false;
            await NotificationsHelper.SendNotification(hasError, notificationMessage, name, deviceId);
        }
    }
}