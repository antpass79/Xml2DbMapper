using System;
using System.IO;

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	// class holding all the information about the interesting paths
	public class Paths
	{
		public string RootPath;
		public string importFilePath
		{
			get
			{
				return RootPath + "\\Utilities\\FeatureManagerDB\\FeatureManagerDB\\Import";
			}
		}

		public string outPath
		{
			get
			{
				return RootPath + "\\Utilities\\FeatureManagerDB\\FeatureManagerDB\\Output";
			}
		}

		public string DBxmlPath
		{
			get
			{
				return RootPath + "\\Utilities\\FeatureManagerDB\\NewDatabaseFiles";
			}
		}

		public string DBfile
		{
			get
			{
				// ANTO FIX
				return outPath + "\\Features.db";
				//return outPath + "\\Features.sdf";
			}
		}

		public string ProbeRootPath
		{
			get
			{
				return RootPath + "\\ProbeSettingsData";
			}
		}

		public string focFilePath
		{
			get
			{
				return RootPath + "\\FocalizationData\\probes";
			}
		}

		public string ProbeSchemaPath
		{
			get
			{
				return ProbeRootPath + @"\Schema";
			}
		}

		public Paths(string _RootPath, bool p_CreateDir = true)
		{
			RootPath = _RootPath;
			if (p_CreateDir)
			{
				Directory.CreateDirectory(outPath);
			}
		}

		public String getProbePath(String swpackName)
		{
			return ProbeRootPath + "\\" + swpackName + "\\UpDataLab\\Probe";
		}
	}
}
