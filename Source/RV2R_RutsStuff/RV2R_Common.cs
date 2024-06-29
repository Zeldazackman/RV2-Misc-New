using RimVore2;
using RimWorld;
using Verse;

namespace RV2R_RutsStuff
{
    internal class RV2R_Common
    {
        public static readonly HediffDef EndoHediff = HediffDef.Named("RV2R_EndoRecovery");
        public static readonly HediffDef Drained = HediffDef.Named("RV2R_Drained");
        public static readonly HediffDef Encumbrance = HediffDef.Named("RV2R_VoreEncumbrence");

        public static readonly HediffDef CapacityAcclimation = HediffDef.Named("RV2R_CapacityAcclimation");
        public static readonly HediffDef MovingAcclimation = HediffDef.Named("RV2R_MovementAcclimation");

        public static readonly TrainableDef PlayVore = DefDatabase<TrainableDef>.GetNamedSilentFail("PlayVore");

        public static readonly JobDef RV2R_GutLovin = DefDatabase<JobDef>.GetNamedSilentFail("RV2R_GutLovin");

        public static readonly ThoughtDef PredLovin_VeryGood = ThoughtDef.Named("RV2R_PredLovin_VeryGood");
        public static readonly ThoughtDef PredLovin_VeryGood_Mood = ThoughtDef.Named("RV2R_PredLovin_VeryGood_Mood");
        public static readonly ThoughtDef PredLovin_Good = ThoughtDef.Named("RV2R_PredLovin_Good");
        public static readonly ThoughtDef PredLovin_Good_Mood = ThoughtDef.Named("RV2R_PredLovin_Good_Mood");
        public static readonly ThoughtDef PredLovin_Normal = ThoughtDef.Named("RV2R_PredLovin_Normal");
        public static readonly ThoughtDef PredLovin_Normal_Mood = ThoughtDef.Named("RV2R_PredLovin_Normal_Mood");
        public static readonly ThoughtDef PredLovin_Meh = ThoughtDef.Named("RV2R_PredLovin_Meh");

        public static readonly ThoughtDef PreyLovin_VeryGood = ThoughtDef.Named("RV2R_PreyLovin_VeryGood");
        public static readonly ThoughtDef PreyLovin_Good = ThoughtDef.Named("RV2R_PreyLovin_Good");
        public static readonly ThoughtDef PreyLovin_Normal = ThoughtDef.Named("RV2R_PreyLovin_Normal");
        public static readonly ThoughtDef PreyLovin_Meh = ThoughtDef.Named("RV2R_PreyLovin_Meh");
        public static readonly ThoughtDef PreyLovin_Bad = ThoughtDef.Named("RV2R_PreyLovin_Bad");

        public static readonly ThoughtDef ActivePred_Normal = ThoughtDef.Named("RV2R_ActivePred");
        public static readonly ThoughtDef ActivePred_Reluctant = ThoughtDef.Named("RV2R_ActivePred_Reluctant");
        public static readonly ThoughtDef ActivePred_Vorny = ThoughtDef.Named("RV2R_ActivePred_Vorny");

        public static readonly ThoughtDef ActivePrey_Normal = ThoughtDef.Named("RV2R_ActivePrey");
        public static readonly ThoughtDef ActivePrey_Reluctant = ThoughtDef.Named("RV2R_ActivePrey_Reluctant");
        public static readonly ThoughtDef ActivePrey_Vorny = ThoughtDef.Named("RV2R_ActivePrey_Vorny");

        public static readonly PawnRelationDef PetPred = DefDatabase<PawnRelationDef>.GetNamedSilentFail("PetPred");
        public static readonly PawnRelationDef PetPrey = DefDatabase<PawnRelationDef>.GetNamedSilentFail("PetPrey");

        public static readonly VoreGoalDef Drain = DefDatabase<VoreGoalDef>.GetNamedSilentFail("RV2R_Drain");

        public static readonly PrisonerInteractionModeDef Fodder = DefDatabase<PrisonerInteractionModeDef>.GetNamedSilentFail("RV2R_Fodder");

        public static readonly DesignationDef Devour = DefDatabase<DesignationDef>.GetNamedSilentFail("RV2R_Devour");
    }
}
