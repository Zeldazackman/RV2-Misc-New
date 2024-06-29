using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    public class JobGiver_DoGutLovin : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.IsActivePredator())
                return null;

            if (Find.TickManager.TicksGame < pawn.mindState.canLovinTick)
                return null;

            if (!pawn.health.capacities.CanBeAwake)
                return null;

            Building_Bed building_Bed = pawn.CurrentBed();
            if (building_Bed != null)
            {
                if (building_Bed.Medical)
                    return null;
                if (building_Bed.SleepingSlotsCount > 1)
                    using (IEnumerator<Pawn> enumerator = building_Bed.CurOccupants.GetEnumerator())
                        while (enumerator.MoveNext())
                            if (enumerator.Current != pawn)
                                return null;
            }
            else
                if (RestUtility.Awake(pawn))
                return null;

            PawnData pawnData = pawn.PawnData(false);
            if (pawnData == null)
                return null;

            float predLib = 1f;
            float preyLib = 0f;
            int bestOp = -100;

            if (pawn.QuirkManager(false).HasValueModifier("Predator_Libido"))
                predLib = pawn.QuirkManager(false).ModifyValue("Predator_Libido", predLib);

            if (predLib <= 0f)
                return null;

            bool loverPresnt = false;
            bool canLove = false;
            bool harknessTest = false;
            bool noAttraction = predLib < 1.5f;

            IEnumerable<Pawn> familyByBlood = pawn.relations.FamilyByBlood;
            VoreTracker voreTracker = pawnData.VoreTracker;
            if (voreTracker != null)
                foreach (VoreTrackerRecord voreTrackerRecord in voreTracker.VoreTrackerRecords)
                {
                    float tempPreyLib = 1f;
                    Pawn preyPawn = voreTrackerRecord.Prey;
                    foreach (Pawn pawn2 in familyByBlood)
                        if (preyPawn == pawn2
                         && !LovePartnerRelationUtility.LovePartnerRelationExists(voreTrackerRecord.Prey, voreTrackerRecord.Predator))
                        {
                            loverPresnt = false;
                            predLib = 0f;
                            break;
                        }

                    if (RV2R_Utilities.IsSapient(preyPawn))
                        if (pawn.IsHumanoid() || RV2_Rut_Settings.rutsStuff.GutLovinSapients)
                            harknessTest = true;

                    if (preyPawn.QuirkManager(false).HasValueModifier("Prey_Libido"))
                        tempPreyLib = preyPawn.QuirkManager(false).ModifyValue("Prey_Libido", tempPreyLib);

                    if (RV2R_Utilities.IsAttracted(pawn, preyPawn))
                        noAttraction = false;

                    if (tempPreyLib < 1.5f && !RV2R_Utilities.IsAttracted(preyPawn, pawn))
                        tempPreyLib /= 2f;

                    bestOp = Math.Max(bestOp, pawn.relations.OpinionOf(voreTrackerRecord.Prey));

                    preyLib = Math.Max(preyLib, tempPreyLib);  // Use the vornyest prey's score

                    if (LovePartnerRelationUtility.LovePartnerRelationExists(voreTrackerRecord.Prey, voreTrackerRecord.Predator) || RV2_Rut_Settings.rutsStuff.GutLovinCheats)
                        loverPresnt = true;

                    canLove = (preyLib > 0 || canLove);
                }
            if (noAttraction)
                predLib /= 2f;

            bool pyscho = RV2_Rut_Settings.rutsStuff.GutLovinNonCon
                      && (!pawn.story.traits.HasTrait(TraitDefOf.Kind)
                      && (pawn.story.traits.HasTrait(TraitDefOf.Psychopath)
                       || bestOp <= -10));

            if (!harknessTest)
                return null;        // If we're not being a barn

            if (!pyscho && !canLove)
                return null;        // And -someone- in there wants it (or we don't care)

            if (!(loverPresnt || RV2_Rut_Settings.rutsStuff.GutLovinCheats)
              && !(LovePartnerRelationUtility.ExistingLovePartner(pawn, false) == null && RV2_Rut_Settings.rutsStuff.GutLovinStands))
                return null;        // And we're romanticly involved (or don't care)

            if (Rand.Chance(pyscho ? predLib : ((predLib + preyLib) / 2f)))  // And we're in the mood
                return JobMaker.MakeJob(RV2R_Common.RV2R_GutLovin, pawn.CurrentBed());                                  // Then the owo begins

            return null;
        }
    }
}
