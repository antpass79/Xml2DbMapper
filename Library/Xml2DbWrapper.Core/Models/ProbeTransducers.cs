using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Name = "Probe_Transducers", Namespace = "")]
    public partial class ProbeTransducers
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public int ProbeId { get; set; }
        //public short TransducerType { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public ProbeType TransducerType { get; set; } // ANTO CAST

        //public short? TransducerPosition { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public TransducerPosition? TransducerPosition { get; set; } // ANTO CAST

        //public virtual Probe Probe { get; set; }
    }
}
