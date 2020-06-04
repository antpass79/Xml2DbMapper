using System.Collections.Generic;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
    [DataContract(Namespace = "")]
    public partial class LicenseRelation
    {
        public LicenseRelation()
        {
            //LicenseRelationException = new HashSet<LicenseRelationException>();
        }

        [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
        public int ParentFeatureId { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
        public int FeatureId { get; set; }
        //public short ParentType { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
        public ParentTypes ParentType { get; set; } // ANTO CAST
                                                    //public short HiderType { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
        public HiderTypes HiderType { get; set; } // ANTO CAST

        //public virtual Feature Feature { get; set; }
        //public virtual Feature ParentFeature { get; set; }
        //public virtual ICollection<LicenseRelationException> LicenseRelationException { get; set; }
    }
}
