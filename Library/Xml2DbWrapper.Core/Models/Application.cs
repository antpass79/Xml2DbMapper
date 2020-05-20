using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class Application
    {
        public Application()
        {
            NormalRule = new HashSet<NormalRule>();
            ProbePreset = new HashSet<ProbePreset>();
            Uirule = new HashSet<Uirule>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public short? AppType { get; set; }
        public string ProbeDescrName { get; set; }
        public string Abbreviation { get; set; }
        public string Localization { get; set; }
        public bool IsFake { get; set; }
        public int FeatureId { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual ICollection<NormalRule> NormalRule { get; set; }
        public virtual ICollection<ProbePreset> ProbePreset { get; set; }
        public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
