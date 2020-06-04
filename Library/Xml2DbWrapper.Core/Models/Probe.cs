using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "", Name = "elemento")]
    public partial class Probe
    {
        public Probe()
        {
            //ProbePreset = new HashSet<ProbePreset>();
            //ProbeSettingsFamily = new HashSet<ProbeSettingsFamily>();
            //ProbeTransducers = new HashSet<ProbeTransducers>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string SaleName { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string ProbeDescription { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public string HwCode { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public string ProbeStringCode { get; set; }
        //public short? ProbeFamily { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false, IsRequired = false)]
        public ProbeFamilies? ProbeFamily { get; set; } // ANTO CAST

        [DataMember(Order = 6, EmitDefaultValue = false, IsRequired = false)]
        public bool? MultiConnector { get; set; }

        [DataMember(Order = 7, EmitDefaultValue = false, IsRequired = false)]
        public bool? Biplane { get; set; }

        [DataMember(Order = 8, EmitDefaultValue = false, IsRequired = false)]
        public string TeeFamily { get; set; }

        [DataMember(Order = 9, EmitDefaultValue = false, IsRequired = false)]
        public bool? Motorized { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false, IsRequired = false)]
        public int? FeatureId { get; set; }

        //public virtual Feature Feature { get; set; }
        //public virtual ICollection<ProbePreset> ProbePreset { get; set; }
        //public virtual ICollection<ProbeSettingsFamily> ProbeSettingsFamily { get; set; }
        //public virtual ICollection<ProbeTransducers> ProbeTransducers { get; set; }
    }
}
