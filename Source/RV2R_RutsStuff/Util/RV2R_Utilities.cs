using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RV2R_RutsStuff
{
    internal static class RV2R_Utilities
    {
        static public bool IsBusy(Pawn pawn, Pawn target, bool respect = false)
        {

            if (pawn.GetLord()?.LordJob is LordJob_FormAndSendCaravan || target.GetLord()?.LordJob is LordJob_FormAndSendCaravan)
                return true;
            if (pawn.CarriedBy != null || target.CarriedBy != null)
                return true;
            if (!pawn.Spawned || !target.Spawned)
                return true;

            if (pawn.Drafted || target.Drafted)
                return true;

            if (pawn.IsBurning() || target.IsBurning())
                return true;

            if (pawn.ShouldBeSlaughtered() || target.ShouldBeSlaughtered())
                return true;

            if (pawn.Faction != null && !pawn.IsHumanoid() && pawn.Faction.IsPlayer)
                if (respect && pawn.playerSettings != null && pawn.playerSettings.RespectedMaster != null
                 && ((pawn.playerSettings.followDrafted && pawn.playerSettings.RespectedMaster.Drafted) || (pawn.playerSettings.followFieldwork && pawn.playerSettings.RespectedMaster.mindState.lastJobTag == JobTag.Fieldwork)))
                    return true;

            return false;
        }

        static public bool IsColonyHostile(Pawn pawn, Pawn target)
        {
            if (pawn.Faction != null && pawn.Faction.IsPlayer)
                if (target.Faction != null && target.Faction.HostileTo(Faction.OfPlayer))
                    if (!target.IsPrisonerOfColony)
                        return true;

            return false;
        }

        static public bool IsSapient(Pawn pawn)
        {
            if (pawn.IsHumanoid())
                return true;
            if (pawn.IsMechanoid())
                return false;
            if (pawn.IsColonistPlayerControlled) // Sentiant Animals
                return true;
            if (pawn.needs.mood != null) // Pawnmorpher, Kyulen
                return true;
            return false;
        }

        static public float GetFodderWeight(Pawn pawn, Pawn target, bool check)
        {
            try
            {
                float weight = 0f;

                if (check && target.IsColonist && Rand.Chance(0.5f)) // 50% chance for any player faction pawn to be spared
                    return 0f;

                if (target.IsPrisonerInPrisonCell())
                    return 6f;

                if (target.IsAnimal())
                {
                    if (check)
                    {
                        List<Pawn> colonySpecies = target.Map.mapPawns.SpawnedColonyAnimals.FindAll((Pawn p) => p.def == target.def && p.gender == target.gender && !target.IsReserved());
                        if (colonySpecies.Count > 2)
                            weight = 1f;
                        else
                            return 0f;
                    }
                    else
                    {
                        int learnedTrainables = 0;
                        foreach (TrainableDef trainable in DefDatabase<TrainableDef>.AllDefsListForReading)
                            if (target.training.HasLearned(trainable))
                                learnedTrainables++;

                        float idealness = Mathf.Abs(target.BodySize * 2f / pawn.BodySize);
                        if (idealness > 1f)
                            idealness = Mathf.Pow(idealness, -1f); // Gives highest weight to prey that are half a predator's size; makes foxxo want to nom chickm and bunbun

                        weight = (2f * idealness) / learnedTrainables;
                    }
                }

                if (target.IsHumanoid())
                {
                    if (pawn.IsAnimal())
                        weight = (1f / (target.IsColonist ? 2f : 1f));
                    else
                        weight = (100 - pawn.relations.OpinionOf(target)) / 50 / (target.IsColonist ? 1.5f : 1f); // Don't eat people you like
                }

                return weight;
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when trying to get " + target.LabelShort + "'s fodder weight: " + e);
                return 0;
            }
        }

        static public bool ShouldFriendlyTarget(Pawn pawn, Pawn target)
        {
            if (pawn.Faction != null
             && (target.Faction != null && pawn.Faction == target.Faction)
               || ((target.Faction != null && target.Faction.AllyOrNeutralTo(pawn.Faction))
               || (target.IsHumanoid() && target.GuestStatus != null)
               || (!target.IsHumanoid() && pawn.Map.designationManager.DesignationOn(target, DesignationDefOf.Tame) != null)))
                return true;

            return false;
        }
        static public bool IsInTargetMidsection(Pawn pawn, Pawn target, bool isLethal)
        {
            if (target.IsActivePredator())
            {
                PawnData pawnData = target.PawnData(false);
                if (pawnData != null)
                {
                    VoreTracker voreTracker = pawnData.VoreTracker;
                    if (voreTracker != null)
                        foreach (VoreTrackerRecord voreTrackerRecord in voreTracker.VoreTrackerRecords)
                        {
                            if (voreTrackerRecord.Prey == pawn
                               && voreTrackerRecord.VoreGoal.IsLethal == isLethal
                               && (voreTrackerRecord.CurrentVoreStage.def.partName.ToLower() == "stomach"
                                || voreTrackerRecord.CurrentVoreStage.def.displayPartName.ToLower() == "intestines"
                                || voreTrackerRecord.CurrentVoreStage.def.displayPartName.ToLower() == "womb"))
                            {
                                return true;
                            }

                        }
                }
            }
            return false;
        }

        static public bool HasPreyIn(Pawn pawn, string organ)
        {
            PawnData pawnData = pawn.PawnData(false) ?? null;
            if (pawnData != null && pawn.IsActivePredator())
            {
                VoreTracker voreTracker = pawnData.VoreTracker;
                if (voreTracker != null)
                    foreach (VoreTrackerRecord voreTrackerRecord in voreTracker.VoreTrackerRecords)
                        if (voreTrackerRecord.CurrentVoreStage.def.partName.ToLower() == organ.ToLower())
                            return true;
            }
            return false;
        }
        static public int GetPreyCount(Pawn pawn)
        {
            int count = 0;
            PawnData pawnData = pawn.PawnData(false) ?? null;
            if (pawnData != null && pawn.IsActivePredator())
            {
                VoreTracker voreTracker = pawnData.VoreTracker;
                if (voreTracker != null)
                    foreach (VoreTrackerRecord voreTrackerRecord in voreTracker.VoreTrackerRecords)
                    {
                        count += 1;
                        if (!voreTrackerRecord.Prey.Dead && voreTrackerRecord.Prey.IsActivePredator())
                            if (voreTrackerRecord.Prey.PawnData(false) != null)
                                count += GetPreyCount(voreTrackerRecord.Prey);
                    }
            }
            return count;
        }
        static public float GetPreySize(Pawn pawn)
        {
            float weight = 0;
            PawnData pawnData = pawn.PawnData(false) ?? null;
            if (pawnData != null && pawn.IsActivePredator())
            {
                VoreTracker voreTracker = pawnData.VoreTracker;
                if (voreTracker != null)
                    foreach (VoreTrackerRecord voreTrackerRecord in voreTracker.VoreTrackerRecords)
                    {
                        weight += voreTrackerRecord.Prey.BodySize;
                        if (!voreTrackerRecord.Prey.Dead && voreTrackerRecord.Prey.IsActivePredator())
                            if (voreTrackerRecord.Prey.PawnData(false) != null)
                                weight += GetPreySize(voreTrackerRecord.Prey);
                    }
            }
            return weight;
        }

        static public int GetHighestPreySkillLevel(Pawn pawn, SkillDef skill)
        {
            try
            {
                int level = 0;
                PawnData pawnData = pawn.PawnData(false) ?? null;
                if (pawnData != null && pawn.IsActivePredator())
                {
                    VoreTracker voreTracker = pawnData.VoreTracker;
                    if (voreTracker != null)
                        foreach (VoreTrackerRecord voreTrackerRecord in voreTracker.VoreTrackerRecords)
                        {
                            if (voreTrackerRecord.Prey.skills?.GetSkill(skill) != null)
                                level = Math.Max(level, voreTrackerRecord.Prey.skills.GetSkill(skill).levelInt);
                        }
                }
                return level;
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when trying to get " + pawn.LabelShort + "'s total prey count: " + e);
                return 0;
            }
        }

        static public bool ShouldBandaid(Pawn pred, Pawn prey) // I'm working on it
        {
            if (pred.genes != null && pred.genes.Xenotype != null)
            {
                if (pred.genes.xenotypeName == "basic android" || pred.genes.xenotypeName == "awakened android")
                    return true;
            }
            if (prey.genes != null && prey.genes.Xenotype != null)
            {
                if (prey.genes.xenotypeName == "basic android" || prey.genes.xenotypeName == "awakened android")
                    return true;
            }
            return false;
        }
    }
}
