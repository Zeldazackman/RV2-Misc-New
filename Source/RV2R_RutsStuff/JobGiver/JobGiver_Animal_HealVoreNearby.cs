using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    public class JobGiver_Animal_HealVoreNearby : ThinkNode_JobGiver
    {

        private float radius = 30f;
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_Animal_HealVoreNearby jobGiver_Animal_HealVoreNearby = (JobGiver_Animal_HealVoreNearby)base.DeepCopy(resolve);
            jobGiver_Animal_HealVoreNearby.radius = this.radius;
            return jobGiver_Animal_HealVoreNearby;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.CanParticipateInVore(out string reason))
                return null;

            if (GenAI.InDangerousCombat(pawn))
                return null;

            if (GenAI.EnemyIsNear(pawn, radius))
                return null;

            try
            {
                bool predicate(Thing t)
                {
                    Pawn pawn3 = (Pawn)t;
                    return pawn.CanEndoVore(pawn3, out reason, false)
                        && !pawn3.IsMechanoid()
                        && pawn3.health.summaryHealth.SummaryHealthPercent <= 0.9f
                        && (pawn3.health.InPainShock
                         || pawn3.Downed
                        && (!pawn3.InBed() || (
                           pawn3.health.summaryHealth.SummaryHealthPercent <= 0.65f
                           && RV2_Rut_Settings.rutsStuff.AnimalDoctors
                           && (pawn3.IsAnimal() 
                           || (pawn3.IsHumanoid() && (!pawn3.health.HasHediffsNeedingTendByPlayer() || HealthUtility.TicksUntilDeathDueToBloodLoss(pawn3) <= 9000))))
                        ))
                        && pawn.CanReserve(pawn3, 1, -1, null, false)
                        && !pawn3.IsForbidden(pawn)
                        && !RV2R_Utilities.IsBusy(pawn, pawn3)
                        && RV2R_Utilities.ShouldFriendlyTarget(pawn, pawn3);
                }
                Pawn prey = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), this.radius, predicate, null, 0, -1, false, RegionType.Set_Passable, false);
                if (prey == null)
                    return null;

                List<VoreGoalDef> list = new List<VoreGoalDef> { VoreGoalDefOf.Heal };
                IEnumerable<VorePathDef> validPaths = VoreInteractionManager.Retrieve(new VoreInteractionRequest(pawn, prey, VoreRole.Predator, true, false, false, null, null, null, null, list)).ValidPaths;
                if (validPaths.EnumerableNullOrEmpty<VorePathDef>())
                {
                    RV2Log.Message("Predator " + pawn.LabelShort + " can't endo-rescue " + prey.LabelShort, "Jobs");
                    return null;
                }
                VorePathDef vorePathDef = validPaths.RandomElement<VorePathDef>();
                RV2Log.Message(prey.LabelShort + "'s getting endo rescued by " + pawn.LabelShort + " via " + vorePathDef.label, "Jobs");
                VoreJob voreJob = VoreJobMaker.MakeJob(VoreJobDefOf.RV2_VoreInitAsPredator, prey);
                voreJob.targetA = prey;
                voreJob.VorePath = vorePathDef;
                voreJob.Initiator = pawn;
                voreJob.count = 1;
                return voreJob;
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when " + pawn.LabelShort + " tried to endo-rescue: " + e);
                return null;
            }
        }
    }
}
