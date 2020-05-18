using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class ProbePreset
    {
        public int Id { get; set; }
        public int ProbeId { get; set; }
        public int ApplicationId { get; set; }
        public int SettingsFamilyId { get; set; }
        public string ProbeDefaultEnum { get; set; }
        public string PresetFolder { get; set; }
        public string PresetFileNameFrontal { get; set; }
        public string PresetFileNameLateral { get; set; }

        public virtual Application Application { get; set; }
        public virtual Probe Probe { get; set; }
        public virtual SettingFamily SettingsFamily { get; set; }
    }
}
