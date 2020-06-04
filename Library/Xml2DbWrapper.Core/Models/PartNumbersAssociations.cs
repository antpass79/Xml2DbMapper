using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class PartNumbersAssociations
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string PartNumber { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false, Name = "FeatureBOSName")]
        public string FeatureBosname { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public int? FeatureId { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public int? LogicalModelId { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false, IsRequired = false, Name = "toExport")]
        public bool ToExport { get; set; }

        [DataMember(Order = 6, EmitDefaultValue = false, IsRequired = false)]
        public string ModeTypeToExport { get; set; }

        //public virtual Feature Feature { get; set; }
        //public virtual LogicalModel LogicalModel { get; set; }
    }
}
