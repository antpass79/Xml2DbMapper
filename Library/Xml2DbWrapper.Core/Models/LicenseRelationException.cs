using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class LicenseRelationException
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public int LicenseRelationId { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public int LogicalModelId { get; set; }

        //public virtual LicenseRelation LicenseRelation { get; set; }
        //public virtual LogicalModel LogicalModel { get; set; }
    }
}
