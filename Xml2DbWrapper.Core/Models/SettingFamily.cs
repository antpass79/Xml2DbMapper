using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class SettingFamily
    {
        public SettingFamily()
        {
            ProbePreset = new HashSet<ProbePreset>();
            ProbeSettingsFamily = new HashSet<ProbeSettingsFamily>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ProbeListFile { get; set; }
        public int? SwpackId { get; set; }

        public virtual Swpack Swpack { get; set; }
        public virtual ICollection<ProbePreset> ProbePreset { get; set; }
        public virtual ICollection<ProbeSettingsFamily> ProbeSettingsFamily { get; set; }
    }
}
