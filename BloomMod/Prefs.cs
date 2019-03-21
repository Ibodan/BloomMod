using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IllusionPlugin;
using UnityEngine;

namespace BloomMod
{
	class Prefs
	{
		public class Params
		{
			public float baseColorBoost = 1.2f;
			public float baseColorBoostThreshold = 0.05f;
			public float bloomIntensity = 0.6f;
			public int bloomIterations = 4;
			public float textureWidth = 2028.0f;
		}

		public event Action<Prefs> ChangedEvent;
		private readonly FileSystemWatcher fileWatcher;

		private static string SECTIONBASE = "BloomMod";

		public Prefs()
		{
			fileWatcher = new FileSystemWatcher(Path.Combine(Environment.CurrentDirectory, "UserData"))
			{
				Filter = "modprefs.ini",
				NotifyFilter = NotifyFilters.LastWrite,
				EnableRaisingEvents = true
			};
			fileWatcher.Changed += OnFileChanged;
		}

		~Prefs()
		{
			fileWatcher.Changed -= OnFileChanged;
		}

		private void OnFileChanged(object sender, FileSystemEventArgs args)
		{
			ChangedEvent(this);
		}

		public bool NeedsReset()
		{
			return ModPrefs.GetBool(SECTIONBASE, "Reset", true);
		}

		public void DoneReset()
		{
			ModPrefs.SetBool(SECTIONBASE, "Reset", false);
		}

		public void Reset(string cameraName, Params resetParams)
		{
			ModPrefs.SetFloat(SECTIONBASE + "!" + cameraName, "BaseColorBoost", resetParams.baseColorBoost);
			ModPrefs.SetFloat(SECTIONBASE + "!" + cameraName, "BaseColorBoostThreshold", resetParams.baseColorBoostThreshold);
			ModPrefs.SetFloat(SECTIONBASE + "!" + cameraName, "BloomIntensity", resetParams.bloomIntensity);
			ModPrefs.SetInt(SECTIONBASE + "!" + cameraName, "BloomIterations", resetParams.bloomIterations);
			ModPrefs.SetFloat(SECTIONBASE + "!" + cameraName, "TextureWidth", resetParams.textureWidth);
		}

		public string[] LoadCameraNames()
		{
			var names = ModPrefs.GetString(SECTIONBASE, "CameraNames");
			return names.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
		}

		public void SaveCameraNames(string[] names)
		{
			var hash = new HashSet<string>(LoadCameraNames());
			hash.UnionWith(names);
			ModPrefs.SetString(SECTIONBASE, "CameraNames", string.Join(",", hash.ToArray()));
		}

		public void ResetCameraNames()
		{
			ModPrefs.SetString(SECTIONBASE, "CameraNames", "");
		}

		public Params LoadForCamera(string cameraName)
		{
			Params ret = new Params();
			var defaults = new Params();

			ret.baseColorBoost = ModPrefs.GetFloat(SECTIONBASE + "!" + cameraName, "BaseColorBoost", defaults.baseColorBoost);
			ret.baseColorBoostThreshold = ModPrefs.GetFloat(SECTIONBASE + "!" + cameraName, "BaseColorBoostThreshold", defaults.baseColorBoostThreshold);
			ret.bloomIntensity = ModPrefs.GetFloat(SECTIONBASE + "!" + cameraName, "BloomIntensity", defaults.bloomIntensity);
			ret.bloomIterations = ModPrefs.GetInt(SECTIONBASE + "!" + cameraName, "BloomIterations", defaults.bloomIterations);
			ret.textureWidth = ModPrefs.GetFloat(SECTIONBASE + "!" + cameraName, "TextureWidth", defaults.textureWidth);

			return ret;
		}
	}
}
