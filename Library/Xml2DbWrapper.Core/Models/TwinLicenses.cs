using System;
using System.Collections.Generic;

namespace Xml2DbMapper.Core.Models
{
    public partial class TwinLicenses
    {
        public int Id { get; set; }
        public int LicenseId { get; set; }
        public int TwinLicenseId { get; set; }

        public virtual License License { get; set; }
        public virtual License TwinLicense { get; set; }
    }
}
