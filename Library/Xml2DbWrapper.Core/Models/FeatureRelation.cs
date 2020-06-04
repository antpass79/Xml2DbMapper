using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class FeatureRelation
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public int ParentFeatureId { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public int FeatureId { get; set; }

        //public virtual Feature Feature { get; set; }
        //public virtual Feature ParentFeature { get; set; }
    }
}
