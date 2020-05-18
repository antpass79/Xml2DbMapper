using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class Swpack
    {
        public Swpack()
        {
            SettingFamily = new HashSet<SettingFamily>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SettingFamily> SettingFamily { get; set; }
    }
}
