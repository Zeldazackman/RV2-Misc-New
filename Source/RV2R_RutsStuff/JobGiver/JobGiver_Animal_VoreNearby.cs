using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    public class JobGiver_Animal_VoreNearby : ThinkNode_JobGiver
    {
        private float radius = 30f;
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_Animal_VoreNearby jobGiver_Animal_EndoNearby = (JobGiver_Animal_VoreNearby)base.DeepCopy(resolve);
            jobGiver_Animal_EndoNearby.radius = this.radius;
            return jobGiver_Animal_EndoNearby;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.CanParticipateInVore(out string reason))
                return null;

            if (GenAI.InDangerousCombat(pawn))
                return null;

            try
            {
                Pawn prey = null;

                if (RV2_Rut_Settings.rutsStuff.VornyBonds && pawn.relations.GetDirectRelationsCount(PawnRelationDefOf.Bond) > 0)
                {
                    Pawn bond = pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond);
                    if (bond != null
                     && !GenAI.EnemyIsNear(pawn, radius)
                     && !RV2R_Utilities.IsBusy(pawn, bond)
                     && pawn.Position.DistanceTo(bond.Position) < radius
                     && pawn.CanReserve(bond, 1, -1, null, false)
                     && pawn.CanVore(bond, out reason)
                     && Rand.Chance(0.20f))
                        prey = bond;
                }
                if (prey == null)
                {
                    bool predicate(Thing t)
                    {
                        Pawn pawn3 = (Pawn)t;
                        return pawn3 != pawn
                            && (RV2_Rut_Settings.rutsStuff.PlayVoreIndescriminate || (pawn3.Faction != null && pawn3.Faction.IsPlayer))
                            && !GenAI.InDangerousCombat(pawn3)
                            && pawn.CanReserve(pawn3, 1, -1, null, false)
                            && !pawn3.IsForbidden(pawn)
                            && !GenAI.EnemyIsNear(pawn, radius)
                            && !RV2R_Utilities.IsBusy(pawn, pawn3)
                            && !RV2R_Utilities.IsColonyHostile(pawn, pawn3)
                            && pawn.CanVore(pawn3, out reason)
                            && (!pawn.IsColonistPlayerControlled || Rand.Chance(RV2_Rut_Settings.rutsStuff.PlayVoreColonistBias));
                    }
                    prey = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false, false, false), this.radius, predicate, null, 0, -1, false, RegionType.Set_Passable, false);
                }
                if (prey == null)
                    return null;
                List<VoreGoalDef> list = DefDatabase<VoreGoalDef>.AllDefsListForReading.Where((VoreGoalDef goal) => !goal.IsLethal && goal.defName != "Store").ToList();
                if (!pawn.Name.Numerical)
                    list.Append(VoreGoalDefOf.Store);

                if (RV2_Rut_Settings.rutsStuff.FatalPlayVore)
                    list = DefDatabase<VoreGoalDef>.AllDefsListForReading;
                VoreInteraction voreInteraction = VoreInteractionManager.Retrieve(new VoreInteractionRequest(pawn, prey, VoreRole.Predator, true, false, false, null, null, null, null, list));
                if (voreInteraction.ValidPaths.EnumerableNullOrEmpty<VorePathDef>())
                {
                    RV2Log.Message("Predator " + pawn.LabelShort + " can't play-pred their target " + prey.LabelShort, "Jobs");
                    return null;
                }
                VorePathDef vorePathDef = (voreInteraction.PreferredPath ?? voreInteraction.ValidPaths.RandomElement<VorePathDef>());
                RV2Log.Message(pawn.LabelShort + " play-predding " + prey.LabelShort + " via " + vorePathDef.label, "Jobs");
                VoreProposal_TwoWay voreProposal_TwoWay = new VoreProposal_TwoWay(voreInteraction.Predator, voreInteraction.Prey, pawn, prey, vorePathDef);
                VoreJob voreJob = VoreJobMaker.MakeJob(prey.IsColonistPlayerControlled ? VoreJobDefOf.RV2_ProposeVore : VoreJobDefOf.RV2_VoreInitAsPredator, pawn);
                voreJob.targetA = prey;
                voreJob.Proposal = voreProposal_TwoWay;
                voreJob.VorePath = vorePathDef;
                voreJob.Initiator = pawn;
                voreJob.count = 1;
                return voreJob;
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when " + pawn.LabelShort + " tried to play-pred: " + e);
                return null;
            }
        }
    }
}
