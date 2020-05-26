using System;
using System.Collections.Generic;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Models
{
    public partial class PhysicalModel
    {
        public int Id { get; set; }
        //public short Name { get; set; }
        public Porting.Contract.Enums.PhysicalModel Name { get; set; } // ANTO CAST        
        public string Description { get; set; }
        public string Code { get; set; }
        public bool SmallConnectorSupport { get; set; }
        public bool LargeConnectorSupport { get; set; }
    }
}
