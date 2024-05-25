using System;
using System.IO;

namespace GXPEngine.Assets
{
	internal static class Assets
	{
		public const string AssetsPath = "Assets";
		public const string TexturesPath = "Assets\\Textures";
		public const string AudioPath = "Assets\\Audio";

		static Assets()
		{
			if (!Directory.Exists(AssetsPath))
			{
				throw new Exception("Assets folder doesn't exist.");
			}
			else
			{
				if (!Directory.Exists(TexturesPath)) Directory.CreateDirectory(TexturesPath);
				if (!Directory.Exists(AudioPath)) Directory.CreateDirectory(AudioPath);
			}
		}

		public static string GetTexturePath(string filename, int maxDepth = 2) => FindFile(filename, TexturesPath, maxDepth);
		public static string GetAudioPath(string filename, int maxDepth = 2) => FindFile(filename, AudioPath, maxDepth);

		private static string FindFile(string filename, string directoryPath, int maxDepth, int currentDepth = 0)
		{
			if (File.Exists(Path.Combine(directoryPath, filename))) return Path.Combine(directoryPath, filename);
			else if (currentDepth <= maxDepth)
			{
				string[] subdirs = Directory.GetDirectories(directoryPath);
				foreach (string subdir in subdirs)
				{
					string foundPath = FindFile(filename, subdir, maxDepth, currentDepth + 1);
					if (!string.IsNullOrEmpty(foundPath)) return foundPath;
				}
				return string.Empty;
			}
			else return string.Empty;
		}
	}
}