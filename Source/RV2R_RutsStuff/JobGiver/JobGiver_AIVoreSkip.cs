using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    internal class JobGiver_AIVoreSkip : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.HasPsylink)
                return null;
            if (pawn.psychicEntropy.Severity > PsychicEntropySeverity.Safe)
                return null;
            if (pawn.mindState.enemyTarget == null)
                return null;
            if (pawn.mindState.enemyTarget as Pawn == null)
                return null;

            List<Ability> abilities = pawn.abilities.abilities;
            if (abilities.Count == 0)
                return null;

            bool valid = false;

            foreach (Ability ability in abilities) 
                if (ability.def == DefDatabase<AbilityDef>.GetNamedSilentFail("RV2_VoreSkip_Chaotic") || ability.def == DefDatabase<AbilityDef>.GetNamedSilentFail("RV2_VoreSkip_Smart"))
                    valid = true;

            if (!valid)
                return null;

            Pawn victim = pawn.mindState.enemyTarget as Pawn;

            bool predicate(Thing t)
            {
                Pawn pawn3 = (Pawn)t;
                return pawn3.HostileTo(pawn)
                    && !pawn3.DestroyedOrNull()
                    && !pawn3.IsInvisible()
                    && !pawn3.Downed
                    && !pawn3.IsForbidden(pawn)
                    && pawn3.GetVoreRecord() == null
                    && !RV2R_Utilities.IsBusy(pawn, pawn3)
                    && GenSight.LineOfSight(pawn.Position, pawn3.Position, pawn.Map, false, null, 0, 0)
                    && pawn.Position.InHorDistOf(pawn3.Position, 17.9f);
            }
            bool allypredicate(Thing t)
            {
                Pawn pawn3 = (Pawn)t;
                return !pawn3.HostileTo(pawn)
                    && !pawn3.DestroyedOrNull()
                    && !pawn3.IsInvisible()
                    && !pawn3.Downed
                    && !pawn3.IsForbidden(pawn)
                    && pawn3.GetVoreRecord() == null
                    && pawn3.CanVore(victim, out _)
                    && !RV2R_Utilities.IsBusy(pawn, pawn3)
                    && GenSight.LineOfSight(pawn.Position, pawn3.Position, pawn.Map, false, null, 0, 0)
                    && pawn.Position.InHorDistOf(pawn3.Position, 17.9f);
            }

            if (victim.DestroyedOrNull()
             || victim.IsInvisible()
             || victim.Downed
             || victim.GetVoreRecord() != null
             || !victim.CanBePrey(out _)
             || !GenSight.LineOfSight(pawn.Position, victim.Position, pawn.Map, false, null, 0, 0))
                victim = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false, false, false), 11.9f, predicate, null, 0, -1, false, RegionType.Set_Passable, false);

            if (victim.DestroyedOrNull())
                return null;

            Pawn ally = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false, false, false), 11.9f, allypredicate, null, 0, -1, false, RegionType.Set_Passable, false);

            if (ally.DestroyedOrNull()) 
                return null;

            foreach (Ability ability in abilities)
                if (ability.def == DefDatabase<AbilityDef>.GetNamedSilentFail("RV2_VoreSkip_Smart"))
                    return JobMaker.MakeJob(RV2R_Common.AI_CastVoreSkip_Smart, victim, ally);

            return JobMaker.MakeJob(RV2R_Common.AI_CastVoreSkip, victim, ally);
        }
    }
}
