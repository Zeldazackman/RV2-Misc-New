using HarmonyLib;
using RimVore2;
using RimWorld;
using System;
using System.Linq;
using Verse;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    [HarmonyPatch(typeof(VoreTrackerRecord), "TickRare")]
    internal class Patch_DoMisc
    {
        [HarmonyPostfix]
        public static void AdditionalEffects(VoreTrackerRecord __instance)
        {
            if (RV2R_Utilities.ShouldBandaid(__instance.Predator, __instance.Prey))
                return;

            if (__instance.VoreGoal.IsLethal)
                return;

            if (!__instance.IsOnMap())
                return;

            SettingsContainer_RutsStuff settings = RV2_Rut_Settings.rutsStuff;

            try
            {
                if (settings.PreyCapacityAclimation > 0f
                 && RV2R_Utilities.GetPreySize(__instance.Predator) / __instance.Predator.CalculateVoreCapacity() >= __instance.Predator.CalculateVoreCapacity() * settings.PreyCapacityAclimationLimit)
                    HandleAcclimation(__instance);

                if (settings.EndoSicknessStrength > 0f)
                    HandleSickness(__instance);

                if (settings.EndoPets > 0f
                 && __instance.CurrentVoreStage.PassedRareTicks >= Math.Floor(GenDate.TicksPerDay / 100 * settings.EndoPets)
                 && __instance.Prey.relations.GetDirectRelationsCount(RV2R_Common.PetPrey) == 0)
                    HandlePets(__instance);

                if (__instance.CurrentVoreStage.def.displayPartName == "womb"
                 && settings.RegressionStrength > 0f)
                    HandleRegression(__instance);

                if (settings.EndoBondChance > 0.0000f)
                    HandleBonding(__instance);

                if (settings.EndoRecruitment
                 && (__instance.Prey.IsPrisonerOfColony && __instance.Predator.IsColonistPlayerControlled))
                    HandleImprisoned(__instance);

                if (settings.EndoOpinion && __instance.CurrentVoreStage.PassedRareTicks % 10 == 0)
                    HandleOpinion(__instance);

                if (__instance.Prey.needs.comfort != null)
                    if (__instance.Predator.QuirkManager(false) == null || !__instance.Predator.QuirkManager(false).HasValueModifier("VoreComfort"))
                        __instance.Prey.needs.comfort.CurLevelPercentage = Math.Min(0.5f, __instance.Prey.needs.comfort.CurLevelPercentage + 0.025f);
                    else if (__instance.Predator.QuirkManager(false).TryGetValueModifier("VoreComfort", ModifierOperation.Multiply, out float comfort))
                        __instance.Prey.needs.comfort.CurLevelPercentage = Math.Min(comfort, __instance.Prey.needs.comfort.CurLevelPercentage + comfort / 20f);

                if (settings.PreyJoy
                 && __instance.Prey.needs.joy != null
                 && (__instance.Prey.QuirkManager(false) == null
                 || __instance.Prey.QuirkManager(false).GetTotalSelectorModifier(VoreRole.Prey, ModifierOperation.Add) >= 0f)
                 && __instance.Prey.needs.joy.CurLevelPercentage < (__instance.IsForced ? 0.2f : 0.5f))
                    __instance.Prey.needs.joy.CurLevelPercentage += 0.025f;

                if (settings.NoBleedOut)
                {
                    Hediff bleedOut = __instance.Prey.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss, false);
                    if (bleedOut != null)
                        bleedOut.Severity = Math.Min(bleedOut.Severity, 0.9f);
                }

                if (settings.StopBleeding)
                    foreach (Hediff hediff in __instance.Prey.health.hediffSet.hediffs.Where((Hediff diff) => diff.Bleeding))
                    {
                        hediff.def.injuryProps.bleedRate *= 0.985f;
                        __instance.Prey.health.Notify_HediffChanged(hediff);
                    }

                if (settings.NoBadTemp)
                {
                    Hediff hypo = __instance.Prey.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia, false);
                    if (hypo != null)
                        hypo.Severity = Math.Min(hypo.Severity, 0.29f);
                    Hediff hyper = __instance.Prey.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Heatstroke, false);
                    if (hyper != null)
                        hyper.Severity = Math.Min(hyper.Severity, 0.29f);
                }
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong during vore tick for pred " + __instance.Predator.LabelShort + ", prey " + __instance.Prey.LabelShort + " : " + e);
                return;
            }

        }

        private static void HandleAcclimation(VoreTrackerRecord record)
        {
            if (!record.Predator.health.hediffSet.HasHediff(RV2R_Common.CapacityAcclimation, false))
                record.Predator.health.AddHediff(RV2R_Common.CapacityAcclimation, null, null, null);

            float mod = Math.Min(1f, 1f - ((RV2R_Utilities.GetPreySize(record.Predator) / record.Predator.CalculateVoreCapacity()) / RV2_Rut_Settings.rutsStuff.PreyCapacityAclimationLimit));
            record.Predator.health.hediffSet.GetFirstHediffOfDef(RV2R_Common.CapacityAcclimation, false).Severity += 0.00025f * mod * RV2_Rut_Settings.rutsStuff.PreyCapacityAclimation;
        }
        private static void HandleSickness(VoreTrackerRecord record)
        {

            Hediff firstHediffOfDef = record.Prey.health.hediffSet.GetFirstHediffOfDef(RV2R_Common.EndoHediff, false);
            if (firstHediffOfDef != null)
            {
                firstHediffOfDef.Severity = Math.Min(firstHediffOfDef.Severity + (0.0135f * RV2_Rut_Settings.rutsStuff.EndoSicknessStrength), 1f);
                if (RV2_Rut_Settings.rutsStuff.EndoPacify && firstHediffOfDef.Severity >= 0.95f)
                {
                    if (record.StruggleManager.shouldStruggle)
                        record.StruggleManager.shouldStruggle = false;
                    if (RV2R_Utilities.IsColonyHostile(record.Predator, record.Prey) && !record.Prey.IsPrisoner)
                        if (!RV2R_Utilities.IsSapient(record.Prey))
                            record.Prey.SetFaction(null, null);
                        else
                            record.Prey.guest?.SetGuestStatus(record.Predator.Faction, GuestStatus.Prisoner);
                }
            }
            else
                record.Prey.health.AddHediff(RV2R_Common.EndoHediff, null, null, null);
        }
        private static void HandlePets(VoreTrackerRecord record)
        {
            if (RV2R_Utilities.IsSapient(record.Predator) && RV2R_Utilities.IsSapient(record.Prey))
            {
                TaggedString taggedString = "RV2R_Pet".Translate(record.Prey.LabelShort, record.Predator.LabelShort);
                if (RV2_Rut_Settings.rutsStuff.EndoPetsJoin && record.Predator.Faction.IsPlayer && (record.Prey.Faction == null || !record.Prey.Faction.IsPlayer))
                {
                    taggedString += " " + "RV2R_PetJoined".Translate(record.Prey.LabelShort, record.Predator.LabelShort);
                }
                Messages.Message(taggedString, record.Prey, MessageTypeDefOf.PositiveEvent, true);
                record.Predator.relations.AddDirectRelation(RV2R_Common.PetPred, record.Prey);
                record.Prey.relations.AddDirectRelation(RV2R_Common.PetPrey, record.Predator);
            }
        }
        private static void HandleRegression(VoreTrackerRecord record)
        {
            if (record.Prey.ageTracker.AgeBiologicalTicks > record.Prey.ageTracker.AdultMinAgeTicks)
            {
                long adultAge = (long)record.Prey.RaceProps.lifeStageAges.Last().minAge * GenDate.TicksPerYear;
                long age = Math.Min(record.Prey.ageTracker.AgeBiologicalTicks - adultAge, (long)Math.Ceiling(GenDate.TicksPerDay * RV2_Rut_Settings.rutsStuff.RegressionStrength));
                record.Prey.ageTracker.AgeBiologicalTicks -= age;
                Message message = new Message();
                if (RV2_Rut_Settings.rutsStuff.ChronicCure)
                {
                    foreach (Hediff hediff in record.Prey.health.hediffSet.hediffs.Where((Hediff diff) => diff.def.chronic))
                    {
                        float ratio = adultAge / record.Prey.ageTracker.AgeBiologicalTicks;
                        if (ratio > 0.5f)
                        {
                            if (hediff.TryGetComp<HediffComp_SeverityPerDay>() != null)
                            {
                                hediff.Severity = Math.Max(hediff.Severity - 0.002f * (ratio / 2f), hediff.def.minSeverity > 0f ? hediff.def.minSeverity : 0f);
                                if (hediff.Severity <= 0f)
                                {
                                    message = new Message("HealingCureHediff".Translate(record.Prey, hediff.def.label), MessageTypeDefOf.PositiveEvent, new LookTargets(record.Prey));
                                    Messages.Message(message, true);
                                    record.Prey.health.RemoveHediff(record.Prey.health.hediffSet.GetFirstHediffOfDef(hediff.def, false));
                                }
                            }
                            else if (Rand.Chance(0.0005f * (ratio / 2f)))
                            {
                                message = new Message("HealingCureHediff".Translate(record.Prey, hediff.def.label), MessageTypeDefOf.PositiveEvent, new LookTargets(record.Prey));
                                Messages.Message(message, true);
                                record.Prey.health.RemoveHediff(record.Prey.health.hediffSet.GetFirstHediffOfDef(hediff.def, false));
                            }
                        }
                    }
                }
                PawnData pawnData = record.Prey.PawnData(false);
                if (pawnData != null && pawnData.VoreTracker != null)
                {
                    VoreTracker voreTracker = pawnData.VoreTracker;
                    foreach (VoreTrackerRecord voreTrackerRecord in voreTracker.VoreTrackerRecords)
                    {
                        HandleRegression(voreTrackerRecord);
                    }
                }
            }
        }

        private static void HandleBonding(VoreTrackerRecord record)
        {
            if (record.IsForced && RV2_Rut_Settings.rutsStuff.WillingOnlyEndoBond)
                return;

            bool validSetup = false;
            Pawn humanoid;
            Pawn animal;
            if (RV2R_Utilities.IsSapient(record.Predator) && RV2R_Utilities.IsSapient(record.Prey))
                return;
            if (RV2R_Utilities.IsSapient(record.Predator))
            {
                humanoid = record.Predator;
                animal = record.Prey;
                validSetup = true;
            }
            else
            {
                humanoid = record.Prey;
                animal = record.Predator;
            }
            if (animal.RaceProps.trainability == TrainabilityDefOf.None)
                return;
            if (animal.relations.GetDirectRelationsCount(PawnRelationDefOf.Bond, null) != 0)
                return;
            if (!validSetup && RV2_Rut_Settings.rutsStuff.PreyOnlyEndoBond)
                return;

            if (humanoid.IsColonist)
                if (Rand.Chance(RV2_Rut_Settings.rutsStuff.EndoBondChance))
                {
                    if (animal.Faction == null)
                        InteractionWorker_RecruitAttempt.DoRecruit(humanoid, animal, false);

                    RelationsUtility.TryDevelopBondRelation(humanoid, animal, 100f);
                    animal.training.Train(TrainableDefOf.Tameness, humanoid, true);
                    animal.training.Train(TrainableDefOf.Obedience, humanoid, true);
                    animal.training.SetWantedRecursive(TrainableDefOf.Obedience, true);
                }
        }
        private static void HandleImprisoned(VoreTrackerRecord record)
        {
#if v1_4
            if (!record.IsForced && record.Prey.guest.Recruitable && record.Prey.guest.resistance > 0f)
                if (record.Prey.guest.interactionMode == PrisonerInteractionModeDefOf.ReduceResistance
                 || record.Prey.guest.interactionMode == PrisonerInteractionModeDefOf.AttemptRecruit)
                {
                    float restistanceLower = Rand.Range(0.00f, 0.01f);
                    if (record.Prey.guest.resistance - restistanceLower <= 0.0f)
                    {
                        TaggedString taggedString = "MessagePrisonerResistanceBroken".Translate(record.Prey.LabelShort, record.Predator.LabelShort, record.Predator.Named("WARDEN"), record.Prey.Named("PRISONER"));
                        if (record.Prey.guest.interactionMode == PrisonerInteractionModeDefOf.AttemptRecruit)
                            taggedString += " " + "RV2R_RecruitAttemptsWillBegin".Translate(record.Predator);

                        Messages.Message(taggedString, record.Prey, MessageTypeDefOf.PositiveEvent, true);

                    }
                    record.Prey.guest.resistance = Math.Max(0.0f, record.Prey.guest.resistance -= restistanceLower);
                }
#else
            if (!record.IsForced && record.Prey.guest.Recruitable && record.Prey.guest.resistance > 0f)
                if (record.Prey.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.ReduceResistance
                 || record.Prey.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.AttemptRecruit)
                {
                    float restistanceLower = Rand.Range(0.00f, 0.01f);
                    if (record.Prey.guest.resistance - restistanceLower <= 0.0f)
                    {
                        TaggedString taggedString = "MessagePrisonerResistanceBroken".Translate(record.Prey.LabelShort, record.Predator.LabelShort, record.Predator.Named("WARDEN"), record.Prey.Named("PRISONER"));
                        if (record.Prey.guest.ExclusiveInteractionMode == PrisonerInteractionModeDefOf.AttemptRecruit)
                            taggedString += " " + "RV2R_RecruitAttemptsWillBegin".Translate(record.Predator);

                        Messages.Message(taggedString, record.Prey, MessageTypeDefOf.PositiveEvent, true);

                    }
                    record.Prey.guest.resistance = Math.Max(0.0f, record.Prey.guest.resistance -= restistanceLower);
                }
#endif
        }
        private static void HandleOpinion(VoreTrackerRecord record)
        {
            if (record.Predator.needs.mood != null && record.Predator.needs.mood.thoughts != null)
            {
                Thought predThought = ThoughtMaker.MakeThought(RV2R_Common.ActivePred_Normal);
                if (record.Predator.QuirkManager(false) != null)
                {
                    if (record.Predator.QuirkManager(false).GetTotalSelectorModifier(VoreRole.Predator, ModifierOperation.Add) <= -2f)
                        predThought = ThoughtMaker.MakeThought(RV2R_Common.ActivePred_Reluctant);
                    else if (record.Predator.QuirkManager(false).GetTotalSelectorModifier(VoreRole.Predator, ModifierOperation.Add) >= 2f)
                        predThought = ThoughtMaker.MakeThought(RV2R_Common.ActivePred_Vorny);
                }
                record.Predator.needs.mood.thoughts.memories.TryGainMemory(predThought.def, record.Prey, null);
            }

            if (record.Prey.needs.mood != null && record.Prey.needs.mood.thoughts != null)
            {
                Thought preyThought = ThoughtMaker.MakeThought(RV2R_Common.ActivePrey_Normal);
                if (record.Prey.QuirkManager(false) != null)
                {
                    if (record.Prey.QuirkManager(false).GetTotalSelectorModifier(VoreRole.Prey, ModifierOperation.Add) <= -2f)
                        preyThought = ThoughtMaker.MakeThought(RV2R_Common.ActivePrey_Reluctant);
                    else if (record.Prey.QuirkManager(false).GetTotalSelectorModifier(VoreRole.Prey, ModifierOperation.Add) >= 2f)
                        preyThought = ThoughtMaker.MakeThought(RV2R_Common.ActivePrey_Vorny);
                }
                record.Prey.needs.mood.thoughts.memories.TryGainMemory(preyThought.def, record.Predator, null);
            }
        }
    }
}
