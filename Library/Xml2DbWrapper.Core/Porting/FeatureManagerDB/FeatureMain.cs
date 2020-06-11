using EFCore.BulkExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using Xml2DbMapper.Core.Models;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.Contract.Interfaces;
using Xml2DbMapper.Core.Porting.Util;

//fg 22022015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	[DataContract(Namespace = "")]
	public partial class FeatureMain
	{
		public FeatureMain(FeatureRegister register)
		{
			DBRegister = register;
		}

		[DataMember (Order = 0, EmitDefaultValue = false, IsRequired = false)]
		private List<NormalRule> NormalRules = new List<NormalRule>();

		private Dictionary<int, List<Int32>> LogicalModelIndexes;
		private Dictionary<int, List<Int32>> ApplicationIndexes;
		private Dictionary<int, List<Int32>> OptionIndexes;
		private Dictionary<int, List<Int32>> ProbeIndexes;
		private Dictionary<int, List<Int32>> KitIndexes;
		private Dictionary<int, List<Int32>> TransducerIndexes;

		private List<int> RuleIndexes;
		private FeatureRegister DBRegister;

		public FeatureRegister DBRegisterSetter
		{
			set
			{
				DBRegister = value;
			}
		}


		private static string BuildXmlFileName(string path, string modelName)
		{
			return BuildXmlName(path, modelName, FeatureRegister.DBFileName);
		}

		private static string BuildXmlKeyFileName(string path, string modelName)
		{
			return BuildXmlName(path, modelName, FeatureRegister.DBFileKey);
		}

		private static string BuildXmlName(string path, string modelName, string filename)
		{
			var Adding = "";
			if (!string.IsNullOrEmpty(modelName))
			{
				Adding = modelName + "_";
			}
			return path + "\\" + Adding + filename;
		}


		public static FeatureMain FullLoadFromFile(string path, string modelName)
		{
			var register = FeatureRegister.LoadFromFile(path);
			return LoadFromFile(path, modelName, register);
		}


		public static FeatureMain LoadFromFile(string path, string modelName, FeatureRegister _register)
		{
			var main = LoadWitoutRegister(path, modelName);
			main.DBRegister = _register;      // assign the preloade register of the DB tables
			return main;
		}

		public static FeatureMain LoadFromFileAndFilter(string path, string modelName, FeatureRegister _register)
		{
			var main = LoadFromFile(path, modelName, _register);
			_register.ModelLicenses = _register.ModelLicenses.Where(m => m.License.Name == modelName).ToList();
			return main;
		}

		public static FeatureMain LoadAllRules(string path, FeatureRegister _register)
		{
			var main = new FeatureMain(_register);

			var ModelNames = _register.ModelLicenses.Select(m => m.License.Name).ToList();
			foreach (var ModelName in ModelNames)
			{
				main.AddModelRules(path, ModelName);
			}
			main.RemoveDoubles();
			return main;
		}

		public static FeatureMain LoadPhysicalModelRules(string path, string pmtoload, FeatureRegister _register)
		{
			var main = new FeatureMain(_register);
			_register.ModelLicenses = _register.ModelLicenses.Where(m => m.Model.PhModId == Int32.Parse(pmtoload)).ToList();

			var ModelNames = _register.ModelLicenses.Select(m => m.License.Name).ToList();
			foreach (var ModelName in ModelNames)
			{
				main.AddModelRules(path, ModelName);
			}
			main.RemoveDoubles();
			return main;
		}

		public static FeatureMain LoadswPackRules(string path, string swpack, FeatureRegister _register)
		{
			var main = new FeatureMain(_register);
			var selectedSettingsFamiliesIds = _register.SettingsFamilies.Where(sf => sf.SwpackId == Int32.Parse(swpack)).Select(x => x.Id).ToList();
			_register.ModelLicenses = _register.ModelLicenses.Where(m => m.Model.SettingsFamilyId != null &&
									  selectedSettingsFamiliesIds.Contains((int)m.Model.SettingsFamilyId)).ToList();

			var ModelNames = _register.ModelLicenses.Select(m => m.License.Name).ToList();
			foreach (var ModelName in ModelNames)
			{
				main.AddModelRules(path, ModelName);
			}
			main.RemoveDoubles();
			return main;
		}

		private void AddModelRules(string path, string modelName)
		{
			try
			{
				var main = LoadWitoutRegister(path, modelName);
				this.NormalRules.AddRange(main.NormalRules);
			}
			catch (Exception ex)
			{
				throw new Exception("Impossible to load rules for " + modelName + ": " + ex.Message + ". Make sure the file was added as link in the WPF project");
			}
		}

		private void RemoveDoubles()
		{
			NormalRules = NormalRules.Distinct(new SameNRId()).ToList();
		}


		public static FeatureMain LoadWitoutRegister(string path, string modelName)
		{
			var FullFileName = BuildXmlFileName(path, modelName);
			var FullKeyFileName = BuildXmlKeyFileName(path, modelName);
			var main = Provider.LoadFromFile<FeatureMain>(FullFileName, FullKeyFileName);
			return main;
		}


		public static void WriteFile(FeatureMain Fmain, string path, string modelName)
		{
			var FullFileName = BuildXmlFileName(path, modelName);
			var FullKeyFileName = BuildXmlKeyFileName(path, modelName);

			Provider.WriteFile<FeatureMain>(Fmain, FullFileName, FullKeyFileName);
		}


		// maps the license filter to the input paramter class of the Feature Manager DB
		private InputParams FromFilterToInputSlim(IInternalLicenseFilter filter)
		{
			var result = new InputParams();
			var NonNullSlimFields = 0;

			if ((filter.ApplicationType != null) && DBRegister.ApplicationDictionary.ContainsKey(filter.ApplicationType.ToString()))
			{
				result.ApplicationId = DBRegister.ApplicationDictionary[filter.ApplicationType.ToString()];
				NonNullSlimFields++;
			}

			if ((filter.OptionName != null) && DBRegister.OptionDictionary.ContainsKey(filter.OptionName))
			{
				result.OptionId = DBRegister.OptionDictionary[filter.OptionName];
				NonNullSlimFields++;
			}

			if ((filter.ProbeName != null) && DBRegister.ProbeDictionary.ContainsKey(filter.ProbeName))
			{
				result.ProbeId = DBRegister.ProbeDictionary[filter.ProbeName];
				NonNullSlimFields++;
			}

			if (filter.TransducerType != null)
			{
				result.TransducerType = filter.TransducerType;
				NonNullSlimFields++;
			}

			if ( (filter.KitName != null) && DBRegister.KitDictionary.ContainsKey(filter.KitName))
			{
				result.KitId = DBRegister.KitDictionary[filter.KitName];
				NonNullSlimFields++;
			}

			if (CheckInput(result, NonNullSlimFields))
			{
				return result;
			}
			else
			{
				var msg = "Wrong input format: forbidden application " + filter.ApplicationType;
				// ANTO LOG
				//Logger.LogTrace("FeatureManager", Flow_Type.Configuration, LogLevel.LogNormal, msg);
				Logger.Log($"FeatureManager {msg}");
				throw new ApplicationException(msg);
			}
		}

		// the parent applications can be called only using license manager input patterns
		private Boolean CheckInput(InputParams input, int NonNullSlimFields)
		{
			if (input.ApplicationId == null)
			{
				return true;
			}
			else
			{
				if (DBRegister.Applications.Single(a => a.Id == input.ApplicationId).ProbeDescrName != null)
				{
					return true;
				}
				else
				{
					return NonNullSlimFields == 1;
				}
			}
		}



		public void FillIndexes()
		{
			LogicalModelIndexes = new Dictionary<int, List<Int32>>();
			ApplicationIndexes = new Dictionary<int, List<Int32>>();
			OptionIndexes = new Dictionary<int, List<Int32>>();
			ProbeIndexes = new Dictionary<int, List<Int32>>();
			KitIndexes = new Dictionary<int, List<Int32>>();
			TransducerIndexes = new Dictionary<int, List<Int32>>();

			foreach (var modelLic in DBRegister.ModelLicenses)
			{
				var rulesIndexes = new List<int>();
				for (int i = 0; i < NormalRules.Count; i++)
				{
					if (ComparableFields.isValueContained(modelLic.Model.Id, NormalRules[i].LogicalModelId))
					{
						rulesIndexes.Add(i);
					}
				}
				LogicalModelIndexes.Add(modelLic.Model.Id, rulesIndexes);
			}

			foreach (var app in DBRegister.Applications)
			{
				var rulesIndexes = new List<int>();
				for (int i = 0; i < NormalRules.Count; i++)
				{
					if (ComparableFields.isValueContained(app.Id, NormalRules[i].ApplicationId))
					{
						rulesIndexes.Add(i);
					}
				}
				ApplicationIndexes.Add(app.Id, rulesIndexes);
			}

			foreach (var opt in DBRegister.Options)
			{
				var rulesIndexes = new List<int>();
				for (int i = 0; i < NormalRules.Count; i++)
				{
					if (ComparableFields.isValueContained(opt.Id, NormalRules[i].OptionId))
					{
						rulesIndexes.Add(i);
					}
				}
				OptionIndexes.Add(opt.Id, rulesIndexes);
			}

			foreach (var probe in DBRegister.Probes)
			{
				var rulesIndexes = new List<int>();
				for (int i = 0; i < NormalRules.Count; i++)
				{
					if (ComparableFields.isValueContained(probe.Id, NormalRules[i].ProbeId))
					{
						rulesIndexes.Add(i);
					}
				}
				ProbeIndexes.Add(probe.Id, rulesIndexes);
			}

			foreach (var kit in DBRegister.Kits)
			{
				var rulesIndexes = new List<int>();
				for (int i = 0; i < NormalRules.Count; i++)
				{
					if (ComparableFields.isValueContained(kit.Id, NormalRules[i].KitId))
					{
						rulesIndexes.Add(i);
					}
				}
				KitIndexes.Add(kit.Id, rulesIndexes);
			}


			foreach (int transd in Enum.GetValues(typeof(ProbeType)))
			{
				var rulesIndexes = new List<int>();
				for (int i = 0; i < NormalRules.Count; i++)
				{
					if (ComparableFields.isValueContained(transd, (int? )NormalRules[i].TransducerType))
					{
						rulesIndexes.Add(i);
					}
				}
				TransducerIndexes.Add(transd, rulesIndexes);
			}

			RuleIndexes = Enumerable.Range(0, NormalRules.Count).ToList();
		}


		// fore each input checks if there is any specific forbidding rule
		//public bool IsAlwaysTrue(InputParams input)
		//{
		//  var interestedRules = this.NormalRules.Where(nr => ComparableFields.isAlwaysContained(input, nr)).ToList();
		//  return !interestedRules.Any(r => r.Allow == Allower.AllowModes.No);
		//}

		public List<NormalRule> getInterestedRulesFast(InputParams input, List<NormalRule> NRlist)
		{
			var overallFilter = RuleIndexes;
			if (input.LogicalModelId != null)
			{
				var ModelFilter = GetRuleIndexList(RuleIndexes, input.LogicalModelId, LogicalModelIndexes);
				overallFilter = overallFilter.IntersectSorted(ModelFilter);
			}
			if (input.ApplicationId != null)
			{
				var ApplicationFilter = GetRuleIndexList(RuleIndexes, input.ApplicationId, ApplicationIndexes);
				overallFilter = overallFilter.IntersectSorted(ApplicationFilter);
			}
			if (input.OptionId != null)
			{
				var OptionFilter = GetRuleIndexList(RuleIndexes, input.OptionId, OptionIndexes);
				overallFilter = overallFilter.IntersectSorted(OptionFilter);
			}
			if (input.ProbeId != null)
			{
				var ProbeFilter = GetRuleIndexList(RuleIndexes, input.ProbeId, ProbeIndexes);
				overallFilter = overallFilter.IntersectSorted(ProbeFilter);
			}
			if (input.KitId != null)
			{
				var KitFilter = GetRuleIndexList(RuleIndexes, input.KitId, KitIndexes);
				overallFilter = overallFilter.IntersectSorted(KitFilter);
			}
			if (input.TransducerType != null)
			{
				var TransducerFilter = GetRuleIndexList(RuleIndexes, (int)input.TransducerType, TransducerIndexes);
				overallFilter = overallFilter.IntersectSorted(TransducerFilter);
			}

			var InterestedRules = new List<NormalRule>();
			for (int i = 0; i < overallFilter.Count; i++)
			{
				if (ComparableFields.isContainedFull(input, NRlist[overallFilter[i]]))
				{
					InterestedRules.Add(NRlist[overallFilter[i]]);
				}
			}
			return InterestedRules;
		}


		private List<int> GetRuleIndexList(List<int> AllRules, int? value, Dictionary<int, List<int>> dictIndexes)
		{
			List<int> result;

			if (value != null)
			{
				result = dictIndexes[(Int32)value];
			}
			else
			{
				result = AllRules;
			}
			return result;
		}

		// full conversion
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private InputParams FromFilterToInput(IInternalLicenseFilter filter)
		{
			var result = FromFilterToInputSlim(filter);
			FillBaseParameters(filter, result);
			return result;
		}


		// iscontained using only fileds not filtered by the license manager
		public Boolean IsAllowedSlim(ILicenseFilter filter, IAllower Allower)
		{
			var input = FromFilterToInputSlim((IInternalLicenseFilter)filter);
			return IsAllowedSlim(input, Allower);
		}

		internal Boolean IsAllowedSlim(InputParams input, IAllower Allower)
		{
			return !NormalRules.Where(nr => ComparableFields.isContainedSlim(input, nr)).Any(ir => Allower.getAllowance(ir.Allow) == false);

		}

		//internal static Boolean IsAllowedFull(InputParams input, IAllower Allower, List<NormalRule> nrules)
		//{
		//  List<NormalRule> InterestedRules = getInterestedRulesFull(input, nrules);
		//  if (InterestedRules.Count == 0)
		//  {
		//      return false;       // CWA: by default the input is not allowed in case the rule is not present
		//  }
		//  else
		//  {
		//      return !InterestedRules.Exists(ir => Allower.getAllowance(ir.Allow) == false);
		//  }
		//}


		//// faster because it is using indexes
		//public Boolean IsAllowedFast(IInternalLicenseFilter filter, IAllower Allower)
		//{
		//  var input = FromFilterToInput(filter);
		//  return IsAllowedFast(input, Allower);
		//}

		public Boolean IsAllowedFast(InputParams input, IAllower Allower)
		{
			List<NormalRule> InterestedRules = getInterestedRulesFast(input, this.NormalRules);
			if (InterestedRules.Count == 0)
			{
				return false;       // CWA: by default the input is not allowed in case the rule is not present
			}
			else
			{
				var result = !InterestedRules.Exists(ir => Allower.getAllowance(ir.Allow) == false);
				return result;
			}
		}



		static List<NormalRule> getInterestedRulesSlim(InputParams input, List<NormalRule> NRlist)
		=> NRlist.Where(nr => ComparableFields.isContainedSlim(input, nr)).ToList();

		//static List<NormalRule> getInterestedRulesFull(InputParams input, List<NormalRule> NRlist)
		//=> NRlist.Where(nr => ComparableFields.isContainedFull(input, nr)).ToList();


		// Preliminar filter of the rules
		private void PreFilterRules(InputParams input)
		{
			this.NormalRules = this.NormalRules.Where(nr => ComparableFields.isBaseContained(input, nr)).ToList();
		}


		// filter rules for model
		private void PreFilterModel(InputParams input)
		{
			this.NormalRules = this.NormalRules.Where(nr => ComparableFields.isModelContained(input, nr)).ToList();
		}


		// creates the xml file from the database
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static FeatureRegister CreateDBFiles(string DBorigin, string OutXmlPath, string LogXml2DB)
		{
			Provider.WriteLogFormat(LogXml2DB, "Write DB xml Files");
			var db = FeaturesContext.Open(DBorigin);
			var AllRules = db.NormalRule.ToList();

			Provider.WriteLogFormat(LogXml2DB, "Create Country Items and Register");
			var register = FeatureRegister.CreateDBFiles(db, OutXmlPath);
			Provider.WriteFile<Dbconfiguration>(db.Dbconfiguration.First(), OutXmlPath + "\\" + FeatureManagerCompatibility.ConfigurationFileName);
			Provider.WriteFile<CountryItems>(register.CountryItems, OutXmlPath + "\\CountryItems.xml");

			// File used in WPF only
			CreatePartNumbersFile(db, OutXmlPath);
			CreateProbePresetFile(db, OutXmlPath);

			Provider.WriteLogFormat(LogXml2DB, "Create Feature Main for all logical models");
			var ModelNames = register.ModelLicenses.Select(m => m.License.Name).ToList();
			foreach (var ModelName in ModelNames)
			{
				WriteMain(register, AllRules, ModelName, OutXmlPath);
			}
			return register;
		}

		// ANTO DB
		public static void CreatePartNumbersFile(FeaturesContext db, String OutXmlPath)
		{
			var PNInfo = new PartNumberInfo
			{
				Associations = db.PartNumbersAssociations.ToList()
			};
			Provider.WriteFile<PartNumberInfo>(PNInfo, OutXmlPath + "\\" + PartNumberInfo.PartNumberFileName);
		}

		public static void CreateProbePresetFile(FeaturesContext db,  String OutXmlPath)
		{
			var ProbePresets = new ProbePresetInfo()
			{
				Items = db.ProbePreset.ToList()
			};
			Provider.WriteFile<ProbePresetInfo>(ProbePresets, OutXmlPath + "\\" + ProbePresetInfo.ProbePresetFileName);
		}

		private static void WriteMain(FeatureRegister register, List<NormalRule> AllRules, string ModelName, string OutXmlPath)
		{
			FeatureMain main = new FeatureMain(register);
			main.FillFromDB(AllRules, ModelName);
			WriteFile(main, OutXmlPath, ModelName);
		}


		// fill rules and squeeze them by model
		public void FillFromDB(List<NormalRule> Nrules, string modelName)
		{
			NormalRules = Nrules.Select(n => (NormalRule)n.Clone()).ToList();      // make a new list
			if (!string.IsNullOrEmpty(modelName))
			{
				SqueezeModel(modelName);
			}
		}

		public void SqueezeNormalRules(IInternalLicenseFilter context, out string Version2Display)
		{
			SqueezeNormalRules(context, null, out Version2Display);
		}


		public void SqueezeNormalRules(IInternalLicenseFilter context, string BuildNumber, out string Version2Display)
		{
			var input = new InputParams();
			FillBaseParameters(context, input);
			PreFilterRules(input);
			Version2Display = GetVersion2Display(input.Version, BuildNumber);
		}

		public string GetVersion2Display(int? InputVersion)
		{
			return GetVersion2Display(InputVersion, null);
		}


		public string GetVersion2Display(int? InputVersion, string BuildNumber)
		{
			if (InputVersion == null)
			{
				return "";
			}

			MinorVersionAssociation l_MinoVersionAssociation = null;
			var major = ComparableFields.GetMajorFromNumericVersion((int)InputVersion);
			int? middle = 0;
			int? least = 0;

			if (!string.IsNullOrEmpty(BuildNumber))
			{
				l_MinoVersionAssociation = DBRegister.MinorVersionAssociations.SingleOrDefault(m => m.Major == major && m.BuildVersion == BuildNumber);
			}

			if (l_MinoVersionAssociation == null)
			{
				l_MinoVersionAssociation = DBRegister.MinorVersionAssociations.SingleOrDefault(m => m.Major == major);
			}

			if (l_MinoVersionAssociation != null)
			{
				middle = l_MinoVersionAssociation.Minor;
				least = l_MinoVersionAssociation.Patch;     // Gibiino [RTC 10963] patch should be inserted with an input file
			}

			return ComparableFields.BuildStringVersion(major, middle, least);
		}

		// filter is supposed to have user, logicalmodel and countryname filled
		public void FillBaseParameters(IInternalLicenseFilter filter, InputParams input)
		{
			input.UserLevel = filter.UserLevel;
			input.LogicalModelId = DBRegister.LogicalModelDictionary[filter.LogicalModelName];
			input.CountryId = DBRegister.CountryDictionary[filter.CountryName];
			input.DistributorId = Provider.GetValueOrDefault_Stuct(DBRegister.DistributorDictionary, filter.CountryName);  // Distributor might be null

			FillVersion(input, (int)input.CountryId, input.DistributorId, (int)input.LogicalModelId, DBRegister);
		}

		public static void FillVersion(InputParams input, int countryId, int? DistributorId, int ModelId, FeatureRegister DBRegister)
		{
			var versionAssociation = VersionAssociationCreator.CreateVersionAssociation(countryId, DistributorId, ModelId, DBRegister);
			input.Version = GetVersion(versionAssociation, DBRegister);
		}


		public static int? GetVersion(IVersionAssociationInput VersionAssociation, FeatureRegister DBRegister)
		{
			bool isnull;
			// filter country/certifier
			var ValidCountryVersions = VersionAssociation.GetVersionList(DBRegister).Where(x => x.GeographyId == VersionAssociation.GeographyId).ToList();

			// filter logical model
			var CountryModelVersionList = MatchValuesOrNull(ValidCountryVersions, VersionAssociation, (y) => y.LogicalModelId, out isnull);

			// filter distributor
			var CountryDistrModelVersionList = MatchValuesOrNull(CountryModelVersionList, VersionAssociation, (y) => y.DistributorId, out isnull);

			var ValidCV = CountryDistrModelVersionList.FirstOrDefault();
			if (ValidCV != null)
			{
				return ComparableFields.GetNumericVersionFromMajor(ValidCV.MajorVersion);
			}
			return null;
		}


		// if the fields is specified it will take all records with its value, otherwise all records with that field equal to null
		// WARNING: IF YOU SPECIFY VERSION LIKE THIS {C1 - NULL - M1} AND {C1 - D1 - NULL} IT WILL RETURN NULL!
		private static List<IVersionAssociation> MatchValuesOrNull(List<IVersionAssociation> InitialList, IVersionAssociationBox Input,
				Func < IVersionAssociationBox, int? > GetField, out bool isnull)
		{
			var result = InitialList.Where(x => GetField(x) == GetField(Input)).ToList();
			isnull = result.Count == 0;
			if (isnull)
			{
				result = InitialList.Where(x => GetField(x) == null).ToList();
			}
			return result;
		}

		private void SqueezeModel(string ModelName)
		{
			if (!string.IsNullOrEmpty(ModelName))
			{
				var input = new InputParams();
				input.LogicalModelId = DBRegister.LogicalModelDictionary[ModelName];
				PreFilterModel(input);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal static List<NormalRule> SqueezeModel(LogicalModel Model, List<NormalRule> nrules)
		{
			var result = new List<NormalRule>();
			var input = new InputParams
			{
				LogicalModelId = Model.Id
			};
			result.AddRange(nrules.Where(nr => ComparableFields.isModelContained(input, nr)).ToList());
			return result;
		}

		// DB Delivery: Import zip file and check its consistency. Starting estension must be .esadb
		public static DeliveryResult ImportFromZip(string zipPath, string extractPath, string destinationPath)
		{
			if (string.IsNullOrEmpty(extractPath) || !Directory.Exists(extractPath))
			{
				Directory.CreateDirectory(extractPath);
			}

			try
			{
				var zipFileName = extractPath + "\\FeatureDB.zip";
				File.Copy(zipPath, zipFileName);
				ZipFile.ExtractToDirectory(zipPath, extractPath);
				File.Delete(zipFileName);
			}
			catch (Exception)
			{
				CleanUpTemp(extractPath);
				return DeliveryResult.UnzipUnsuccessful;
			}


			var CheckResult = IsDBValid(destinationPath, extractPath);
			if (CheckResult == DeliveryResult.Ok)
			{
				Provider.DirectoryUtilities.DeleteDirectory2(destinationPath);
				Provider.DirectoryUtilities.DirectoryCopy(extractPath, destinationPath, true);
			}
			CleanUpTemp(extractPath);
			return CheckResult;
		}

		private static DeliveryResult IsDBValid(string destinationPath, string extractPath)
		{
			Dbconfiguration NewConfig = null;
			Dbconfiguration OldConfig = null;
			if (!CheckConfigurations(ref NewConfig, ref OldConfig, destinationPath, extractPath))
			{
				return DeliveryResult.InvalidFiles;
			}

			if (!CheckCompatibility(NewConfig, extractPath))
			{
				return DeliveryResult.InvalidCompatibility;
			}

			if (!CheckVersion(NewConfig, OldConfig, extractPath))
			{
				return DeliveryResult.InvalidVersion;
			}

			if (!CheckFiles(extractPath))
			{
				return DeliveryResult.InvalidFiles;
			}

			return DeliveryResult.Ok;
		}

		private static Boolean CheckConfigurations(ref Dbconfiguration NewConfig, ref Dbconfiguration OldConfig, string destinationPath, string extractPath)
		{
			try
			{
				NewConfig = Provider.LoadFromFile<Dbconfiguration>(extractPath + "\\" + FeatureManagerCompatibility.ConfigurationFileName);
				OldConfig = Provider.LoadFromFile<Dbconfiguration>(destinationPath + "\\" + FeatureManagerCompatibility.ConfigurationFileName);
				if (NewConfig == null || OldConfig == null)
				{
					throw new Exception("Loaded null configurations");
				}
			}
			catch (Exception ex)
			{
				var message = "Input Database cannote be porperly loaded: " + ex.Message;
				Catcher(message, extractPath);
				return false;
			}
			return true;
		}

		private static Boolean CheckFiles(string extractPath)
		{
			try
			{
				var FRegister = FeatureRegister.LoadFromFile(extractPath);
				var FMain = FeatureMain.LoadAllRules(extractPath, FRegister);
			}
			catch (Exception ex)
			{
				var message = "Input Database cannote be porperly loaded: " + ex.Message;
				Catcher(message, extractPath);
				return false;
			}
			return true;
		}

		private static Boolean CheckCompatibility(Dbconfiguration NewConfig, string extractPath)
		{
			if (NewConfig.Compatibility != FeatureManagerCompatibility.Compatibility)
			{
				var message = "Input Database is not compatible with current software: current software compatiblity = " + FeatureManagerCompatibility.Compatibility
							  + ", Database compatibility = " + NewConfig.Compatibility + ". Database cannot be loaded.";
				Catcher(message, extractPath);
				return false;
			}
			return true;
		}

		private static Boolean CheckVersion(Dbconfiguration NewConfig, Dbconfiguration OldConfig, string extractPath)
		{
			if (NewConfig.Version <= OldConfig.Version)
			{
				var message = "Downgrading database version: current version = " + OldConfig.Version + ", new version = " + NewConfig.Version;
				Catcher(message, extractPath);
				return false;
			}
			return true;
		}

		private static void Catcher(string message, string extractPath)
		{
			//Logger.LogTrace("Feature Manager", Enums.Flow_Type.Configuration, Enums.LogLevel.LogNormal, message);
			CleanUpTemp(extractPath);
		}

		private static void CleanUpTemp(string extractPath)
		{
			if (Directory.Exists(extractPath))
			{
				Provider.DirectoryUtilities.DeleteDirectory2(extractPath);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static void CreateDatabase(Paths Paths, string logFileXml2DB, int BuildNumber, DatabaseType databaseType, string connectionString)
		{
			try
			{
				FeaturesContext.Create(Paths.DBfile, databaseType, connectionString);
				var db = FeaturesContext.Open(Paths.DBfile);
				// ANTO DB LOG
				//db.EnableLog();
				var buffer = new DBBuffer();

				/* data insertion in the DB */
				Provider.WriteLogFormat(logFileXml2DB, "Filling up Tables");

				InsertSWPacks(db, buffer, Paths, logFileXml2DB);
				InsertSettingsFamilies(db, buffer, Paths, logFileXml2DB);
				var dictLicenses = InsertLicenses(db, buffer, Paths, logFileXml2DB);
				var dictPhysMod = InsertPhysicalModels(db, buffer, Paths, logFileXml2DB);
				InsertLogicalModels(db, buffer, dictPhysMod, dictLicenses, Paths, logFileXml2DB);
				InsertTwinLicenses(db, dictLicenses, Paths, logFileXml2DB);
				var dictFeat = InsertFeatures(db, buffer, dictLicenses, Paths, logFileXml2DB);
				var dictOptions = InsertOptions(db, buffer, dictFeat, Paths, logFileXml2DB);
				InsertApplications(db, buffer, dictFeat, Paths, logFileXml2DB);
				InsertBundles(db, buffer, dictFeat, Paths, logFileXml2DB);
				InsertFeatureRelations(db, buffer, dictFeat, Paths, logFileXml2DB);
				InsertLicenseRelations(db, buffer, Paths, logFileXml2DB);
				InsertLicenseRelationsExceptions(db, buffer, Paths, logFileXml2DB);
				InsertDeprecated(db, buffer, dictFeat, Paths, logFileXml2DB);
				InsertCertifiers(db, buffer, Paths, logFileXml2DB);
				InsertCountries(db, buffer, Paths, logFileXml2DB);
				InsertDistributors(db, buffer, Paths, logFileXml2DB);
				InsertCountryDistributors(db, buffer, Paths, logFileXml2DB);
				InsertCountryLicenses(db, buffer, Paths, logFileXml2DB);
				InsertMinorVersions(db, buffer, Paths, logFileXml2DB);

				ImportRules(db, buffer, dictLicenses, Paths, logFileXml2DB);

				InsertPartNumbers(db, buffer, Paths, logFileXml2DB);
				ImportVersions(db, buffer, Paths, logFileXml2DB);

				buffer.p_UIRules = db.Uirule.ToList();

				Provider.WriteLogFormat(logFileXml2DB, "Convert UIRules to NormalRules");
				db.BulkInsert(FeaturesContext.createNormalRules(buffer));
				SetDBConfigurations(db, BuildNumber);
				Provider.WriteLogFormat(logFileXml2DB, "Database Created");
			}
			catch (Exception ex)
			{
				var errormessage = Provider.WriteErrorLogFormat("Error while creating the Database: " + ex.Message);
				throw new ApplicationException(errormessage, ex);
			}
		}

		#region Table Insertions

		private static void SetDBConfigurations(FeaturesContext db, int BuildNumber)
		{
			db.BulkInsert(new List<Dbconfiguration>()
			{
				new Dbconfiguration(_Compatibility: FeatureManagerCompatibility.Compatibility, _Version: BuildNumber)
			});
		}

		private static void ImportRules(FeaturesContext db, DBBuffer buffer, Dictionary<string, int> dictLicenses, Paths Paths, string LogXml2DB)
		{
			db.readLogModLicRules(buffer, dictLicenses, Paths, LogXml2DB);         // Rules from license manager
			db.ImportProbesWithRules(buffer, Paths, LogXml2DB);                    // import Probes and probe rules

			db.BulkInsert(Uirule.ImportOptionProbes(buffer, Paths, LogXml2DB));       // probe option associations
			db.BulkInsert(Uirule.ImportVersionRules(buffer, Paths, LogXml2DB));       // versions Rules
			ImportRegulatoryFeatures(db, buffer, Paths, LogXml2DB);

			db.BulkInsert(Uirule.ImportRegulatoryRules(buffer, Paths, LogXml2DB));    // Regulatory blocks for country
			db.BulkInsert(Uirule.ImportRDBlockingRules(buffer, Paths, LogXml2DB));        // R&D blocks for country
		}

		private static void InsertSWPacks(FeaturesContext db, DBBuffer buffer, Paths Paths, string logFileXml2DB)
		{
			db.BulkInsert(Swpack.Import(Paths, logFileXml2DB));
			buffer.p_swPacks = db.Swpack.ToList();
		}

		private static void InsertSettingsFamilies(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(SettingFamily.Import(buffer.p_swPacks, Paths, log));
			buffer.p_settingsFamilies = db.SettingFamily.ToList();
		}

		private static Dictionary<string, int> InsertLicenses(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(License.Import(Paths, log));
			buffer.p_licenses = db.License.ToList();
			var dictLicenses = buffer.p_licenses.Select(pm => new
			{
				pm.Name,
				pm.Id
			}).ToDictionary(m => m.Name, m => m.Id);
			return dictLicenses;
		}

		private static Dictionary<string, int> InsertPhysicalModels(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(Models.PhysicalModel.Import(Paths, log));
			buffer.p_physicalModels = db.PhysicalModel.ToList();
			var dictPhysMod = buffer.p_physicalModels.Select(pm => new
			{
				pm.Description,
				pm.Id
			}).ToDictionary(m => m.Description, m => m.Id);
			return dictPhysMod;
		}

		private static void InsertLogicalModels(FeaturesContext db, DBBuffer buffer, Dictionary<string, int> dictPhysMod, Dictionary<string, int> dictLicenses,
												Paths Paths, string log)
		{
			db.BulkInsert(LogicalModel.Import(dictPhysMod, dictLicenses, buffer, Paths, log));
			buffer.p_logicalModels = db.LogicalModel.ToList();
			buffer.ModelLicenses = buffer.p_logicalModels.Join(buffer.p_licenses, m => m.LicenseId, l => l.Id, (m, l) => new ModelLicense(m, l)).ToList();
		}

		private static void InsertTwinLicenses(FeaturesContext db, Dictionary<string, int> dictLicenses, Paths Paths, string log)
		{
			db.BulkInsert(TwinLicenses.Import(dictLicenses, Paths, log));
		}

		private static Dictionary<string, int> InsertFeatures(FeaturesContext db, DBBuffer buffer, Dictionary<string, int> dictLicenses, Paths Paths, string log)
		{
			db.BulkInsert(Feature.Import(dictLicenses, Paths, log));
			buffer.p_features = db.Feature.ToList();
			var dictFeat = buffer.p_features.Select(pm => new
			{
				pm.Name,
				pm.Id
			}).ToDictionary(m => m.Name, m => m.Id);
			return dictFeat;
		}

		private static Dictionary<string, int> InsertOptions(FeaturesContext db, DBBuffer buffer, Dictionary<string, int> dictFeat, Paths Paths, string log)
		{
			db.BulkInsert(Option.Import(dictFeat, Paths, log));
			buffer.p_option = db.Option.ToList();
			var dictOptions = buffer.p_option.Select(pm => new
			{
				pm.Name,
				pm.Id
			}).ToDictionary(m => m.Name, m => m.Id);
			return dictOptions;
		}

		private static void InsertApplications(FeaturesContext db, DBBuffer buffer, Dictionary<string, int> dictFeat, Paths Paths, string log)
		{
			db.BulkInsert(Application.Import(dictFeat, Paths, log));
			buffer.p_applications = db.Application.ToList();
		}

		private static void InsertBundles(FeaturesContext db, DBBuffer buffer, Dictionary<string, int> dictFeat, Paths Paths, string log)
		{
			db.BulkInsert(Bundle.Import(dictFeat, Paths, log));
			buffer.p_bundles = db.Bundle.ToList();
		}

		private static void InsertFeatureRelations(FeaturesContext db, DBBuffer buffer, Dictionary<string, int> dictFeat, Paths Paths, string log)
		{
			db.BulkInsert(FeatureRelation.Import(dictFeat, Paths, log, buffer));
		}

		private static void InsertLicenseRelations(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(LicenseRelation.Import(buffer, Paths, log));
			buffer.p_LicenseRelation = db.LicenseRelation.ToList();
		}

		private static void InsertLicenseRelationsExceptions(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(LicenseRelationException.Import(buffer, Paths, log));
			buffer.p_LicenseRelationExceptions = db.LicenseRelationException.ToList();
		}

		private static void InsertDeprecated(FeaturesContext db, DBBuffer buffer, Dictionary<string, int> dictFeat, Paths Paths, string log)
		{
			db.BulkInsert(Deprecated.Import(dictFeat, Paths, log));
			buffer.p_Deprecated = db.Deprecated.ToList();
		}


		private static void InsertCertifiers(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(Certifier.Import(Paths, log));
			buffer.p_certifiers = db.Certifier.ToList();
		}

		private static void InsertCountries(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(Country.Import(buffer, Paths, log));
			buffer.p_countries = db.Country.ToList();
		}

		private static void InsertDistributors(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(Distributor.Import(Paths, log));
			buffer.p_Distributors = db.Distributor.ToList();
		}

		private static void InsertCountryDistributors(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(CountryDistributor.Import(buffer, Paths, log));
			buffer.p_Country_Distributors = db.CountryDistributor.ToList();
		}

		private static void InsertMinorVersions(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(MinorVersionAssociation.Import(Paths, log));
			buffer.p_MinorVersionAssociation = db.MinorVersionAssociation.ToList();
		}

		private static void InsertPartNumbers(FeaturesContext db, DBBuffer buffer,  Paths Paths, string log)
		{
			db.BulkInsert(PartNumbersAssociations.Import(Paths, buffer, log));
			buffer.p_PartNumbersAssociations = db.PartNumbersAssociations.ToList();
		}

		private static void ImportVersions(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.ReadVersion(buffer, Paths, log);
		}

		private static void InsertCountryLicenses(FeaturesContext db, DBBuffer buffer, Paths Paths, string log)
		{
			db.BulkInsert(CountryLicense.Import(buffer, Paths, log));
			buffer.p_CountryLicenses = db.CountryLicense.ToList();
		}

		private static void ImportRegulatoryFeatures(FeaturesContext db, DBBuffer buffer, Paths Paths, string LogXml2DB)
		{
			db.BulkInsert(RegulatoryFeature.Import(buffer, Paths, LogXml2DB));
			buffer.p_RegualtoryFeatures = db.RegulatoryFeature.ToList();
		}

		#endregion

	}
}
