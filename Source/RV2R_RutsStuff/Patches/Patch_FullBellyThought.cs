using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace RV2R_RutsStuff
{
    [HarmonyPatch(typeof(ThoughtWorker_NeedFood), "CurrentStateInternal")]
    internal class Patch_FullBellyThought
    {
        [HarmonyPostfix]
        public static void PreyKeepsBellyFull(Pawn p, ref ThoughtState __result)
        {
            try
            {
                if (p.needs.food != null && RV2R_Utilities.HasPreyIn(p, "stomach"))
                    switch (p.needs.food.CurCategory)
                    {
                        case HungerCategory.Fed:
                            __result = ThoughtState.Inactive;
                            break;
                        case HungerCategory.Hungry:
                            __result = ThoughtState.Inactive;
                            break;
                        case HungerCategory.UrgentlyHungry:
                            __result = ThoughtState.ActiveAtStage(0);
                            break;
                        case HungerCategory.Starving:
                            {
                                Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false);
                                int num = ((firstHediffOfDef == null) ? 0 : firstHediffOfDef.CurStageIndex);
                                __result = ThoughtState.ActiveAtStage(1 + num);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when trying to modify "+p.LabelShort+"'s hunger thought : " + e);
                return;
            }
        }
    }
}
