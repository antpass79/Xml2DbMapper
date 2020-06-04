using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class CountryLicense
    {
        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public int CountryId { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public int? DistributorId { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public int LicenseId { get; set; }

        //public virtual Country Country { get; set; }
        //public virtual Distributor Distributor { get; set; }
        //public virtual License License { get; set; }
    }
}
