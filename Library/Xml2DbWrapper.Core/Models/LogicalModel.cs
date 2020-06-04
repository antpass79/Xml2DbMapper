using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class LogicalModel
    {
        public LogicalModel()
        {
            //CountryVersion = new HashSet<CountryVersion>();
            //LicenseRelationException = new HashSet<LicenseRelationException>();
            //NormalRule = new HashSet<NormalRule>();
            //PartNumbersAssociations = new HashSet<PartNumbersAssociations>();
            //Uirule = new HashSet<Uirule>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string ModelFamily { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public int? PhModId { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public string StyleName { get; set; }
        //public short ContexVisionModule { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false, IsRequired = false)]
        public CVModule ContexVisionModule { get; set; } // ANTO CAST        
                                                         //public short SubPhysModel { get; set; }

        [DataMember(Order = 6, EmitDefaultValue = false, IsRequired = false)]
        public PhysicalSubModel SubPhysModel { get; set; } // ANTO CAST

        //public short? Type { get; set; }
        [DataMember(Order = 7, EmitDefaultValue = false, IsRequired = false)]
        public SystemEnvironment? Type { get; set; } // ANTO CAST

        [DataMember(Order = 8, EmitDefaultValue = false, IsRequired = false, Name = "isDefault")]
        public bool? IsDefault { get; set; }

        [DataMember(Order = 9, EmitDefaultValue = false, IsRequired = false)]
        public int? LicenseId { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false, IsRequired = false)]
        public int? SettingsFamilyId { get; set; }

        //public virtual License License { get; set; }
        //public virtual ICollection<CountryVersion> CountryVersion { get; set; }
        //public virtual ICollection<LicenseRelationException> LicenseRelationException { get; set; }
        //public virtual ICollection<NormalRule> NormalRule { get; set; }
        //public virtual ICollection<PartNumbersAssociations> PartNumbersAssociations { get; set; }
        //public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
