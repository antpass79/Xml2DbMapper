using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xml2DbMapper.Core.Models;
using Xml2DbMapper.Core.Porting.Contract.Enums;
using Xml2DbMapper.Core.Porting.Util;

// fg18112015

namespace Xml2DbMapper.Core.Porting.FeatureManagerDB
{
	public static class Provider
	{
		public static class DirectoryUtilities
		{
			public static Boolean MoveFolder(String Origin, String Destination)
			{
				if (Directory.Exists(Destination))
				{
					DirectoryUtilities.DeleteDirectory(Destination);
				}
				else
				{
					return false;
				}
				System.Threading.Thread.Sleep(300);
				DirectoryUtilities.DirectoryCopy(Origin, Destination, true);
				return true;
			}

			public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
			{
				// Get the subdirectories for the specified directory.
				DirectoryInfo dir = new DirectoryInfo(sourceDirName);

				if (!dir.Exists)
				{
					throw new DirectoryNotFoundException(
						"Source directory does not exist or could not be found: "
						+ sourceDirName);
				}

				DirectoryInfo[] dirs = dir.GetDirectories();
				// If the destination directory doesn't exist, create it.
				if (!Directory.Exists(destDirName))
				{
					Directory.CreateDirectory(destDirName);
				}

				// Get the files in the directory and copy them to the new location.
				FileInfo[] files = dir.GetFiles();
				foreach (FileInfo file in files)
				{
					string temppath = Path.Combine(destDirName, file.Name);
					file.CopyTo(temppath, false);
				}

				// If copying subdirectories, copy them and their contents to new location.
				if (copySubDirs)
				{
					foreach (DirectoryInfo subdir in dirs)
					{
						string temppath = Path.Combine(destDirName, subdir.Name);
						DirectoryCopy(subdir.FullName, temppath, copySubDirs);
					}
				}
			}

			// uses recursive option of Delete funcion
			public static void DeleteDirectory2(String p_Directory)
			{
				Int32 l_Attempt = 0;
				while (true)
				{
					try
					{
						if (System.IO.Directory.Exists(p_Directory))
						{
							System.IO.Directory.Delete(p_Directory, true);
						}
						else
						{
							return;
						}
					}
					catch
					{
						// It often happens because the System.IO.Directory.Delete method is asynchronous.
						// In such a case, retrying to delete p_Directory works.
						++l_Attempt;
						if (l_Attempt > 5)
						{
							throw;
						}
						System.Threading.Thread.Sleep(250);  // System.IO.Directory.Delete is asynchronus
					}
				}
			}

			public static void DeleteDirectory(string targetDir)
			{
				File.SetAttributes(targetDir, FileAttributes.Normal);

				string[] files = Directory.GetFiles(targetDir);
				string[] dirs = Directory.GetDirectories(targetDir);

				foreach (string file in files)
				{
					File.SetAttributes(file, FileAttributes.Normal);
					File.Delete(file);
				}

				foreach (string dir in dirs)
				{
					DeleteDirectory(dir);
				}

				System.Threading.Thread.Sleep(100);
				Directory.Delete(targetDir);
			}
		}

		public static void WriteErrorHeader(string LogPath, Exception ex)
		{
			string message = Environment.NewLine + "***************** ERROR ***************** " + Environment.NewLine + ex.Message;
			using (StreamWriter Log = new StreamWriter(LogPath, true))
			{
				Log.WriteLine(message);
			}
			Console.WriteLine(message);
		}

		public static void WriteLogFormat(string LogPath, string message)
		{
			string result = ">>> " + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture) + " - " + message;
			using (StreamWriter Log = new StreamWriter(LogPath, true))
			{
				Log.WriteLine(result);
			}
			Console.WriteLine(result);
		}

		public static string WriteErrorLogFormat(string message)
		{
			return "*** " + message;
		}

		public static void WriteImportLogFormat(string LogPath, string message)
		{
			WriteLogFormat(LogPath, "Import " + message);
		}

		/// read file with tab delimited elements in the lines
		public static List<List<String>> CreateFile(string percorso)
		{
			return File.ReadAllLines(percorso).Select(a => a.Split('\t').ToList()).ToList();
		}

		/// Class dictionary returning null in case no value matches with the keys
		public static TValue GetValueOrDefault_Class<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key) where TValue :
		class
		{

			TValue value;
			if (dict.TryGetValue(key, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		/// Struct dictionary returning null in case no value matches with the keys
		public static TValue ? GetValueOrDefault_Stuct<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key) where TValue : struct
		{
			TValue value;
			if (dict.TryGetValue(key, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		public static String GetFocFileName(String FocPath, XDocument defFile)
		{
			return FocPath + "\\" + Path.GetFileNameWithoutExtension(defFile.Descendants("cFocFileName").FirstOrDefault().Value) + ".xml";
		}

		public static String GetFocFileName(String FocPath, XDocument defFile, XDocument defAllProbesFile)
		{
			if (defFile.Descendants("cFocFileName").FirstOrDefault() != null)
			{
				return FocPath + "\\" + Path.GetFileNameWithoutExtension(defFile.Descendants("cFocFileName").FirstOrDefault().Value) + ".xml";
			}
			return FocPath + "\\" + Path.GetFileNameWithoutExtension(defAllProbesFile.Descendants("cFocFileName").FirstOrDefault().Value) + ".xml";
		}


		public static XDocument readEquipmentDescription(String swpName, Paths Paths)
		{
			return XDocument.Load(Paths.getProbePath(swpName) + "\\..\\EquipmentDescr.xml");
		}

		public static XDocument readProbeDescr(String swpName, ProbeSettingsFamily psf, Paths Paths)
		{
			return XDocument.Load(Paths.getProbePath(swpName) + "\\" + psf.ProbeFolder + "\\" + psf.ProbeDescFileName);
		}

		public static XDocument readProbeList(String swpName, SettingFamily settings, Paths Paths)
		{
			return XDocument.Load(Paths.getProbePath(swpName) + "\\" + settings.ProbeListFile);
		}

		//[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		//private static T ReadVirtualRoot<T>(String path)
		//{
		//  T result;
		//  // deserialize the xml using the class built from the xsd file of the Descriptor file
		//  using (FileStream fs = new FileStream(path, FileMode.Open))
		//  {
		//      using (XmlReader reader = XmlReader.Create(fs))
		//      {
		//          XmlSerializer serializer = new XmlSerializer(typeof(T));
		//          result = (T)serializer.Deserialize(reader);
		//      }
		//  }
		//  return result;
		//}

		// writes serializable object to xml file
		public static void WriteToLog<T>(T obj, String fileName)
		{
			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Indent = true;

			DataContractSerializer serializer = new DataContractSerializer(typeof(T));
			using (XmlWriter wr = XmlWriter.Create(fileName, ws))
			{
				serializer.WriteObject(wr, obj);
			}
		}


		// read from xml file
		public static T ReadFromLog<T>(String fileName)
		{
			try
			{
				XmlReaderSettings ws = new XmlReaderSettings();
				DataContractSerializer dcs = new DataContractSerializer(typeof(T));
				using (XmlReader wr = XmlReader.Create(fileName, ws))
				{
					return (T)dcs.ReadObject(wr);
				}
			}
			catch
			{
				return default(T);
			}
		}

		public static T FromStringToEnum<T>(String str, T value) where T : struct
		{
			if (typeof(T) == typeof(ApplicationType))
			{
				throw new Exception("Use FromStringToAppType() for Application Type, do not use FromStringToEnum(): default value of ApplicationType is not NoType!");
			}
			T result;
			if (Enum.TryParse(str, out result))
			{
				return result;
			}
			else
			{
				return value;
			}
		}

		public static ApplicationType FromStringToAppType(String str)
		{
			ApplicationType result;
			if (Enum.TryParse(str, out result))
			{
				return result;
			}
			else
			{
				return ApplicationType.NoType;
			}
		}

		// generates the file name and if it finds that the file name is already used, it puts an incremental number in the end of the name
		public static String generateName(String fullFileNameNoExtension, String extension, Func<String, String> GetExtension, Func<String, Boolean> Exists)
		{
			Int32 counter = 0;
			String fileName;
			do
			{
				counter++;
				if (counter > 1)
				{
					fileName = fullFileNameNoExtension + "_" + counter.ToString() + GetExtension(extension);
				}
				else
				{
					fileName = fullFileNameNoExtension + GetExtension(extension);
				}
			}
			while (Exists(fileName));

			return fileName;
		}


		public static String generateFileName(String fullFileNameNoExtension, String extension)
		{
			return generateName(fullFileNameNoExtension, extension, ext => "." + ext, System.IO.File.Exists);
		}


		public static String generateFolderName(String fullFolderName)
		{
			return generateName(fullFolderName, "", NoExtension => "", System.IO.Directory.Exists);
		}

		// ANTO UNUSED

		//// fill DB buffer and dispose the database instance
		//public static DBBuffer initBuffer(Paths Paths)
		//{
		//	DBBuffer buffer = new DBBuffer();
		//	using (var db = Database.Open(Paths.DBfile))
		//	{

		//		buffer.FillBuffer(db);
		//		//db.Dispose();
		//	}
		//	return buffer;
		//}

		public static String LogName(String p_outPath, String Model)
		{
			return p_outPath + "\\" + Model + ".xml";
		}

		public static String LogNameDB(String p_outPath, String Model)
		{
			return p_outPath + "\\" + Model + "_DB.xml";
		}

		public static String LogNameIndexedDB(String p_outPath, String Model)
		{
			return p_outPath + "\\" + Model + "_IndexedDB.xml";
		}

		// ANTO UNUSED
		//public static Boolean IsDiscoveryTest(UserLevel l_CurrentUser)
		//{
		//	bool IsTest = false;
		//	if (Registry.ExistsRegistryValue(Registry.RegistryKey.LocalMachine, Registry.s_74XX, "TestFlag"))
		//	{
		//		IsTest = Convert.ToBoolean(Registry.GetRegistryValue(Registry.RegistryKey.LocalMachine, Registry.s_74XX, "TestFlag", false))
		//				 && l_CurrentUser == UserLevel.Service;
		//	}
		//	return IsTest;
		//}

		// fast version of intersection for lists used for sorted lists only
		public static List<int> IntersectSorted(this List<int> source1, List<int> source2)
		{
			var ints = new List<int>(Math.Min(source1.Count, source2.Count));
			var i = 0;
			var j = 0;
			while (i < source1.Count && j < source2.Count)
			{
				switch (source1[i].CompareTo(source2[j]))
				{
					case -1:
						i++;
						continue;
					case 1:
						j++;
						continue;
					default:
						ints.Add(source1[i++]);
						j++;
						continue;
				}
			}
			return ints;
		}


		static String FeatureKey = "NVPAxfvJ7Vp86Gd4";
		public static void WriteFile<T>(T obj, string FullFileName, string FullKeyFileName)
		{
			var ws = new XmlWriterSettings();
			ws.Indent = true;

			var serializer = new DataContractSerializer(typeof(T));
			using (XmlWriter wr = XmlWriter.Create(FullFileName, ws))
			{
				serializer.WriteObject(wr, obj);
			}

			if (!string.IsNullOrEmpty(FullKeyFileName))
			{
				File.WriteAllText(FullKeyFileName, CreateSHA1Code(FullFileName));
			}
		}

		public static void WriteFile<T>(T obj, string FullFileName)
		{
			WriteFile<T>(obj, FullFileName, null);
		}

		// write xml database signature
		public static String CreateSHA1Code(String fileName)
		{
			using (var sha1 = SHA1.Create())
			{
				return CreateCode(fileName, sha1);
			}
		}

		private static String CreateCode(String fileName, HashAlgorithm HA)
		{
			using (var stream = File.OpenRead(fileName))
			{
				String fileHash = BitConverter.ToString(HA.ComputeHash(stream));
				String envelope = fileHash + FeatureKey;
				return BitConverter.ToString(HA.ComputeHash(Encoding.UTF8.GetBytes(envelope)));
			}
		}


		public static T LoadFromFile<T>(string FullFileName, string FullKeyFileName)
		{
			var TrueCode = CreateSHA1Code(FullFileName);
			var CurrentKey = File.ReadAllText(FullKeyFileName);

			// check signature
			if (TrueCode != CurrentKey)
			{
				var errLog = new StringBuilder();
				errLog.AppendLine("FeatureManager database signature not recognized.");
				errLog.AppendLine("current code size: " + CurrentKey.Length);
				errLog.AppendLine("correct size: " + TrueCode.Length);

				// ANTO LOG
				//Logger.LogTrace("FeatureManager", Flow_Type.Configuration, LogLevel.LogNormal, errLog.ToString());
				Logger.Log($"FeatureManager {errLog.ToString()}");

				throw new ApplicationException("Feature Manager unrecognized signature.");
			}

			return LoadFromFile<T>(FullFileName);
		}

		public static T LoadFromFile<T>(string FullFileName)
		{
			using (var file_stream = XmlReader.Create(FullFileName))
			{
				var xml_serializer = new DataContractSerializer(typeof(T));
				var result = (T)xml_serializer.ReadObject(file_stream);

				return result;
			}
		}


		public static String GetCVModule(CVModule EnumModule)
		{
			if (EnumModule == CVModule.CPUModule)
			{
				return "CVIE-Esaote-USPlusView-CPU-DSP";
			}
			else
			{
				return "CVIE-Esaote-USPlusView-DSP";
			}
		}

		public static String CleanFileName(string filename)
		{
			string file = filename;
			file = string.Concat(file.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

			if (file.Length > 250)
			{
				file = file.Substring(0, 250);
			}
			return file;
		}


		public static String RunShellCommand(String p_CommandPath, String p_CommandParameters, String p_WorkingDirectory, Boolean p_WaitForCompletion)
		{
			if (String.IsNullOrEmpty(p_CommandPath))
			{
				throw new ApplicationException("Missing Command Name");
			}
			String l_CommandPath = System.IO.Path.GetFullPath(p_CommandPath);
			String l_WorkingDirectory = p_WorkingDirectory;
			if (String.IsNullOrEmpty(l_WorkingDirectory))
			{
				l_WorkingDirectory = System.IO.Path.GetDirectoryName(l_CommandPath) + "\\";
			}
			String l_CommandParameters = "";
			if (p_CommandParameters != null)
			{
				l_CommandParameters = p_CommandParameters;
			}

			var l_CommandInfo = new System.Diagnostics.ProcessStartInfo();
			l_CommandInfo.FileName = "\"" + l_CommandPath + "\"";
			l_CommandInfo.Arguments = l_CommandParameters;
			l_CommandInfo.WorkingDirectory = l_WorkingDirectory;
			l_CommandInfo.UseShellExecute = false;
			l_CommandInfo.RedirectStandardOutput = true;
			l_CommandInfo.RedirectStandardError = true;
			l_CommandInfo.CreateNoWindow = true;

			System.Diagnostics.Process l_NewProcess = System.Diagnostics.Process.Start(l_CommandInfo);

			String standard_output = l_NewProcess.StandardOutput.ReadToEnd();
			String standard_error = l_NewProcess.StandardError.ReadToEnd();
			String output = standard_output + Environment.NewLine + standard_error;
			output = output.Trim();

			if (p_WaitForCompletion)
			{
				l_NewProcess.WaitForExit();
			}

			return output;
		}

		public static int GetMaxVersion(FeatureRegister register)
		{
			return register.MinorVersionAssociations.Max(x => x.Major);
		}
		public static int GetMinVersion(FeatureRegister register)
		{
			return register.MinorVersionAssociations.Min(x => x.Major);
		}
	}
}
