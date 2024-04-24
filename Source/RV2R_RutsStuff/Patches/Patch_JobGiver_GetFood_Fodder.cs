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
    [HarmonyPatch(typeof(JobGiver_GetFood), "TryGiveJob")]
    public static class Patch_JobGiver_GetFodder
    {
        private static bool CanFodder(Pawn pawn)
        {
            if (pawn.Faction.IsPlayer && RV2R_Utilities.IsSapient(pawn))
                return true;
            if (!pawn.Name.Numerical && RV2_Rut_Settings.rutsStuff.FodderNamedAllowed)
                return true;
            if (pawn.IsAnimal())
            {
                if (pawn.RaceProps.predator && RV2_Rut_Settings.rutsStuff.FodderPredatorsAllowed)
                    return true;
                if (!pawn.RaceProps.predator && RV2_Rut_Settings.rutsStuff.FodderAnimalsAllowed)
                    return true;
            }
            return false;
        }

        [HarmonyPostfix]
        public static void InterceptGetFodder(ref Job __result, Pawn pawn)
        {
            if (!pawn.Faction.IsPlayer)
                return;

            if (!pawn.CanBePredator(out string outText))
                return;

            if (pawn.RaceProps.FenceBlocked)
                return;

            if (!CanFodder(pawn))
                return;

            Job job = __result;

            try
            {
                if (job != null
                 && (job.def != JobDefOf.Ingest || job.def != JobDefOf.TakeFromOtherInventory)
                 && (job != null && job.GetTarget(TargetIndex.A).Thing is Corpse))
                    return;

                if (job != null
                 && pawn.needs.food.CurLevelPercentage < 0.10f
                 && Rand.Chance(0.5f)) // Give up if no-one's saying yes
                    return;

                List<VoreGoalDef> validGoals = new List<VoreGoalDef>() {
                    VoreGoalDefOf.Digest,
                    RV2R_Common.Drain
                };

                if (pawn.IsActivePredator() && pawn.needs.food.CurLevelPercentage > 0.10f)
                {
                    PawnData pawnData = pawn.PawnData(false);
                    if (pawnData != null)
                    {
                        VoreTracker voreTracker = pawnData.VoreTracker;
                        if (voreTracker != null)
                            foreach (VoreTrackerRecord record in voreTracker.VoreTrackerRecords)
                            {
                                if (record.VoreGoal == VoreGoalDefOf.Digest || record.VoreGoal == RV2R_Common.Drain)
                                {
                                    string defName = record.CurrentVoreStage.def.defName.ToLower();
                                    if (defName == "Intestines_Enter" || defName.Contains("warmup") || defName.Contains("digest") || defName.Contains("drain") || defName.Contains("churn"))
                                        if (!Rand.Chance((0.1f - pawn.needs.food.CurLevelPercentage) / 0.225f))
                                        {
                                            __result = null;
                                            return;
                                        }
                                }
                                if ((record.VoreType == VoreTypeDefOf.Oral || record.VoreType == VoreTypeDefOf.Anal) && record.HasReachedEntrance)
                                {
                                    __result = null;
                                    return;
                                }
                            }
                    }
                }

                if (pawn.PreferenceFor(VoreRole.Predator, ModifierOperation.Add) < 0f
                 || (pawn.PreferenceFor(VoreGoalDefOf.Digest, VoreRole.Predator, ModifierOperation.Add) < 0f
                  && pawn.PreferenceFor(RV2R_Common.Drain, VoreRole.Predator, ModifierOperation.Add) < 0f))
                    return;

                if (Rand.Chance(RV2_Rut_Settings.rutsStuff.FodderChance / (pawn.IsColonistPlayerControlled ? (pawn.RaceProps.predator ? 10f : 50f) : 1f)))
                {
                    //RV2Log.Message("Predator " + pawn.LabelShort + " has munchies", "Jobs");
                    Dictionary<Pawn, float> fodderList = new Dictionary<Pawn, float>();
                    bool baseCheck(Thing t)
                    {
                        Pawn target = (Pawn)t;
                        return pawn.CanVore(target, out outText)
                            && pawn.Position.DistanceTo(target.Position) <= 40f
                            && pawn.Position.DistanceTo(target.Position) >= 2f // So they don't spam requests to the same snack over and over
                            && pawn.CanReach(target.Position, PathEndMode.Touch, Danger.None)
                            && pawn.CanReserve(target)
                            && !RV2R_Utilities.IsBusy(pawn, target, true)
                            && !target.IsForbidden(pawn)
                            && !VoreInteractionManager.Retrieve(new VoreInteractionRequest(pawn, target, VoreRole.Predator, true, false, false, null, null, null, null, validGoals)).ValidPaths.NullOrEmpty()
                        ;
                    };
                    bool prisonCheck(Thing t)
                    {
#if v1_4
                        Pawn target = (Pawn)t;
                        return target.IsPrisonerInPrisonCell() && target.guest.interactionMode == RV2R_Common.Fodder
                        ;
#else
                        Pawn target = (Pawn)t;
                        return target.IsPrisonerInPrisonCell() && target.guest.ExclusiveInteractionMode == RV2R_Common.Fodder
                        ;
#endif
                    };
                    bool animalCheck(Thing t)
                    {
                        Pawn target = (Pawn)t;
                        return pawn.Position.DistanceTo(target.Position) <= 15f
                             && target.IsAnimal() && target.Name != null && target.Name.Numerical
                             && ((RV2_Rut_Settings.rutsStuff.FodderPenAnimals && target.RaceProps.FenceBlocked)
                              || (RV2_Rut_Settings.rutsStuff.FodderAnimals && (!target.RaceProps.predator || target.BodySize <= pawn.BodySize * 0.65f))
                              || (RV2_Rut_Settings.rutsStuff.FodderPredators && target.RaceProps.predator)
                                )
                        ;
                    };
                    bool humanoidCheck(Thing t)
                    {
                        Pawn target = (Pawn)t;
                        return pawn.Position.DistanceTo(target.Position) <= 15f
                             && ((RV2_Rut_Settings.rutsStuff.FodderGuests && !target.IsColonist && !RV2R_Utilities.IsColonyHostile(pawn, target))
                              || (RV2_Rut_Settings.rutsStuff.FodderColonists && target.IsColonistPlayerControlled)
                                )
                        ;
                    };

                    List<Pawn> nearPawns = pawn.Map.mapPawns.AllPawns.FindAll((Pawn p) => p != pawn
                                                                                              && baseCheck(p)
                                                                                              && (prisonCheck(p)
                                                                                               || ((pawn.IsColonistPlayerControlled || RV2_Rut_Settings.rutsStuff.FodderAnimalsFull)
                                                                                                &&
                                                                                                (animalCheck(p)
                                                                                               || humanoidCheck(p)))));

                    if (nearPawns.NullOrEmpty())
                        return;

                    //RV2Log.Message("Predator " + pawn.LabelShort + " found " + nearPawns.Count.ToString() + " nearby pawns", "Jobs");
                    foreach (Pawn p in nearPawns)
                    {
                        if (RV2R_Utilities.GetFodderWeight(pawn, p, true) > 0f)
                            fodderList.Add(p, RV2R_Utilities.GetFodderWeight(pawn, p, false) * pawn.PreferenceFor(p, ModifierOperation.Add));
                    }

                    if (fodderList.NullOrEmpty())
                        return;

                    RV2Log.Message("Predator " + pawn.LabelShort + " has " + fodderList.Count.ToString() + " nearby fodder", "Jobs");

                    Pawn prey = fodderList.RandomElementByWeightWithDefault(fodder => fodder.Value, 0.01f).Key;

                    if (prey == null)
                        return;

                    if (job != null && !prey.IsPrisonerInPrisonCell() && !Rand.Chance(RV2_Rut_Settings.rutsStuff.MiscFodderChance))
                        return;

                    RV2Log.Message("Predator " + pawn.LabelShort + " picked " + prey.LabelShort, "Jobs");
                    VoreInteraction voreInteraction = VoreInteractionManager.Retrieve(new VoreInteractionRequest(pawn, prey, VoreRole.Predator, true, false, false, null, null, null, null, validGoals));

                    VoreJob voreJob = VoreJobMaker.MakeJob(VoreJobDefOf.RV2_VoreInitAsPredator, pawn, prey);
                    voreJob.VorePath = voreInteraction.PreferredPath ?? voreInteraction.ValidPaths.RandomElement();
                    voreJob.targetA = prey;
                    voreJob.IsForced = prey.PreferenceFor(voreJob.VorePath.voreGoal, VoreRole.Prey, ModifierOperation.Add) <= 3f;
                    if (job != null && voreJob.IsForced && (prey.IsColonistPlayerControlled || pawn.QuirkManager(false).HasQuirk(DefDatabase<QuirkDef>.GetNamedSilentFail("StrugglePreference_Hated"))))
                    {
                        VoreProposal_TwoWay proposal = new VoreProposal_TwoWay(pawn, prey, pawn, prey, voreJob.VorePath);
                        voreJob = VoreJobMaker.MakeJob(VoreJobDefOf.RV2_ProposeVore, pawn, prey);
                        voreJob.targetA = prey;
                        voreJob.Proposal = proposal;
                        voreJob.VorePath = proposal.VorePath;
                    }

                    job = JobMaker.MakeJob(JobDefOf.Goto, prey);
                    //RV2Log.Message("Predator " + pawn.LabelShort + " eating " + prey.LabelShort, "Jobs");
                    pawn.jobs.jobQueue.EnqueueFirst(voreJob, new JobTag?(JobTag.Idle));
                    __result = job;
                    return;

                }
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when trying to intercept get-food logic: " + e);
                __result = job;
            }
        }
    }
}

