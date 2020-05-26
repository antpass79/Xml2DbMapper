using System.Collections.Generic;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

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
        //public short ParentType { get; set; }
        public ParentTypes ParentType { get; set; } // ANTO CAST
        //public short HiderType { get; set; }
        public HiderTypes HiderType { get; set; } // ANTO CAST

        public virtual Feature Feature { get; set; }
        public virtual Feature ParentFeature { get; set; }
        public virtual ICollection<LicenseRelationException> LicenseRelationException { get; set; }
    }
}
