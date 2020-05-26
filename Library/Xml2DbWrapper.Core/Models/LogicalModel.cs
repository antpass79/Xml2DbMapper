﻿using System.Collections.Generic;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
    public partial class LogicalModel
    {
        public LogicalModel()
        {
            CountryVersion = new HashSet<CountryVersion>();
            LicenseRelationException = new HashSet<LicenseRelationException>();
            NormalRule = new HashSet<NormalRule>();
            PartNumbersAssociations = new HashSet<PartNumbersAssociations>();
            Uirule = new HashSet<Uirule>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ModelFamily { get; set; }
        public int? PhModId { get; set; }
        public string StyleName { get; set; }
        //public short ContexVisionModule { get; set; }
        public CVModule ContexVisionModule { get; set; } // ANTO CAST        
        //public short SubPhysModel { get; set; }
        public PhysicalSubModel SubPhysModel { get; set; } // ANTO CAST
        //public short? Type { get; set; }
        public SystemEnvironment? Type { get; set; } // ANTO CAST
        public bool? IsDefault { get; set; }
        public int? LicenseId { get; set; }
        public int? SettingsFamilyId { get; set; }

        public virtual License License { get; set; }
        public virtual ICollection<CountryVersion> CountryVersion { get; set; }
        public virtual ICollection<LicenseRelationException> LicenseRelationException { get; set; }
        public virtual ICollection<NormalRule> NormalRule { get; set; }
        public virtual ICollection<PartNumbersAssociations> PartNumbersAssociations { get; set; }
        public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
