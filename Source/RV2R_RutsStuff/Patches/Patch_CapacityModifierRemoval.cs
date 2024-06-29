using HarmonyLib;
using RimVore2;
using Verse;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    [HarmonyPatch(typeof(Patch_PawnCapacityUtility), "ModifyOffsetWithQuirks")]
    internal class Patch_Return1f
    {
        internal static bool Prefix(float value, Pawn pawn, PawnCapacityDef capacity, ref float __result)
        {
            // This code only exists in RV2 to mess with movement capacity penalties in the vore hediffs, which this submod superceeds
            // This causes problems, since encumberance takes a pawn's movement quirks into account, ending up doubling bonuses/penalties
            // (It also started affecting non-vore hediffs on my end, so, uh)
            // So, here's a dead-simple super gross destructive patch that just says "sorry Nabber, but I'm turning this off"
            // (Unless you set Encumbrance multiplier to 0%, at which point I'm assuming you needed to revert)

            if (RV2_Rut_Settings.rutsStuff.EncumberanceModifier > 0.0f)
            {
                __result = value;
                return false;
            }
            return true;

        }
    }
}