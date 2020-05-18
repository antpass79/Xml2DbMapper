using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class Dbconfiguration
    {
        public int Id { get; set; }
        public int Compatibility { get; set; }
        public int Version { get; set; }
    }
}
