using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    public partial class ProbePreset
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int ProbeId { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int ApplicationId { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int SettingsFamilyId { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string ProbeDefaultEnum { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string PresetFolder { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string PresetFileNameFrontal { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string PresetFileNameLateral { get; set; }

        //public virtual Application Application { get; set; }
        //public virtual Probe Probe { get; set; }
        //public virtual SettingFamily SettingsFamily { get; set; }
    }
}
