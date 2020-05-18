using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class CertifierVersion
    {
        public int Id { get; set; }
        public int CertifierId { get; set; }
        public int? DistributorId { get; set; }
        public int? LogicalModelId { get; set; }
        public int MajorVersion { get; set; }
    }
}
