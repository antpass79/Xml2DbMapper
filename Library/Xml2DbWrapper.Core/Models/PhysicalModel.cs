using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class PhysicalModel
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }
        //public short Name { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public Porting.Contract.Enums.PhysicalModel Name { get; set; } // ANTO CAST        

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string Description { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public string Code { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public bool SmallConnectorSupport { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false, IsRequired = false)]
        public bool LargeConnectorSupport { get; set; }
    }
}
