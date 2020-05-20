using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class RegulatoryFeature
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FeatureId { get; set; }

        public virtual Feature Feature { get; set; }
    }
}
