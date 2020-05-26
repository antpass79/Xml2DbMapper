using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Models;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class DBBuffer
	{
		public List<Probe> p_probes = new List<Probe>();
		public List<Feature> p_features = new List<Feature>();
		public List<Bundle> p_bundles = new List<Bundle>();
		public List<LicenseRelation> p_LicenseRelation = new List<LicenseRelation>();
		public List<LicenseRelationException> p_LicenseRelationExceptions = new List<LicenseRelationException>();
		public List<Deprecated> p_Deprecated = new List<Deprecated>();
		public List<Application> p_applications = new List<Application>();
		public List<Option> p_option = new List<Option>();
		public List<SettingFamily> p_settingsFamilies = new List<SettingFamily>();
		public List<License> p_licenses = new List<License>();
		public List<Models.PhysicalModel> p_physicalModels = new List<Models.PhysicalModel>();
		public List<Certifier> p_certifiers = new List<Certifier>();
		public List<Swpack> p_swPacks = new List<Swpack>();
		public List<LogicalModel> p_logicalModels = new List<LogicalModel>();
		public List<BiopsyKits> p_biospyKits = new List<BiopsyKits>();
		public List<Country> p_countries = new List<Country>();
		public List<ProbeSettingsFamily> p_probe_SettingsFamily = new List<ProbeSettingsFamily>();
		public List<ProbePreset> p_probe_Presets = new List<ProbePreset>();
		public List<ProbeApp> p_probe_Apps = new List<ProbeApp>();
		public List<Uirule> p_UIRules = new List<Uirule>();
		public List<NormalRule> p_NormalRules = new List<NormalRule>();
		public List<ProbeTransducers> p_ProbeTransducers = new List<ProbeTransducers>();
		public List<Distributor> p_Distributors = new List<Distributor>();
		public List<CountryDistributor> p_Country_Distributors = new List<CountryDistributor>();
		public List<CountryLicense> p_CountryLicenses = new List<CountryLicense>();
		public List<RegulatoryFeature> p_RegualtoryFeatures = new List<RegulatoryFeature>();
		public List<MinorVersionAssociation> p_MinorVersionAssociation = new List<MinorVersionAssociation>();
		public List<PartNumbersAssociations> p_PartNumbersAssociations = new List<PartNumbersAssociations>();

		public IEnumerable<IGrouping<ProbeFamilies?, Probe>> AllProbeFamilies;
		public IEnumerable<IGrouping<ProbeType, ProbeTransducers>> AllProbeTypes;

		public List<ProbeBind> probeBindLst;
		public List<Probe> p_multiTransducerProbes;

		public List<ModelLicense> ModelLicenses = new List<ModelLicense>();

		// ANTO DB
		public void FillBuffer(FeaturesContext _db)
		{
			p_probes = _db.Probe.ToList();
			p_features = _db.Feature.ToList();
			p_bundles = _db.Bundle.ToList();
			p_LicenseRelation = _db.LicenseRelation.ToList();
			p_LicenseRelationExceptions = _db.LicenseRelationException.ToList();
			p_Deprecated = _db.Deprecated.ToList();
			p_applications = _db.Application.ToList();
			p_option = _db.Option.ToList();
			p_settingsFamilies = _db.SettingFamily.ToList();
			p_licenses = _db.License.ToList();
			p_physicalModels = _db.PhysicalModel.ToList();
			p_certifiers = _db.Certifier.ToList();
			p_swPacks = _db.Swpack.ToList();
			p_logicalModels = _db.LogicalModel.ToList();
			p_biospyKits = _db.BiopsyKits.ToList();
			p_countries = _db.Country.ToList();
			p_UIRules = _db.Uirule.ToList();
			p_NormalRules = _db.NormalRule.ToList();
			p_ProbeTransducers = _db.ProbeTransducers.ToList();
			p_probe_SettingsFamily = _db.ProbeSettingsFamily.ToList();
			p_probe_Presets = _db.ProbePreset.ToList();
			p_Distributors = _db.Distributor.ToList();
			p_Country_Distributors = _db.CountryDistributor.ToList();
			p_CountryLicenses = _db.CountryLicense.ToList();
			p_RegualtoryFeatures = _db.RegulatoryFeature.ToList();
			p_MinorVersionAssociation = _db.MinorVersionAssociation.ToList();
			p_PartNumbersAssociations = _db.PartNumbersAssociations.ToList();

			AllProbeFamilies = p_probes.GroupBy(p => p.ProbeFamily);
			AllProbeTypes = p_ProbeTransducers.GroupBy(t => t.TransducerType);
			p_multiTransducerProbes = p_ProbeTransducers.GroupBy(t => t.ProbeId).Where(g => g.Count() > 1)
									  .Join(p_probes, g => g.Key, p => p.Id, (t, p) => p).ToList();

			ModelLicenses = p_logicalModels.Join(p_licenses, m => m.LicenseId, l => l.Id, (m, l) => new ModelLicense(m, l)).ToList();
		}

		// this is the name of the object, not the name of the feature
		public Feature GetFeatureFromName(String ObjectName)
		{
			var option = p_option.SingleOrDefault(o => o.Name == ObjectName);
			if (option != null)
			{
				return p_features.Single(f => f.Id == option.FeatureId);
			}

			var app = p_applications.SingleOrDefault(o => o.Name == ObjectName);
			if (app != null)
			{
				return p_features.Single(f => f.Id == app.FeatureId);
			}

			var probe = p_probes.SingleOrDefault(o => o.SaleName == ObjectName);
			if (probe != null)
			{
				return p_features.Single(f => f.Id == probe.FeatureId);
			}

			var kit = p_biospyKits.SingleOrDefault(o => o.Name == ObjectName);
			if (kit != null)
			{
				return p_features.Single(f => f.Id == kit.FeatureId);
			}

			return null;
		}

		public void SetRuleFeatureField(Uirule BaseRule, int FeatureId)
		{
			var option = p_option.SingleOrDefault(o => o.FeatureId == FeatureId);
			if (option != null)
			{
				BaseRule.OptionId = option.Id;
				return;
			}

			var application = p_applications.SingleOrDefault(o => o.FeatureId == FeatureId);
			if (application != null)
			{
				BaseRule.ApplicationId = application.Id;
				return;
			}

			var probe = p_probes.SingleOrDefault(o => o.FeatureId == FeatureId);
			if (probe != null)
			{
				BaseRule.ProbeId = probe.Id;
				return;
			}

			var kit = p_biospyKits.SingleOrDefault(o => o.FeatureId == FeatureId);
			if (kit != null)
			{
				BaseRule.KitId = kit.Id;
				return;
			}

		}


	}
}
