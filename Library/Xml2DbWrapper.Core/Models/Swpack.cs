using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Name = "SWpack", Namespace = "")]
    public partial class Swpack
    {
        public Swpack()
        {
            //SettingFamily = new HashSet<SettingFamily>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        //public virtual ICollection<SettingFamily> SettingFamily { get; set; }
    }
}
