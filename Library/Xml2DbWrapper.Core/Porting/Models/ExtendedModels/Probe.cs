using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Xml2DbMapper.Core.Porting.FeatureManagerDB;

// FG 16112015
namespace Xml2DbMapper.Core.Models
{
    public partial class Probe
	{
		// import probes from probelists of different settings families
		public static List<ProbeBind> Import(DBBuffer _buffer, Paths Paths, string LogXml2DB)
		{
			Provider.WriteLogFormat(LogXml2DB, "Import Probes from probelist");
			List<ProbeBind> result = new List<ProbeBind>();
			List<ProbeBind> validProbesBinds, newProbes, oldProbes;
			try
			{
				foreach (var setting in _buffer.p_settingsFamilies)
				{
					var swpackName = _buffer.p_swPacks.SingleOrDefault(swp => swp.Id == setting.SwpackId).Name;
					var probeLst = Provider.readProbeList(swpackName, setting, Paths);
					validProbesBinds = probeLst.Descendants("elemento").Where(x => x.Element("sSaleName").Value != "NULLSTRING")
									   .Select(p => FromXElement2ProbeBind(p, setting)).ToList();

					// select old probes from previously inserted probes
					oldProbes = result.Where(x => validProbesBinds.Select(p => p.probe.SaleName.ToLowerInvariant()).Contains(x.probe.SaleName.ToLowerInvariant())).ToList();
					updateProbes(oldProbes, validProbesBinds, setting);

					// select new probes from the new probeList
					newProbes = validProbesBinds.Where(p => !result.Select(x => x.probe.SaleName.ToLowerInvariant()).Contains(p.probe.SaleName.ToLowerInvariant())).ToList();
					completeNewProbes(newProbes);
					result.AddRange(newProbes);
				}
				return result;
			}
			catch (Exception ex)
			{
				throw new ParsingException("Probelist", ex);
			}
		}

		private static  ProbeBind FromXElement2ProbeBind(XElement elemento, SettingFamily settings)
		{
			Probe probe = new Probe();
			probe.HwCode = elemento.Element("wHwCode").Value.ToString();
			var prbstrcode = elemento.Element("sProbeStringCode").Value.ToString();
			probe.ProbeStringCode = prbstrcode == "NULL" ? null : prbstrcode;
			probe.MultiConnector = Convert.ToBoolean(elemento.Element("fMulticonnSupported").Value);
			probe.SaleName = elemento.Element("sSaleName").Value.ToString();
			probe.ProbeDescription = elemento.Element("sProbeLayoutIcon").Value.ToString();

			probeInfo probeInfo = new probeInfo(elemento, settings);
			return new ProbeBind(probe, new List<probeInfo> { probeInfo });
		}


		// add to the probe's infoList the information about the current settings family (the info was loaded in the validProbes structure)
		private static void updateProbes(List<ProbeBind> oldProbes, List<ProbeBind> validProbes, SettingFamily setting)
		{
			foreach (ProbeBind probeBind in oldProbes)
			{
				ProbeBind newProbeBind = validProbes.SingleOrDefault(v => v.probe.SaleName.ToLowerInvariant() == probeBind.probe.SaleName.ToLowerInvariant());
				probeBind.probeInfoLst.Add(new probeInfo(setting, newProbeBind));

				UpdateProbeFields(probeBind.probe, newProbeBind.probe);
			}
		}

		private static void UpdateProbeFields(Probe oldprobe, Probe newprobe)
		{
			if (newprobe.HwCode != "0")
			{
				oldprobe.HwCode = newprobe.HwCode;
			}
			if (newprobe.ProbeStringCode != null)
			{
				oldprobe.ProbeStringCode = newprobe.ProbeStringCode;
			}
		}

		// correct the structure deserialized from xml
		private static void completeNewProbes(List<ProbeBind> probeLst)
		{
			var numProbes = probeLst.Count;
			Parallel.For(0, numProbes, x =>
			{
				probeLst[x].probe.ProbeDescription = probeLst[x].probe.ProbeDescription.Split('.')[0];
			});
		}
	}
}
