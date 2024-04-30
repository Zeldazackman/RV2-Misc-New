using HarmonyLib;
using RimVore2;
using Verse;

namespace RV2R_RutsStuff
{
    internal class Patch_RV2R_Settings
    {
        public class RV2_RutsStuff_Mod : Mod
        {
            public static RV2_RutsStuff_Mod mod;
            public RV2_RutsStuff_Mod(ModContentPack content) : base(content)
            {
                mod = this;
                GetSettings<RV2_Rut_Settings>();  // create !static! settings
                WriteSettings();
            }
        }

        public class RV2_Rut_Settings : ModSettings
        {
            public static SettingsContainer_RutsStuff rutsStuff;

            public RV2_Rut_Settings()
            {
                rutsStuff = new SettingsContainer_RutsStuff();
            }

            public override void ExposeData()
            {
                base.ExposeData();
                Scribe_Deep.Look(ref rutsStuff, "rutsStuff", new object[0]);
            }
        }

        [HarmonyPatch(typeof(RV2Settings), "DefsLoaded")]
        public static class Patch_RV2Settings_DefsLoaded
        {
            [HarmonyPostfix]
            private static void AddRutSettings()
            {
                RV2_Rut_Settings.rutsStuff.DefsLoaded();
            }
        }

        [HarmonyPatch(typeof(RV2Settings), "Reset")]
        public static class Patch_RV2Settings_Reset
        {
            [HarmonyPostfix]
            private static void AddRutSettings()
            {
                RV2_Rut_Settings.rutsStuff.Reset();
            }
        }

        [HarmonyPatch(typeof(RV2Mod), "WriteSettings")]
        public static class Patch_RV2Mod_WriteSettings
        {
            [HarmonyPostfix]
            private static void AddRutSettings()
            {
                RV2_RutsStuff_Mod.mod.WriteSettings();
            }
        }

        [HarmonyPatch(typeof(Window_Settings), "InitializeTabs")]
        public static class Patch_Window_Settings
        {
            [HarmonyPostfix]
            private static void AddRutSettings()
            {
                Window_Settings.AddTab(new SettingsTab_RutsStuff("RV2R_Settings_Tab".Translate(), null, null));
            }
        }
    }
}
