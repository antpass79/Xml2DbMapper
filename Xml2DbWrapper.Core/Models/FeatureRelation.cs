using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class FeatureRelation
    {
        public int Id { get; set; }
        public int ParentFeatureId { get; set; }
        public int FeatureId { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual Feature ParentFeature { get; set; }
    }
}
