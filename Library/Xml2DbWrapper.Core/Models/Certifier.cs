using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class Certifier
    {
        public Certifier()
        {
            //Country = new HashSet<Country>();
            //Uirule = new HashSet<Uirule>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Code { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        //public virtual ICollection<Country> Country { get; set; }
        //public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
