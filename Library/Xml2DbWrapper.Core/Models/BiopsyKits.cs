using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class BiopsyKits
    {
        public BiopsyKits()
        {
            //NormalRule = new HashSet<NormalRule>();
            //Uirule = new HashSet<Uirule>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public int? FeatureId { get; set; }

        //public virtual Feature Feature { get; set; }
        //public virtual ICollection<NormalRule> NormalRule { get; set; }
        //public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
