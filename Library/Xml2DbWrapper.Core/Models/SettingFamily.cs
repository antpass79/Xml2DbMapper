using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class SettingFamily
    {
        public SettingFamily()
        {
            //ProbePreset = new HashSet<ProbePreset>();
            //ProbeSettingsFamily = new HashSet<ProbeSettingsFamily>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string ProbeListFile { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false, Name = "SWpackId")]
        public int? SwpackId { get; set; }

        //public virtual Swpack Swpack { get; set; }
        //public virtual ICollection<ProbePreset> ProbePreset { get; set; }
        //public virtual ICollection<ProbeSettingsFamily> ProbeSettingsFamily { get; set; }
    }
}
