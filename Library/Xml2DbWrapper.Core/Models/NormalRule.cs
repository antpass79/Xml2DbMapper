using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class NormalRule
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }
        //public short Allow { get; set; }
     
        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public AllowModes Allow { get; set; } // ANTO CAST
        
        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public override int? LogicalModelId { get; set; }
        
        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public override int? ApplicationId { get; set; }
        
        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public override int? OptionId { get; set; }
        // public short? UserLevel { get; set; }
        
        [DataMember(Order = 5, EmitDefaultValue = false, IsRequired = false)]
        public override UserLevel? UserLevel { get; set; } // ANTO CAST
        
        [DataMember(Order = 6, EmitDefaultValue = false, IsRequired = false)]
        public override int? Version { get; set; }
        
        [DataMember(Order = 7, EmitDefaultValue = false, IsRequired = false)]
        public override int? CountryId { get; set; }
        
        [DataMember(Order = 8, EmitDefaultValue = false, IsRequired = false)]
        public override int? DistributorId { get; set; }
        
        [DataMember(Order = 9, EmitDefaultValue = false, IsRequired = false)]
        public override int? ProbeId { get; set; }
        //public short? TransducerType { get; set; }
        
        [DataMember(Order = 10, EmitDefaultValue = false, IsRequired = false)]
        public override ProbeType? TransducerType { get; set; } // ANTO CAST
        
        [DataMember(Order = 11, EmitDefaultValue = false, IsRequired = false)]
        public override int? KitId { get; set; }
        
        [DataMember(Order = 12, EmitDefaultValue = false, IsRequired = false, Name = "UIRuleId")]
        public int UiruleId { get; set; }

        //public virtual Application Application { get; set; }
        //public virtual Country Country { get; set; }
        //public virtual Distributor Distributor { get; set; }
        //public virtual BiopsyKits Kit { get; set; }
        //public virtual LogicalModel LogicalModel { get; set; }
    }
}
