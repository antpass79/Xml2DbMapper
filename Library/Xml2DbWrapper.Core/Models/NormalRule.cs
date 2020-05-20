using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class NormalRule
    {
        public int Id { get; set; }
        public short Allow { get; set; }
        public int? LogicalModelId { get; set; }
        public int? ApplicationId { get; set; }
        public int? OptionId { get; set; }
        public short? UserLevel { get; set; }
        public int? Version { get; set; }
        public int? CountryId { get; set; }
        public int? DistributorId { get; set; }
        public int? ProbeId { get; set; }
        public short? TransducerType { get; set; }
        public int? KitId { get; set; }
        public int UiruleId { get; set; }

        public virtual Application Application { get; set; }
        public virtual Country Country { get; set; }
        public virtual Distributor Distributor { get; set; }
        public virtual BiopsyKits Kit { get; set; }
        public virtual LogicalModel LogicalModel { get; set; }
    }
}
