using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Name = "DBConfiguration", Namespace = "")]
    public partial class Dbconfiguration
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public int Compatibility { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int Version { get; set; }
    }
}
