using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;
using EFCore.BulkExtensions;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
    /// association between probes and applications
    public partial class ProbePreset
	{
		public XDocument presetFile;
		public String defaultName;
		public String AllProbeDefaultName;

		public ProbePreset()
		{
		}

		public ProbePreset(ProbePreset copy)
		{
			ProbeId = copy.ProbeId;
			ApplicationId = copy.ApplicationId;
			SettingsFamilyId = copy.SettingsFamilyId;
			ProbeDefaultEnum = copy.ProbeDefaultEnum;
			PresetFolder = copy.PresetFolder;
			PresetFileNameFrontal = copy.PresetFileNameFrontal;
			PresetFileNameLateral = copy.PresetFileNameLateral;
		}

		public String getPresetFile(TransducerPosition type)
		{
			if (type == TransducerPosition.Frontal)
			{
				return this.PresetFileNameFrontal;
			}
			else
			{
				return this.PresetFileNameLateral;
			}
		}

		public String getPresetFile(Int32 ProbeId, ProbeType transducerType, List<ProbeTransducers> ProbeTransducers)
		{
			var type = ProbeTransducers.Single(x => x.ProbeId == ProbeId && x.TransducerType == transducerType).TransducerPosition;
			if (type == TransducerPosition.Frontal)
			{
				return this.PresetFileNameFrontal;
			}
			else
			{
				return this.PresetFileNameLateral;
			}
		}

		public static List<Uirule> ImportProbeAppRules(FeaturesContext _db, DBBuffer _buffer, Paths Paths, string LogXml2DB)
		{
			Provider.WriteImportLogFormat(LogXml2DB, "Read Probe-Applications relations");
			string settingsName = "", ProbeName = "";
			try
			{
				// ANTO DB
				_buffer.p_probe_SettingsFamily = _db.ProbeSettingsFamily.ToList();
				var num_sf = _buffer.p_settingsFamilies.Count;
				var resultProbeApp = new List<ProbePreset>[num_sf];
				Parallel.For(0, num_sf, sfindex =>
				{
					settingsName = _buffer.p_settingsFamilies[sfindex].Name;
					String swPackName = _buffer.p_swPacks.Single(s => s.Id == _buffer.p_settingsFamilies[sfindex].SwpackId).Name;
					// select current probeList and join it with probeBind and probe Id already inserted in the DB
					List<ProbeBind> currentProbeList = _buffer.p_probe_SettingsFamily.Where(x => x.SettingsFamilyId == _buffer.p_settingsFamilies[sfindex].Id).
													   Join(_buffer.p_probes, ps => ps.ProbeId, p => p.Id, (ps, p) => p).ToList().
													   Join(_buffer.probeBindLst, p => p.SaleName, x => x.probe.SaleName, (p, x) => new ProbeBind(p, x.probeInfoLst)).ToList();
					resultProbeApp[sfindex] = new List<ProbePreset>();
					foreach (ProbeBind probeBind in currentProbeList)   // loop probes in probelist to complete probe and add probeApplication association
					{
						var currentProbeSetting = _buffer.p_probe_SettingsFamily.Single(x => x.ProbeId == probeBind.probe.Id
												  && x.SettingsFamilyId == _buffer.p_settingsFamilies[sfindex].Id);
						var probeDescEquip = (Provider.readProbeDescr(swPackName, currentProbeSetting, Paths)).Descendants("equipment").First();
						ProbeName = probeBind.probe.SaleName;

						probeBind.probe.Biplane = Convert.ToBoolean(probeDescEquip.Element("fBiplane").Value);       // complete probe field
						currentProbeSetting.ProbeDataFileNameFrontal = probeDescEquip.Element("sProbeDataFileNameFrontal").Value.ToString();
						currentProbeSetting.ProbeDataFileNameLateral = probeDescEquip.Element("sProbeDataFileNameLateral").Value.ToString() == "NULLFILE" ? null :
								probeDescEquip.Element("sProbeDataFileNameLateral").Value.ToString();

						// fill appNode for the current settings family in the current probe
						var currentProbeInfo = probeBind.getProbeInfo(_buffer.p_settingsFamilies[sfindex]);
						currentProbeInfo.appNodes = probeDescEquip.Element("sApplicationDesc").Elements("element")
													.Where(a => a.Element("eApplication").Value.ToString() != "APPL_INVALID_PD").ToList();

						resultProbeApp[sfindex].AddRange(createProbeApps(probeBind, currentProbeInfo, _buffer));
					}
				});

				return finalizeImport(resultProbeApp.SelectMany(rp => rp).ToList(), _db, _buffer);
			}
			catch (Exception ex)
			{
				throw new ParsingException("Probe-Applications relations for settings family " + settingsName + " probe " + ProbeName, ex);
			}
		}

		private static List<Uirule> finalizeImport(List<ProbePreset> resultProbeApp, FeaturesContext _db, DBBuffer _buffer)
		{
			// ANTO DB

			// for the changes in the probe table
			//_db.SubmitChanges();
			_db.SaveChanges();

			// insert probe_applications and create UIRules for them
			//			_db.BulkInsert(resultProbeApp);
			//			_db.ProbePreset.AddRange(resultProbeApp);
			_db.BulkInsert(resultProbeApp);

			var result = _db.createProbeAppRules(resultProbeApp, _buffer);
			_buffer.p_probe_Apps = resultProbeApp.Join(_buffer.p_applications, x => x.ApplicationId, a => a.Id, (x, a) =>
								   new ProbeApp(x.ProbeId, x.SettingsFamilyId, a)).ToList();
			return result;
		}

		// reads probe preset name and location for the current application
		private static List<ProbePreset> createProbeApps(ProbeBind probeBind, probeInfo currentProbeInfo, DBBuffer _buffer)
		{
			List<ProbePreset> resultProbeApp = new List<ProbePreset>();
			foreach (var app in currentProbeInfo.appNodes)
			{
				Application application = _buffer.p_applications.SingleOrDefault(a => a.ProbeDescrName == app.Element("eApplication").Value.ToString());
				ProbePreset baseProbeApp = new ProbePreset()
				{
					ProbeId = probeBind.probe.Id,
					ApplicationId = application.Id,
					SettingsFamilyId = currentProbeInfo.settingsFam.Id
				};

				resultProbeApp.AddRange(createPresetProbeApps(baseProbeApp, app));
			}
			return resultProbeApp;
		}

		private static IEnumerable<ProbePreset> createPresetProbeApps(ProbePreset baseProbeApp, XElement app)
		{
			var Presets = app.Element("sFactoryDefault").Descendants("element").Where(p =>
						  p.Element("sProbeDefaultFileNameFrontal").Value.ToString() != "NULLFILE"
						  || p.Element("sProbeDefaultFileNameLateral").Value.ToString() != "NULLFILE").ToList();
			ProbePreset tmpProbeApp;
			foreach (var preset in Presets)
			{
				String PresetFileNameFrontal = preset.Element("sProbeDefaultFileNameFrontal").Value.ToString() == "NULLFILE" ?
											   null : preset.Element("sProbeDefaultFileNameFrontal").Value.ToString();
				String PresetFileNameLateral = preset.Element("sProbeDefaultFileNameLateral").Value.ToString() == "NULLFILE" ?
											   null : preset.Element("sProbeDefaultFileNameLateral").Value.ToString();

				String probeDir;
				if (PresetFileNameFrontal != null)
				{
					probeDir = Path.GetDirectoryName(PresetFileNameFrontal);
				}
				else
				{
					probeDir = Path.GetDirectoryName(PresetFileNameLateral);
				}

				if (probeDir == "")
				{
					probeDir = null;
				}

				// probe application association
				tmpProbeApp = new ProbePreset(baseProbeApp);
				tmpProbeApp.PresetFolder = probeDir;
				tmpProbeApp.PresetFileNameFrontal = Path.GetFileName(PresetFileNameFrontal);
				tmpProbeApp.PresetFileNameLateral = Path.GetFileName(PresetFileNameLateral);
				tmpProbeApp.ProbeDefaultEnum = preset.Element("eProbeDefaultId").Value.ToString();

				yield return tmpProbeApp;
			}
		}

	}
}
