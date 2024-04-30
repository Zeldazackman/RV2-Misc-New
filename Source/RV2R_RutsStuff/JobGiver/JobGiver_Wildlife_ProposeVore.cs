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
    public class JobGiver_Wildlife_ProposeVore : ThinkNode_JobGiver
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_Wildlife_ProposeVore jobGiver_Wildlife_ProposeVore = (JobGiver_Wildlife_ProposeVore)base.DeepCopy(resolve);
            return jobGiver_Wildlife_ProposeVore;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.CanParticipateInVore(out _))
                return null;

            if (GenAI.InDangerousCombat(pawn))
                return null;

            try
            {
                Pawn target = null;
                bool predicate(Thing t)
                {
                    Pawn pawn3 = (Pawn)t;
                    return pawn3 != pawn
                        && !GenAI.InDangerousCombat(pawn3)
                        && pawn.CanReserve(pawn3, 1, -1, null, false)
                        && !pawn3.IsForbidden(pawn)
                        && !RV2R_Utilities.IsBusy(pawn, pawn3)
                        && pawn3.CanParticipateInVore(out _)
                        && (pawn3.IsWildAnimal() || (RV2_Rut_Settings.rutsStuff.WildToColonistPrey || RV2_Rut_Settings.rutsStuff.WildToColonistPred))
                        && (pawn.RaceProps.predator || (pawn3.RaceProps.predator || RV2R_Utilities.IsSapient(pawn3)))
                        && (pawn.RaceProps.predator || RV2_Rut_Settings.rutsStuff.WildPreyProposals);
                }
                target = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false, false, false), 30f, predicate, null, 0, -1, false, RegionType.Set_Passable, false);

                if (target == null)
                    return null;

                List<VoreGoalDef> list = DefDatabase<VoreGoalDef>.AllDefsListForReading.ToList();

                VoreRole role = VoreRole.Invalid;

                if (RV2_Rut_Settings.rutsStuff.WildPredatorProposals && !pawn.RaceProps.predator)
                    role = VoreRole.Prey;
                else if (!RV2_Rut_Settings.rutsStuff.WildToColonistPred)
                    role = VoreRole.Prey;

                VoreInteraction voreInteraction = VoreInteractionManager.Retrieve(new VoreInteractionRequest(pawn, target, role, true, false, false, null, null, null, null, list));
                
                if (voreInteraction.ValidPaths.EnumerableNullOrEmpty<VorePathDef>())
                {
                    return null;
                }

                VorePathDef vorePathDef = (voreInteraction.PreferredPath ?? voreInteraction.ValidPaths.RandomElement<VorePathDef>());
                VoreProposal_TwoWay voreProposal_TwoWay = new VoreProposal_TwoWay(voreInteraction.Predator, voreInteraction.Prey, pawn, target, vorePathDef);
                VoreJob voreJob = VoreJobMaker.MakeJob(VoreJobDefOf.RV2_ProposeVore, pawn);
                voreJob.targetA = target;
                voreJob.Proposal = voreProposal_TwoWay;
                voreJob.VorePath = vorePathDef;
                voreJob.Initiator = pawn;
                voreJob.count = 1;
                return voreJob;
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when wild " + pawn.LabelShort + " tried to propose vore: " + e);
                return null;
            }
        }
    }
}
