using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lab5.GameSystem
{
	internal static class GameSystemRegister
	{
		private static readonly string[] DefaultAssemblies =
		{
		"mscorlib",
		"netstandard",
		"System",
		"System.Core",
		"System.Configuration",
		"System.Xml",
		"System.Runtime.Serialization",
		"System.ServiceModel.Internals",
		"System.Xml.Linq",
		"Unity.Cecil",
		"Unity.Cecil.Mdb",
		"Unity.Cecil.Pdb",
		"UnityEditor.CoreModule",
		"UnityEditor.DeviceSimulatorModule",
		"UnityEditor.DiagnosticsModule",
		"UnityEditor",
		"UnityEditor.EditorToolbarModule",
		"UnityEditor.GraphViewModule",
		"UnityEditor.PresetsUIModule",
		"UnityEditor.QuickSearchModule",
		"UnityEditor.SceneTemplateModule",
		"UnityEditor.SceneViewModule",
		"UnityEditor.TextCoreFontEngineModule",
		"UnityEditor.TextCoreTextEngineModule",
		"UnityEditor.UIBuilderModule",
		"UnityEditor.UIElementsModule",
		"UnityEditor.UIElementsSamplesModule",
		"UnityEditor.UnityConnectModule",
		"UnityEngine.AccessibilityModule",
		"UnityEngine.AIModule",
		"UnityEngine.AndroidJNIModule",
		"UnityEngine.AnimationModule",
		"UnityEngine.ARModule",
		"UnityEngine.AssetBundleModule",
		"UnityEngine.AudioModule",
		"UnityEngine.ClothModule",
		"UnityEngine.ClusterInputModule",
		"UnityEngine.ClusterRendererModule",
		"UnityEngine.ContentLoadModule",
		"UnityEngine.CoreModule",
		"UnityEngine.CrashReportingModule",
		"UnityEngine.DirectorModule",
		"UnityEngine",
		"UnityEngine.DSPGraphModule",
		"UnityEngine.GameCenterModule",
		"UnityEngine.GIModule",
		"UnityEngine.GridModule",
		"UnityEngine.HotReloadModule",
		"UnityEngine.ImageConversionModule",
		"UnityEngine.IMGUIModule",
		"UnityEngine.InputLegacyModule",
		"UnityEngine.InputModule",
		"UnityEngine.JSONSerializeModule",
		"UnityEngine.LocalizationModule",
		"UnityEngine.NVIDIAModule",
		"UnityEngine.ParticleSystemModule",
		"UnityEngine.PerformanceReportingModule",
		"UnityEngine.Physics2DModule",
		"UnityEngine.PhysicsModule",
		"UnityEngine.ProfilerModule",
		"UnityEngine.PropertiesModule",
		"UnityEngine.RuntimeInitializeOnLoadManagerInitializerModule",
		"UnityEngine.ScreenCaptureModule",
		"UnityEngine.SharedInternalsModule",
		"UnityEngine.SpriteMaskModule",
		"UnityEngine.SpriteShapeModule",
		"UnityEngine.StreamingModule",
		"UnityEngine.SubstanceModule",
		"UnityEngine.SubsystemsModule",
		"UnityEngine.TerrainModule",
		"UnityEngine.TerrainPhysicsModule",
		"UnityEngine.TextCoreFontEngineModule",
		"UnityEngine.TextCoreTextEngineModule",
		"UnityEngine.TextRenderingModule",
		"UnityEngine.TilemapModule",
		"UnityEngine.TLSModule",
		"UnityEngine.UIElementsModule",
		"UnityEngine.UIModule",
		"UnityEngine.UmbraModule",
		"UnityEngine.UnityAnalyticsCommonModule",
		"UnityEngine.UnityAnalyticsModule",
		"UnityEngine.UnityConnectModule",
		"UnityEngine.UnityCurlModule",
		"UnityEngine.UnityTestProtocolModule",
		"UnityEngine.UnityWebRequestAssetBundleModule",
		"UnityEngine.UnityWebRequestAudioModule",
		"UnityEngine.UnityWebRequestModule",
		"UnityEngine.UnityWebRequestTextureModule",
		"UnityEngine.UnityWebRequestWWWModule",
		"UnityEngine.VehiclesModule",
		"UnityEngine.VFXModule",
		"UnityEngine.VideoModule",
		"UnityEngine.VirtualTexturingModule",
		"UnityEngine.VRModule",
		"UnityEngine.WindModule",
		"UnityEngine.XRModule",
	};

#if UNITY_EDITOR
		[InitializeOnLoadMethod]
		private static void EditorInitialize()
		{
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange change)
		{
			if (change == PlayModeStateChange.ExitingPlayMode)
				PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
		}
#endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Initialize()
		{
			List<object> systems = default;

			foreach (var type in AppDomain.CurrentDomain.GetAssemblies().Where(IsNonDefaultAssemblie).SelectMany(GameSystemTypes))
			{
				try
				{
					var system = Activator.CreateInstance(type, true);
					systems ??= new();
					systems.Add(system);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}

			if (systems == default)
				return;

			// Copy systems
			var loop = PlayerLoop.GetCurrentPlayerLoop();
			var subsystems = new Dictionary<Type, List<PlayerLoopSystem>>(loop.subSystemList.Length);
			foreach (var stage in loop.subSystemList)
				subsystems.Add(stage.type, new(stage.subSystemList));

			// Update systems
			foreach (var system in systems)
			{
				if (system is IInitialization initialization)
					subsystems[typeof(Initialization)].Add(new() { type = initialization.GetType(), updateDelegate = initialization.Initialization });
				if (system is IEarlyUpdate earlyUpdate)
					subsystems[typeof(EarlyUpdate)].Add(new() { type = earlyUpdate.GetType(), updateDelegate = earlyUpdate.EarlyUpdate });
				if (system is IFixedUpdate fixedUpdate)
					subsystems[typeof(FixedUpdate)].Add(new() { type = fixedUpdate.GetType(), updateDelegate = fixedUpdate.FixedUpdate });
				if (system is IPreUpdate preUpdate)
					subsystems[typeof(PreUpdate)].Add(new() { type = preUpdate.GetType(), updateDelegate = preUpdate.PreUpdate });
				if (system is IUpdate update)
					subsystems[typeof(Update)].Add(new() { type = update.GetType(), updateDelegate = update.Update });
				if (system is IPreLateUpdate preLateUpdate)
					subsystems[typeof(PreLateUpdate)].Add(new() { type = preLateUpdate.GetType(), updateDelegate = preLateUpdate.PreLateUpdate });
				if (system is IPostLateUpdate postLateUpdate)
					subsystems[typeof(PostLateUpdate)].Add(new() { type = postLateUpdate.GetType(), updateDelegate = postLateUpdate.PostLateUpdate });
			}

			// Paste systems
			for (int c = 0; c < loop.subSystemList.Length; c++)
				loop.subSystemList[c].subSystemList = subsystems[loop.subSystemList[c].type].ToArray();
			PlayerLoop.SetPlayerLoop(loop);
		}

		private static bool IsNonDefaultAssemblie(Assembly assembly)
		{
			return !DefaultAssemblies.Contains(assembly.GetName().Name);
		}

		private static IEnumerable<Type> GameSystemTypes(Assembly assembly)
		{
			return assembly.GetTypes().Where(ValidGameSystem);
		}

		private static bool ValidGameSystem(Type type)
		{
			if (type.GetCustomAttribute<GameSystemAttribute>() == null)
				return false;

			if (type.IsAbstract)
			{
				Debug.LogErrorFormat("Error instantianting GameSystem of type {0}. A custom GameSystem cannot be abstract.", type);
				return false;
			}
			if (type.ContainsGenericParameters)
			{
				Debug.LogErrorFormat("Error instantianting GameSystem of type {0}. A custom GameSystem cannot have open generic parameters.", type);
				return false;
			}
			if (type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null) == null)
			{
				Debug.LogErrorFormat("Error instantianting GameSystem of type {0}. A custom GameSystem must have a default constructor.", type);
				return false;
			}
			return true;
		}
	}
}