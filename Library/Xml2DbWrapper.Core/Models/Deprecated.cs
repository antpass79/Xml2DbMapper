using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class Deprecated
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public int DeprecatedFeatureId { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public int? SubstituteFeatureId { get; set; }

        //public virtual Feature DeprecatedFeature { get; set; }
        //public virtual Feature SubstituteFeature { get; set; }
    }
}
