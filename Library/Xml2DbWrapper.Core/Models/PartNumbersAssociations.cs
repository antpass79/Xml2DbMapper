using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class PartNumbersAssociations
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string FeatureBosname { get; set; }
        public int? FeatureId { get; set; }
        public int? LogicalModelId { get; set; }
        public bool ToExport { get; set; }
        public string ModeTypeToExport { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual LogicalModel LogicalModel { get; set; }
    }
}
