using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class MinorVersionAssociation
    {
        public int Id { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string BuildVersion { get; set; }
    }
}
