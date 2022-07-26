using Base.Levels;
using PhoenixPoint.Common.Game;
using PhoenixPoint.Modding;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System;

namespace FullAugmentations
{
	/// <summary>
	/// This is the main mod class. Only one can exist per assembly.
	/// If no ModMain is detected in assembly, then no other classes/callbacks will be called.
	/// </summary>
	public class FullAugmentationsMain : ModMain
	{
		/// Config is accessible at any time, if any is declared.
		public new FullAugmentationsConfig Config => (FullAugmentationsConfig)base.Config;

		/// This property indicates if mod can be Safely Disabled from the game.
		/// Safely sisabled mods can be reenabled again. Unsafely disabled mods will need game restart ot take effect.
		/// Unsafely disabled mods usually cannot revert thier changes in OnModDisabled
		public override bool CanSafelyDisable => true;

		public static int MaxAugmentations = 3;

		/// <summary>
		/// Callback for when mod is enabled. Called even on game starup.
		/// </summary>
		public override void OnModEnabled() {

			/// All mod dependencies are accessible and always loaded.
			int c = Dependencies.Count();
			/// Mods have their own logger. Message through this logger will appear in game console and Unity log file.
			Logger.LogInfo($"Full Augmentations mod enabled");
			/// Metadata is whatever is written in meta.json
			string v = MetaData.Version.ToString();
			/// Game creates Harmony object for each mod. Accessible if needed.
			HarmonyLib.Harmony harmony = (HarmonyLib.Harmony)HarmonyInstance;
			/// Mod instance is mod's runtime representation in game.
			string id = Instance.ID;
			/// Game creates Game Object for each mod. 
			GameObject go = ModGO;
			/// PhoenixGame is accessible at any time.
			PhoenixGame game = GetGame();

			Main = this;

			try {
				((HarmonyLib.Harmony)HarmonyInstance).PatchAll(GetType().Assembly);
			} 
			catch (Exception e) {
				Logger.LogInfo($"{e.ToString()}");
			}

		}

		/// <summary>
		/// Callback for when mod is disabled. This will be called even if mod cannot be safely disabled.
		/// Guaranteed to have OnModEnabled before.
		/// </summary>
		public override void OnModDisabled() {
			/// Undo any game modifications if possible. Else "CanSafelyDisable" must be set to false.
			try { ((HarmonyLib.Harmony)HarmonyInstance).UnpatchAll( ((Harmony)base.HarmonyInstance).Id); }
            catch (Exception e) {
				Logger.LogInfo($"{e.ToString()}");
			}
			//
			Main = null;
		}

		public static ModMain Main { get; private set; }
	}
}
