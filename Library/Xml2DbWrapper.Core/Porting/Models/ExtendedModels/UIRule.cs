﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

//fg01122015

namespace Xml2DbMapper.Core.Models
{
    public partial class Uirule : FeaturesFields
	{
		public Uirule()
		{
		}

		// copy constructor
		public Uirule(Uirule copy)
		{
			Allow = copy.Allow;
			RuleName = copy.RuleName;
			PhysicalModelId = copy.PhysicalModelId;
			CertifierId = copy.CertifierId;
			LogicalModelId = copy.LogicalModelId;
			ApplicationId = copy.ApplicationId;
			OptionId = copy.OptionId;
			UserLevel = copy.UserLevel;
			Version = copy.Version;
			CountryId = copy.CountryId;
			DistributorId = copy.DistributorId;
			ProbeId = copy.ProbeId;
			TransducerType = copy.TransducerType;
			KitId = copy.KitId;
			RuleOrigin = copy.RuleOrigin;
		}

		/// <summary>
		/// Import UIRules from file
		///
		/// *THIS CODE SHOULD BE AUTOMATICALLY GENERATED BY THE TEXT TEMPLATE*
		///
		/// </summary>
		public static List<Uirule> Import(String fileName, DBBuffer _buffer, ruleOrigins _rOrigin, Paths Paths)
		{
			List<Uirule> result = new List<Uirule>();
			try
			{
				List<List<String>> file = Provider.CreateFile(Paths.importFilePath + fileName);
				Dictionary<string, AllowModes> dictAllowModes = new Dictionary<string, AllowModes>
				{
					{ "A", AllowModes.A },
					{ "Def", AllowModes.Def },
					{ "No", AllowModes.No },
				};
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					result.Add(new Uirule
					{
						Allow = dictAllowModes[riga[0]],
						RuleName = String.IsNullOrEmpty(riga[1]) ? null : riga[1],
						PhysicalModelId = String.IsNullOrEmpty(riga[2]) ? null : (Int32?)_buffer.p_physicalModels.Single(pm => pm.Description == riga[2]).Id,
						LogicalModelId = String.IsNullOrEmpty(riga[3]) ? null : (Int32?)_buffer.p_logicalModels.Single(lm => lm.Name == riga[3]).Id,
						ApplicationId = String.IsNullOrEmpty(riga[4]) ? null : (Int32?)_buffer.p_applications
							.ToList()
							.Join(_buffer.p_features, o => o.FeatureId, f => f.Id, (o, f) => new
							{
								app = o,
								feature = f
							})
							.Single(fo => fo.feature.NameInCode == riga[4]).app.Id,
						OptionId = String.IsNullOrEmpty(riga[5]) ? null : (Int32?)_buffer.p_option
							.ToList()
							.Join(_buffer.p_features, o => o.FeatureId, f => f.Id, (o, f) => new
							{
								option = o,
								feature = f
							})
							.Single(fo => fo.feature.NameInCode == riga[5]).option.Id,
						UserLevel = String.IsNullOrEmpty(riga[6]) ? null : (UserLevel?)Convert.ToInt16(riga[6]),
						Version = String.IsNullOrEmpty(riga[7]) ? null : riga[7],
						ProbeId = String.IsNullOrEmpty(riga[8]) ? null : (Int32?)_buffer.p_probes.Single(p => p.SaleName == riga[8]).Id,
						CertifierId = String.IsNullOrEmpty(riga[9]) ? null : (Int32?)_buffer.p_certifiers.Single(ce => ce.Name == riga[9]).Id,
						CountryId = String.IsNullOrEmpty(riga[10]) ? null : (Int32?)_buffer.p_countries.Single(c => c.Name == riga[10]).Id,
						KitId = String.IsNullOrEmpty(riga[11]) ? null : (Int32?)_buffer.p_biospyKits.Single(k => k.Name == riga[11]).Id,
						TransducerType = String.IsNullOrEmpty(riga[12]) ? null : (ProbeType?)Convert.ToInt16(riga[12]),
						DistributorId = String.IsNullOrEmpty(riga[13]) ? null : (Int32?)_buffer.p_Distributors.Single(k => k.Name == riga[13]).Id,
						RuleOrigin = _rOrigin
					});
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("error while reading UIRules: " + ex.Message);
			}
			return result;
		}

		public static List<Uirule> ImportVersionRules(DBBuffer _buffer, Paths Paths, string log)
		{
			List<Uirule> result = new List<Uirule>();
			var filename = "REQUIREMENTS.txt";
			Provider.WriteImportLogFormat(log, filename);
			try
			{
				var file = Provider.CreateFile(Paths.importFilePath + "\\" + filename);
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					var tmpRule = new Uirule();
					tmpRule.Allow = AllowModes.No;
					tmpRule.RuleOrigin = ruleOrigins.Requirements;

					if (!string.IsNullOrEmpty(riga[0]))
					{
						tmpRule.Version = ComparableFields.GetStringVersion(ComparableFields.GetNumericVersionFromMajor(Convert.ToDouble(riga[0])));
					}

					FillRuleFromFile(tmpRule, 1, riga, _buffer);

					IsOkToAddRule(tmpRule, _buffer);

					result.Add(tmpRule);
				}
			}
			catch (Exception ex)
			{
				throw new ParsingException(filename, ex);
			}
			return result;
		}

		// Gibiino [RTC 12169]
		private static void IsOkToAddRule(Uirule uirule, DBBuffer buffer)
		{
			// Make sure the AlwaysPresent features are not blocked by the version
			FeatureRegister.FeatureType FeatureType;
			var feature = uirule.GetFeature(buffer, out FeatureType);
			if (feature.AlwaysPresent == true)
			{
				var tmpNormalRules = uirule.ToNormalRule(buffer);

				var dummyRule = new NormalRule();
				dummyRule.SetFieldFromType(FeatureType, uirule);
				dummyRule.Version = 0;

				// Checks if the only specified fields are feature and version
				foreach (var trule in tmpNormalRules)
				{
					if (ComparableFields.isContained(dummyRule, trule))
					{
						throw new Exception(buffer.p_features.Single(o => o.Id == feature.Id).Name + " feature cannot be blocked unti version " + uirule.Version);
					}
				}
			}
		}

		public static List<Uirule> ImportRegulatoryRules(DBBuffer _buffer, Paths Paths, string log)
		{
			var result = new List<Uirule>();
			var fileName = "RegulatoryExceptionsMatrix.txt";
			Provider.WriteImportLogFormat(log, fileName);
			try
			{
				var file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
				// first line contains column names
				foreach (List<string> riga in file.Skip(2))
				{
					var tmpRule = new Uirule();
					tmpRule.Allow = AllowModes.No;
					tmpRule.RuleOrigin = ruleOrigins.Regulatory;

					// parse rule only for recognized model (avoid MRI models)
					var stringmodel = riga[1].Trim();
					var model = _buffer.p_logicalModels.SingleOrDefault(l => l.Name == stringmodel);
					if (model != null)
					{
						tmpRule.LogicalModelId = model.Id;
					}
					else
					{
						continue;
					}

					var countrycode = riga[0].Trim();
					var country = _buffer.p_countries.SingleOrDefault(x => x.Code == countrycode);
					if (country != null)
					{
						tmpRule.CountryId = country.Id;
					}

					var stringfeature = riga[3].Trim();
					var feature = _buffer.p_features.SingleOrDefault(f => f.Name == stringfeature);
					if (feature != null)
					{
						var application = _buffer.p_applications.SingleOrDefault(x => x.FeatureId == feature.Id);
						var option = _buffer.p_option.SingleOrDefault(x => x.FeatureId == feature.Id);
						var kit = _buffer.p_biospyKits.SingleOrDefault(x => x.FeatureId == feature.Id);
						if (application != null)
						{
							tmpRule.ApplicationId = application.Id;
						}
						if (option != null)
						{
							tmpRule.OptionId = option.Id;
						}
						if (kit != null)
						{
							tmpRule.KitId = kit.Id;
						}
					}

					var probename = riga[4].Trim();
					var probe = _buffer.p_probes.SingleOrDefault(p => p.SaleName == probename);
					if (probe != null)
					{
						tmpRule.ProbeId = probe.Id;
					}

					result.Add(tmpRule);
				}
			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName, ex);
			}
			return result;
		}

		// this file is tipically used to block some features in countries where a patent is protecting them
		public static List<Uirule> ImportRDBlockingRules(DBBuffer _buffer, Paths Paths, string log)
		{
			var result = new List<Uirule>();
			var fileName = "BLOCKED_FEATURES-COUNTRIES_RD.txt";
			Provider.WriteImportLogFormat(log, fileName);
			List<string> CurrentRow = null;
			try
			{
				var file = Provider.CreateFile(Paths.importFilePath + "\\" + fileName);
				// first line contains column names
				foreach (List<string> riga in file.Skip(1))
				{
					CurrentRow = riga;
					var tmpRule = new Uirule();
					tmpRule.Allow = AllowModes.No;
					tmpRule.RuleOrigin = ruleOrigins.RDCountryBlock;

					FillRuleFromFile(tmpRule, 0, riga, _buffer);

					var l_Country = riga[6];
					var l_Distributor = riga[7];
					var l_Certifier = riga[8];
					var IsCountryPresent = !string.IsNullOrEmpty(riga[6]);
					var IsCertifierPresent = !string.IsNullOrEmpty(riga[8]);
					var IsUserPresent = !string.IsNullOrEmpty(riga[9]);

					if (IsCountryPresent && IsCertifierPresent)
					{
						throw new Exception("Cannot specify both cerifier and country: " + l_Country + ", " + l_Certifier);
					}

					if (IsCountryPresent)
					{
						tmpRule.CountryId = _buffer.p_countries.Single(c => c.Code == l_Country).Id;
					}

					if (!string.IsNullOrEmpty(l_Distributor))
					{
						tmpRule.DistributorId = _buffer.p_Distributors.Single(c => c.Code == l_Distributor).Id;
					}

					if (IsCertifierPresent)
					{
						tmpRule.CertifierId = _buffer.p_certifiers.Single(c => c.Code == l_Certifier).Id;
					}

					if (IsUserPresent)
					{
						tmpRule.UserLevel = Provider.FromStringToEnum<UserLevel>(riga[9], Xml2DbMapper.Core.Porting.Contract.Enums.UserLevel.Unknown);
					}

					result.Add(tmpRule);
				}
			}
			catch (Exception ex)
			{
				throw new ParsingException(fileName, ex, CurrentRow);
			}
			return result;
		}

		//private static List<UIRule> GetRegulatoryRules(List<UIRule> BaseRules, string RegulatoryFeatureName, DBBuffer _buffer)
		//{
		//  var result = new List<UIRule>();
		//  var ChildFeatureIDs = _buffer.p_RegualtoryFeatures.Where(rf => rf.Name == RegulatoryFeatureName).Select(x => x.FeatureId).ToList();
		//  foreach (var BaseRule in BaseRules)
		//  {
		//      foreach (var ChildFeatureID in ChildFeatureIDs)
		//      {
		//          var tmpRule = new UIRule(BaseRule);
		//          _buffer.SetRuleFeatureField(tmpRule, ChildFeatureID);
		//          result.Add(tmpRule);
		//      }
		//  }
		//  return result;
		//}

		private static void FillRuleFromFile(Uirule tmpRule, int StartingIndex, List<string> riga, DBBuffer _buffer)
		{
			if (!string.IsNullOrEmpty(riga[StartingIndex]))
			{
				tmpRule.OptionId = _buffer.p_option.Single(p => p.Name == riga[StartingIndex]).Id;
			}
			if (!string.IsNullOrEmpty(riga[StartingIndex + 1]))
			{
				tmpRule.ApplicationId = _buffer.p_applications.Single(p => p.Name == riga[StartingIndex + 1]).Id;
			}
			if (!string.IsNullOrEmpty(riga[StartingIndex + 2]))
			{
				tmpRule.ProbeId = _buffer.p_probes.Single(p => p.SaleName == riga[StartingIndex + 2]).Id;
			}
			if (!string.IsNullOrEmpty(riga[StartingIndex + 3]))
			{
				tmpRule.KitId = _buffer.p_biospyKits.Single(p => p.Name == riga[StartingIndex + 3]).Id;
			}
			if (!string.IsNullOrEmpty(riga[StartingIndex + 4]))
			{
				tmpRule.LogicalModelId = _buffer.p_logicalModels.Single(p => p.Name == riga[StartingIndex + 4]).Id;
			}
			if (!string.IsNullOrEmpty(riga[StartingIndex + 5]))
			{
				tmpRule.PhysicalModelId = _buffer.p_physicalModels.Single(p => p.Description == riga[StartingIndex + 5]).Id;
			}
		}


		// generate rules for options associated to probes (and related forbidden probes)
		public static List<Uirule> ImportOptionProbes(DBBuffer _buffer, Paths Paths, string log)
		{
			List<Uirule> result = new List<Uirule>();
			var filename = "OptionProbeRules.txt";
			Provider.WriteImportLogFormat(log, filename);
			try
			{
				List<List<String>> file = Provider.CreateFile(Paths.importFilePath + "\\" + filename);
				var Options = file.Skip(1).GroupBy(r => r[0]);  // first line contains column names

				foreach (var OptionGroup in Options)
				{
					var OptionId = _buffer.p_option.Single(o => o.Name == OptionGroup.Key).Id;
					var AvailableProbes = OptionGroup.ToList().Join(_buffer.p_probes, r => r[1], p => p.SaleName, (r, p) => p).ToList();
					var ForbiddenProbes = _buffer.p_probes.Except(AvailableProbes).ToList();

					// allow for all listed probes
					foreach (var AvailableProbe in AvailableProbes)
					{
						result.Add(new Uirule()
						{
							Allow = AllowModes.Def,
							OptionId = OptionId,
							ProbeId = AvailableProbe.Id,
							RuleOrigin = ruleOrigins.InputOptionProbeRule
						});
					}

					// forbid the missing probes
					foreach (var forbiddenProbe in ForbiddenProbes)
					{
						result.Add(new Uirule()
						{
							Allow = AllowModes.No,
							OptionId = OptionId,
							ProbeId = forbiddenProbe.Id,
							RuleOrigin = ruleOrigins.InputOptionProbeRule
						});
					}
				}
			}
			catch (Exception ex)
			{
				throw new ParsingException(filename, ex);
			}
			return result;
		}


		// check if the rules are equivalent excluding the country
		public Boolean IsEquivalentNoCountry(Uirule f2)
		{

			if (this.Allow != f2.Allow)
			{
				return false;
			}

			if (this.ApplicationId != f2.ApplicationId)
			{
				return false;
			}

			if (this.LogicalModelId != f2.LogicalModelId)
			{
				return false;
			}

			if (this.OptionId != f2.OptionId)
			{
				return false;
			}

			if (this.UserLevel != f2.UserLevel)
			{
				return false;
			}

			if (this.Version != f2.Version)
			{
				return false;
			}

			if (this.ProbeId != f2.ProbeId)
			{
				return false;
			}

			if (this.TransducerType != f2.TransducerType)
			{
				return false;
			}

			if (this.KitId != f2.KitId)
			{
				return false;
			}

			if (this.PhysicalModelId != f2.PhysicalModelId)
			{
				return false;
			}

			return true;
		}


		/// <summary>
		/// returns a normal rule equivalent to the UI RULE
		///
		/// *THIS CODE SHOULD BE AUTOMATICALLY GENERATED BY THE TEXT TEMPLATE*
		///
		/// </summary>
		public NormalRule ToSingleNormalRule()
		{
			return new NormalRule
			{
				Allow = this.Allow,
				LogicalModelId = this.LogicalModelId,
				ApplicationId = this.ApplicationId,
				OptionId = this.OptionId,
				UserLevel = this.UserLevel,
				Version = ComparableFields.GetNumericVersion(this.Version),
				UiruleId = this.Id,
				ProbeId = this.ProbeId,
				TransducerType = this.TransducerType,
				KitId = this.KitId,
				CountryId = this.CountryId,
				DistributorId = this.DistributorId
			};
		}

		/// converts the UIRule to the list of corresponding normalized rules
		public List<NormalRule> ToNormalRule(DBBuffer _buffer)
		{
			List<NormalRule> nrules = new List<NormalRule>();
			nrules.Add(ToSingleNormalRule());
			var functionList = new List<Func<NormalRule, List<NormalRule>>>
			{
				nr => { return cloneFromPhysModel(nr, _buffer); },
				nr => { return cloneFromCertifier(nr, _buffer); },
				nr => { return cloneFromParentApplication(nr, _buffer); },
				nr => { return cloneFromParentOption(nr, _buffer); }
			};

			nrules = ToNormalRule(functionList, nrules);
			return nrules;
		}

		/// recursively clones the rules depending on the input parameters in the UIRule
		private List<NormalRule> ToNormalRule(List<Func<NormalRule, List<NormalRule>>> Actions, List<NormalRule> currentRules)
		{
			if (Actions.Count == 0)
			{
				return currentRules;
			}
			var currentAction = Actions.First();
			List<NormalRule> newRules = new List<NormalRule>();
			foreach (var currentRule in currentRules)
			{
				newRules.AddRange(currentAction(currentRule));
			}
			return ToNormalRule(Actions.Skip(1).ToList(), newRules);
		}

		#region Cloning Functions

		/// Clone Normalized Rules for each logical model included in the physical model
		private List<NormalRule> cloneFromPhysModel(NormalRule nr, DBBuffer _buffer)
		{
			if (this.PhysicalModelId.HasValue)
			{
				var logicalModelsId = _buffer.p_logicalModels.Where(lm => lm.PhModId == this.PhysicalModelId).Select(x => x.Id).ToList();
				if (logicalModelsId.Count > 0)
				{
					List<NormalRule> cloni = new List<NormalRule>();
					foreach (var lmodel in logicalModelsId)
					{
						NormalRule tmpRule = new NormalRule(nr);
						tmpRule.LogicalModelId = lmodel;
						cloni.Add(tmpRule);
					}
					return cloni;
				}
			}

			// if nothing was to be done, return the input rule itself
			return new List<NormalRule> { nr };
		}


		/// Clone rule for each child of the Application bundle
		private List<NormalRule> cloneFromParentApplication(NormalRule nr, DBBuffer _buffer)
		{
			if (this.ApplicationId.HasValue)
			{
				var childAppId = _buffer.p_bundles.Where(x => x.ParentFeatureId == _buffer.p_applications.Single(a => a.Id == this.ApplicationId).FeatureId).
								 Join(_buffer.p_applications, b => b.FeatureId, a => a.FeatureId, (b, a) => a.Id).ToList();

				if (childAppId.Count > 0)
				{
					List<NormalRule> cloni = new List<NormalRule>();
					cloni.Add(nr);    // add the rule for the parent
					foreach (var kidId in childAppId)
					{
						NormalRule tmpRule = new NormalRule(nr);
						tmpRule.ApplicationId = kidId;
						cloni.Add(tmpRule);
					}
					return cloni;
				}
			}

			// if nothing was to be done, retunr the input rule itself
			return new List<NormalRule> { nr };
		}


		/// Clone rule for each child of the Option bundle
		private List<NormalRule> cloneFromParentOption(NormalRule nr, DBBuffer _buffer)
		{
			if (this.OptionId.HasValue)
			{
				var childOptionId = _buffer.p_bundles.Where(x => x.ParentFeatureId == _buffer.p_option.Single(o => o.Id == this.OptionId).FeatureId).
									Join(_buffer.p_option, b => b.FeatureId, o => o.FeatureId, (b, o) => o.Id).ToList();

				if (childOptionId.Count > 0)
				{
					List<NormalRule> cloni = new List<NormalRule>();
					cloni.Add(nr);    // add the rule for the parent
					foreach (var kidId in childOptionId)
					{
						NormalRule tmpRule = new NormalRule(nr);
						tmpRule.OptionId = kidId;
						cloni.Add(tmpRule);
					}
					return cloni;
				}
			}

			// if nothing was to be done, retunr the input rule itself
			return new List<NormalRule> { nr };
		}

		/// Clone Normalized Rules for each country associated with the certifier
		private List<NormalRule> cloneFromCertifier(NormalRule nr, DBBuffer _buffer)
		{
			if (this.CertifierId.HasValue)
			{
				var CountriesId = _buffer.p_countries.Where(c => c.CertifierId == this.CertifierId).Select(x => x.Id).ToList();
				if (CountriesId.Count > 0)
				{
					List<NormalRule> cloni = new List<NormalRule>();
					foreach (var country in CountriesId)
					{
						NormalRule tmpRule = new NormalRule(nr);
						tmpRule.CountryId = country;
						cloni.Add(tmpRule);
					}
					return cloni;
				}
			}

			// if nothing was to be done, retunr the input rule itself
			return new List<NormalRule> { nr };
		}
		#endregion
	}
}