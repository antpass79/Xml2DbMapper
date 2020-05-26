using System;
using System.Collections.Generic;
using System.Linq;
using Xml2DbMapper.Core.Models;
using Xml2DbMapper.Core.Porting.Contract.Enums;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public class Dictionaries
	{
		private DBBuffer p_buffer;

		public Dictionary<string, ProbeType> dictProbeTypes;
		public Dictionary<string, ProbeFamilies> dictProbeFamilies;
		public Dictionary<string, int> dictOpt;
		public Dictionary<string, int> dictApp;
		public Dictionary<int, List<String>> dictParents;

		public Dictionary<Int32, String> dictOptionLogger;
		public Dictionary<Int32, String> dictAppLogger;
		public Dictionary<Int32, String> dictLogicalModelsLogger;
		public Dictionary<Int32, String> dictKitsLogger;
		public Dictionary<Int32, String> dictCountriesLogger;
		public Dictionary<Int32, String> dictUserLogger;
		public Dictionary<Int32, String> dictProbeLogger;

		public Dictionaries(FeaturesContext _db)
		{
			p_buffer = new DBBuffer();
			p_buffer.p_probes = _db.Probe.ToList();
			p_buffer.p_applications = _db.Application.ToList();
			p_buffer.p_bundles = _db.Bundle.ToList();
			p_buffer.p_option = _db.Option.ToList();
			p_buffer.p_logicalModels = _db.LogicalModel.ToList();
			p_buffer.p_biospyKits = _db.BiopsyKits.ToList();
			p_buffer.p_countries = _db.Country.ToList();
		}

		public Dictionaries(DBBuffer _buffer)
		{
			p_buffer = _buffer;
		}

		public void fillDicts()
		{
			dictProbeTypes = new Dictionary<string, ProbeType>
			{
				{ "CONVEX", ProbeType.CONVEX},
				{ "LINEAR", ProbeType.LINEAR},
				{ "PENCIL", ProbeType.PENCIL},
				{ "PHASED ARRAY", ProbeType.PHASE_ARRAY}
			};

			dictProbeFamilies = new Dictionary<string, ProbeFamilies>
			{
				{ "PF_BISCAN_PD", ProbeFamilies.Biscan},
				{ "PF_PENCIL_PD", ProbeFamilies.Pencil},
				{ "PF_STANDARD_PD", ProbeFamilies.Standard}
			};

			// map the id in the table to the name
			dictOpt = p_buffer.p_option.Join(p_buffer.p_features, o => o.FeatureId, f => f.Id, (o, f) => new
			{
				option = o,
				feature = f
			}).ToDictionary(m => m.feature.NameInCode, m => m.option.Id);

			dictApp = p_buffer.p_applications.Join(p_buffer.p_features, a => a.FeatureId, f => f.Id, (a, f) => new
			{
				application = a,
				feature = f
			}).ToDictionary(m => m.feature.NameInCode, m => m.application.Id);

			// maps the application id to its parent application name
			dictParents = p_buffer.p_bundles.Join(p_buffer.p_applications, b => b.FeatureId, a => a.FeatureId, (b, a) => new
			{
				childApp = a.Id,
				parentfeat = b.ParentFeatureId
			})
			.Join(p_buffer.p_features, ba => ba.parentfeat, f => f.Id, (ba, f) => new
			{
				childApp = ba.childApp,
				parentAppName = f.NameInCode
			}).GroupBy(x => x.childApp).ToDictionary(x => x.Key, x => x.Select(y => y.parentAppName).ToList());
		}

		public void fillLoggers()
		{
			dictOptionLogger = p_buffer.p_option.Select(pm => new
			{
				pm.Id,
				pm.Name
			}).ToDictionary(m => m.Id, m => m.Name);
			dictAppLogger = p_buffer.p_applications.Select(pm => new
			{
				pm.Id,
				pm.Name
			}).ToDictionary(m => m.Id, m => m.Name);
			dictLogicalModelsLogger = p_buffer.p_logicalModels.Select(pm => new
			{
				pm.Id,
				pm.Name
			}).ToDictionary(m => m.Id, m => m.Name);
			dictKitsLogger = p_buffer.p_biospyKits.Select(pm => new
			{
				pm.Id,
				pm.Name
			}).ToDictionary(m => m.Id, m => m.Name);
			dictCountriesLogger = p_buffer.p_countries.Select(pm => new
			{
				pm.Id,
				pm.Name
			}).ToDictionary(m => m.Id, m => m.Name);
			dictProbeLogger = p_buffer.p_probes.Select(pm => new
			{
				pm.Id,
				pm.SaleName
			}).ToDictionary(m => m.Id, m => m.SaleName);
		}
	}
}
