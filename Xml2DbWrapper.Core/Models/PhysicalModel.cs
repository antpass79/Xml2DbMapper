using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class PhysicalModel
    {
        public int Id { get; set; }
        public short Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool SmallConnectorSupport { get; set; }
        public bool LargeConnectorSupport { get; set; }
    }
}
