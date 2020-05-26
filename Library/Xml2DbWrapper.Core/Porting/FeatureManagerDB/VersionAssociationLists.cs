using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Models;

// FG 16112015
namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	// Class intended for the import of the country version template
	public class VersionAssociationLists
	{
		public List<CountryVersion> CountryVersions = new List<CountryVersion>();
		public List<CertifierVersion> CertifierVersions = new List<CertifierVersion>();

		// explode all records in country and certifier version and check consistency
		// for models we handle the default model version, which can be overwritten by specifying the model. Therefore records will not replicated for model
		public void CheckConsistency(DBBuffer _buffer, string logFileXml2DB)
		{
			var VirtualAssociations = new List<CountryVersion>();
			VirtualAssociations.AddRange(CountryVersions);

			// replicate certifier records
			foreach (var CertifierVersion in CertifierVersions)
			{
				var ChildrenCounttries = _buffer.p_countries.Where(c => c.CertifierId == CertifierVersion.CertifierId).ToList();
				foreach (var ChildrenCountry in ChildrenCounttries)
				{
					VirtualAssociations.Add(new CountryVersion
					{
						CountryId = ChildrenCountry.Id,
						MajorVersion = CertifierVersion.MajorVersion,
						DistributorId = CertifierVersion.DistributorId,
						LogicalModelId = CertifierVersion.LogicalModelId
					});
				}
			}

			// replicate Distributor records
			var GroupedCountryWithDistributors = _buffer.p_Country_Distributors.GroupBy(x => x.CountryId).ToList();
			foreach (var CountryDistributorGroup in GroupedCountryWithDistributors)
			{
				var Vas2Replicate4Distributor = VirtualAssociations.Where(va => va.CountryId == CountryDistributorGroup.Key && va.DistributorId == null).ToList();
				foreach (var Va2Replicate4Distributor in Vas2Replicate4Distributor)
				{
					foreach (var DistributorId in CountryDistributorGroup.ToList().Select(g => g.DistributorId))
					{
						VirtualAssociations.Add(new CountryVersion
						{
							CountryId = CountryDistributorGroup.Key,
							MajorVersion = Va2Replicate4Distributor.MajorVersion,
							DistributorId = DistributorId,
							LogicalModelId = Va2Replicate4Distributor.LogicalModelId,
						});
					}
				}
			}

			// check if there is any double record in the Virtual Associations: Do not check Version in distinguishing records
			var DoubleRecords = VirtualAssociations.GroupBy(va => new
			{
				va.CountryId, va.DistributorId, va.LogicalModelId
			}).Where(g => g.Count() > 1).ToList();
			if (DoubleRecords.Count > 0)
			{
				var message = "Versions for "
							  + string.Join(", ", DoubleRecords.Select(dr => dr.First().CountryId).Join(_buffer.p_countries, x => x, c => c.Id, (x, c) => c.Name))
							  + " are specified more than once. Please check the countries specified in the certifiers.";
			}


			// database index makes sure {country, distributor, logical model} are unique

			// check if all country items (i.e. each country license) is correctly associated with a version for all models with part number
			foreach (var CountryLicense in _buffer.p_CountryLicenses)
			{
				foreach (var model in _buffer.p_logicalModels)
				{
					if (!VirtualAssociations.Any(cl => cl.CountryId == CountryLicense.CountryId && cl.DistributorId == CountryLicense.DistributorId
												 && cl.LogicalModelId == model.Id))
					{
						var country = _buffer.p_countries.Single(c => c.Id == CountryLicense.CountryId);
						if (country.IsObsolete)
						{
							Provider.WriteImportLogFormat(logFileXml2DB, "Warning: " + country.Name + " (obsolete country), not associated with any major in " + model.Name);
						}
						else
						{
							throw new Exception(country.Name + " " + GetDistributorString(CountryLicense.DistributorId, _buffer)
												+ " Has no specified version for model " + model.Name);
						}
					}
				}
			}
		}

		private string GetDistributorString(int? DistributorId, DBBuffer _buffer)
		{
			var distributorString = "";
			if (DistributorId != null)
			{
				var distributor = _buffer.p_Distributors.SingleOrDefault(d => d.Id == DistributorId);
				if (distributor != null)
				{
					distributorString = " with " + distributor.Name;
				}
			}
			return distributorString;
		}
	}
}
