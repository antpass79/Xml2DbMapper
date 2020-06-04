using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class Option
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public bool IsFake { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public bool IsPreset { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public int FeatureId { get; set; }

        //public virtual Feature Feature { get; set; }
    }
}
