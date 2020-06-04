using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class Application
    {
        public Application()
        {
            // ANTO FIX
            //NormalRule = new HashSet<NormalRule>();
            //ProbePreset = new HashSet<ProbePreset>();
            //Uirule = new HashSet<Uirule>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }
        //public short? AppType { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public SystemEnvironment? AppType { get; set; } // ANTO CAST

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public string ProbeDescrName { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public string Abbreviation { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false, IsRequired = false)]
        public string Localization { get; set; }

        [DataMember(Order = 6, EmitDefaultValue = false, IsRequired = false)]
        public bool IsFake { get; set; }

        [DataMember(Order = 7, EmitDefaultValue = false, IsRequired = false)]
        public int FeatureId { get; set; }

        // ANTO FIX

        //public virtual Feature Feature { get; set; }
        //public virtual ICollection<NormalRule> NormalRule { get; set; }
        //public virtual ICollection<ProbePreset> ProbePreset { get; set; }
        //public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
