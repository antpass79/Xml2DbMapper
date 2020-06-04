using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class License
    {
        public License()
        {
            //CountryLicense = new HashSet<CountryLicense>();
            //Feature = new HashSet<Feature>();
            //LogicalModel = new HashSet<LogicalModel>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }


        private const int _CodeMaxLength = 50;
        private string _Code;
        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public string Code
        {
            get
            {
                return this._Code;
            }
            set
            {
                if (value != null && value.Length > _CodeMaxLength)
                {
                    throw new ApplicationException("Field too long: Code (max " + _CodeMaxLength + ")");
                }

                this._Code = value;
            }
        }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public bool BuyableOnly { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public bool Unremovable { get; set; }

        //public virtual TwinLicenses TwinLicensesLicense { get; set; }
        //public virtual TwinLicenses TwinLicensesTwinLicense { get; set; }
        //public virtual ICollection<CountryLicense> CountryLicense { get; set; }
        //public virtual ICollection<Feature> Feature { get; set; }
        //public virtual ICollection<LogicalModel> LogicalModel { get; set; }
    }
}
