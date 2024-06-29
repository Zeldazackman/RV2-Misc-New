using HarmonyLib;
using RimVore2;
using RimVore2.Tabs;
using RimWorld;
using Verse;

namespace RV2R_RutsStuff
{
    internal class Patch_CapacityAcclimation
    {
        [HarmonyPatch(typeof(VoreValidator), "CalculateVoreCapacity")]
        internal class Patch_AddAcclimation
        {
            [HarmonyPostfix]
            public static void AddAcclimation(ref Pawn pawn, ref float __result)
            {
                if (pawn.health.hediffSet.HasHediff(RV2R_Common.CapacityAcclimation, false))
                {
                    // Severity of the acclimation hediff adds to bodysize capacity for the predator pawn 1 to 1
                    // It grows slowly, but is only lost on death (or by messing with the hediff in Char. Editor)
                    __result += pawn.health.hediffSet.GetFirstHediffOfDef(RV2R_Common.CapacityAcclimation, false).Severity;
                }
            }
        }
    }
}
