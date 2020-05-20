using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class ProbeSettingsFamily
    {
        public int Id { get; set; }
        public int ProbeId { get; set; }
        public int SettingsFamilyId { get; set; }
        public string ProbeFolder { get; set; }
        public string ProbeDescFileName { get; set; }
        public string ProbeDataFileNameFrontal { get; set; }
        public string ProbeDataFileNameLateral { get; set; }

        public virtual Probe Probe { get; set; }
        public virtual SettingFamily SettingsFamily { get; set; }
    }
}
