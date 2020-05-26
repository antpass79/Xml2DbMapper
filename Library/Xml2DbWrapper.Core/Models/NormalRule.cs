using System;
using System.Collections.Generic;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
    public partial class NormalRule
    {
        public int Id { get; set; }
        //public short Allow { get; set; }
        public AllowModes Allow { get; set; } // ANTO CAST
        public override int? LogicalModelId { get; set; }
        public override int? ApplicationId { get; set; }
        public override int? OptionId { get; set; }
        // public short? UserLevel { get; set; }
        public override UserLevel? UserLevel { get; set; } // ANTO CAST
        public override int? Version { get; set; }
        public override int? CountryId { get; set; }
        public override int? DistributorId { get; set; }
        public override int? ProbeId { get; set; }
        //public short? TransducerType { get; set; }
        public override ProbeType? TransducerType { get; set; } // ANTO CAST
        public override int? KitId { get; set; }
        public int UiruleId { get; set; }

        public virtual Application Application { get; set; }
        public virtual Country Country { get; set; }
        public virtual Distributor Distributor { get; set; }
        public virtual BiopsyKits Kit { get; set; }
        public virtual LogicalModel LogicalModel { get; set; }
    }
}
