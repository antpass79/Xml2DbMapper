using System;
using System.Collections.Generic;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Models
{
    public partial class ProbeTransducers
    {
        public int Id { get; set; }
        public int ProbeId { get; set; }
        //public short TransducerType { get; set; }
        public ProbeType TransducerType { get; set; } // ANTO CAST
        
        //public short? TransducerPosition { get; set; }
        public TransducerPosition? TransducerPosition { get; set; } // ANTO CAST

        public virtual Probe Probe { get; set; }
    }
}
