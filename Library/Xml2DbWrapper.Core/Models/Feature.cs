using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class Feature
    {
        public Feature()
        {
            Application = new HashSet<Application>();
            BiopsyKits = new HashSet<BiopsyKits>();
            BundleFeature = new HashSet<Bundle>();
            BundleParentFeature = new HashSet<Bundle>();
            DeprecatedDeprecatedFeature = new HashSet<Deprecated>();
            DeprecatedSubstituteFeature = new HashSet<Deprecated>();
            FeatureRelationFeature = new HashSet<FeatureRelation>();
            FeatureRelationParentFeature = new HashSet<FeatureRelation>();
            LicenseRelationFeature = new HashSet<LicenseRelation>();
            LicenseRelationParentFeature = new HashSet<LicenseRelation>();
            Option = new HashSet<Option>();
            PartNumbersAssociations = new HashSet<PartNumbersAssociations>();
            Probe = new HashSet<Probe>();
            RegulatoryFeature = new HashSet<RegulatoryFeature>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameInCode { get; set; }
        public int? LicenseId { get; set; }
        public bool? AlwaysPresent { get; set; }

        public virtual License License { get; set; }
        public virtual ICollection<Application> Application { get; set; }
        public virtual ICollection<BiopsyKits> BiopsyKits { get; set; }
        public virtual ICollection<Bundle> BundleFeature { get; set; }
        public virtual ICollection<Bundle> BundleParentFeature { get; set; }
        public virtual ICollection<Deprecated> DeprecatedDeprecatedFeature { get; set; }
        public virtual ICollection<Deprecated> DeprecatedSubstituteFeature { get; set; }
        public virtual ICollection<FeatureRelation> FeatureRelationFeature { get; set; }
        public virtual ICollection<FeatureRelation> FeatureRelationParentFeature { get; set; }
        public virtual ICollection<LicenseRelation> LicenseRelationFeature { get; set; }
        public virtual ICollection<LicenseRelation> LicenseRelationParentFeature { get; set; }
        public virtual ICollection<Option> Option { get; set; }
        public virtual ICollection<PartNumbersAssociations> PartNumbersAssociations { get; set; }
        public virtual ICollection<Probe> Probe { get; set; }
        public virtual ICollection<RegulatoryFeature> RegulatoryFeature { get; set; }
    }
}
