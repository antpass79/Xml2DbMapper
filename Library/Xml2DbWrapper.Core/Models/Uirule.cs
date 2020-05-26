using System;
using System.Collections.Generic;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
    public partial class Uirule
    {
        public int Id { get; set; }

        //public short Allow { get; set; }
        public AllowModes Allow { get; set; } // ANTO CAST

        public string RuleName { get; set; }
        public int? PhysicalModelId { get; set; }
        public int? CertifierId { get; set; }
        public int? LogicalModelId { get; set; }
        public override int? ApplicationId { get; set; }
        public override int? OptionId { get; set; }
        // public short? UserLevel { get; set; }
        public UserLevel? UserLevel { get; set; } // ANTO CAST
        
        public string Version { get; set; }
        public int? CountryId { get; set; }
        public int? DistributorId { get; set; }
        public override int? ProbeId { get; set; }
        //public short? TransducerType { get; set; }
        public ProbeType? TransducerType { get; set; } // ANTO CAST
        
        public override int? KitId { get; set; }
        //public short RuleOrigin { get; set; }
        public ruleOrigins RuleOrigin { get; set; } // ANTO CAST

        public virtual Application Application { get; set; }
        public virtual Certifier Certifier { get; set; }
        public virtual Country Country { get; set; }
        public virtual Distributor Distributor { get; set; }
        public virtual BiopsyKits Kit { get; set; }
        public virtual LogicalModel LogicalModel { get; set; }
    }
}
