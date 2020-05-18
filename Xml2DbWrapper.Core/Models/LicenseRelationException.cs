using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class LicenseRelationException
    {
        public int Id { get; set; }
        public int LicenseRelationId { get; set; }
        public int LogicalModelId { get; set; }

        public virtual LicenseRelation LicenseRelation { get; set; }
        public virtual LogicalModel LogicalModel { get; set; }
    }
}
