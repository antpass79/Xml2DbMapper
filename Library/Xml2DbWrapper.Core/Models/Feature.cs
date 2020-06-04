using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class Feature
    {
        public Feature()
        {
            //Application = new HashSet<Application>();
            //BiopsyKits = new HashSet<BiopsyKits>();
            //BundleFeature = new HashSet<Bundle>();
            //BundleParentFeature = new HashSet<Bundle>();
            //DeprecatedDeprecatedFeature = new HashSet<Deprecated>();
            //DeprecatedSubstituteFeature = new HashSet<Deprecated>();
            //FeatureRelationFeature = new HashSet<FeatureRelation>();
            //FeatureRelationParentFeature = new HashSet<FeatureRelation>();
            //LicenseRelationFeature = new HashSet<LicenseRelation>();
            //LicenseRelationParentFeature = new HashSet<LicenseRelation>();
            //Option = new HashSet<Option>();
            //PartNumbersAssociations = new HashSet<PartNumbersAssociations>();
            //Probe = new HashSet<Probe>();
            //RegulatoryFeature = new HashSet<RegulatoryFeature>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string NameInCode { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public int? LicenseId { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public bool? AlwaysPresent { get; set; }

        //public virtual License License { get; set; }
        //public virtual ICollection<Application> Application { get; set; }
        //public virtual ICollection<BiopsyKits> BiopsyKits { get; set; }
        //public virtual ICollection<Bundle> BundleFeature { get; set; }
        //public virtual ICollection<Bundle> BundleParentFeature { get; set; }
        //public virtual ICollection<Deprecated> DeprecatedDeprecatedFeature { get; set; }
        //public virtual ICollection<Deprecated> DeprecatedSubstituteFeature { get; set; }
        //public virtual ICollection<FeatureRelation> FeatureRelationFeature { get; set; }
        //public virtual ICollection<FeatureRelation> FeatureRelationParentFeature { get; set; }
        //public virtual ICollection<LicenseRelation> LicenseRelationFeature { get; set; }
        //public virtual ICollection<LicenseRelation> LicenseRelationParentFeature { get; set; }
        //public virtual ICollection<Option> Option { get; set; }
        //public virtual ICollection<PartNumbersAssociations> PartNumbersAssociations { get; set; }
        //public virtual ICollection<Probe> Probe { get; set; }
        //public virtual ICollection<RegulatoryFeature> RegulatoryFeature { get; set; }
    }
}
