using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class Deprecated
    {
        public int Id { get; set; }
        public int DeprecatedFeatureId { get; set; }
        public int? SubstituteFeatureId { get; set; }

        public virtual Feature DeprecatedFeature { get; set; }
        public virtual Feature SubstituteFeature { get; set; }
    }
}
