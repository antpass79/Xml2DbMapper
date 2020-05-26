using System;
using System.IO;

namespace Xml2DbMapper.Reader.Models
{
	public class InputPaths
	{
		const string IMPORT_FILE_PATH = "\\Utilities\\FeatureManagerDB\\FeatureManagerDB\\Import";
		const string OUTPUT_PATH = "\\Utilities\\FeatureManagerDB\\FeatureManagerDB\\Output";
		const string DATABASE_XML_FILE = "\\Utilities\\FeatureManagerDB\\DatabaseFiles";
		const string DATABASE_FILE = "\\Features.sdf";
		const string PROBE_ROOT_PATH = "\\ProbeSettingsData";
		const string FOCALIZATION_FILE_PATH = "\\FocalizationData\\probes";
		const string PROBE_SCHEMA_PATH = @"\Schema";

		public string RootPath { get; }
		public string importFilePath => RootPath + IMPORT_FILE_PATH;
		public string OutPath => RootPath + OUTPUT_PATH;
		public string DBxmlPath => RootPath + DATABASE_XML_FILE;
		public string DBfile => OutPath + DATABASE_FILE;
		public string ProbeRootPath => RootPath + PROBE_ROOT_PATH;
		public string focFilePath => RootPath + FOCALIZATION_FILE_PATH;
		public string ProbeSchemaPath => ProbeRootPath + PROBE_SCHEMA_PATH;

		public InputPaths(string rootPath, bool createDirectory = true)
		{
			RootPath = rootPath;
			if (createDirectory)
			{
				Directory.CreateDirectory(OutPath);
			}
		}

		public String GetProbePath(String softwarePackName)
		{
			return ProbeRootPath + "\\" + softwarePackName + "\\UpDataLab\\Probe";
		}
	}
}
