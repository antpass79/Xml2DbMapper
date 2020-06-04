using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

namespace Xml2DbMapper.Core.Models
{
	public partial class FeaturesContext
	{
		static FeaturesContext _context;

		public static void Create(String p_DatabasePath)
		{
			// ANTO DB LIFECYCLE
			//GC.TryStartNoGCRegion(1000 * 10000);
			//SqlServerCeUtil<DatabaseTemplate>.ResetDatabase(p_DatabasePath, c => new DatabaseTemplate(c));
			//Upgrade(p_DatabasePath);
			var options = new DbContextOptionsBuilder<FeaturesContext>()
				.UseSqlite($"Data Source={p_DatabasePath}")
				.Options;

			_context = new FeaturesContext(options);
			_context.Database.EnsureDeleted();
			_context.Database.EnsureCreated();

			//GC.EndNoGCRegion();
		}

		public static FeaturesContext Open(String p_DatabasePath)
		{
			// ANTO DB LIFECYCLE
			//return SqlServerCeUtil<Database>.OpenDatabase(p_DatabasePath, c => new Database(c));
			return _context;
		}


		/// Generates the normalized rules. the Macro rules must be executed as last
		public static List<NormalRule> createNormalRules(DBBuffer _buffer)
		{
			List<NormalRule> lstNR = new List<NormalRule>();
			foreach (var br in _buffer.p_UIRules)
			{
				lstNR.AddRange(br.ToNormalRule(_buffer));
			}

			return lstNR;
		}

		// imports probe and probes rules from all xml files
		public void ImportProbesWithRules(DBBuffer _buffer, Paths Paths, string LogXml2DB)
		{
			_buffer.probeBindLst = Models.Probe.Import(_buffer, Paths, LogXml2DB);
			this.BulkInsert(_buffer.probeBindLst.Select(pb => pb.probe).ToList());
			_buffer.p_probes = this.Probe.ToList();

			// Probe Users rules, Probe_SettingsFamily associations
			var AllRules = ReadProbeUserRelations(_buffer, LogXml2DB);

			// import probe information and file names
			AllRules.AddRange(Models.ProbePreset.ImportProbeAppRules(this, _buffer, Paths, LogXml2DB));

			// import rules from presets
			var presetRules = ReadPresetRules(_buffer, Paths, LogXml2DB);
			this.BulkInsert(presetRules);

			AllRules.AddRange(presetRules);
			//var fixingrules = FixOptionProbeRules(AllRules, _buffer, LogXml2DB);
			//BulkInsert(fixingrules);
			//SubmitChanges();
			//Provider.WriteLogFormat(LogXml2DB, fixingrules.Count + " Fixing rules added");
			Provider.WriteLogFormat(LogXml2DB, "Rules Imported");
		}

		// from the input list it creates the correspondant UIRule
		public List<Uirule> createProbeAppRules(List<ProbePreset> ProbeApps, DBBuffer _buffer)
		{
			List<Uirule> result = new List<Uirule>();
			var ParentAppsIDs = _buffer.p_bundles.Join(_buffer.p_applications, b => b.ParentFeatureId, a => a.FeatureId, (b, a) => a.Id).ToList().Distinct().ToList();
			// in the list each couple probe-app is replicated for each present preset
			var groups = ProbeApps.GroupBy(x => new
			{
				probeId = x.ProbeId,
				settingId = x.SettingsFamilyId
			});
			foreach (var group in groups)
			{
				var logModelsGroups = getLogicalModels(group.Key.settingId, _buffer).GroupBy(m => m.Type).ToList();
				var ValidAppIds = group.Select(x => x.ApplicationId).Distinct().ToList();
				foreach (var logModels in logModelsGroups)
				{
					// Add rule only if the application type and model type are coherent (human/vet)
					var ValidApplications = ValidAppIds.Join(_buffer.p_applications, x => x, a => a.Id, (x, a) => a)
											.Where(a => a.AppType == logModels.Key).ToList();

					var ForbiddenApplications = _buffer.p_applications.Where(a => !ValidAppIds.Contains(a.Id) && !ParentAppsIDs.Contains(a.Id)).ToList();
					foreach (var model in logModels)
					{
						foreach (var application in ValidApplications)
						{
							var frule = BaseAppProbeRule(application.Id, group.Key.probeId, model.Id);
							frule.Allow = AllowModes.Def;
							result.Add(frule);
						}

						foreach (var ForbApp in ForbiddenApplications)
						{
							var frule = BaseAppProbeRule(ForbApp.Id, group.Key.probeId, model.Id);
							frule.Allow = AllowModes.No;
							result.Add(frule);
						}
					}
				}
			}
			this.BulkInsert(result);
			return result;
		}


		// checks if the listed applications are containing the parents and if so the parent is added
		//private void AddParents(List<Application> appList, DBBuffer _buffer)
		//{
		//  var ParentIds = appList.Join(_buffer.p_bundles, a => a.FeatureId, b => b.FeatureId, (a, b) => b.ParentFeatureId).Distinct().ToList();
		//  List<Application> parents = new List<Application>();
		//  foreach (var parentId in ParentIds)
		//  {
		//      var AllChildren = _buffer.p_bundles.Where(b => b.ParentFeatureId == parentId).Select(b => b.FeatureId).ToList();
		//      if (appList.Select(a => a.FeatureId).ToList().Intersect(AllChildren).Count() == AllChildren.Count)
		//      {
		//          parents.Add(_buffer.p_applications.Single(x => x.FeatureId == parentId));
		//      }
		//  }
		//  appList.AddRange(parents);
		//}

		private Uirule BaseAppProbeRule(int appId, int probeId, int modelId)
		{
			return new Uirule
			{
				RuleOrigin = ruleOrigins.ProbeXml,
				ApplicationId = appId,
				ProbeId = probeId,
				LogicalModelId = modelId,
			};
		}

		public static List<LogicalModel> getLogicalModels(Int32 _SettingsFamilyId, DBBuffer _buffer)
		{
			return _buffer.p_logicalModels.Where(l => l.SettingsFamilyId == _SettingsFamilyId).ToList();
		}

		/// Add user interface rules of association for Probe - User
		public List<Uirule> ReadProbeUserRelations(DBBuffer _buffer, string LogXml2DB)
		{
			Provider.WriteLogFormat(LogXml2DB, "Read Probe User Relations");
			List<Uirule> resultRules = new List<Uirule>();
			var resultProbeSettings = new List<ProbeSettingsFamily>();
			string ProbeName = "";
			Probe l_Probe;
			var vnavfeature = _buffer.p_features.Single(o => o.NameInCode == "VirtualNavBundle");
			var vnavoptionid = _buffer.p_option.Single(o => o.FeatureId == vnavfeature.Id).Id;
			try
			{
				foreach (ProbeBind probeBind in _buffer.probeBindLst)
				{
					// probeLst does not contain the Id
					l_Probe = _buffer.p_probes.Single(p => p.SaleName == probeBind.probe.SaleName);
					ProbeName = probeBind.probe.SaleName;
					// loop settings families
					foreach (var probeInfo in probeBind.probeInfoLst)
					{
						var probeSetting = new ProbeSettingsFamily(l_Probe.Id, probeInfo);
						resultProbeSettings.Add(probeSetting);

						var LogPhysModels = getLogicalModels(probeInfo.settingsFam.Id, _buffer).Join(_buffer.p_physicalModels,
											l => l.PhModId, p => p.Id, (l, p) => new LogPhysModel(l, p));
						var ResultSettingsFamily = new List<Uirule>();
						foreach (var model in LogPhysModels)
						{
							resultRules.Add(ProbeUserRule(l_Probe, model, UserLevel.WIP, probeInfo.SupportedKeys.fHwKeyWIP, probeInfo));
							resultRules.Add(ProbeUserRule(l_Probe, model, UserLevel.Test, probeInfo.SupportedKeys.fHwKeyTesting, probeInfo));
							resultRules.Add(ProbeUserRule(l_Probe, model, UserLevel.Standard, probeInfo.SupportedKeys.fHwKeyProduction, probeInfo));
							resultRules.Add(ProbeUserRule(l_Probe, model, UserLevel.Service, probeInfo.SupportedKeys.fHwKeyService, probeInfo));
							resultRules.Add(ProbeUserRule(l_Probe, model, UserLevel.RS, probeInfo.SupportedKeys.fHwKeyRSDept, probeInfo));

							// Virtual navigator rules Gibiino [RTC 14690]
							resultRules.Add(VNavRules(l_Probe, model.LogModel, probeInfo.IsVNAVEnabled, vnavoptionid));
						}
					}

					// exclude probes not belongig to the missing probe families
					resultRules.AddRange(ExcludeProbesFromFamily(probeBind, l_Probe.Id, _buffer));
				}
			}
			catch (Exception ex)
			{
				throw new ParsingException("Probe-User Relation for probe " + ProbeName, ex);
			}
			this.BulkInsert(resultRules);
			this.BulkInsert(resultProbeSettings);
			return resultRules;
		}

		private static Uirule VNavRules(Probe probe, LogicalModel model, bool IsVNAVEnabled, int vnavoptionid)
		{
			return new Uirule
			{
				RuleOrigin = ruleOrigins.ProbeXml,
				ProbeId = probe.Id,
				LogicalModelId = model.Id,
				OptionId = vnavoptionid,
				Allow = IsVNAVEnabled ? AllowModes.Def : AllowModes.No
			};
		}

		private class LogPhysModel
		{
			public LogicalModel LogModel;
			public PhysicalModel PhyModel;
			public LogPhysModel(LogicalModel _LogModel, PhysicalModel _PhyModel)
			{
				LogModel = _LogModel;
				PhyModel = _PhyModel;
			}
		}

		// if a probe does not appear in the probelist of a settings family it should be forbidden for the related models
		static List<Uirule> ExcludeProbesFromFamily(ProbeBind probeBind, int l_ProbeId, DBBuffer _buffer)
		{
			var resultRules = new List<Uirule>();
			var families4Probe = probeBind.probeInfoLst.Select(x => x.settingsFam.Id);
			var missingFamilies = _buffer.p_settingsFamilies.Select(s => s.Id).Except(families4Probe).ToList();
			foreach (var missingFamily in missingFamilies)
			{
				var logModels = getLogicalModels(missingFamily, _buffer);
				foreach (var model in logModels)
				{
					resultRules.Add(ProbeModelRule(l_ProbeId, model.Id));
				}
			}

			var NoFamilyModels = _buffer.p_logicalModels.Where(m => m.SettingsFamilyId == null).ToList();       // e.g. Desktop
			foreach (var NFmodel in NoFamilyModels)
			{
				resultRules.Add(ProbeModelRule(l_ProbeId, NFmodel.Id));
			}

			return resultRules;
		}

		private static Uirule ProbeModelRule(int probeId, int modelId)
		{
			return new Uirule
			{
				RuleOrigin = ruleOrigins.ProbeXml,
				Allow = AllowModes.No,
				ProbeId = probeId,
				LogicalModelId = modelId
			};
		}


		// create probe-user-logicalModel rules
		private static Uirule ProbeUserRule(Probe probe, LogPhysModel model, UserLevel userLevel, bool tagValue, probeInfo l_probeInfo)
		{
			var rl = new Uirule
			{
				RuleOrigin = ruleOrigins.ProbeXml,
				ProbeId = probe.Id,
				LogicalModelId = model.LogModel.Id,
				Allow = tagValue ? AllowModes.Def : AllowModes.No,
				UserLevel = userLevel
			};

			// Gibiino [RTC 10968]
			// if the model and the probe connector are matching for large or small connector, the block should not apply
			if (rl.UserLevel == UserLevel.Standard && rl.Allow == AllowModes.Def &&
					!((model.PhyModel.SmallConnectorSupport && l_probeInfo.ProbeStringCode != null) || (model.PhyModel.LargeConnectorSupport && l_probeInfo.wHwCode != "0")))
			{
				rl.RuleOrigin = ruleOrigins.ProbeConnectorSource;
				rl.Allow = AllowModes.No;
			}

			return rl;
		}

		/// imports the Biopsy Kits and relative rules associating to the probes
		private void readKits(SettingsInfo settings, XDocument datafile)
		{
			List<BiopsyKits> kits = datafile.Descendants("KitName").Select(x => x.Element("stringa").Value).Where(n => n != "NULL").Select(x => new BiopsyKits { Name = x }).ToList();
			if (kits.Count > 0)
			{
				// ANTO DB ToList for bulk
				var items = kits
					.Where(k => !BiopsyKits
						.Select(x => x.Name)
						.AsEnumerable()
						.Contains(k.Name, StringComparer.OrdinalIgnoreCase))
					.DistinctBy(item => item.Name, StringComparer.OrdinalIgnoreCase);

				this.BulkInsert(items.ToList());

				var kitItems = BiopsyKits
					.ToList()
					.Where(b => kits
						.Select(x => x.Name)
						.Contains(b.Name, StringComparer.OrdinalIgnoreCase))
					.DistinctBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();

				settings.probe_Kits.Single(pk => pk.probe.Id == settings.currentProbe.Id && pk.settingsFamilyIdl == settings.currentProbeSettings.SettingsFamilyId)
				.kits.AddRange(kitItems);
			}
		}

		private class ProbeDataset
		{
			public Probe currentProbe;
			public XDocument dataFile;
			public String focPath;
			public ProbeSettingsFamily currentProbeSettings;
			public ProbeType currentTransducerType;
			public ProbeDataset(SettingsInfo settingsInfo, XDocument _dataFile, ProbeType _currentTransducerType, Paths Paths)
			{
				currentProbe = settingsInfo.currentProbe;
				dataFile = _dataFile;
				focPath = Paths.focFilePath + "\\" + settingsInfo.swPackName;
				currentProbeSettings = settingsInfo.currentProbeSettings;
				currentTransducerType = _currentTransducerType;
			}
		}
		private class PresetData
		{
			public ProbeDataset prbData;
			public List<ProbePreset> appSet;
			public XDocument defaultFile;
			public XDocument ProbeDefaultAllFile;
			public DBBuffer buffer;

			public PresetData(ProbeDataset _prbData, List<ProbePreset> _appSet, XDocument _defaultFile, XDocument _ProbeDefaultAllFile, DBBuffer _buffer)
			{
				prbData = _prbData;
				appSet = _appSet;
				defaultFile = _defaultFile;
				buffer = _buffer;
				ProbeDefaultAllFile = _ProbeDefaultAllFile;

			}
		}

		private class Probe_Kits
		{
			public Probe_Kits(Probe prb, int sf)
			{
				probe = prb;
				settingsFamilyIdl = sf;
			}

			public Probe probe;
			public int settingsFamilyIdl;
			public List<BiopsyKits> kits = new List<BiopsyKits>();
		}

		private class SettingsInfo
		{
			public IGrouping<int, ProbePreset> grpProbe;
			public String swPackName;
			public Int32 settingsId;
			public DBBuffer _buffer;
			public Dictionaries dicts;
			public Probe currentProbe;
			public ProbeSettingsFamily currentProbeSettings;
			public List<Probe_Kits> probe_Kits = new List<Probe_Kits>();
			public Dictionary<string, XDocument> AllProbeDefaultDict = new Dictionary<string, XDocument>();
			public SettingsInfo(IGrouping<int, ProbePreset> _grpProbe, string _swpackname, Int32 _settingsId, DBBuffer __buffer, Dictionaries _dicts,
								List<Probe_Kits> _probe_Kits, Dictionary<string, XDocument> _alldefdic)
			{
				grpProbe = _grpProbe;
				swPackName = _swpackname;
				settingsId = _settingsId;
				_buffer = __buffer;
				dicts = _dicts;
				currentProbe = _buffer.p_probes.Single(p => p.Id == grpProbe.FirstOrDefault().ProbeId);
				currentProbeSettings = _buffer.p_probe_SettingsFamily.Single(x => x.ProbeId == currentProbe.Id
									   && x.SettingsFamilyId == settingsId);
				probe_Kits = _probe_Kits;
				AllProbeDefaultDict = _alldefdic;
			}
		}

		private List<Probe_Kits> initProbeKits(DBBuffer _buffer)
		{
			var result = new List<Probe_Kits>();
			foreach (var probeBind in _buffer.probeBindLst)
			{
				foreach (var inforList in probeBind.probeInfoLst)
				{
					result.Add(new Probe_Kits(_buffer.p_probes.Single(p => p.SaleName == probeBind.probe.SaleName), inforList.settingsFam.Id));
				}
			}
			return result;
		}

		private bool[,] OptionProbes, OptionApplications;   // memory of the associations
		private bool[] IsValidOption, IsValidApplication;   // memory of the options and probes really interested in the import
		private List<ProbeTransducers> p_probeTransducers = new List<ProbeTransducers>();
		/// Add user interface rules of association for Probe - Option
		public List<Uirule> ReadPresetRules(DBBuffer _buffer, Paths Paths, string LogXml2DB)
		{
			Provider.WriteLogFormat(LogXml2DB, "Read Preset Rules");
			var result = new List<Uirule>();
			Dictionaries dicts = new Dictionaries(_buffer);
			dicts.fillDicts();
			var l_probeKits = initProbeKits(_buffer);

			// ANTO FIX
			//var SettingsFamilies = ProbePreset.GroupBy(x => x.SettingsFamilyId).ToList();
			var SettingsFamilies = ProbePreset.AsEnumerable().GroupBy(x => x.SettingsFamilyId).ToList();

			int SettingsId = -2;
			string probeName = "";
			try
			{
				foreach (var settingsFam in SettingsFamilies)           // loop settings families
				{
					OptionProbes = new bool[_buffer.p_option.Count + 1, _buffer.p_probes.Count + 1];   // first row and column are dummy
					OptionApplications = new bool[_buffer.p_option.Count + 1, _buffer.p_applications.Count + 1];
					IsValidOption = new bool[_buffer.p_option.Count + 1];
					IsValidApplication = new bool[_buffer.p_applications.Count + 1];
					Provider.WriteLogFormat(LogXml2DB, "Read Settings family Id = " + settingsFam.Key);
					var settinfsFamilyResult = new List<Uirule>();
					SettingsId = settingsFam.Key;
					String swPackName = _buffer.p_settingsFamilies.Join(_buffer.p_swPacks, f => f.SwpackId, s => s.Id, (f, s) => new
					{
						feature = f,
						swpack = s
					}).Single(x => x.feature.Id == settingsFam.Key).swpack.Name;
					var AllProbesDefaultsDict = new Dictionary<string, XDocument>();
					foreach (var grpProbe in settingsFam.GroupBy(gs => gs.ProbeId).ToList())        // loop probes
					{
						var ProbeResult = new List<Uirule>();
						var settingsInfo = new SettingsInfo(grpProbe, swPackName, settingsFam.Key, _buffer, dicts, l_probeKits, AllProbesDefaultsDict);
						probeName = settingsInfo.currentProbe.SaleName;
						Provider.WriteLogFormat(LogXml2DB, "family " + settingsFam.Key + " --- " + probeName);
						if (settingsInfo.currentProbeSettings.ProbeDataFileNameFrontal != null)
						{
							ProbeResult.AddRange(ReadProbeFiles(settingsInfo, TransducerPosition.Frontal, Paths));
						}
						if (settingsInfo.currentProbeSettings.ProbeDataFileNameLateral != null)
						{
							ProbeResult.AddRange(ReadProbeFiles(settingsInfo, TransducerPosition.Lateral, Paths));
						}
						else
						{
							ProbeResult.ForEach(r => r.TransducerType = null);  // if only one transducer, remove transducer type from rules
						}
						settinfsFamilyResult.AddRange(ProbeResult);
					}

					// set logical model in the rules created for the settings family
					result.AddRange(replicateRules(settinfsFamilyResult, settingsFam.Key, _buffer));
					result.AddRange(ForceReplicateRules(GetOptionRules(_buffer, LogXml2DB), settingsFam.Key, _buffer));
				}
			}
			catch (Exception ex)
			{
				throw new ParsingException("Presets for settings family Id " + SettingsId.ToString() + " probe " + probeName, ex);
			}

			// ANTO DB

			//SubmitChanges();
			SaveChanges();
			result.AddRange(GenerateKitRules(l_probeKits, _buffer, LogXml2DB));

			// ANTO DB before bulk
			SaveChanges();

			Provider.WriteLogFormat(LogXml2DB, "Insert Probe Transducer Rules");
			this.BulkInsert(p_probeTransducers);
			// ANTO DB bulk saves
			//SubmitChanges();

			Provider.WriteLogFormat(LogXml2DB, "Preset Rules Read. Now Insert Rules.");

			return result;
		}

		// [RTC 31507] produces the rules to filter option-probe availabilities: if a probe-option is avaiable for only vet (or human) appliations,
		// it will be blocked for human (or vet) models.
		//private List<UIRule> FixOptionProbeRules(List<UIRule> _uirules, DBBuffer _buffer, String LogXml2DB)
		//{
		//  Provider.WriteLogFormat(LogXml2DB, "Fixing option probe app rules");

		//  var result = new List<UIRule>();

		//  // convert to Normal Rules to check allowance. ToSingleNormalRule is enought because we are looking for the application type
		//  var AllNRules = _uirules.Select(x => x.ToSingleNormalRule()).ToList();
		//  var allower = new AvailableAllower();

		//  var validusers = Enum.GetValues(typeof(UserLevel)).Cast<UserLevel>().ToList();
		//  validusers.Remove(UserLevel.Unknown);

		//  foreach (var settingsfamily in _buffer.p_settingsFamilies)
		//  {
		//      Provider.WriteLogFormat(LogXml2DB, "Fixing settings family " + settingsfamily.Name);

		//      var SFmodels = getLogicalModels(settingsfamily.Id, _buffer);
		//      var modelDict = new Dictionary<SystemEnvironment, List<LogicalModel>>()
		//      {
		//          { SystemEnvironment.Human, SFmodels.Where(m => m.Type == SystemEnvironment.Human).ToList()},
		//          { SystemEnvironment.Veterinary, SFmodels.Where(m => m.Type == SystemEnvironment.Veterinary).ToList()}
		//      };
		//      var modelsToCheck = new List<LogicalModel>()
		//      {
		//          modelDict[SystemEnvironment.Human].FirstOrDefault(), modelDict[SystemEnvironment.Veterinary].FirstOrDefault()
		//      };
		//      modelsToCheck = modelsToCheck.Where(x => x != null).ToList();

		//      foreach (var model in modelsToCheck)
		//      {
		//          if (model.Type == null)
		//          {
		//              continue;
		//          }

		//          var nrules = FeatureMain.SqueezeModel(model, AllNRules);
		//          var probeResult = new List<UIRule>[_buffer.p_probes.Count];
		//          Parallel.For(0, _buffer.p_probes.Count, ProbeIndex =>
		//          {
		//              foreach (var user in validusers)
		//              {
		//                  var Input = new InputParams
		//                  {
		//                      ProbeId = ProbeIndex,
		//                      LogicalModelId = model.Id,
		//                      UserLevel = user,
		//                  };
		//                  if (FeatureMain.IsAllowedFull(Input, allower, nrules))
		//                  {
		//                      foreach (var option in _buffer.p_option)
		//                      {
		//                          Input.OptionId = option.Id;
		//                          Input.ApplicationId = null;
		//                          if (FeatureMain.IsAllowedFull(Input, allower, nrules))
		//                          {
		//                              var validAppType = model.Type;
		//                              var found = false;
		//                              // loop over valid applications for the model and if none is available add blocking rule
		//                              foreach (var app in _buffer.p_applications.Where(x => x.AppType == validAppType && !x.IsFake))
		//                              {
		//                                  Input.ApplicationId = app.Id;
		//                                  if (FeatureMain.IsAllowedFull(Input, allower, nrules))
		//                                  {
		//                                      found = true;
		//                                      break;
		//                                  }
		//                              }


		//                              if (!found)
		//                              {
		//                                  if (probeResult[ProbeIndex] == null)
		//                                  {
		//                                      probeResult[ProbeIndex] = new List<UIRule>();
		//                                  }
		//                                  foreach (var typedmodel in modelDict[(SystemEnvironment)model.Type])
		//                                  {
		//                                      var tmprule = new UIRule()
		//                                      {
		//                                          Allow = AllowModes.No,
		//                                          OptionId = option.Id,
		//                                          ProbeId = ProbeIndex,
		//                                          UserLevel = user,
		//                                          RuleOrigin = ruleOrigins.OptionProbeSyntesis,
		//                                          LogicalModelId = typedmodel.Id
		//                                      };
		//                                      probeResult[ProbeIndex].Add(tmprule);
		//                                  }
		//                              }
		//                          }
		//                      }
		//                  }
		//              }
		//          });

		//          for (int prind = 0; prind < _buffer.p_probes.Count; prind++)
		//          {
		//              if (probeResult[prind] != null)
		//              {
		//                  result.AddRange(probeResult[prind]);
		//              }
		//          }
		//      }
		//  }
		//  return result;
		//}

		private List<Uirule> GetOptionRules(DBBuffer _buffer, string log)
		{
			Provider.WriteLogFormat(log, "Get Option Rules");
			var Result = new List<Uirule>();
			var OptProbeRows = new List<Uirule>[_buffer.p_option.Count + 1];
			var OptAppRows = new List<Uirule>[_buffer.p_option.Count + 1];
			OptProbeRows[0] = new List<Uirule>();
			OptAppRows[0] = new List<Uirule>();
			var IsValidProbe = Enumerable.Repeat(true, _buffer.p_probes.Count + 1).ToArray();
			Parallel.For(1, OptionProbes.GetLength(0), OptionIndex =>
			{
				if (IsValidOption[OptionIndex])
				{
					Parallel.Invoke(
						() => OptProbeRows[OptionIndex] = ConverFromMatrix(OptionProbes, OptionIndex, (Uirule r, int index) => r.ProbeId = index, IsValidProbe),
						() => OptAppRows[OptionIndex] = ConverFromMatrix(OptionApplications, OptionIndex, (Uirule r, int index) => r.ApplicationId = index, IsValidApplication)
					);
				}
				else
				{
					OptProbeRows[OptionIndex] = new List<Uirule>();
					OptAppRows[OptionIndex] = new List<Uirule>();
				}
			});
			Result.AddRange(OptProbeRows.SelectMany(r => r).ToList());
			Result.AddRange(OptAppRows.SelectMany(r => r).ToList());
			return Result;
		}

		private List<Uirule> ConverFromMatrix(bool[,] Relations, int OptionIndex, Action<Uirule, int> Setter, bool[] ValidObjects)
		{
			var result = new List<Uirule>();
			for (int index = 1; index < Relations.GetLength(1); index++)
			{
				if (ValidObjects[index])
				{
					var tmpRule = new Uirule()
					{
						Allow = Relations[OptionIndex, index] == true ? AllowModes.Def : AllowModes.No,
						OptionId = OptionIndex,
						RuleOrigin = ruleOrigins.ProbeXml
					};
					Setter(tmpRule, index);
					result.Add(tmpRule);
				}
			}
			return result;
		}

		private void SetKitFeatures(string log, DBBuffer _buffer)
		{
			try
			{
				Provider.WriteLogFormat(log, "SetKitFeatures");
				List<Feature> KitFeatures = new List<Feature>();
				_buffer.p_biospyKits = BiopsyKits.ToList();
				foreach (var kit in _buffer.p_biospyKits)
				{
					KitFeatures.Add(new Feature
					{
						Name = kit.Name,
						NameInCode = kit.Name
					});
				}

				// ANTO DB
				this.BulkInsert(KitFeatures);
				//this.AddRange(KitFeatures);
				//this.SaveChanges();

				//var CommandsToDB = this.GetChangeSet(); // ANTO DB not used
				//this.SubmitChanges();
				_buffer.p_features = this.Feature.ToList();
				foreach (var kit in _buffer.p_biospyKits)
				{
					kit.FeatureId = _buffer.p_features.Single(f => f.Name == kit.Name).Id;
				}
				// ANTO DB
				//this.SubmitChanges();
				SaveChanges();
				_buffer.p_biospyKits = BiopsyKits.ToList();
			}
			catch (Exception ex)
			{
				throw new ApplicationException(ex.ToString());

			}
		}

		private void SetProbeFeatures(string log, DBBuffer _buffer)
		{
			try
			{
				Provider.WriteLogFormat(log, "SetProbeFeatures");
				List<Feature> ProbeFeatures = new List<Feature>();
				foreach (var probe in _buffer.p_probes)
				{
					ProbeFeatures.Add(new Feature
					{
						Name = probe.SaleName,
						NameInCode = probe.SaleName
					});
				}
				this.BulkInsert(ProbeFeatures);
				//this.SubmitChanges();
				_buffer.p_features = this.Feature.ToList();
				foreach (var probe in _buffer.p_probes)
				{
					probe.FeatureId = _buffer.p_features.Single(f => f.Name == probe.SaleName).Id;
				}
				// ANTO DB
				// this.SubmitChanges();
				SaveChanges();
				_buffer.p_probes = Probe.ToList();
			}
			catch (Exception ex)
			{
				throw new ApplicationException(ex.ToString());
			}
		}

		private List<Uirule> GenerateKitRules(List<Probe_Kits> l_probeKits, DBBuffer _buffer, string log)
		{
			Provider.WriteLogFormat(log, "Generate Kit Rules");
			var KitResult = new List<Uirule>();
			SetKitFeatures(log, _buffer);
			SetProbeFeatures(log, _buffer);
			foreach (var kit in _buffer.p_biospyKits)
			{
				Provider.WriteLogFormat(log, "*** Kit name: " + kit.Name);
				foreach (var family in _buffer.p_settingsFamilies)
				{
					var ProbeRules = new List<Uirule>();
					var validProbeIds = l_probeKits.Where(pk => pk.settingsFamilyIdl == family.Id && pk.kits.Select(k => k.Id).Contains(kit.Id))
										.Select(pk => pk.probe.Id).ToList();
					var missingProbeIds = _buffer.p_probes.Where(p => !validProbeIds.Contains(p.Id)).Select(p => p.Id).ToList();

					// if the kit has probes add it to the model
					if (validProbeIds.Count > 0)
					{
						ProbeRules.Add(new Uirule()
						{
							Allow = AllowModes.Def,
							RuleOrigin = ruleOrigins.ProbeXml,
							KitId = kit.Id
						});
					}

					var VPRules = new Uirule[validProbeIds.Count];
					Parallel.For(0, validProbeIds.Count, VPIndex =>
					{
						VPRules[VPIndex] = new Uirule()
						{
							Allow = AllowModes.Def,
							RuleOrigin = ruleOrigins.ProbeXml,
							ProbeId = validProbeIds[VPIndex],
							KitId = kit.Id
						};
					});
					if (VPRules.Any(r => r == null))
					{
						throw new ApplicationException("Generate Kit Rules: VPRules contains null elements");
					}
					ProbeRules.AddRange(VPRules.ToList());

					var MPRules = new Uirule[missingProbeIds.Count];
					Parallel.For(0, missingProbeIds.Count, MPIndex =>
					{
						MPRules[MPIndex] = new Uirule()
						{
							Allow = AllowModes.No,
							RuleOrigin = ruleOrigins.ProbeXml,
							ProbeId = missingProbeIds[MPIndex],
							KitId = kit.Id
						};
					});
					if (MPRules.Any(r => r == null))
					{
						throw new ApplicationException("Generate Kit Rules: MPRules contains null elements");
					}
					ProbeRules.AddRange(MPRules.ToList());
					KitResult.AddRange(ForceReplicateRules(ProbeRules, family.Id, _buffer));
				}
			}
			return KitResult;
		}

		private List<Uirule> ReadProbeFiles(SettingsInfo settingsInfo, TransducerPosition biplanarType, Paths Paths)
		{
			var ProbeResult = new List<Uirule>();
			var probePath = Paths.getProbePath(settingsInfo.swPackName) + "\\" + settingsInfo.currentProbeSettings.ProbeFolder;
			var dataFile = XDocument.Load(probePath + "\\" + settingsInfo.currentProbeSettings.getProbeDataFile(biplanarType));
			var defDict = new Dictionary<string, XDocument>();
			ReadAppDefaults(probePath, settingsInfo.grpProbe.ToList(), biplanarType, defDict, settingsInfo.AllProbeDefaultDict);

			updateCurrentProbe(settingsInfo.currentProbe, dataFile, settingsInfo.dicts);
			var firstFocFile = XDocument.Load(Provider.GetFocFileName(Paths.focFilePath + "\\" + settingsInfo.swPackName, defDict.First().Value,
											  settingsInfo.AllProbeDefaultDict.First().Value));
			var currentTransducerType = InsertProbeTransducers(settingsInfo.currentProbe, firstFocFile, settingsInfo.dicts, biplanarType);

			readKits(settingsInfo, dataFile);       // adds Kits
			var prbData = new ProbeDataset(settingsInfo, dataFile, currentTransducerType, Paths);
			ProbeResult.AddRange(RulesNotInDefault(prbData, settingsInfo.dicts));

			// loop application groups associated to probe
			foreach (var appSet in settingsInfo.grpProbe.GroupBy(x => x.ApplicationId).ToList())
			{
				SetOptionApps_4OptionsInProbeData(appSet.Key, settingsInfo.currentProbe.Id, settingsInfo.dicts);      // associate app to probedata options

				// default file must be unique for each application
				if (appSet.GroupBy(d => Path.GetFileName(d.defaultName)).Count() > 1)
				{
					throw new Exception("Default files are not coherent for probe " + appSet.First().ProbeId + " App " + appSet.First().ApplicationId);
				}
				var defFile = defDict[appSet.First().defaultName];
				var AllProbeDefFile = settingsInfo.AllProbeDefaultDict[appSet.First().AllProbeDefaultName];
				var prstData = new PresetData(prbData, appSet.ToList(), defFile, AllProbeDefFile, settingsInfo._buffer);
				var IsBAllowed = checkPresetAndDef(prstData, 0, new directEnableTag("xfB")).Allow;
				ProbeResult.AddRange(RulesInDefault(prstData, IsBAllowed, settingsInfo.dicts));
			}
			return ProbeResult;
		}

		// rules not deriving from the preset flags
		private List<Uirule> RulesNotInDefault(ProbeDataset prbData, Dictionaries dicts)
		{
			var result = new Uirule[9];
			Parallel.Invoke(
				/* REMEMBER TO PUT THE OPTION IN SetOptionApps_4OptionsInProbeData TO ALLOW ALL APPLICATIONS IN THE PROBE*/
				() => result[0] = (_4D3DRule(prbData, dicts)),
				() => result[1] = (Adv4D3DRule(prbData, dicts)),
				() => result[2] = (XlightRule(prbData, dicts)),
				() => result[3] = (VpanRule(prbData, dicts)),
				() => result[4] = (ADARule(prbData, dicts)),
				() => result[5] = (BreastNavRule(prbData, dicts)),
				() => result[6] = (BreastBodyMapRule(prbData, dicts)),
				() => result[7] = (BreastBiopsyRule(prbData, dicts)),
				() => result[8] = (MRBreastNavRule(prbData, dicts))
			);
			/* REMEMBER TO PUT THE OPTION IN SetOptionApps_4OptionsInProbeData TO ALLOW ALL APPLICATIONS IN THE PROBE*/
			return result.ToList();
		}

		private void SetOptionApps_4OptionsInProbeData(int AppId, int ProbeId, Dictionaries dicts)
		{
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["4D3D"], AppId, ProbeId);
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["4D3DAdv"], AppId, ProbeId);
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["XLight"], AppId, ProbeId);
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["VPan"], AppId, ProbeId);
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["ADA"], AppId, ProbeId);
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["BreastNav"], AppId, ProbeId);
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["BreastBodyMap"], AppId, ProbeId);
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["BreastBiopsy"], AppId, ProbeId);
			SetOptionApps_4OptionsInProbeData(dicts.dictOpt["MRBreastNav"], AppId, ProbeId);
		}


		// rules deriving from the preset
		private List<Uirule> RulesInDefault(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var ParallelResult = new Uirule[27];
			var ParallelListResult = new List<Uirule>[6];
			Parallel.Invoke(
				() => ParallelResult[0] = (QimtRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[1] = (QasRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[2] = (TvmRule(prstData, dicts)),
				() => ParallelResult[3] = (CFMRule(prstData, dicts)),
				() => ParallelResult[4] = (CMMRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[5] = (EDRRule(prstData, dicts)),
				() => ParallelResult[6] = (DopplerPwRule(prstData, dicts)),
				() => ParallelResult[7] = (DopplerCwRule(prstData, dicts)),
				() => ParallelResult[8] = (TrpzRule(prstData, dicts)),
				() => ParallelResult[9] = (MviewRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[10] = (XviewCVRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[11] = (XviewRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[12] = (StressRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[13] = (AutoEFRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[14] = (XstrainRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[15] = (NeedleEnhanceRule(prstData, dicts)),
				() => ParallelResult[16] = (AutoEFFARule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[17] = (BModeRule(prstData, dicts)),
				() => ParallelResult[18] = (MModeRule(prstData, dicts)),
				() => ParallelResult[19] = (MicroVRule(prstData, dicts)),
				() => ParallelResult[20] = (SticRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[21] = (AutoCFMRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[22] = (EasyTuningRule(prstData, dicts)),
				() => ParallelResult[23] = (AntiRibsRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[24] = (VortexRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[25] = (BreastCalcRule(prstData, IsBAllowed, dicts)),
				() => ParallelResult[26] = (UroFusionNavRule(prstData, dicts)),

				() => ParallelListResult[0] = LibraryRules(prstData, IsBAllowed, dicts),
				() => ParallelListResult[1] = CntiRules(prstData, IsBAllowed, dicts),
				() => ParallelListResult[2] = LvoRules(prstData, IsBAllowed, dicts),
				() => ParallelListResult[3] = ElastoRule(prstData, IsBAllowed, dicts),
				() => ParallelListResult[4] = (QElaxtoRule(prstData, IsBAllowed, dicts)),
				() => ParallelListResult[5] = (QElaxto2DRule(prstData, IsBAllowed, dicts))
			);

			var result = ParallelResult.ToList();
			result.AddRange(ParallelListResult.SelectMany(pr => pr));
			return result;
		}

		// update probe fields with the focalization file information
		private void updateCurrentProbe(Probe currentProbe, XDocument dataFile, Dictionaries dicts)
		{
			currentProbe.ProbeFamily = dicts.dictProbeFamilies[dataFile.Descendants("eProbeFamily").FirstOrDefault().Value.ToString()];
			// set transesophag information
			currentProbe.TeeFamily = dataFile.Descendants("eTEEFamilyType").FirstOrDefault().Value;
			if (currentProbe.TeeFamily == "NO_TEE_TYPE_PD")
			{
				currentProbe.TeeFamily = null;
			}
			currentProbe.Motorized = Convert.ToBoolean(dataFile.Descendants("fMotorizedTee").FirstOrDefault().Value);
		}

		// read the focalization file and save the transducet type
		private ProbeType InsertProbeTransducers(Probe currentProbe, XDocument focFile, Dictionaries dicts, TransducerPosition biplanarType)
		{
			var strProbeType = focFile.Descendants("ProbeData").FirstOrDefault().Attribute("ProbeType").Value;
			var TransducerType = dicts.dictProbeTypes[strProbeType];
			if (!p_probeTransducers.Any(t => t.ProbeId == currentProbe.Id && t.TransducerType == TransducerType))
			{
				p_probeTransducers.Add(new ProbeTransducers
				{
					ProbeId = currentProbe.Id,
					TransducerType = TransducerType,
					TransducerPosition = biplanarType
				});
			}
			return TransducerType;
		}

		//private StringBuilder baseLogComment(Int32 probeId, Int32 ? appId, String optName)
		//{
		//  StringBuilder str = new StringBuilder();
		//  str.AppendLine().Append("Probe: ").Append(Probe.SingleOrDefault(p => p.Id == probeId).SaleName)
		//  .Append(", Option: ").Append(optName);
		//  if (appId != null)
		//  {
		//      str.Append(", Application: ").Append(Application.SingleOrDefault(a => a.Id == appId).Name);
		//  }
		//  return str;
		//}
		Uirule XviewCVRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var xviewCVrule = checkPresetAndDef(prstData, dicts.dictOpt["XViewII"], new containerTag("sXvEnhFilter", ".gop"));
			if (xviewCVrule.Allow != AllowModes.No && IsBAllowed == AllowModes.No)
			{
				xviewCVrule.Allow = AllowModes.No;
			}
			FillOptionProbeApp(xviewCVrule);
			return xviewCVrule;
		}

		// B only
		Uirule XviewRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var xrule = BRule(prstData, IsBAllowed, dicts.dictOpt["XView"]);
			FillOptionProbeApp(xrule);
			return xrule;
		}
		Uirule EDRRule(PresetData prstData, Dictionaries dicts)
		{
			var edrrule = checkPresetAndDef(prstData, dicts.dictOpt["BreathingCurve"], new directEnableTag("fEcgActivable"));
			FillOptionProbeApp(edrrule);
			return edrrule;
		}

		Uirule DopplerPwRule(PresetData prstData, Dictionaries dicts)
		{
			var dpwrule = checkPresetAndDef(prstData, dicts.dictOpt["DopplerPW"], new directEnableTag("xfPW"));
			FillOptionProbeApp(dpwrule);
			return dpwrule;
		}

		Uirule DopplerCwRule(PresetData prstData, Dictionaries dicts)
		{
			var dcwrule = checkPresetAndDef(prstData, dicts.dictOpt["DopplerCW"], new directEnableTag("xfCW"));
			FillOptionProbeApp(dcwrule);
			return dcwrule;
		}

		// [RTC 13755]
		Uirule BModeRule(PresetData prstData, Dictionaries dicts)
		{
			var Brule = checkPresetAndDef(prstData, dicts.dictOpt["Bmode"], new directEnableTag("xfB"));
			FillOptionProbeApp(Brule);
			return Brule;
		}

		// [RTC 13755]
		Uirule MModeRule(PresetData prstData, Dictionaries dicts)
		{
			var Mrule = checkPresetAndDef(prstData, dicts.dictOpt["Mmode"], new directEnableTag("xfM"));
			FillOptionProbeApp(Mrule);
			return Mrule;
		}

		private void FillOptionProbe(Uirule rule)
		{
			var ProbeId = (int)rule.ProbeId;
			var OptionId = (int)rule.OptionId;
			SetOptionProbeAssociation(OptionId, ProbeId, rule.Allow);
		}

		private void FillOptionProbeApp(Uirule rule)
		{
			var ProbeId = (int)rule.ProbeId;
			var OptionId = (int)rule.OptionId;
			var AppId = (int)rule.ApplicationId;
			SetOptionProbeAssociation(OptionId, ProbeId, rule.Allow);
			SetOptionAppAssociation(OptionId, AppId, rule.Allow);
		}

		private void SetOptionProbeAssociation(int optionid, int probeid, AllowModes allow)
		{
			if (!IsValidOption[optionid])
			{
				IsValidOption[optionid] = true;
			}

			if (OptionProbes[optionid, probeid] == false && allow != AllowModes.No)
			{
				OptionProbes[optionid, probeid] = true;
			}
		}


		private void SetOptionAppAssociation(int optionid, int appid, AllowModes allow)
		{
			if (!IsValidApplication[appid])
			{
				IsValidApplication[appid] = true;
			}

			if (OptionApplications[optionid, appid] == false && allow != AllowModes.No)
			{
				OptionApplications[optionid, appid] = true;
			}
		}

		// set association of option-application for option whose availability is taken from probedata (not dependent from application presets)
		private void SetOptionApps_4OptionsInProbeData(int OptionId, int AppId, int ProbeId)
		{
			var allow = OptionProbes[OptionId, ProbeId] ? AllowModes.Def : AllowModes.No;
			SetOptionAppAssociation(OptionId, AppId, allow);
		}

		private void FillOptionProbeApp(List<Uirule> rules)
		{
			foreach (var rule in rules)
			{
				FillOptionProbeApp(rule);
			}
		}


		// 4D3D: biscan only + no transesophag
		private Uirule get_4D3DRule(string OptionName, ProbeDataset prbData, Dictionaries dicts)
		{
			var rule3d4d = get_MotorizedRule(OptionName, prbData, dicts);

			FillOptionProbe(rule3d4d);
			return rule3d4d;
		}

		private Uirule get_MotorizedRule(string OptionName, ProbeDataset prbData, Dictionaries dicts)
		{
			var rule3d4d = checkDataFile(prbData, dicts.dictOpt[OptionName], new valueEnableTag("eProbeFamily", "PF_BISCAN_PD"));
			if (rule3d4d.Allow != AllowModes.No && prbData.currentProbe.TeeFamily != null)
			{
				rule3d4d.Allow = AllowModes.No;
			}
			return rule3d4d;
		}

		// not using 3DBundle because 3D is both in 3Dbundle and 4D3D, therefore the rules for 3D would be doubled
		Uirule _4D3DRule(ProbeDataset prbData, Dictionaries dicts)
		{
			return get_4D3DRule("4D3D", prbData, dicts);
		}
		Uirule Adv4D3DRule(ProbeDataset prbData, Dictionaries dicts)
		{
			return get_4D3DRule("4D3DAdv", prbData, dicts);
		}
		Uirule XlightRule(ProbeDataset prbData, Dictionaries dicts)
		{
			return get_4D3DRule("XLight", prbData, dicts);
		}

		// NEEDLE_ENHANCE: no pencil, no phased array, bnumkit>0, needle visibility in foc files
		Uirule NeedleEnhanceRule(PresetData prstData, Dictionaries dicts)
		{
			var needleFocRule = checkPresetAndDef(prstData, dicts.dictOpt["NeedleEnhImg"], new FocalBlockTag("fNeedleVisibility", prstData.prbData.focPath, "1"));
			var appId = prstData.appSet.First().ApplicationId;

			if ((needleFocRule.Allow != AllowModes.No) &&
					(prstData.prbData.currentTransducerType == ProbeType.PENCIL
					 || prstData.prbData.currentTransducerType == ProbeType.PHASE_ARRAY
					 || checkCardioApplication(appId, dicts)))
			{
				needleFocRule.Allow = AllowModes.No;        // if no needle visibility in foc file do not allow
			}
			FillOptionProbeApp(needleFocRule);
			return needleFocRule;
		}


		// VPAN: no pencil, no phased array, no transo
		Uirule VpanRule(ProbeDataset prbData, Dictionaries dicts)
		{
			Uirule vpanRule = buildProbeAppOptRule(prbData.currentProbe.Id, prbData.currentTransducerType, null, dicts.dictOpt["VPan"]);
			if (prbData.currentTransducerType == ProbeType.PENCIL
					|| prbData.currentTransducerType == ProbeType.PHASE_ARRAY
					|| prbData.currentProbe.TeeFamily != null)
			{
				vpanRule.Allow = AllowModes.No;
			}
			else
			{
				vpanRule.Allow = AllowModes.Def;
			}
			FillOptionProbe(vpanRule);
			return vpanRule;
		}

		// ADA: Linear probes only      // Gibiino [RTC 12552]
		Uirule ADARule(ProbeDataset prbData, Dictionaries dicts)
		{
			Uirule adaRule = buildProbeAppOptRule(prbData.currentProbe.Id, prbData.currentTransducerType, null, dicts.dictOpt["ADA"]);
			if (prbData.currentTransducerType != ProbeType.LINEAR)
			{
				adaRule.Allow = AllowModes.No;
			}
			else
			{
				adaRule.Allow = AllowModes.Def;
			}
			FillOptionProbe(adaRule);
			return adaRule;
		}

		// BreastNav: Linear probes only - inherits from virtual navigator bundle      // PD [RTC 31359]
		Uirule BreastNavRule(ProbeDataset prbData, Dictionaries dicts)
		{
			Uirule rule = buildProbeAppOptRule(prbData.currentProbe.Id, prbData.currentTransducerType, null, dicts.dictOpt["BreastNav"]);
			return BreastNavCoreRule(rule, prbData);
		}

		// BreastBiopsy: as BreastNav in order to be clearer      // PD [RTC 32990]
		Uirule BreastBiopsyRule(ProbeDataset prbData, Dictionaries dicts)
		{
			Uirule rule = buildProbeAppOptRule(prbData.currentProbe.Id, prbData.currentTransducerType, null, dicts.dictOpt["BreastBiopsy"]);
			return BreastNavCoreRule(rule, prbData);
		}

		// feature behaving like BreastNav and associated with the same license in the FeatureRelations
		Uirule BreastBodyMapRule(ProbeDataset prbData, Dictionaries dicts)
		{
			Uirule rule = buildProbeAppOptRule(prbData.currentProbe.Id, prbData.currentTransducerType, null, dicts.dictOpt["BreastBodyMap"]);
			return BreastNavCoreRule(rule, prbData);
		}

		//@PD MRBreastNav as BreastNav [RTC 40361]
		Uirule MRBreastNavRule(ProbeDataset prbData, Dictionaries dicts)
		{
			Uirule rule = buildProbeAppOptRule(prbData.currentProbe.Id, prbData.currentTransducerType, null, dicts.dictOpt["MRBreastNav"]);
			return BreastNavCoreRule(rule, prbData);
		}

		Uirule BreastNavCoreRule(Uirule rule, ProbeDataset prbData)
		{
			rule.Allow = (prbData.currentTransducerType != ProbeType.LINEAR) ? AllowModes.No : AllowModes.Def;
			FillOptionProbe(rule);
			return rule;
		}

		//@PD UroFusion only in Urology [RTC 46396]
		Uirule UroFusionNavRule(PresetData prstData, Dictionaries dicts)
		{
			var appId = prstData.appSet.First().ApplicationId;
			var urofusionrule = buildProbeAppOptRule(prstData.prbData.currentProbe.Id, prstData.prbData.currentTransducerType, appId, dicts.dictOpt["UroFusion"]);
			urofusionrule.Allow = checkApplication(appId, "Urologic", dicts) ? AllowModes.Def : AllowModes.No;
			return urofusionrule;
		}

		// QIMT: vascular only + linear probes + B only + IsQIMTEnable
		Uirule QimtRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var appId = prstData.appSet.First().ApplicationId;
			var qimtRule = checkPresetAndDef(prstData, dicts.dictOpt["Qimt"], new directEnableTag("IsQIMTEnable"));
			if (qimtRule.Allow != AllowModes.No &&
					(IsBAllowed == AllowModes.No
					 || !checkApplication(appId, "Vascular", dicts)
					 || prstData.prbData.currentTransducerType != ProbeType.LINEAR))
			{
				qimtRule.Allow = AllowModes.No;
			}
			FillOptionProbeApp(qimtRule);
			return qimtRule;
		}

		// QElaxto and KPA => same probe flag
		List<Uirule> QElaxtoRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var appId = prstData.appSet.First().ApplicationId;
			var qelxRule = checkPresetAndDef(prstData, dicts.dictOpt["QElaxto"], new containerTagEnable("sQElaxtoData", "fQElaxtoEnable"));
			if (qelxRule.Allow != AllowModes.No && IsBAllowed == AllowModes.No)
			{
				qelxRule.Allow = AllowModes.No;
			}
			var kpaRule = new Uirule(qelxRule);
			kpaRule.OptionId = dicts.dictOpt["KPA"];

			var result = new List<Uirule>()
			{
				qelxRule, kpaRule
			};

			FillOptionProbeApp(result);
			return result;
		}

		// QElaxto2D convex probes only. 2DSWELinear available for all probes where tag is true
		List<Uirule> QElaxto2DRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var appId = prstData.appSet.First().ApplicationId;
			var qelxLinRule = checkPresetAndDef(prstData, dicts.dictOpt["2DSWELinear"], new containerTagEnable("sQElaxtoData2D", "fQElaxtoEnable"));
			if (qelxLinRule.Allow != AllowModes.No && IsBAllowed == AllowModes.No)
			{
				qelxLinRule.Allow = AllowModes.No;
			}

			var QElaxtoRule = new Uirule(qelxLinRule);
			QElaxtoRule.OptionId = dicts.dictOpt["QElaxto2D"];

			if (QElaxtoRule.Allow != AllowModes.No && prstData.prbData.currentTransducerType != ProbeType.CONVEX)
			{
				QElaxtoRule.Allow = AllowModes.No;
			}

			var result = new List<Uirule>()
			{
				qelxLinRule, QElaxtoRule
			};

			FillOptionProbeApp(result);
			return result;
		}


		// QAS: vascular only + linear probes + B only + IsQASEnable
		Uirule QasRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var appId = prstData.appSet.First().ApplicationId;
			var qasRule = checkPresetAndDef(prstData, dicts.dictOpt["QAS"], new directEnableTag("IsQASEnable"));
			if (qasRule.Allow != AllowModes.No &&
					(IsBAllowed == AllowModes.No
					 || !checkApplication(appId, "Vascular", dicts)
					 || prstData.prbData.currentTransducerType != ProbeType.LINEAR))
			{
				qasRule.Allow = AllowModes.No;
			}
			FillOptionProbeApp(qasRule);
			return qasRule;
		}

		// CFM: multiple tags
		Uirule CFMRule(PresetData prstData, Dictionaries dicts)
		{
			var CFMRule = checkPresetAndDef(prstData, dicts.dictOpt["CFM"], new tagList(
												new List<String> {  "xfB_CFM",
														"xfM_CFM",
														"xfB_M_CFM_LIVE",
														"xfB_CFM_PW_LIVE",
														"xfB_CFM_M_CFM_LIVE",
														"xfB_CFM_PW_UPDT",
														"xfB_CFM_CW_UPDT",
														"xfB_CFM_M_CFM_UPDT",
														"xfB_CFM_PW_BLSF",
														"xfB_CFM_CW_BLSF",
														"xfB_CFM_M_CFM_BLSF",
																 }));
			FillOptionProbeApp(CFMRule);
			return CFMRule;
		}

		// [RTC 14131] MicroV rule: child of CFM working for all but cardiac
		Uirule MicroVRule(PresetData prstData, Dictionaries dicts)
		{
			var probeApp = prstData.appSet.First();
			var mcvrule = buildProbeAppOptRule(probeApp.ProbeId, prstData.prbData.currentTransducerType, probeApp.ApplicationId, dicts.dictOpt["MicroV"]);
			mcvrule.Allow = checkCardioApplication(probeApp.ApplicationId, dicts) ? AllowModes.No : AllowModes.Def;
			FillOptionProbeApp(mcvrule);
			return mcvrule;
		}
		//UIRule XFlowRule(PresetData prstData, Dictionaries dicts)
		//{
		//  var probeApp = prstData.appSet.First();
		//  //var xfwrule = buildProbeAppOptRule(probeApp.ProbeId, prstData.prbData.currentTransducerType, probeApp.ApplicationId, dicts.dictOpt["XFlow"]); //POG FIXME
		//  var xfwrule = buildProbeAppOptRule(probeApp.ProbeId, prstData.prbData.currentTransducerType, probeApp.ApplicationId, dicts.dictOpt["MicroV"]);
		//  xfwrule.Allow = checkCardioApplication(probeApp.ApplicationId, dicts) ? AllowModes.No : AllowModes.Def;
		//  FillOptionProbeApp(xfwrule);
		//  return xfwrule;
		//}
		// CMM: B + M
		Uirule CMMRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var cmmRule = checkPresetAndDef(prstData, dicts.dictOpt["CMM"], new tagList(new List<String> { "xfB_CFM", "xfM_CFM" }));
			FillOptionProbeApp(cmmRule);
			return cmmRule;
		}

		// TVM-TV: cardiac only
		Uirule TvmRule(PresetData prstData, Dictionaries dicts)
		{
			var appId = prstData.appSet.First().ApplicationId;
			var tvmRle = checkPresetAndDef(prstData, dicts.dictOpt["TVM"], new directEnableTag("xfTvm"));
			if (tvmRle.Allow != AllowModes.No && (
						!checkCardioApplication(appId, dicts)))
			{
				tvmRle.Allow = AllowModes.No;
			}
			FillOptionProbeApp(tvmRle);
			return tvmRle;
		}

		// CnTI: B only + sCnTIData user data + storeNumFreq > 0 in Foc files, [RTC 14211] no cardiac
		List<Uirule> CntiRules(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			List<Uirule> cntiRules = new List<Uirule>();
			foreach (var user in getKnownUsers(prstData))
			{
				cntiRules.Add(checkPresetAndDef(prstData, dicts.dictOpt["Cnti"], new userEnableTag("sCnTIData", user)));
			}

			var cntiFocRule = checkPresetAndDef(prstData, dicts.dictOpt["Cnti"], new FocalModeTag("Frequency", prstData.prbData.focPath, "CNTI"));

			if (cntiRules.Any(r => r.Allow != AllowModes.No) && (
						cntiFocRule.Allow == AllowModes.No
						|| IsBAllowed == AllowModes.No
						|| checkCardioApplication(prstData.appSet.First().ApplicationId, dicts)))
			{
				cntiRules.ForEach(r => r.Allow = AllowModes.No);
			}
			FillOptionProbeApp(cntiRules);
			return cntiRules;
		}

		// LVO: Bonly + cardiacl only + cnti angles in focalization files
		List<Uirule> LvoRules(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var lvoRules = new List<Uirule>();
			foreach (var user in getKnownUsers(prstData))
			{
				lvoRules.Add(checkPresetAndDef(prstData, dicts.dictOpt["LVO"], new userEnableTag("sCnTIData", user)));
			}

			var cntiFocRule = checkPresetAndDef(prstData, dicts.dictOpt["Cnti"], new FocalModeTag("Frequency", prstData.prbData.focPath, "CNTI"));

			if (lvoRules.Any(r => r.Allow != AllowModes.No) && (
						cntiFocRule.Allow == AllowModes.No
						|| IsBAllowed == AllowModes.No
						|| !checkCardioApplication(prstData.appSet.First().ApplicationId, dicts)))
			{
				lvoRules.ForEach(r => r.Allow = AllowModes.No);
			}
			FillOptionProbeApp(lvoRules);
			return lvoRules;
		}

		// Elasto: B only +  sElastographySettings + ModeId in folcalization files
		List<Uirule> ElastoRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			List<Uirule> elasoRules = new List<Uirule>();
			foreach (UserLevel user in getKnownUsers(prstData))
			{
				elasoRules.Add(checkPresetAndDef(prstData, dicts.dictOpt["ELAXTOBundle"], new userEnableTag("sElastographySettings", user)));
			}

			var ElastoFocRule = checkPresetAndDef(prstData, dicts.dictOpt["ELAXTOBundle"], new FocalModeTag("Frequency", prstData.prbData.focPath, "ELASTO"));

			if (elasoRules.Any(r => r.Allow != AllowModes.No) &&
					(IsBAllowed == AllowModes.No) || (ElastoFocRule.Allow == AllowModes.No))
			{
				elasoRules.ForEach(r => r.Allow = AllowModes.No);
			}
			FillOptionProbeApp(elasoRules);
			return elasoRules;
		}

		// TP-VIEW: Virtual Apex in Foc File, [RTC 27663 extend to convex]
		Uirule TrpzRule(PresetData prstData, Dictionaries dicts)
		{
			//var trpzRule = checkPresetAndDef(prstData, dicts.dictOption["TP-VIEW"], new tagList(new List<String>() { "fTrapezoidBEnable", "fTrapezoidCFMEnable" }));
			var trpzFocRule = checkPresetAndDef(prstData, dicts.dictOpt["TPView"], new FocalBlockTag("VirtualScan", prstData.prbData.focPath, "true"));
			FillOptionProbeApp(trpzFocRule);
			return trpzFocRule;
		}

		// MVIEW : no phased array + B only + shNumValidMViewSet>0
		Uirule MviewRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var mviewRule = checkPresetAndDef(prstData, dicts.dictOpt["MView"], new thresholdEnableTag("shNumValidMViewSet"));
			if (mviewRule.Allow != AllowModes.No &&
					(prstData.prbData.currentTransducerType == ProbeType.PHASE_ARRAY
					 || IsBAllowed == AllowModes.No))
			{
				mviewRule.Allow = AllowModes.No;
			}
			FillOptionProbeApp(mviewRule);
			return mviewRule;
		}

		// STRESSo ECHO : cardiac only + B only
		Uirule StressRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var stressrule = CardiacBonlyRule(prstData, IsBAllowed, dicts, "Stress");
			FillOptionProbeApp(stressrule);
			return stressrule;
		}

		// AutoEF : cardiac only + B only + Phase ArrayOnly
		Uirule AutoEFRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var arule = CardiacBonlyPhaseArrayRule(prstData, IsBAllowed, dicts, "AutoEF");
			FillOptionProbeApp(arule);
			return arule;
		}

		// AutoEFFA : cardiac only + B only + Phase ArrayOnly
		Uirule AutoEFFARule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var arule = CardiacBonlyPhaseArrayRule(prstData, IsBAllowed, dicts, "AutoEFFA");
			FillOptionProbeApp(arule);
			return arule;
		}

		// XSTRAIN: cardiac only + B only + Phase Array Only
		Uirule XstrainRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			Uirule result = BRule(prstData, IsBAllowed, dicts.dictOpt["XSTRAINBundle"]);
			Boolean IsXstrainApp = checkCardioHumanBundle(prstData.appSet.First().ApplicationId, dicts)
								   || checkApplication(prstData.appSet.First().ApplicationId, "CardioCanine", dicts)
								   || checkApplication(prstData.appSet.First().ApplicationId, "CardioEquine", dicts)
								   || checkApplication(prstData.appSet.First().ApplicationId, "CardioFeline", dicts);

			if (result.Allow != AllowModes.No && (!IsXstrainApp || prstData.prbData.currentTransducerType != ProbeType.PHASE_ARRAY))
			{
				result.Allow = AllowModes.No;
			}
			FillOptionProbeApp(result);
			return result;
		}

		Uirule AntiRibsRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var AntiribsRule = checkPresetAndDef(prstData, dicts.dictOpt["AntiRibs"], new thresholdEnableTag("bAntiRibsPercentileCnTI"));
			var AntiribsCntiRuleFundTei = checkPresetAndDef(prstData, dicts.dictOpt["AntiRibs"], new thresholdEnableTag("bAntiRibsPercentileFundTei"));
			AntiribsRule.Allow = AntiribsRule.Allow == AllowModes.Def || AntiribsCntiRuleFundTei.Allow == AllowModes.Def ?
								 AllowModes.Def : AllowModes.No;

			if (AntiribsRule.Allow != AllowModes.No &&
					(IsBAllowed == AllowModes.No || !checkApplication(prstData.appSet.First().ApplicationId, "Cardiac", dicts)))
			{
				AntiribsRule.Allow = AllowModes.No;
			}
			FillOptionProbeApp(AntiribsRule);
			return AntiribsRule;
		}

		// Vortex: cardiac only + B only + phase array
		Uirule VortexRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var VortexRule = CardiacBonlyPhaseArrayRule(prstData, IsBAllowed, dicts, "Vortex");
			FillOptionProbeApp(VortexRule);
			return VortexRule;
		}

		//BreastCalcification enabled only in Breast application with specific probes
		Uirule BreastCalcRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var probeApp = prstData.appSet.First();
			var brcrule = checkPresetAndDef(prstData, dicts.dictOpt["BreastCalc"], new directEnableTag("xfCea"));

			if (brcrule.Allow != AllowModes.No)
			{
				brcrule.Allow = (checkApplication(prstData.appSet.First().ApplicationId, "Breast", dicts)
								 || checkApplication(prstData.appSet.First().ApplicationId, "Thyroid", dicts)
								 || checkApplication(prstData.appSet.First().ApplicationId, "AbdominalCanine", dicts)
								 || checkApplication(prstData.appSet.First().ApplicationId, "AbdominalFeline", dicts)
								) ? AllowModes.Def : AllowModes.No;
			}

			FillOptionProbeApp(brcrule);
			return brcrule;
		}

		Uirule EasyTuningRule(PresetData prstData, Dictionaries dicts)
		{
			var EasyTuningRule = checkPresetAndDef(prstData, dicts.dictOpt["EasyTuning"], new tagList(new List<String>
			{
				"fIsEasyTuningBWFUNDActive",
				"fIsEasyTuningBWTEIActive",
				"fIsEasyTuningBWCNTIActive",
				"fIsEasyTuningBCFMActive"
			}));
			FillOptionProbeApp(EasyTuningRule);
			return EasyTuningRule;
		}

		Uirule AutoCFMRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var AutoCFMrule = checkPresetAndDef(prstData, dicts.dictOpt["AutoCFM"], new anyEnableTag("fAutoCFM_Supported"));
			FillOptionProbeApp(AutoCFMrule);
			return AutoCFMrule;
		}

		Uirule SticRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			Uirule sticrule = BRule(prstData, IsBAllowed, dicts.dictOpt["4DStic"]);
			if (sticrule.Allow != AllowModes.No && !checkApplication(prstData.appSet.First().ApplicationId, "ObGyn", dicts))
			{
				sticrule.Allow = AllowModes.No;
			}

			var ruleMoto = get_MotorizedRule("4DStic", prstData.prbData, dicts);

			if (sticrule.Allow != AllowModes.No && ruleMoto.Allow == AllowModes.No)
			{
				sticrule.Allow = AllowModes.No;
			}

			FillOptionProbeApp(sticrule);
			return sticrule;
		}

		// LIBRARIES: B only
		List<Uirule> LibraryRules(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		{
			var result = new List<Uirule>();
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["LibraryViewer"]));
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["81S29X1"]));      //RHEUMATOLOGY Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["81S27X1"]));      //EQUINE TENDON Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["81S37X1"]));      //EQUINE REPRO Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["81S22X2"]));      //ABDOMINAL Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["81S36X1"]));      //VASCULAR Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["Cuscvr"]));      //CAROTID Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["Uspath"]));      //USPATH Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["81S21X1"]));      //REG ANEST Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["81S29X2"]));      //PHYSIO Library
			result.Add(BRule(prstData, IsBAllowed, dicts.dictOpt["LivePreview"]));      //liver preview
			FillOptionProbeApp(result);
			return result;
		}

		// LIBRARY Rheumatology: B only
		//UIRule LibraryRheumaRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts)
		//{
		//  return BRule(prstData, IsBAllowed, dicts.dictOpt["81S29X1"]);
		//}

		// Cardiac only + B only + PhaseArray probe only
		Uirule CardiacBonlyPhaseArrayRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts, String optionName)
		{
			var arule = CardiacBonlyRule(prstData, IsBAllowed, dicts, optionName);
			if (arule.Allow != AllowModes.No && prstData.prbData.currentTransducerType != ProbeType.PHASE_ARRAY)
			{
				arule.Allow = AllowModes.No;
			}
			return arule;
		}

		// Cardiac only + B only
		Uirule CardiacBonlyRule(PresetData prstData, AllowModes IsBAllowed, Dictionaries dicts, String optionName)
		{
			Uirule result = BRule(prstData, IsBAllowed, dicts.dictOpt[optionName]);
			if (result.Allow != AllowModes.No && (
						!checkCardioApplication(prstData.appSet.First().ApplicationId, dicts)))
			{
				result.Allow = AllowModes.No;
			}
			return result;
		}

		// B only rule
		Uirule BRule(PresetData prstData, AllowModes IsBAllowed, Int32 optId)
		{
			Uirule bRule = buildProbeAppOptRule(prstData.prbData.currentProbe.Id, prstData.prbData.currentTransducerType, prstData.appSet.First().ApplicationId, optId);
			bRule.Allow = IsBAllowed;
			return bRule;
		}

		private List<UserLevel> getKnownUsers(PresetData prstData)
		{
			List<UserLevel> result = new List<UserLevel>();
			var currentSupportedKeys = prstData.buffer.probeBindLst.Single(x => x.probe.SaleName == prstData.prbData.currentProbe.SaleName).probeInfoLst
									   .Single(y => y.settingsFam.Id == prstData.prbData.currentProbeSettings.SettingsFamilyId).SupportedKeys;
			if (currentSupportedKeys.fHwKeyProduction)
			{
				result.Add(UserLevel.Standard);
			}
			if (currentSupportedKeys.fHwKeyRSDept)
			{
				result.Add(UserLevel.RS);
			}
			if (currentSupportedKeys.fHwKeyService)
			{
				result.Add(UserLevel.Service);
			}
			if (currentSupportedKeys.fHwKeyTesting)
			{
				result.Add(UserLevel.Test);
			}
			if (currentSupportedKeys.fHwKeyWIP)
			{
				result.Add(UserLevel.WIP);
			}
			return result;
		}
		private Boolean checkCardioApplication(Int32 appid, Dictionaries dicts)
		{
			return checkCardioHumanBundle(appid, dicts) || checkCardioVetBundle(appid, dicts);
		}

		private Boolean checkCardioHumanBundle(Int32 appid, Dictionaries dicts)
		{
			return checkApplication(appid, "CardioBundle", dicts);
		}

		private Boolean checkCardioVetBundle(Int32 appid, Dictionaries dicts)
		{
			return checkApplication(appid, "CardioVet", dicts);
		}

		// checks if the application or its parent is correspondent to the input name
		private Boolean checkApplication(Int32 appid, String name, Dictionaries dicts)
		{

			if (appid == dicts.dictApp[name])
			{
				return true;
			}

			var parentNames = Provider.GetValueOrDefault_Class(dicts.dictParents, appid);
			foreach (var parentName in parentNames)
			{
				if (name == parentName)
				{
					return true;
				}
			}

			return false;
		}

		/// reads Xdocument of the preset file and the default files in the list.
		/// if the default files are not coherent an exception is thrown
		private void ReadAppDefaults(String basePath, List<ProbePreset> appLst, TransducerPosition biplanaType,
									 Dictionary<String, XDocument> pdef, Dictionary<String, XDocument> probealldef)
		{
			// load the XDocument of the preset files
			foreach (var app in appLst)
			{
				// build full path
				String fullPath = basePath;
				if (!String.IsNullOrEmpty(app.PresetFolder))
				{
					fullPath += "\\" + app.PresetFolder;
				}
				app.presetFile = XDocument.Load(fullPath + "\\" + app.getPresetFile(biplanaType));
				app.defaultName = fullPath + "\\" + app.presetFile.Element("VirtualRoot").Attribute("defaultfile").Value;
				if (!pdef.Any(x => x.Key == app.defaultName))
				{
					pdef.Add(app.defaultName, XDocument.Load(app.defaultName));
				}
				var probealldefcurrent = pdef[app.defaultName];
				app.AllProbeDefaultName = System.IO.Path.GetFullPath(fullPath + "\\" + probealldefcurrent.Element("VirtualRoot").Attribute("defaultfile").Value);
				if (!probealldef.Any(x => x.Key == app.AllProbeDefaultName))
				{
					probealldef.Add(app.AllProbeDefaultName, XDocument.Load(app.AllProbeDefaultName));
				}
			}
		}
		// checks the data file associated with the probe
		private Uirule checkDataFile(ProbeDataset prbData, Int32 optId, IProbeXmlTags tagSet)
		{
			if (tagSet.isPresentInDoc(prbData.dataFile))    // if all tag are present in the document
			{
				return tagSet.generateRule(buildProbeAppOptRule(prbData.currentProbe.Id, prbData.currentTransducerType, null, optId), prbData.dataFile);
			}
			else
			{
				return tagSet.handleTagMiss(buildProbeAppOptRule(prbData.currentProbe.Id, prbData.currentTransducerType, null, optId));
			}
		}

		// checks the input preset document and if the tag is not found it checks for the tag in the default fie
		private Uirule checkPresetAndDef(PresetData prstData, Int32 optId, IProbeXmlTags tagSet)
		{
			List<Uirule> presetRules = new List<Uirule>();
			Int32 numUsedPreset = 0;

			// loop all presets associated to the application
			foreach (var preset in prstData.appSet)
			{
				try
				{
					if (tagSet.isPresentInDoc(preset.presetFile))    // if all tag are present in the preset
					{
						numUsedPreset++;
						presetRules.Add(tagSet.generateRule(buildProbeAppOptRule(preset.ProbeId, prstData.prbData.currentTransducerType, preset.ApplicationId, optId),
															preset.presetFile));
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error reading preset " + preset.getPresetFile(preset.ProbeId, prstData.prbData.currentTransducerType, p_probeTransducers), ex);
				}
			}
			//checkAndWriteLog(prstData, presetRules, optId);

			// if presets did not contain the tag then the rule is generated from the default file
			if (numUsedPreset == 0)
			{
				return generateDefaultRule(prstData, optId, tagSet);
			}
			else
			{
				// if not all presets have been used, the missing ones inherit from the default
				if (numUsedPreset != prstData.appSet.Count)
				{
					presetRules.Add(generateDefaultRule(prstData, optId, tagSet));
				}
				//checkAndWriteLog(prstData, presetRules, optId);
				var allowRule = presetRules.FirstOrDefault(g => g.Allow != AllowModes.No);
				if (allowRule != null)
				{
					return allowRule;
				}
				else
				{
					return presetRules.FirstOrDefault();
				}
			}
		}

		/// replicate rules for each logical mode in the settings family: replicate rules only if the probe has available applications for that model
		/// and in case of rule on the application, it is replicated only if the application type is the same as the mode type (human/vet)
		private IEnumerable<Uirule> replicateRules(List<Uirule> baseRules, Int32 SettingsFamilyId, DBBuffer _buffer)
		{
			foreach (var baseRule in baseRules)
			{
				Application application = null;
				if (baseRule.ApplicationId != null)
				{
					application = _buffer.p_applications.Single(a => a.Id == baseRule.ApplicationId);
				}

				foreach (var model in getLogicalModels(SettingsFamilyId, _buffer))
				{
					if (application == null || model.Type == application.AppType)
					{
						if (hasProbeApps(baseRule.ProbeId, SettingsFamilyId, model, _buffer))
						{
							Uirule tmpRule = new Uirule(baseRule);
							tmpRule.LogicalModelId = model.Id;
							yield return tmpRule;
						}
					}
				}
			}
		}

		// forces rules to replicate
		private IEnumerable<Uirule> ForceReplicateRules(List<Uirule> baseRules, Int32 SettingsFamilyId, DBBuffer _buffer)
		{
			foreach (var baseRule in baseRules)
			{
				foreach (var model in getLogicalModels(SettingsFamilyId, _buffer))
				{
					Uirule tmpRule = new Uirule(baseRule);
					tmpRule.LogicalModelId = model.Id;
					yield return tmpRule;
				}
			}
		}


		// checks if the probe has any valid application for the input model
		private bool hasProbeApps(Int32? probeId, Int32 SettingsFamilyId, LogicalModel model, DBBuffer _buffer)
		{
			return _buffer.p_probe_Apps.Where(x => x.ProbeId == probeId && x.SettingsId == SettingsFamilyId).Any(x => x.app.AppType == model.Type);
		}

		private Uirule generateDefaultRule(PresetData prstData, Int32 optId, IProbeXmlTags tagSet)
		{
			var probeApp = prstData.appSet.First();
			if (tagSet.isPresentInDoc(prstData.defaultFile))
			{
				return tagSet.generateRule(buildProbeAppOptRule(probeApp.ProbeId, prstData.prbData.currentTransducerType, probeApp.ApplicationId, optId), prstData.defaultFile);
			}
			if (tagSet.isPresentInDoc(prstData.ProbeDefaultAllFile))
			{
				return tagSet.generateRule(buildProbeAppOptRule(probeApp.ProbeId, prstData.prbData.currentTransducerType, probeApp.ApplicationId, optId),
										   prstData.ProbeDefaultAllFile);
			}
			return tagSet.handleTagMiss(buildProbeAppOptRule(probeApp.ProbeId, prstData.prbData.currentTransducerType, probeApp.ApplicationId, optId));
		}

		// checks if there are any incoherences in the preset rules and write them down in the LogFile
		//private void checkAndWriteLog(PresetData prstData, List<UIRule> presetRules, Int32 optId)
		//{
		//  if (presetRules.GroupBy(x => x.Allow).Count() > 1)
		//  {
		//      var prs = prstData.appSet.FirstOrDefault();
		//      Preset_Incoherences.InsertOnSubmit(new Preset_Incoherences(_ProbeId: prs.ProbeId, _ApplicationId: prs.ApplicationId,
		//                                         _OptionId: optId, _SettingsFamilyId: prstData.prbData.currentProbeSettings.SettingsFamilyId, _UserLevel: presetRules.First().UserLevel));
		//  }
		//}

		// creates an incomplete rule templated with the information of probe application and option of the iteration
		private Uirule buildProbeAppOptRule(Int32? _probeId, ProbeType? _TransducerType, Int32? _ApplicationId, Int32? _optionId)
		{
			return new Uirule()
			{
				RuleOrigin = ruleOrigins.ProbeXml,
				ApplicationId = _ApplicationId,
				ProbeId = _probeId,
				TransducerType = _TransducerType, 
				OptionId = _optionId,
			};
		}

		// LogicalModel_License rules: the License or the feature can be specified in the first column
		public void readLogModLicRules(DBBuffer _buffer, Dictionary<string, int> dictLicenses, Paths Paths, string logFileXml2DB)
		{
			var fileName = "LogModLicense.txt";
			Provider.WriteImportLogFormat(logFileXml2DB, fileName);
			List<Uirule> resultRules = new List<Uirule>();
			string modelName = "", featureName = "";
			try
			{
				List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
				List<string> ModelLicenses = file[0];
				foreach (List<string> riga in file.Skip(1))
				{
					var LicenseId = Provider.GetValueOrDefault_Stuct(dictLicenses, riga[0].Trim());
					Feature feature = null;
					if (LicenseId != null)
					{
						feature = _buffer.p_features.SingleOrDefault(f => f.LicenseId == LicenseId);    // License specified
					}
					else
					{
						feature = _buffer.p_features.SingleOrDefault(f => f.Name == riga[0].Trim());    // Feature specified
					}
					if (feature == null)
					{
						throw new Exception(riga[0] + " is not a feature");
					}
					featureName = feature.Name;
					Application app = _buffer.p_applications.SingleOrDefault(a => a.FeatureId == feature.Id);
					Option opt = _buffer.p_option.SingleOrDefault(o => o.FeatureId == feature.Id);

					if (app == null && opt == null)
					{
						throw new Exception(riga[0] + " is not an application or an option.");
					}

					List<Int32> availModelIds = new List<Int32>();

					for (int col = 1; col < riga.Count; col++)
					{
						var model = _buffer.p_logicalModels.SingleOrDefault(m => m.LicenseId == dictLicenses[ModelLicenses[col]]);
						modelName = model.Name;
						var tmpRule = new Uirule()
						{
							RuleOrigin = ruleOrigins.LicenseManager,
							LogicalModelId = model.Id
						};
						if (app != null)
						{
							tmpRule.ApplicationId = app.Id;
						}
						else
						{
							tmpRule.OptionId = opt.Id;
						}
						var userRules = setAllowance(tmpRule, riga[col]);
						resultRules.AddRange(userRules);
						if (resultRules.Any(r => r.Allow != AllowModes.No))
						{
							availModelIds.Add(model.Id);
						}
					}
				}
				this.BulkInsert(resultRules);

			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName, ex);
			}
		}


		//private static int GetMaxClassIndex(Dictionary<int, List<UIRule>> classList)
		//{
		//  if (classList.Count == 0)
		//  {
		//      return 0;
		//  }
		//  else
		//  {
		//      return classList.Max(x => x.Key) + 1;
		//  }
		//}


		public void ReadVersion(DBBuffer _buffer, Paths Paths, string logFileXml2DB)
		{
			var result = new VersionAssociationLists();
			int RowIndex = -2;
			int ColumnIndex = -2;
			var fileName = "DeviceSWverForCountries.txt";
			Provider.WriteImportLogFormat(logFileXml2DB, fileName);
			try
			{
				var file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
				var ModelParNumbers = file[1].ToList();
				var LatestVersion = GetLatestVersion(_buffer.p_MinorVersionAssociation);

				var ModelsWithoutPN = _buffer.p_logicalModels.Where(l => !_buffer.p_PartNumbersAssociations.Where(x => x.FeatureId == null)
									  .Select(x => x.LogicalModelId).Contains(l.Id));

				for (RowIndex = 3; RowIndex < file.Count; RowIndex++)
				{
					var countryCode = file[RowIndex][0].Trim();
					for (ColumnIndex = 2; ColumnIndex < file[RowIndex].Count; ColumnIndex++)
					{
						var recognizedPartNumber = _buffer.p_PartNumbersAssociations.Where(p => p.PartNumber == ModelParNumbers[ColumnIndex].Trim()).ToList();
						if (recognizedPartNumber != null)
						{
							int version = LatestVersion;
							var readversion = file[RowIndex][ColumnIndex].Trim().ToLowerInvariant();
							if (!string.IsNullOrEmpty(readversion) && readversion != "maj" && readversion != "major")
							{
								var numericVersion = ComparableFields.GetNumericVersion(readversion);
								if (numericVersion != null)
								{
									version = ComparableFields.GetMajorFromNumericVersion((int)numericVersion);
								}
								else
								{
									throw new ApplicationException(readversion + " could not be parsed");
								}
							}

							foreach (var pnr in recognizedPartNumber)
							{
								if (pnr.LogicalModelId == null)
								{
									throw new ApplicationException(recognizedPartNumber.ToString() + " was read as model but part number is associated to any model");
								}
								var Association = VersionAssociationCreator.CreateCountryVersionAssociation(countryCode, (int)pnr.LogicalModelId, _buffer);
								Association.AddToRegister(result, version);
							}
						}
					}

					// set major to models without part number
					foreach (var model in ModelsWithoutPN)
					{
						var Association = VersionAssociationCreator.CreateCountryVersionAssociation(countryCode, model.Id, _buffer);
						Association.AddToRegister(result, LatestVersion);
					}
				}

				result.CheckConsistency(_buffer, logFileXml2DB);
				this.BulkInsert(result.CountryVersions);
			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName + " at row " + (RowIndex + 1).ToString() + ", column " + (ColumnIndex + 1).ToString(), ex);
			}
		}

		//private IEnumerable<int> GetVetColumns(List<string> HeaderNames, DBBuffer buffer, int ModelOffset)
		//{
		//  for (int i = ModelOffset; i < HeaderNames.Count; i++)
		//  {
		//      if (buffer.p_logicalModels.Single(m => m.Name == HeaderNames[i]).Type == SystemEnvironment.Veterinary)
		//      {
		//          yield return i;
		//      }
		//  }
		//}

		// latest is the highest value in MINOR-VERSION file
		public int GetLatestVersion(List<MinorVersionAssociation> associations)
		{
			return associations.Max(a => a.Major);
		}

		/// sets allowance depending on the string value read from file
		public static List<Uirule> setAllowance(Uirule baseRule, String allowValue)
		{
			Dictionary<string, AllowModes> dictAllowModes = new Dictionary<string, AllowModes>
			{
				{ "A", AllowModes.A },
				{ "Def", AllowModes.Def },
				{ "No", AllowModes.No },
			};

			if (allowValue == "A(R&D)")
			{
				var result = new List<Uirule>();

				var rsrule = new Uirule(baseRule);
				rsrule.Allow = AllowModes.A;
				rsrule.UserLevel = UserLevel.RS;
				result.Add(rsrule);

				var testrule = new Uirule(baseRule);
				testrule.Allow = AllowModes.No;
				testrule.UserLevel = UserLevel.Test;
				result.Add(testrule);


				var strule = new Uirule(baseRule);
				strule.Allow = AllowModes.No;
				strule.UserLevel = UserLevel.Standard;
				result.Add(strule);


				var srvrule = new Uirule(baseRule);
				srvrule.Allow = AllowModes.No;
				srvrule.UserLevel = UserLevel.Service;
				result.Add(srvrule);


				var wiprule = new Uirule(baseRule);
				wiprule.Allow = AllowModes.No;
				wiprule.UserLevel = UserLevel.WIP;
				result.Add(wiprule);

				return result;
			}
			else
			{
				baseRule.Allow = dictAllowModes[allowValue];
				return new List<Uirule> { baseRule };
			}
		}
	}
}
