using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.Contract.Interfaces;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
    [DataContract(Namespace = "")]
	public class FeatureRegister
	{
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public Dictionary<string, int> OptionDictionary;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public Dictionary<string, int> ApplicationDictionary;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public Dictionary<string, int> ProbeDictionary;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public Dictionary<string, int> KitDictionary;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public Dictionary<string, int> LogicalModelDictionary;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public Dictionary<string, int> CountryDictionary;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public Dictionary<string, int> DistributorDictionary;


		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<ModelLicense> ModelLicenses;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<FeatureLicense> FeatureLicenses;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<License> Licenses;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<TwinLicenses> Twins;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<Models.PhysicalModel> PhysicalModels;
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public List<LicenseRelation> LicenseRelations;
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public List<LicenseRelationException> LicenseRelationExceptions;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<Application> Applications;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<Option> Options;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<FeatureRelation> FeatureRelations;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<Deprecated> Deprecates;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<Feature> Features;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<LicenseName> LicenseNames = new List<LicenseName>();
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<Country> Countries = new List<Country>();
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<Certifier> Certifiers = new List<Certifier>();
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<Probe> Probes = new List<Probe>();
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<BiopsyKits> Kits = new List<BiopsyKits>();
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<ProbeTransducers> ProbeTransducers = new List<ProbeTransducers>();
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public List<SettingFamily> SettingsFamilies;
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<CountryLicense> CountryLicenses = new List<CountryLicense>();
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<CountryVersion> CountryVersions = new List<CountryVersion>();
		[DataMember (EmitDefaultValue = false, IsRequired = false)]
		public List<CertifierVersion> CertifierVersions = new List<CertifierVersion>();
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public List<Distributor> Distributors = new List<Distributor>();
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public List<MinorVersionAssociation> MinorVersionAssociations = new List<MinorVersionAssociation>();
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public List<Swpack> SWpacks = new List<Swpack>();
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public CountryItems CountryItems = new CountryItems();

		private List<String> DeprecateNames = new List<String>();
		private List<String> HandledPresetNames = new List<String>();   // Gibiino [RTC 19200]
		public List<String> GetHandledPresets() => Options.Where(o => o.IsPreset).Join(Features, o => o.FeatureId, f => f.Id, (o, f) => f.NameInCode).ToList();

		public static String DBFileName = "FeatureDB.xml";
		public static String DBFileKey = "FeatureDBKey.txt";

		// part number information is a member NOT used in the machine, it is used in the WPF only to communicate with Regulatory.
		public PartNumberInfo PartNumberInformation = new PartNumberInfo();

		public static FeatureRegister LoadFromFile(string path)
		{
			var FullFileName = BuildXmlFileName(path);
			var FullKeyFileName = BuildXmlKeyFileName(path);
			var register = Provider.LoadFromFile<FeatureRegister>(FullFileName, FullKeyFileName);

			InitRegister(register);

			return register;
		}

		private static void InitRegister(FeatureRegister register)
		{
			register.DeprecateNames = register.Deprecates.Join(register.Features, d => d.DeprecatedFeatureId, f => f.Id, (d, f) => f.NameInCode).ToList();
		}

		// ANTO DB
		public static FeatureRegister CreateDBFiles(FeaturesContext db, string OutXmlPath)
		{
			var FullFileName = BuildXmlFileName(OutXmlPath);
			var FullKeyFileName = BuildXmlKeyFileName(OutXmlPath);

			FeatureRegister register = new FeatureRegister();
			register.FillFromDB(db);
			Provider.WriteFile<FeatureRegister>(register, FullFileName, FullKeyFileName);
			return register;
		}

		private static string BuildXmlFileName(string path)
		{
			return path + "\\Register_" + DBFileName;
		}

		private static string BuildXmlKeyFileName(string path)
		{
			return path + "\\Register_" + DBFileKey;
		}

		public static string GetCountryComboDisplay(Country c, Distributor d)
		{
			var result = c.Name;
			var CodeResult = c.Code;
			if (d != null)
			{
				result += " - " + d.Name;
				CodeResult += d.Code;
			}
			result += " (" + CodeResult + ")";
			return result;
		}

		// ANTO DB
		public void FillFromDB(FeaturesContext _db)
		{
			Licenses = _db.License.ToList();
			Twins = _db.TwinLicenses.ToList();
			PhysicalModels = _db.PhysicalModel.ToList();
			LicenseRelations = _db.LicenseRelation.ToList();
			LicenseRelationExceptions = _db.LicenseRelationException.ToList();
			Applications = _db.Application.ToList();
			Options = _db.Option.ToList();
			FeatureRelations = _db.FeatureRelation.ToList();
			Deprecates = _db.Deprecated.ToList();
			Features = _db.Feature.ToList();
			SettingsFamilies = _db.SettingFamily.ToList();
			SWpacks = _db.Swpack.ToList();
			Probes = _db.Probe.ToList();
			Kits = _db.BiopsyKits.ToList();
			ProbeTransducers = _db.ProbeTransducers.ToList();
			Countries = _db.Country.ToList();
			Certifiers = _db.Certifier.ToList();
			CertifierVersions = _db.CertifierVersion.ToList();
			CountryVersions = _db.CountryVersion.ToList();
			CountryLicenses = _db.CountryLicense.ToList();
			Distributors = _db.Distributor.ToList();
			MinorVersionAssociations = _db.MinorVersionAssociation.ToList();

			CountryDictionary = new Dictionary<string, int>();
			DistributorDictionary = new Dictionary<string, int>();
			foreach (var countryLicense in CountryLicenses)
			{
				var country = Countries.Single(c => c.Id == countryLicense.CountryId);
				Distributor distributor = null;
				if (countryLicense.DistributorId != null)
				{
					distributor = Distributors.Single(d => d.Id == countryLicense.DistributorId);
				}
				var license = Licenses.Single(l => l.Id == countryLicense.LicenseId);
				CountryItems.CountryItemList.Add(new CountryItem(country, distributor, license));

				var comboValue = GetCountryComboDisplay(country, distributor);
				CountryDictionary.Add(comboValue, country.Id);
				if (distributor != null)
				{
					DistributorDictionary.Add(comboValue, distributor.Id);
				}
			}

			var LModels = _db.LogicalModel.ToList();
			ModelLicenses = LModels.Join(Licenses, m => m.LicenseId, l => l.Id, (m, l) => new ModelLicense(m, l)).ToList();
			FeatureLicenses = Features.Join(Licenses, f => f.LicenseId, l => l.Id, (f, l) => new FeatureLicense(f, l)).ToList();

			OptionDictionary = Options.Join(Features, o => o.FeatureId, f => f.Id, (o, f) => new
			{
				option = o,
				feature = f
			}).ToDictionary(x => x.feature.NameInCode, x => x.option.Id);

			ApplicationDictionary = Applications.ToList().Join(Features, a => a.FeatureId, f => f.Id, (a, f) => new
			{
				application = a,
				feature = f
			}).ToDictionary(x => x.feature.NameInCode, x => x.application.Id);

			ProbeDictionary = Probes.ToDictionary(x => x.SaleName, y => y.Id);
			KitDictionary = Kits.ToDictionary(x => x.Name, y => y.Id);

			LogicalModelDictionary = LModels.Join(Licenses, m => m.LicenseId, l => l.Id, (m, l) => new
			{
				modelId = m.Id,
				licenseName = l.Name
			}).ToDictionary(x => x.licenseName, y => y.modelId);

			// license names: if the feature is not associated to any license, take the name of the licensed parent in the FeatureRelation
			foreach (var feature in Features)
			{
				if (feature.LicenseId != null)
				{
					LicenseNames.Add(new LicenseName(feature.NameInCode, Licenses.Single(l => l.Id == feature.LicenseId)));
				}
				else
				{
					var parentLicenseFeatures = FeatureRelations.Where(b => b.FeatureId == feature.Id)
												.Join(FeatureLicenses, b => b.ParentFeatureId, fl => fl.Feature.Id, (b, fl) => fl.License).ToList();
					foreach (var parent in parentLicenseFeatures)
					{
						LicenseNames.Add(new LicenseName(feature.NameInCode, parent));
					}
				}
			}
		}


		public enum FeatureType { Application, Option, Probe, Kit, Nothing }

		public FeatureType GetFeatureType(String featureName)
		{
			if (OptionDictionary.Select(d => d.Key).Contains(featureName))
			{
				return FeatureType.Option;
			}

			if (ApplicationDictionary.Select(d => d.Key).Contains(featureName))
			{
				return FeatureType.Application;
			}

			if (ProbeDictionary.Select(d => d.Key).Contains(featureName))
			{
				return FeatureType.Probe;
			}

			if (KitDictionary.Select(d => d.Key).Contains(featureName))
			{
				return FeatureType.Kit;
			}

			return FeatureType.Nothing;
		}


		// fills the feature differently if it is an option or an application
		public ILicenseFilter FillFeature(String FeatureStringName)
		{
			var filter = CreateLicenseFilter();
			var FeatureType = GetFeatureType(FeatureStringName);

			if (FeatureType == FeatureType.Option)
			{
				filter.OptionName = FeatureStringName;
			}

			if (FeatureType == FeatureType.Application)
			{
				filter.ApplicationType = Provider.FromStringToAppType(FeatureStringName);
			}

			if (FeatureType == FeatureType.Nothing)
			{
				return null;
			}
			return filter;
		}

		public bool IsFeatureDeprecated(string FeatureName)
		{
			return DeprecateNames.Contains(FeatureName);
		}

		public static ILicenseFilter CreateLicenseFilter() => new LicenseFilter();

		public static ILicensePositionFilter CreateLicensePositionFilter() => new LicensePositionFilter();



		public static void SortLicenses(FeatureRegister FullRegister, out List<License> LicenseList, out List<License> AppLicenses, out List<License> VetAppLicenses,
										out List<License> OptionLicenses, Dictionary<String, FeatureInfo> FeatureAvailabilities = null)
		{
			var register = new FeatureRegisterSubset(FullRegister);

			// consider only license relations where features are of the same type
			register.LicenseRelations = register.LicenseRelations.Join(register.Features, lr => lr.FeatureId, f => f.Id, (lr, f) => new
			{
				lr, f
			})
			.Join(register.Features, x => x.lr.ParentFeatureId, f => f.Id, (x, f) => new
			{
				x.lr, child = x.f, parent = f
			})
			.Where(x => FullRegister.GetFeatureType(x.child.NameInCode) == FullRegister.GetFeatureType(x.parent.NameInCode)).Select(x => x.lr).ToList();


			// filter out the unavailale features in order not o mix the dependencie relative to features associated with the same license.
			if (FeatureAvailabilities != null && FeatureAvailabilities.Count > 0)
			{
				register.Features = register.Features.Where(f => FeatureAvailabilities.Select(x => x.Key).Contains(f.NameInCode)
									&& FeatureAvailabilities[f.NameInCode].IsAvailable).ToList();
				var selectedFeaturesIds = register.Features.Select(f => f.Id).ToList();

				register.FeatureLicenses = register.FeatureLicenses.Where(x => selectedFeaturesIds.Contains(x.Feature.Id)).ToList();
				register.Applications = register.Applications.Where(a => selectedFeaturesIds.Contains(a.FeatureId)).ToList();
				register.Options = register.Options.Where(x => selectedFeaturesIds.Contains(x.FeatureId)).ToList();
				register.LicenseRelations = register.LicenseRelations.Where(lr => selectedFeaturesIds.Contains(lr.FeatureId)
											&& selectedFeaturesIds.Contains(lr.ParentFeatureId)).ToList();
			}

			// do not consider collapsers for license ordering
			var ParentFeatureIds = register.LicenseRelations.Where(lr => lr.HiderType != HiderTypes.Collapser).Select(x => x.ParentFeatureId).Join(register.Features,
								   k => k, f => f.Id, (k, f) => f.Id).Distinct().ToList();
			var UndistinctChildFeatureIds = register.LicenseRelations.Where(lr => lr.HiderType != HiderTypes.Collapser).Select(x => x.FeatureId).Join(register.Features,
											k => k, f => f.Id, (k, f) => f.Id).ToList();
			var ChildFeatureIds = UndistinctChildFeatureIds.Distinct().ToList();
			var _MultipleParentLicenses = UndistinctChildFeatureIds.GroupBy(l => l).Where(g => g.Count() > 1).Select(g => g.First())
										  .Join(register.FeatureLicenses, k => k, f => f.Feature.Id, (k, fl) => fl).ToList();


			// Human applications
			var MultipleParent_HumanAppFeatLicenses = _MultipleParentLicenses.Join(register.Applications.Where(a => a.AppType ==
					SystemEnvironment.Human), fl => fl.Feature.Id, a => a.FeatureId, (fl, a) => fl).ToList();

			var _HumanFeatNoChildren = register.Applications.Where(a => a.AppType == SystemEnvironment.Human)
									   .Join(register.FeatureLicenses, a => a.FeatureId, fl => fl.Feature.Id, (a, fl) => fl.Feature).Where(x => !ChildFeatureIds.Contains(x.Id)).ToList();
			var HumanAppLicenses = FillChildren(_HumanFeatNoChildren, ParentFeatureIds, MultipleParent_HumanAppFeatLicenses, register);

			// Vet applications
			var MultipleParent_VetAppFeatLicenses = _MultipleParentLicenses.Join(register.Applications.Where(a => a.AppType ==
													SystemEnvironment.Veterinary), fl => fl.Feature.Id, a => a.FeatureId, (fl, a) => fl).ToList();
			var _VetFeatNoChildren = register.Applications.Where(a => a.AppType == SystemEnvironment.Veterinary)
									 .Join(register.FeatureLicenses, a => a.FeatureId, fl => fl.Feature.Id, (a, fl) => fl.Feature).Where(x => !ChildFeatureIds.Contains(x.Id)).ToList();
			VetAppLicenses = FillChildren(_VetFeatNoChildren, ParentFeatureIds, MultipleParent_VetAppFeatLicenses, register);

			// Options
			var MultipleParent_OptFeatLicenses = _MultipleParentLicenses.Join(register.Options, fl => fl.Feature.Id, o => o.FeatureId, (fl, o) => fl).ToList();
			var _OptionFeatNoChildren = register.Options
										.Join(register.FeatureLicenses, o => o.FeatureId, fl => fl.Feature.Id, (o,
												fl) => fl.Feature).Where(x => !ChildFeatureIds.Contains(x.Id)).OrderBy(fl => fl.LicenseId).ToList();
			OptionLicenses = FillChildren(_OptionFeatNoChildren, ParentFeatureIds, MultipleParent_OptFeatLicenses, register);

			AppLicenses = HumanAppLicenses.Union(VetAppLicenses).ToList();
			LicenseList = AppLicenses.Union(OptionLicenses).ToList();
		}

		private class FeatureRegisterSubset
		{
			public List<Feature> Features;
			public List<LicenseRelation> LicenseRelations;
			public List<FeatureLicense> FeatureLicenses;
			public List<Application> Applications;
			public List<Option> Options;

			public FeatureRegisterSubset(FeatureRegister register)
			{
				Features = register.Features;
				LicenseRelations = register.LicenseRelations;
				FeatureLicenses = register.FeatureLicenses;
				Applications = register.Applications;
				Options = register.Options;
			}
		}

		private static List<License> FillChildren(List<Feature> baseFeatureList, List<int> ParentFeatureIds,
				List<FeatureLicense> MultipleParentFeatLicenses, FeatureRegisterSubset register)
		{
			var Licenses = new List<License>();
			var MultipleParentLicenseIds = MultipleParentFeatLicenses.Select(x => x.License.Id).ToList();
			List<Feature> l_FeatureList = new List<Feature>(baseFeatureList);

			foreach (var Feature in baseFeatureList)
			{
				AddLicense2Enum(Licenses, Feature, ParentFeatureIds, MultipleParentLicenseIds, register, ref l_FeatureList);
			}

			return Licenses;
		}


		// Gibiino [RTC 19507] get index in list for multiple parent features
		// the license with multiple parents is placed before the first license in the list without any parent that follows the last parent of the license to insert
		//private static int? GetIndexForMPFeature(List<License> Licenses, FeatureLicense MPfeatureL, FeatureRegisterSubset register)
		//{
		//  var Parents = register.LicenseRelations.Where(lr => lr.FeatureId == MPfeatureL.Feature.Id)
		//                .Join(register.FeatureLicenses, lr => lr.ParentFeatureId, fl => fl.Feature.Id, (lr, fl) => fl).ToList();
		//  if (Parents.Count > 0)
		//  {
		//      var parentindex = Parents.Max(p => Licenses.IndexOf(p.License));
		//      if (parentindex < Licenses.Count - 1)
		//      {
		//          for (int indx = parentindex + 1; indx < Licenses.Count; indx++)
		//          {
		//              var featureIds = register.Features.Where(f => f.LicenseId == Licenses[indx].Id).ToList();
		//              // check if any of the feature associated with the license has a parent
		//              if (!register.LicenseRelations.Join(featureIds, lr => lr.FeatureId, f => f.Id, (lr, f) => lr).Any())
		//              {
		//                  // if the license has no parent return its value
		//                  return indx;
		//              }
		//          }
		//      }
		//  }
		//  return null;  // i.e. place the license as last in the list
		//}

		private static void AddLicense2Enum(List<License> Licenses, Feature Feature, List<int> ParentFeatureIds,
											List<int> MultipleParentLicenseIds, FeatureRegisterSubset register, ref List<Feature> FeaturesToAdd, int? index = null)
		{
			var l_license = register.FeatureLicenses.Single(fl => fl.Feature.Id == Feature.Id).License;
			if (!Licenses.Any(l => l.Id == l_license.Id))       // do not add replicates
			{
				if (index == null)
				{
					Licenses.Add(l_license);
				}
				else
				{
					Licenses.Insert((int)index, l_license);
				}

				FeaturesToAdd.Remove(Feature);

				if (ParentFeatureIds.Contains(Feature.Id))
				{
					var Children = register.LicenseRelations.Where(lr => lr.ParentFeatureId == Feature.Id)
								   .Join(register.FeatureLicenses, lr => lr.FeatureId, fl => fl.Feature.Id, (lr, fl) => fl);

					// Skip children with multiple parents
					var ChildrenOneOnlyParent = Children.Where(c => !MultipleParentLicenseIds.Contains(c.License.Id)).ToList();
					foreach (var child in ChildrenOneOnlyParent)
					{
						AddLicense2Enum(Licenses, child.Feature, ParentFeatureIds, MultipleParentLicenseIds, register, ref FeaturesToAdd);
					}

					//@PD Get children with multiple parents and if no more parent is to be added, add them
					var ChildrenMoreParents = Children.Where(c => MultipleParentLicenseIds.Contains(c.License.Id)).ToList();
					foreach (var child in ChildrenMoreParents)
					{
						var ParentsToAdd = register.LicenseRelations.Where(lr => (lr.FeatureId == child.Feature.Id) && (lr.HiderType == HiderTypes.NoHide)).
										   Join(FeaturesToAdd, lr => lr.ParentFeatureId, fl => fl.Id, (lr, fl) => fl).ToList();

						if ( ParentsToAdd.Count() == 0)
						{
							AddLicense2Enum(Licenses, child.Feature, ParentFeatureIds, MultipleParentLicenseIds, register, ref FeaturesToAdd);
						}
					}
				}
			}
		}

		// Remove license relation exceptions
		public void SqueezeLicenseRelations(string LogicalModelName)
		{
			var lrIds2remove = LicenseRelationExceptions.Where(lre => lre.LogicalModelId ==
							   LogicalModelDictionary[LogicalModelName]).Select(x => x.LicenseRelationId).ToList();
			if (lrIds2remove != null && lrIds2remove.Count > 0)
			{
				LicenseRelations = LicenseRelations.Where(lr => !lrIds2remove.Contains(lr.Id)).ToList();
			}
		}
	}
}
