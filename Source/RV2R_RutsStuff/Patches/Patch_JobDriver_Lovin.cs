using HarmonyLib;
using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    [HarmonyPatch(typeof(JobDriver_Lovin), "MakeNewToils")]
    public class Patch_JobDriver_Lovin
    {
        [HarmonyPostfix]
        public static IEnumerable<Toil> GrindUpPrey(IEnumerable<Toil> __result, JobDriver_Lovin __instance)
        {
            if (RV2_Rut_Settings.rutsStuff.PreyLovin)
            {
                foreach (Toil toil in __result)
                {
                    yield return toil;
                }
                try
                {
                    Pawn pawn = __instance.pawn;
                    PawnData pawnData = pawn.PawnData(true);
                    List<VoreTrackerRecord> list;
                    List<Pawn> preyList = new List<Pawn>();
                    if (pawnData == null)
                    {
                        list = null;
                    }
                    else
                    {
                        VoreTracker voreTracker = pawnData.VoreTracker;
                        list = (voreTracker?.VoreTrackerRecords);
                    }
                    if (list.NullOrEmpty())
                        yield break;
                    List<VoreTrackerRecord> recordList = list.FindAll((VoreTrackerRecord record) => RV2R_Utilities.IsInTargetMidsection(record.Prey, record.Predator, false));
                    if (recordList.NullOrEmpty<VoreTrackerRecord>())
                    {
                        yield break;
                    }
                    IEnumerable<Pawn> familyByBlood = pawn.relations.FamilyByBlood;
                    float num = 1f;
                    bool skip = false;
                    ThoughtDef thoughtDef = RV2R_Common.PreyLovin_Normal;
                    foreach (VoreTrackerRecord voreTrackerRecord in recordList)
                    {
                        skip = false;
                        if (((voreTrackerRecord.Prey.needs.mood != null && !voreTrackerRecord.IsForced) || !voreTrackerRecord.StruggleManager.ShouldStruggle))
                        {
                            foreach (Pawn pawn2 in familyByBlood)
                            {
                                if (voreTrackerRecord.Prey == pawn2
                                 && !LovePartnerRelationUtility.LovePartnerRelationExists(voreTrackerRecord.Prey, voreTrackerRecord.Predator))
                                {
                                    skip = true;
                                }
                            }
                            num = voreTrackerRecord.Prey.QuirkManager(false)?.ModifyValue("Prey_Libido", num) ?? 1f;
                            thoughtDef = RV2R_Common.PreyLovin_Normal;
                            switch (num)
                            {
                                case 2f:
                                    thoughtDef = RV2R_Common.PreyLovin_VeryGood;
                                    break;
                                case 1.5f:
                                    thoughtDef = RV2R_Common.PreyLovin_Good;
                                    break;
                                case 0.66f:
                                case 0.33f:
                                    thoughtDef = RV2R_Common.PreyLovin_Meh;
                                    break;
                                case 0.00f:
                                    thoughtDef = null;
                                    break;

                            }
                            if (thoughtDef != null && !skip)
                            {
                                preyList.Add(voreTrackerRecord.Prey);
                                Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef);
                                voreTrackerRecord.Prey.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, pawn);
                            }

                            if (LovePartnerRelationUtility.ExistingLovePartner(voreTrackerRecord.Prey, false) != null
                              && preyList.Contains(LovePartnerRelationUtility.ExistingLovePartner(voreTrackerRecord.Prey, false)))
                            {
                                Pawn lover = LovePartnerRelationUtility.ExistingLovePartner(voreTrackerRecord.Prey, false);
                                voreTrackerRecord.Prey.needs.mood?.thoughts.memories.TryGainMemory(ThoughtDefOf.GotSomeLovin, lover, null);
                                lover.needs.mood?.thoughts.memories.TryGainMemory(ThoughtDefOf.GotSomeLovin, voreTrackerRecord.Prey, null);
                            }
                        }
                    }
                    yield break;
                }
                catch (Exception e)
                {
                    Log.Warning("RV-2R: Something went wrong when trying to make a predatory 3 way : " + e);
                    yield break;
                }
            }
        }
    }
}
