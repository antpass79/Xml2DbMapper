using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class ProbeTransducers
    {
        public int Id { get; set; }
        public int ProbeId { get; set; }
        public short TransducerType { get; set; }
        public short? TransducerPosition { get; set; }

        public virtual Probe Probe { get; set; }
    }
}
