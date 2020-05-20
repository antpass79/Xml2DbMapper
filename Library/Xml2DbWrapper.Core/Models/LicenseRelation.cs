using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class LicenseRelation
    {
        public LicenseRelation()
        {
            LicenseRelationException = new HashSet<LicenseRelationException>();
        }

        public int Id { get; set; }
        public int ParentFeatureId { get; set; }
        public int FeatureId { get; set; }
        public short ParentType { get; set; }
        public short HiderType { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual Feature ParentFeature { get; set; }
        public virtual ICollection<LicenseRelationException> LicenseRelationException { get; set; }
    }
}
