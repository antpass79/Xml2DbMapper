using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class Distributor
    {
        public Distributor()
        {
            //CountryDistributor = new HashSet<CountryDistributor>();
            //CountryLicense = new HashSet<CountryLicense>();
            //CountryVersion = new HashSet<CountryVersion>();
            //NormalRule = new HashSet<NormalRule>();
            //Uirule = new HashSet<Uirule>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        //public virtual ICollection<CountryDistributor> CountryDistributor { get; set; }
        //public virtual ICollection<CountryLicense> CountryLicense { get; set; }
        //public virtual ICollection<CountryVersion> CountryVersion { get; set; }
        //public virtual ICollection<NormalRule> NormalRule { get; set; }
        //public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
