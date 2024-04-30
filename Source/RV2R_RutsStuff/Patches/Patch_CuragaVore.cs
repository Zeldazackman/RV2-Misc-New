using HarmonyLib;
using RimVore2;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    [HarmonyPatch(typeof(RollAction_Heal), "TryAction")]
    internal class Patch_CuragaVore
    {
        internal static bool Prefix(ref float rollStrength)
        {
            if (RV2_Rut_Settings.rutsStuff.CuragaVore)
                rollStrength *= 2.5f;

            return true;
        }
    }
}
