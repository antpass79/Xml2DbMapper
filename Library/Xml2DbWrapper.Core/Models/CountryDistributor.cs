using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class CountryDistributor
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public int? DistributorId { get; set; }

        public virtual Country Country { get; set; }
        public virtual Distributor Distributor { get; set; }
    }
}
