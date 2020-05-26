using System.Linq;

// FG 16112015
namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public static class VersionAssociationCreator
	{
		public static IVersionAssociationInput CreateVersionAssociation(int countryId, int? DistributorId, int? ModelId, FeatureRegister DBRegister)
		{
			var CertifierId = DBRegister.Countries.Single(x => x.Id == countryId).CertifierId;
			if (CertifierId != null)
			{
				return new CertifierVersionInput((int)CertifierId, DistributorId, ModelId);
			}
			else
			{
				return new CountryVersionInput(countryId, DistributorId, ModelId);
			}
		}

		public static IVersionAssociationInput CreateVersionAssociationFromNames(string CountryCode, string CountryName,
				string DistributorCode, string ModelName, DBBuffer _buffer)
		{
			int? DistributorId = null;
			var Distributor = _buffer.p_Distributors.SingleOrDefault(x => x.Code == DistributorCode);
			if (Distributor != null)
			{
				DistributorId = Distributor.Id;
			}

			int? ModelId = null;
			var Model = _buffer.p_logicalModels.SingleOrDefault(x => x.Name == ModelName);
			if (Model != null)
			{
				ModelId = Model.Id;
			}

			var Country = _buffer.p_countries.SingleOrDefault(x => x.Code == CountryCode && x.Name == CountryName);
			if (Country == null)
			{
				var Certifier = _buffer.p_certifiers.Single(x => x.Code == CountryCode && x.Name == CountryName);
				return new CertifierVersionInput(Certifier.Id, DistributorId, ModelId);
			}
			else
			{
				return new CountryVersionInput(Country.Id, DistributorId, ModelId);
			}
		}

		public static CountryVersionInput CreateCountryVersionAssociation(string CountryCode, int logicalmodelId, DBBuffer _buffer)
		{
			var Country = _buffer.p_countries.SingleOrDefault(x => x.Code == CountryCode);
			return new CountryVersionInput(Country.Id, null, logicalmodelId);
		}
	}
}
