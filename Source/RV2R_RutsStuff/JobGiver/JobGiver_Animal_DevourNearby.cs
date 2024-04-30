using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RV2R_RutsStuff
{
    public class JobGiver_Animal_DevourNearby : ThinkNode_JobGiver
    {

        private float radius = 30f;
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_Animal_DevourNearby jobGiver_Animal_DevourNearby = (JobGiver_Animal_DevourNearby)base.DeepCopy(resolve);
            jobGiver_Animal_DevourNearby.radius = this.radius;
            return jobGiver_Animal_DevourNearby;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.CanParticipateInVore(out string _))
                return null;

            if (GenAI.InDangerousCombat(pawn))
                return null;

            if (GenAI.EnemyIsNear(pawn, radius))
                return null;

            try
            {
                bool predicate(Thing t)
                {
                    Pawn target = (Pawn)t;
                    return (target.Downed || !pawn.Faction.HostileTo(target.Faction))
                        && pawn.CanVore(target, out _)
                        && pawn.CanReserve(target, 1, -1, null, false)
                        && !target.IsMechanoid()
                        && !target.IsForbidden(pawn)
                        && target.Map.designationManager.DesignationOn(target, RV2R_Common.Devour) != null;
                }
                Pawn prey = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), this.radius, predicate, null, 0, -1, false, RegionType.Set_Passable, false);
                if (prey == null)
                    return null;

                VoreJob voreJob = VoreJobMaker.MakeJob(VoreJobDefOf.RV2_VoreInitAsPredator, prey);
                voreJob.targetA = prey;
                voreJob.Initiator = pawn;
                voreJob.IsForced = true;
                voreJob.count = 1;

                VorePathDef vorePathDef;

                List<VoreGoalDef> list = DefDatabase<VoreGoalDef>.AllDefsListForReading.Where((VoreGoalDef goal) => goal.IsLethal).ToList();

                IEnumerable<VorePathDef> interaction = VoreInteractionManager.Retrieve(new VoreInteractionRequest(pawn, prey, VoreRole.Predator, true, false, false, null, null, null, null, list)).ValidPaths;

                if (!interaction.EnumerableNullOrEmpty<VorePathDef>())
                {
                    vorePathDef = interaction.RandomElement<VorePathDef>();
                    RV2Log.Message(pawn.LabelShort + " eating hostile " + prey.LabelShort + " via " + vorePathDef.ToString(), "Jobs");
                    voreJob.VorePath = vorePathDef;
                    voreJob.IsForced = true;
                    return voreJob;
                }
                RV2Log.Message("Predator " + pawn.LabelShort + " can't fatal vore " + prey.LabelShort + "; checking for healing instead", true, "Jobs");

                list = new List<VoreGoalDef> { VoreGoalDefOf.Heal };
                if (prey.Faction.IsPlayer && !prey.health.HasHediffsNeedingTend())
                    list = DefDatabase<VoreGoalDef>.AllDefsListForReading.Where((VoreGoalDef goal) => !goal.IsLethal).ToList();
                interaction = VoreInteractionManager.Retrieve(new VoreInteractionRequest(pawn, prey, VoreRole.Predator, true, false, false, null, null, null, null, list)).ValidPaths;
                if (interaction.EnumerableNullOrEmpty<VorePathDef>())
                {
                    RV2Log.Message("Predator " + pawn.LabelShort + " can't heal vore " + prey.LabelShort + ", no predation", true, "Jobs");
                    return null;
                }
                vorePathDef = interaction.RandomElement<VorePathDef>();
                RV2Log.Message(pawn.LabelShort + " capturing hostile " + prey.LabelShort + " via " + vorePathDef.ToString(), "Jobs");
                voreJob.VorePath = vorePathDef;
                voreJob.IsForced = true;
                return voreJob;
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when " + pawn.LabelShort + " tried to nom downed enemy: " + e);
                return null;
            }
        }
    }
}
