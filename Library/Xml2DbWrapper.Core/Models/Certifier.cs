using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class Certifier
    {
        public Certifier()
        {
            Country = new HashSet<Country>();
            Uirule = new HashSet<Uirule>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Country> Country { get; set; }
        public virtual ICollection<Uirule> Uirule { get; set; }
    }
}
