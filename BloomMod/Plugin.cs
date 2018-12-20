using System;
using System.Collections;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

namespace BloomMod
{
	public class Plugin : IPlugin
	{
		public string Name => "BloomMod";
		public string Version => "1.0.2";

		private Prefs prefs = new Prefs();
		private bool prefsChangeEventRegistered = false;
		private bool resetMode = false;
		private Coroutine co = null;
		private GameScenesManager scenesManager;

		public void OnApplicationStart()
		{
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
			resetMode = prefs.NeedsReset();
			if (resetMode) prefs.ResetCameraNames();
		}

		public void OnApplicationQuit()
		{
			SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
			if (scenesManager != null) scenesManager.transitionDidFinishEvent -= SceneTransitionDidFinish;
			if (resetMode) prefs.DoneReset();
		}

		private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scenesManager == null)
			{
				scenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
				if (scenesManager != null) scenesManager.transitionDidFinishEvent += SceneTransitionDidFinish;
			}
		}

		private void SceneTransitionDidFinish()
		{
			if (co != null) SharedCoroutineStarter.instance.StopCoroutine(co);

			if (resetMode)
			{
				co = SharedCoroutineStarter.instance.StartCoroutine(resetEffectParamsCoroutine());
			}
			else
			{
				co = SharedCoroutineStarter.instance.StartCoroutine(modifyEffectParamsCoroutine());
			}
		}

		private IEnumerator modifyEffectParamsCoroutine()
		{
			yield return new WaitForSecondsRealtime(0.2f);

			var flags = prefs.LoadCameraNames().ToDictionary(n => n, n => false);

			var timeout = Time.realtimeSinceStartup;
			while(Time.realtimeSinceStartup - timeout < 2.0f)
			{
				modifyEffectParams(flags);
				yield return new WaitForSecondsRealtime(0.2f);
			}

			if (prefsChangeEventRegistered == false)
			{
				prefs.ChangedEvent += prefsChanged;
				prefsChangeEventRegistered = true;
			}
		}

		private void modifyEffectParams(Dictionary<string,bool> flags)
		{
			foreach (var c in Camera.allCameras)
			{
				if (flags != null)
				{
					if (flags.ContainsKey(c.name) == false) continue;
					if (flags[c.name] == true) continue;
					flags[c.name] = true;
				}

				var efx = c.gameObject.GetComponent<MainEffect>();
				if (efx == null) continue;
				var param = ReflectionUtil.GetPrivateField<MainEffectParams>(efx, "_mainEffectParams");
				if (param == null) continue;
				param = UnityEngine.Object.Instantiate(param);
				var modParams = prefs.LoadForCamera(c.name);
				param.baseColorBoost = modParams.baseColorBoost;
				param.baseColorBoostThreshold = modParams.baseColorBoostThreshold;
				param.bloomAlphaWeightScale = modParams.bloomAlphaWeightScale;
				param.bloomIntensity = modParams.bloomIntensity;
				param.bloomIterations = modParams.bloomIterations;

				param.textureHeight = (modParams.textureHeight <= 1.0)
					? (int)(c.pixelHeight * modParams.textureHeight) 
					: (int)modParams.textureHeight;

				param.textureWidth = (modParams.textureWidth <= 1.0) 
					? (int)(c.pixelWidth * modParams.textureWidth) 
					: (int)modParams.textureWidth;

				ReflectionUtil.SetPrivateField(efx, "_mainEffectParams", param);
				var efxRenderer = efx.GetComponent<MainEffectRenderer>();
				if (efxRenderer) ReflectionUtil.SetPrivateField(efxRenderer, "_mainEffectParams", param);
				Plugin.Log($"set: {c.name} ({c.pixelWidth} => {param.textureWidth}, {c.pixelHeight} => {param.textureHeight})");
			}
		}

		private IEnumerator resetEffectParamsCoroutine()
		{
			yield return new WaitForSecondsRealtime(0.2f);

			var flags = new Dictionary<string, bool>();

			var timeout = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup - timeout < 2.0f)
			{
				foreach (var c in Camera.allCameras)
				{
					if (flags.ContainsKey(c.name)) continue;
					var efx = c.gameObject.GetComponent<MainEffect>();
					if (efx == null) continue;
					var param = ReflectionUtil.GetPrivateField<MainEffectParams>(efx, "_mainEffectParams");
					if (param == null) continue;
					var resetParams = new Prefs.Params();
					resetParams.baseColorBoost = param.baseColorBoost;
					resetParams.baseColorBoostThreshold = param.baseColorBoostThreshold;
					resetParams.bloomAlphaWeightScale = param.bloomAlphaWeightScale;
					resetParams.bloomIntensity = param.bloomIntensity;
					resetParams.bloomIterations = param.bloomIterations;
					resetParams.textureHeight = param.textureHeight;
					resetParams.textureWidth = param.textureWidth;
					prefs.Reset(c.name, resetParams);
					flags.Add(c.name, true);
					Plugin.Log("reset: " + c.name);
				}
				yield return new WaitForSecondsRealtime(0.2f);
			}

			prefs.SaveCameraNames(flags.Keys.ToArray());
		}

		private void prefsChanged(Prefs sender)
		{
			Plugin.Log("pref changed");
			modifyEffectParams(null);
		}

		public static void Log(string s)
		{
			Console.WriteLine("[BloomMod] " + s);
		}

		public void OnLevelWasLoaded(int level)
		{
		}

		public void OnLevelWasInitialized(int level)
		{
		}

		public void OnUpdate()
		{
		}

		public void OnFixedUpdate()
		{
		}
	}
}
