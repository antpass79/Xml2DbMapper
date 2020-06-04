using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class Country
    {
        public Country()
        {
            //CountryDistributor = new HashSet<CountryDistributor>();
            //CountryLicense = new HashSet<CountryLicense>();
            //CountryVersion = new HashSet<CountryVersion>();
            //NormalRule = new HashSet<NormalRule>();
            //Uirule = new HashSet<Uirule>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Code { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public int? CertifierId { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public bool IsObsolete { get; set; }

        //public virtual Certifier Certifier { get; set; }
        //public virtual ICollection<CountryDistributor> CountryDistributor { get; set; }
        //public virtual ICollection<CountryLicense> CountryLicense { get; set; }
        //public virtual ICollection<CountryVersion> CountryVersion { get; set; }
        //public virtual ICollection<NormalRule> NormalRule { get; set; }
        //public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
