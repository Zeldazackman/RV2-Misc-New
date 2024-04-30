using HarmonyLib;
using Verse;

namespace RV2R_RutsStuff
{
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            new Harmony("Rutrix.RV2R_RutsStuff").PatchAll();
        }

        public const string Id = "Rutrix.RV2R_RutsStuff";

        public const string ModName = "RV2R_RutsStuff";

        public const string Version = "0.888";

    }
}
