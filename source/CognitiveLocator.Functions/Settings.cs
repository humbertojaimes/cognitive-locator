﻿using System;

namespace CognitiveLocator.Functions
{
    public class Settings
    {
        public static string AzureWebJobsStorage = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public static string FaceAPIKey = Environment.GetEnvironmentVariable("Face_API_Subscription_Key");
        public static string PersonGroupId = Environment.GetEnvironmentVariable("Face_API_PersonGroupId");
        public static string Zone = Environment.GetEnvironmentVariable("Face_API_Zone");
        public static string FaceListId = Environment.GetEnvironmentVariable("Face_API_FaceList");

        public static string DocumentDB = Environment.GetEnvironmentVariable("CosmosDB_URI");
        public static string DocumentDBAuthKey = Environment.GetEnvironmentVariable("CosmosDB_AuthKey");
        public static string DatabaseId = Environment.GetEnvironmentVariable("CosmosDB_DatabaseId");
        public static string PersonCollectionId = Environment.GetEnvironmentVariable("CosmosDB_PersonCollection");

        public static string NotificationAccessSignature = Environment.GetEnvironmentVariable("NotificationHub_Access_Signature");
        public static string NotificationHubName = Environment.GetEnvironmentVariable("NotificationHub_Name");

        public static string CryptographyKey = Environment.GetEnvironmentVariable("Cryptography_Key");

        public static string MobileCenterID_Android = Environment.GetEnvironmentVariable("MobileCenterID_Android");
        public static string MobileCenterID_iOS = Environment.GetEnvironmentVariable("MobileCenterID_iOS");

        public static string ImageStorageUrl = Environment.GetEnvironmentVariable("ImageStorageUrl");

        public static string ModeratorImageAPIKey = Environment.GetEnvironmentVariable("ModeratorImage_API_Subscription_Key");

        public static string APPINSIGHTS_INSTRUMENTATIONKEY = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
    }
}