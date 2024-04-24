using RimVore2;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    public class JobDriver_GutLovin : JobDriver
    {
        private Building_Bed Bed
        {
            get
            {
                return (Building_Bed)(Thing)this.job.GetTarget(this.BedInd);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksLeft, "ticksLeft", 0, false);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (this.Bed != null)
                return this.pawn.Reserve(this.Bed, this.job, this.Bed.SleepingSlotsCount, 0, null, errorOnFailed);

            return true;
        }

        public override bool CanBeginNowWhileLyingDown()
        {
            return Bed == null || JobInBedUtility.InBedOrRestSpotNow(this.pawn, this.job.GetTarget(this.BedInd));
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            float predMod = 1f;
            float preyMod = 0f;

            List<Pawn> willingList = new List<Pawn>();
            List<Pawn> unwillingList = new List<Pawn>();

            QuirkManager quirkManager = this.pawn.QuirkManager(false);

            predMod = quirkManager?.ModifyValue("Prey_Libido", predMod) ?? 0.66f;

            RimVore2.PawnData pawnData = this.pawn.PawnData(false);
            VoreTracker voreTracker = null;

            if (pawnData != null
             && pawnData.VoreTracker != null)
                voreTracker = pawnData.VoreTracker;

            RV2Log.Message("Predator " + this.pawn.ToString() + " starting gut lovin' ", "Gutlovin");
            foreach (VoreTrackerRecord voreTrackerRecord in voreTracker.VoreTrackerRecords)
                if (voreTrackerRecord.Prey.IsHumanoid() || (RV2_Rut_Settings.rutsStuff.GutLovinSapients && RV2R_Utilities.IsSapient(voreTrackerRecord.Prey)))
                {
                    float num = 1f;
                    if (voreTrackerRecord.Prey.QuirkManager(false) != null && voreTrackerRecord.Prey.QuirkManager(false).HasValueModifier("Prey_Libido"))
                        num = voreTrackerRecord.Prey.QuirkManager(false).ModifyValue("Prey_Libido", num);

                    if (num <= .33f && voreTrackerRecord.Prey.relations.OpinionOf(pawn) < 0)
                        unwillingList.Add(voreTrackerRecord.Prey);
                    else
                        willingList.Add(voreTrackerRecord.Prey);

                    RV2Log.Message("Prey " + voreTrackerRecord.Prey.ToString() + " involved, willingness at " + num.ToString(), "Gutlovin");
                    preyMod = Mathf.Max(preyMod, num);
                }

            if (Bed != null)
            {
                this.FailOnDespawnedOrNull(this.BedInd);
                this.KeepLyingDown(this.BedInd);
                yield return Toils_Bed.ClaimBedIfNonMedical(this.BedInd, TargetIndex.None);
                yield return Toils_Bed.GotoBed(this.BedInd);
            }
            else
            {
                pawn.jobs.jobQueue.EnqueueFirst(JobMaker.MakeJob(JobDefOf.LayDown, pawn.Position));
            }

            this.ticksLeft = (int)(2000f * ((predMod + preyMod) / 2f) * Mathf.Clamp(Rand.Range(0.5f, 1.1f), 0.5f, 1.5f));
            //RV2Log.Message("Gut lovin' lasts " + this.ticksLeft.ToString() + " ticks", "Gutlovin");

            Toil toil = Toils_LayDown.LayDown(this.BedInd, Bed != null, false, false, false, PawnPosture.LayingOnGroundNormal);
            toil.AddPreTickAction(delegate
            {
                this.ticksLeft--;
                if (this.ticksLeft <= 0)
                {
                    this.ReadyForNextToil();
                    return;
                }
                if (this.pawn.IsHashIntervalTick(100))
                {
                    FleckMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, FleckDefOf.Heart, 0.42f);
                    if (this.pawn.needs.joy != null)
                        this.pawn.needs.joy.CurLevel += 0.002f * predMod;
                    if (pawnData.VoreTracker != null)
                        foreach (VoreTrackerRecord voreTrackerRecord2 in voreTracker.VoreTrackerRecords)
                        {
                            if (voreTrackerRecord2.Prey.needs.joy != null)
                            {
                                float willingness = 1f;
                                if (voreTrackerRecord2.Prey.QuirkManager(false) != null && voreTrackerRecord2.Prey.QuirkManager().HasValueModifier("Prey_Libido"))
                                    willingness = quirkManager.ModifyValue("Prey_Libido", willingness);

                                if (willingness > 0f)
                                    voreTrackerRecord2.Prey.needs.joy.CurLevel += 0.015f * willingness;
                            }
                        }
                }
            });

            toil.AddFinishAction(delegate
            {
                // Okay, so, to explain: The pred needs a mood boost, and an opinion boost for each prey.
                // Since getting +120 mood from belly humping 10 prey is maybe a little much,
                // we handle the mood boost and opinion boosts with seperate thoughts.

                ThoughtDef predThought = RV2R_Common.PredLovin_Normal_Mood;
                ThoughtDef predOpinion = RV2R_Common.PredLovin_Normal;
                if (predMod > 1.5f)
                {
                    predThought = RV2R_Common.PredLovin_VeryGood_Mood;
                    predOpinion = RV2R_Common.PredLovin_VeryGood;
                }
                else if (predMod >= 1f)
                {
                    predThought = RV2R_Common.PredLovin_Good_Mood;
                    predOpinion = RV2R_Common.PredLovin_Good;
                }
                else if (predMod <= 0.5f)
                {
                    predThought = RV2R_Common.PreyLovin_Meh; // Meh contains no mood boosts, so doubling up does nothing
                    predOpinion = RV2R_Common.PreyLovin_Meh;
                }
                Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(predThought);
                Thought_Memory thought_MemoryOpinion = (Thought_Memory)ThoughtMaker.MakeThought(predOpinion);

                if (this.pawn.health != null && this.pawn.health.hediffSet != null)
                {
                    thought_Memory.moodOffset += (int)predMod;
                    if (this.pawn.health.hediffSet.hediffs.Any((Hediff h) => h.def == HediffDefOf.LoveEnhancer))
                        thought_Memory.moodPowerFactor = 1.5f;

                }
                this.pawn.needs.mood?.thoughts.memories.TryGainMemory(predThought, null, null);

                this.pawn.mindState.canLovinTick = Find.TickManager.TicksGame + this.GenerateRandomMinTicksToNextLovin(this.pawn);
                RV2Log.Message("Gave predator " + this.pawn.ToString() + " mood memory " + predThought.ToString(), "Gutlovin");

                ThoughtDef preyThought = RV2R_Common.PreyLovin_Normal;
                float preyLib = 1f;

                foreach (VoreTrackerRecord voreTrackerRecord2 in voreTracker.VoreTrackerRecords)
                {
                    if (willingList.Contains(voreTrackerRecord2.Prey))
                    {
                        preyLib = 1f;
                        preyLib = voreTrackerRecord2.Prey.QuirkManager(false)?.ModifyValue("Prey_Libido", preyLib) ?? 0.66f;
                        preyThought = RV2R_Common.PreyLovin_Normal;
                        if (preyLib > 0f)
                            willingList.Add(voreTrackerRecord2.Prey);

                        if (preyLib > 1.5f)
                            preyThought = RV2R_Common.PreyLovin_VeryGood;
                        else if (preyLib >= 1f)
                            preyThought = RV2R_Common.PreyLovin_Good;
                        else if (preyLib <= 0.0f)
                            preyThought = null;
                        else if (preyLib <= 0.5f)
                            preyThought = RV2R_Common.PreyLovin_Meh;

                        voreTrackerRecord2.Prey.mindState.canLovinTick = Find.TickManager.TicksGame + this.GenerateRandomMinTicksToNextLovin(voreTrackerRecord2.Prey);

                        // You're welcome for the mental images this little bit may generate
                        if (LovePartnerRelationUtility.ExistingLovePartner(voreTrackerRecord2.Prey, true) != null
                         && willingList.Contains(LovePartnerRelationUtility.ExistingLovePartner(voreTrackerRecord2.Prey, true)))
                            if (LovePartnerRelationUtility.ExistingLovePartner(voreTrackerRecord2.Prey, true).GetVoreRecord() != null)
                                if (LovePartnerRelationUtility.ExistingLovePartner(voreTrackerRecord2.Prey, true).GetVoreRecord().Predator == voreTrackerRecord2.Predator
                                  && LovePartnerRelationUtility.ExistingLovePartner(voreTrackerRecord2.Prey, true).GetVoreRecord().CurrentBodyPart == voreTrackerRecord2.CurrentBodyPart)
                                    voreTrackerRecord2.Prey.needs.mood?.thoughts.memories.TryGainMemory(ThoughtDefOf.GotSomeLovin, pawn, null);
                    }

                    // For vore rape; only applied if no-one was willing
                    if (unwillingList.Contains(voreTrackerRecord2.Prey) && willingList.NullOrEmpty<Pawn>())
                        preyThought = RV2R_Common.PreyLovin_Bad;

                    if (preyThought != null)
                    {
                        thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(preyThought);
                        if (voreTrackerRecord2.Prey.needs.mood != null)
                        {
                            voreTrackerRecord2.Prey.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, this.pawn);
                            RV2Log.Message("Gave prey " + voreTrackerRecord2.Prey.ToString() + " memory " + thought_Memory.ToString(), "Gutlovin");
                        }
                        thought_MemoryOpinion = (Thought_Memory)ThoughtMaker.MakeThought(predOpinion);
                        if (this.pawn.needs.mood != null)
                        {
                            this.pawn.needs.mood.thoughts.memories.TryGainMemory(thought_MemoryOpinion, voreTrackerRecord2.Prey);
                            RV2Log.Message("Gave predator " + this.pawn.ToString() + " opinion memory " + thought_MemoryOpinion.ToString() + "for prey " + thought_MemoryOpinion.otherPawn.LabelShort, "Gutlovin");
                        }
                    }
                }
            });

            toil.socialMode = RandomSocialMode.Off;
            yield return toil;
            yield break;
        }

        private int GenerateRandomMinTicksToNextLovin(Pawn pawn)
        {
            if (DebugSettings.alwaysDoLovin)
                return 300;

            float relAge = (pawn.ageTracker.AgeBiologicalYearsFloat / pawn.ageTracker.AdultMinAge);
            float num = JobDriver_GutLovin.LovinIntervalHoursFromAgeCurve.Evaluate(relAge);
            num = Rand.Gaussian(num, 0.3f);

            return (int)(num * 2500f);
        }

        private int ticksLeft;

        private readonly TargetIndex BedInd = TargetIndex.A;

        private static readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
        {
            {
                new CurvePoint(1f, 1.5f),
                true
            },
            {
                new CurvePoint(1.375f, 1.5f),
                true
            },
            {
                new CurvePoint(1.875f, 4f),
                true
            },
            {
                new CurvePoint(3.125f, 12f),
                true
            },
            {
                new CurvePoint(4.675f, 36f),
                true
            }
        };
    }
}
