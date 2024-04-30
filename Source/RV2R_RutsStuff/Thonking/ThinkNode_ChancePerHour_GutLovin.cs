using RimVore2;
using RimWorld;
using System;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    internal class ThinkNode_ChancePerHour_GutLovin : ThinkNode_ChancePerHour
    {
        protected override float MtbHours(Pawn pawn)
        {
            if (RV2_Rut_Settings.rutsStuff.GutLovinChance <= 0f)
                return -1f;

            if (!(pawn.ageTracker.AgeBiologicalYears > 18f || pawn.ageTracker.AgeBiologicalYearsFloat > pawn.ageTracker.AdultMinAge))
                return -1f;

            if (RestUtility.Awake(pawn))
                return -1f;

            if (!pawn.IsActivePredator())
                return -1f;

            if (!RV2R_Utilities.IsSapient(pawn))
                return -1f;

            if (!pawn.IsHumanoid() && !RV2_Rut_Settings.rutsStuff.GutLovinSapients)
                return -1f;

            float drive = 1f;
            QuirkManager quirkManager = pawn.QuirkManager(false) ?? null;
            if (quirkManager != null && quirkManager.HasValueModifier("Predator_Libido"))
                drive = quirkManager.ModifyValue("Predator_Libido", drive);


            if (drive > 0f)
            {
                RV2Log.Message(string.Concat(new string[]
                {
                    "Predator ",
                    pawn.LabelShort,
                    " gut loves every ",
                    ((24f/RV2_Rut_Settings.rutsStuff.GutLovinChance) / drive / ThinkNode_ChancePerHour_GutLovin.LovinMtbSinglePawnFactor(pawn)).ToString(),
                    " h/a ((36h/",
                    RV2_Rut_Settings.rutsStuff.GutLovinChance.ToString(),
                    "(chance mod))/",
                    drive.ToString(),
                    "(pred drive)/",
                    ThinkNode_ChancePerHour_GutLovin.LovinMtbSinglePawnFactor(pawn).ToString(),
                    "(lovin factor))"
                }), "Gutlovin");
                return (36f / RV2_Rut_Settings.rutsStuff.GutLovinChance) / drive / ThinkNode_ChancePerHour_GutLovin.LovinMtbSinglePawnFactor(pawn);
            }

            return -1f;
        }

        private static float LovinMtbSinglePawnFactor(Pawn pawn)
        {
            float painMod = 1f / (1f - pawn.health.hediffSet.PainTotal);
            float conMod = Math.Min(pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness), 1.5f);
            if (conMod < 0.5f)
                painMod /= conMod * 2f;                                  // Some HAR races have silly adult ages; Racc, for example are formaly adults at <i><b>5000</i></b> years
            float adultAge = Math.Min(18f, pawn.ageTracker.AdultMinAge); // This basicaly means they never score, so I'm helping them here
            return painMod / Math.Max(0.01f, GenMath.FlatHill(0f, adultAge * 0.77f, adultAge * 0.88f, adultAge * 1.38f, adultAge * 4.44f, 0.2f, pawn.ageTracker.AgeBiologicalYearsFloat));
        }
    }
}
