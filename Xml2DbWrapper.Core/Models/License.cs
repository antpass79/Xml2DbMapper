using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class License
    {
        public License()
        {
            CountryLicense = new HashSet<CountryLicense>();
            Feature = new HashSet<Feature>();
            LogicalModel = new HashSet<LogicalModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool BuyableOnly { get; set; }
        public bool Unremovable { get; set; }

        public virtual TwinLicenses TwinLicensesLicense { get; set; }
        public virtual TwinLicenses TwinLicensesTwinLicense { get; set; }
        public virtual ICollection<CountryLicense> CountryLicense { get; set; }
        public virtual ICollection<Feature> Feature { get; set; }
        public virtual ICollection<LogicalModel> LogicalModel { get; set; }
    }
}
