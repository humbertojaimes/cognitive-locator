//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CognitiveLocatorDAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            this.PersonFaces = new HashSet<PersonFaces>();
        }
    
        public int IdPerson { get; set; }
        public int IsFound { get; set; }
        public string NameAlias { get; set; }
        public int Age { get; set; }
        public string Picture { get; set; }
        public string Location { get; set; }
        public System.Data.Entity.Spatial.DbGeography GeoLocation { get; set; }
        public int IdHospital { get; set; }
        public string Notes { get; set; }
        public string Source { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public int IsActive { get; set; }
        public Nullable<int> FaceId { get; set; }
        public Nullable<double> Height { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> LeftMargin { get; set; }
        public Nullable<double> RightMargin { get; set; }
    
        public virtual Hospital Hospital { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonFaces> PersonFaces { get; set; }
    }
}