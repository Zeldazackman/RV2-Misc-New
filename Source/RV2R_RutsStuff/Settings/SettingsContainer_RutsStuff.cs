using RimVore2;
using UnityEngine;
using Verse;

namespace RV2R_RutsStuff
{
    internal class SettingsContainer_RutsStuff : SettingsContainer
    {

        private BoolSmartSetting sizedEncumberance;
        private FloatSmartSetting encumberanceModifier;
        private BoolSmartSetting visibleEncumberance;
        //private FloatSmartSetting encumberanceCap;

        private FloatSmartSetting endoSicknessStrength;
        private BoolSmartSetting endoPacify;
        private FloatSmartSetting regressionStrength;
        private BoolSmartSetting chronicCure;

        private BoolSmartSetting endoThoughts;
        private FloatSmartSetting endoPets;
        private BoolSmartSetting endoPetsJoin;

        private BoolSmartSetting endoCapture;
        private BoolSmartSetting insectoidCapture;
        private BoolSmartSetting scariaCapture;
        private BoolSmartSetting endoRecruitment;
        private BoolSmartSetting devourFriendlies;
        private BoolSmartSetting devourColonists;
        private BoolSmartSetting devourColonistsFull;

        private BoolSmartSetting preyLovin;
        private FloatSmartSetting gutLovinChance;
        private BoolSmartSetting gutLovinSapients;
        private BoolSmartSetting gutLovinStands;
        private BoolSmartSetting gutLovinCheats;
        private BoolSmartSetting gutLovinNonCon;

        private FloatSmartSetting fodderChance;
        private BoolSmartSetting fodderNamedAllowed;
        private BoolSmartSetting fodderPredatorsAllowed;
        private BoolSmartSetting fodderAnimalsAllowed;
        private BoolSmartSetting fodderAnimalsFull;
        private FloatSmartSetting miscFodderChance;
        private BoolSmartSetting fodderPenAnimals;
        private BoolSmartSetting fodderAnimals;
        private BoolSmartSetting fodderPredators;
        private BoolSmartSetting fodderGuests;
        private BoolSmartSetting fodderColonists;

        private FloatSmartSetting playVoreChance;
        private FloatSmartSetting playVoreModifier;
        private FloatSmartSetting playVoreColonistBias;
        private BoolSmartSetting playVoreIndescriminate;
        private BoolSmartSetting fatalPlayVore;

        private FloatSmartSetting endoBondChance;
        private BoolSmartSetting preyOnlyEndoBond;
        private BoolSmartSetting willingOnlyEndoBond;
        private BoolSmartSetting vornyBonds;

        private BoolSmartSetting preyJoy;
        private BoolSmartSetting noBleedOut;
        private BoolSmartSetting stopBleeding;
        private BoolSmartSetting noBadTemp;
        private BoolSmartSetting curagaVore;

        public bool SizedEncumberance => this.sizedEncumberance.value;
        public float EncumberanceModifier => this.encumberanceModifier.value / 100f;
        public bool VisibleEncumberance => this.visibleEncumberance.value;
        //public float EncumberanceCap => this.encumberanceCap.value / 100f;

        public float EndoSicknessStrength => this.endoSicknessStrength.value / 100f;
        public bool EndoPacify => this.endoPacify.value;
        public float RegressionStrength => this.regressionStrength.value / 100f;
        public bool ChronicCure => this.chronicCure.value;

        public bool EndoThoughts => this.endoThoughts.value;
        public float EndoPets => this.endoPets.value;
        public bool EndoPetsJoin => this.endoPetsJoin.value;

        public bool EndoCapture => this.endoCapture.value;
        public bool InsectoidCapture => this.insectoidCapture.value;
        public bool ScariaCapture => this.scariaCapture.value;
        public bool EndoRecruitment => this.endoRecruitment.value;
        public bool DevourFriendlies => this.devourFriendlies.value;
        public bool DevourColonists => this.devourColonists.value;
        public bool DevourColonistsFull => this.devourColonistsFull.value;

        public bool PreyLovin => this.preyLovin.value;
        public float GutLovinChance => this.gutLovinChance.value / 100f;
        public bool GutLovinSapients => this.gutLovinSapients.value;
        public bool GutLovinStands => this.gutLovinStands.value;
        public bool GutLovinCheats => this.gutLovinCheats.value;
        public bool GutLovinNonCon => this.gutLovinNonCon.value;

        public float PlayVoreChance => this.playVoreChance.value;
        public float PlayVoreModifier => this.playVoreModifier.value / 100f;
        public float PlayVoreColonistBias => this.playVoreColonistBias.value / 100f;
        public bool PlayVoreIndescriminate => this.playVoreIndescriminate.value;
        public bool FatalPlayVore => this.fatalPlayVore.value;

        public float FodderChance => this.fodderChance.value;
        public bool FodderNamedAllowed => this.fodderNamedAllowed.value;
        public bool FodderPredatorsAllowed => this.fodderPredatorsAllowed.value;
        public bool FodderAnimalsAllowed => this.fodderAnimalsAllowed.value;
        public bool FodderAnimalsFull => this.fodderAnimalsFull.value;
        public float MiscFodderChance => this.miscFodderChance.value / 100f;
        public bool FodderPenAnimals => this.fodderPenAnimals.value;
        public bool FodderAnimals => this.fodderAnimals.value;
        public bool FodderPredators => this.fodderPredators.value;
        public bool FodderGuests => this.fodderGuests.value;
        public bool FodderColonists => this.fodderColonists.value;

        public float EndoBondChance => this.endoBondChance.value / 100f;
        public bool PreyOnlyEndoBond => this.preyOnlyEndoBond.value;
        public bool WillingOnlyEndoBond => this.willingOnlyEndoBond.value;
        public bool VornyBonds => this.vornyBonds.value;

        public bool PreyJoy => this.preyJoy.value;
        public bool NoBleedOut => this.noBleedOut.value;
        public bool StopBleeding => this.stopBleeding.value;
        public bool NoBadTemp => this.noBadTemp.value;
        public bool CuragaVore => this.curagaVore.value;

        public override void EnsureSmartSettingDefinition()
        {
            if (this.sizedEncumberance == null || this.sizedEncumberance.IsInvalid())
                this.sizedEncumberance = new BoolSmartSetting("RV2R_Settings_SizedEncumberance", true, true, "RV2R_Settings_SizedEncumberance_Tip");
            if (this.encumberanceModifier == null || this.encumberanceModifier.IsInvalid())
                this.encumberanceModifier = new FloatSmartSetting("RV2R_Settings_EncumberanceModifier", 100f, 100f, 0f, 400f, "RV2R_Settings_EncumberanceModifier_Tip", "0", "%");
            if (this.visibleEncumberance == null || this.visibleEncumberance.IsInvalid())
                this.visibleEncumberance = new BoolSmartSetting("RV2R_Settings_EncumberanceVisible", true, true, "RV2R_Settings_EncumberanceVisible_Tip");
            //   if (this.encumberanceCap == null || this.encumberanceCap.IsInvalid())
            //       this.encumberanceCap = new FloatSmartSetting("RV2R_Settings_EncumberanceCap", 100f, 100f, 0f, 100f, "RV2R_Settings_EncumberanceCap_Tip", "0", "%");

            if (this.endoSicknessStrength == null || this.endoSicknessStrength.IsInvalid())
                this.endoSicknessStrength = new FloatSmartSetting("RV2R_Settings_EndoSickness", 100f, 100f, 0f, 400f, "RV2R_Settings_EndoSickness_Tip", "0", "%");
            if (this.endoPacify == null || this.endoPacify.IsInvalid())
                this.endoPacify = new BoolSmartSetting("RV2R_Settings_EndoSicknessPacification", true, true, "RV2R_Settings_EndoSicknessPacification_Tip");
            if (this.regressionStrength == null || this.regressionStrength.IsInvalid())
                this.regressionStrength = new FloatSmartSetting("RV2R_Settings_AgeRegression", 5f, 5f, 0f, 60f, "RV2R_Settings_AgeRegression_Tip", "0.0");
            if (this.chronicCure == null || this.chronicCure.IsInvalid())
                this.chronicCure = new BoolSmartSetting("RV2R_Settings_AgeRegressionCure", true, true, "RV2R_Settings_AgeRegressionCure_Tip");

            if (this.endoThoughts == null || this.endoThoughts.IsInvalid())
                this.endoThoughts = new BoolSmartSetting("RV2R_Settings_EndoThoughts", true, true, "RV2R_Settings_EndoThoughts_Tip");
            if (this.endoPets == null || this.endoPets.IsInvalid())
                this.endoPets = new FloatSmartSetting("RV2R_Settings_Pets", 30f, 30f, 0f, 60f, "RV2R_Settings_Pets_Tip", "0.0");
            if (this.endoPetsJoin == null || this.endoPetsJoin.IsInvalid())
                this.endoPetsJoin = new BoolSmartSetting("RV2R_Settings_PetRecruitment", true, true, "RV2R_Settings_PetRecruitment_Tip");

            if (this.preyLovin == null || this.preyLovin.IsInvalid())
                this.preyLovin = new BoolSmartSetting("RV2R_Settings_PreyLovin", true, true, "RV2R_Settings_PreyLovin_Tip");
            if (this.gutLovinChance == null || this.gutLovinChance.IsInvalid())
                this.gutLovinChance = new FloatSmartSetting("RV2R_Settings_GutLovin", 100f, 100f, 0f, 1000f, "RV2R_Settings_GutLovin_Tip", "0", "%");
            if (this.gutLovinSapients == null || this.gutLovinSapients.IsInvalid())
                this.gutLovinSapients = new BoolSmartSetting("RV2R_Settings_GutLovinSapient", false, false, "RV2R_Settings_GutLovinSapient_Tip");
            if (this.gutLovinStands == null || this.gutLovinStands.IsInvalid())
                this.gutLovinStands = new BoolSmartSetting("RV2R_Settings_GutLovinONS", true, true, "RV2R_Settings_GutLovinONS_Tip");
            if (this.gutLovinCheats == null || this.gutLovinCheats.IsInvalid())
                this.gutLovinCheats = new BoolSmartSetting("RV2R_Settings_GutLovinCheats", false, false, "RV2R_Settings_GutLovinCheats_Tip");
            if (this.gutLovinNonCon == null || this.gutLovinNonCon.IsInvalid())
                this.gutLovinNonCon = new BoolSmartSetting("RV2R_Settings_GutLovinNonCon", false, false, "RV2R_Settings_GutLovinNonCon_Tip");

            if (this.endoCapture == null || this.endoCapture.IsInvalid())
                this.endoCapture = new BoolSmartSetting("RV2R_Settings_EndoCapture", true, true, "RV2R_Settings_EndoCapture_Tip");
            if (this.insectoidCapture == null || this.insectoidCapture.IsInvalid())
                this.insectoidCapture = new BoolSmartSetting("RV2R_Settings_EndoCapture_Insectoids", false, false, "RV2R_Settings_EndoCapture_Insectoids_Tip");
            if (this.scariaCapture == null || this.scariaCapture.IsInvalid())
                this.scariaCapture = new BoolSmartSetting("RV2R_Settings_EndoCapture_Scaria", false, false, "RV2R_Settings_EndoCapture_Scaria_Tip");
            if (this.endoRecruitment == null || this.endoRecruitment.IsInvalid())
                this.endoRecruitment = new BoolSmartSetting("RV2R_Settings_EndoCapture_Recruitment", false, false, "RV2R_Settings_EndoCapture_Recruitment_Tip");
            if (this.devourFriendlies == null || this.devourFriendlies.IsInvalid())
                this.devourFriendlies = new BoolSmartSetting("RV2R_Settings_EndoCaptureDevourFriendlies", false, false, "RV2R_Settings_EndoCaptureDevourFriendlies_Tip");
            if (this.devourColonists == null || this.devourColonists.IsInvalid())
                this.devourColonists = new BoolSmartSetting("RV2R_Settings_EndoCaptureDevourColonists", false, false, "RV2R_Settings_EndoCaptureDevourColonists_Tip");
            if (this.devourColonistsFull == null || this.devourColonistsFull.IsInvalid())
                this.devourColonistsFull = new BoolSmartSetting("RV2R_Settings_EndoCaptureDevourColonistsFull", false, false, "RV2R_Settings_EndoCaptureDevourColonistsFull_Tip");

            if (this.playVoreChance == null || this.playVoreChance.IsInvalid())
                this.playVoreChance = new FloatSmartSetting("RV2R_Settings_AnimalRandomNomChance", 24f, 24f, 6f, 168f, "RV2R_Settings_AnimalRandomNomChance_Tip", "0.0");
            if (this.playVoreModifier == null || this.playVoreModifier.IsInvalid())
                this.playVoreModifier = new FloatSmartSetting("RV2R_Settings_AnimalRandomNomChanceModifier", 50f, 50f, 0f, 800f, "RV2R_Settings_AnimalRandomNomChanceModifier_Tip", "0", "%");
            if (this.playVoreColonistBias == null || this.playVoreColonistBias.IsInvalid())
                this.playVoreColonistBias = new FloatSmartSetting("RV2R_Settings_AnimalRandomNomColonistModifier", 50f, 50f, 0f, 100f, "RV2R_Settings_AnimalRandomNomColonistModifier_Tip", "0", "%");
            if (this.playVoreIndescriminate == null || this.playVoreIndescriminate.IsInvalid())
                this.playVoreIndescriminate = new BoolSmartSetting("RV2R_Settings_AnimalRandomNomIndiscriminate", false, false, "RV2R_Settings_AnimalRandomNomIndiscriminate_Tip");
            if (this.fatalPlayVore == null || this.fatalPlayVore.IsInvalid())
                this.fatalPlayVore = new BoolSmartSetting("RV2R_Settings_AnimalRandomNomFatal", false, false, "RV2R_Settings_AnimalRandomNomFatal_Tip");

            if (this.fodderChance == null || this.fodderChance.IsInvalid())
                this.fodderChance = new FloatSmartSetting("RV2R_Settings_Fodder_Chance", 50f, 50f, 0f, 100f, "RV2R_Settings_Fodder_Chance_Tip", "0", "%");
            if (this.fodderNamedAllowed == null || this.fodderNamedAllowed.IsInvalid())
                this.fodderNamedAllowed = new BoolSmartSetting("RV2R_Settings_Fodder_AllowNamed", true, true, "RV2R_Settings_Fodder_AllowNamed_Tip");
            if (this.fodderPredatorsAllowed == null || this.fodderPredatorsAllowed.IsInvalid())
                this.fodderPredatorsAllowed = new BoolSmartSetting("RV2R_Settings_Fodder_AllowPredators", true, true, "RV2R_Settings_Fodder_AllowPredators_Tip");
            if (this.fodderAnimalsAllowed == null || this.fodderAnimalsAllowed.IsInvalid())
                this.fodderAnimalsAllowed = new BoolSmartSetting("RV2R_Settings_Fodder_AllowAnimals", false, false, "RV2R_Settings_Fodder_AllowAnimals_Tip");
            if (this.fodderAnimalsFull == null || this.fodderAnimalsFull.IsInvalid())
                this.fodderAnimalsFull = new BoolSmartSetting("RV2R_Settings_Fodder_AnimalsFull", false, false, "RV2R_Settings_Fodder_AnimalsFull_Tip");
            if (this.miscFodderChance == null || this.miscFodderChance.IsInvalid())
                this.miscFodderChance = new FloatSmartSetting("RV2R_Settings_MiscFodder_Chance", 5f, 5f, 0f, 100f, "RV2R_Settings_MiscFodder_Chance_Tip", "0", "%");
            if (this.fodderPenAnimals == null || this.fodderPenAnimals.IsInvalid())
                this.fodderPenAnimals = new BoolSmartSetting("RV2R_Settings_Fodder_Penned", false, false, "RV2R_Settings_Fodder_Penned_Tip");
            if (this.fodderAnimals == null || this.fodderAnimals.IsInvalid())
                this.fodderAnimals = new BoolSmartSetting("RV2R_Settings_Fodder_Animal", false, false, "RV2R_Settings_Fodder_Animal_Tip");
            if (this.fodderPredators == null || this.fodderPredators.IsInvalid())
                this.fodderPredators = new BoolSmartSetting("RV2R_Settings_Fodder_Predator", false, false, "RV2R_Settings_Fodder_Predator_Tip");
            if (this.fodderGuests == null || this.fodderGuests.IsInvalid())
                this.fodderGuests = new BoolSmartSetting("RV2R_Settings_Fodder_Guests", false, false, "RV2R_Settings_Fodder_Guests_Tip");
            if (this.fodderColonists == null || this.fodderColonists.IsInvalid())
                this.fodderColonists = new BoolSmartSetting("RV2R_Settings_Fodder_Colonists", false, false, "RV2R_Settings_Fodder_Colonists_Tip");

            if (this.endoBondChance == null || this.endoBondChance.IsInvalid())
                this.endoBondChance = new FloatSmartSetting("RV2R_Settings_EndoBondChance", 0.005f, 0.010f, 0.000f, 0.200f, "RV2R_Settings_EndoBondChance_Tip", "0.000", "%");
            if (this.preyOnlyEndoBond == null || this.preyOnlyEndoBond.IsInvalid())
                this.preyOnlyEndoBond = new BoolSmartSetting("RV2R_Settings_EndoBondPredOnly", false, false, "RV2R_Settings_EndoBondPredOnly_Tip");
            if (this.willingOnlyEndoBond == null || this.willingOnlyEndoBond.IsInvalid())
                this.willingOnlyEndoBond = new BoolSmartSetting("RV2R_Settings_EndoBondUnwilling", false, false, "RV2R_Settings_EndoBondUnwilling_Tip");
            if (this.vornyBonds == null || this.vornyBonds.IsInvalid())
                this.vornyBonds = new BoolSmartSetting("RV2R_Settings_VornyBonds", false, false, "RV2R_Settings_VornyBonds_Tip");

            if (this.preyJoy == null || this.preyJoy.IsInvalid())
                this.preyJoy = new BoolSmartSetting("RV2R_Cheats_PreyJoy", true, true, "RV2R_Cheats_PreyJoy_Tip");
            if (this.noBleedOut == null || this.noBleedOut.IsInvalid())
                this.noBleedOut = new BoolSmartSetting("RV2R_Cheats_NoBleedOut", false, false, "RV2R_Cheats_NoBleedOut_Tip");
            if (this.stopBleeding == null || this.stopBleeding.IsInvalid())
                this.stopBleeding = new BoolSmartSetting("RV2R_Cheats_StopBleeding", false, false, "RV2R_Cheats_StopBleeding_Tip");
            if (this.noBadTemp == null || this.noBadTemp.IsInvalid())
                this.noBadTemp = new BoolSmartSetting("RV2R_Cheats_NoExtremeTemp", false, false, "RV2R_Cheats_NoExtremeTemp_Tip");
            if (this.curagaVore == null || this.curagaVore.IsInvalid())
                this.curagaVore = new BoolSmartSetting("RV2R_Cheats_HyperHealing", false, false, "RV2R_Cheats_HyperHealing_Tip");

        }

        private bool heightStale = true;

        private float height;

        private Vector2 scrollPosition;

        public override void Reset()
        {
            this.sizedEncumberance = null;
            this.encumberanceModifier = null;
            this.visibleEncumberance = null;
            //this.encumberanceCap = null;
            this.endoSicknessStrength = null;
            this.endoPacify = null;
            this.regressionStrength = null;
            this.chronicCure = null;
            this.endoThoughts = null;
            this.endoPets = null;
            this.endoPetsJoin = null;
            this.endoCapture = null;
            this.preyLovin = null;
            this.gutLovinChance = null;
            this.gutLovinSapients = null;
            this.gutLovinStands = null;
            this.gutLovinNonCon = null;
            this.insectoidCapture = null;
            this.scariaCapture = null;
            this.endoRecruitment = null;
            this.devourFriendlies = null;
            this.devourColonists = null;
            this.devourColonistsFull = null;
            this.playVoreChance = null;
            this.playVoreModifier = null;
            this.playVoreIndescriminate = null;
            this.playVoreColonistBias = null;
            this.fatalPlayVore = null;
            this.fodderChance = null;
            this.fodderNamedAllowed = null;
            this.fodderPredatorsAllowed = null;
            this.fodderAnimalsAllowed = null;
            this.fodderAnimalsFull = null;
            this.fodderPenAnimals = null;
            this.fodderAnimals = null;
            this.fodderPredators = null;
            this.fodderGuests = null;
            this.fodderColonists = null;
            this.endoBondChance = null;
            this.preyOnlyEndoBond = null;
            this.preyJoy = null;
            this.noBleedOut = null;
            this.stopBleeding = null;
            this.noBadTemp = null;
            this.curagaVore = null;

            EnsureSmartSettingDefinition();
        }
        public void FillRect(Rect inRect)
        {
            UIUtility.MakeAndBeginScrollView(inRect, this.height, ref this.scrollPosition, out Listing_Standard listing_Standard);
            if (listing_Standard.ButtonText("RV2_Settings_Reset".Translate(), null))
                this.Reset();

            listing_Standard.Gap(12f);
            listing_Standard.Label("RV2R_Encumberance_Section".Translate());
            listing_Standard.Gap(12f);
            this.sizedEncumberance.DoSetting(listing_Standard);
            this.encumberanceModifier.DoSetting(listing_Standard);
            this.visibleEncumberance.DoSetting(listing_Standard);
            listing_Standard.Gap(6f);
            listing_Standard.Label("RV2R_Settings_EncumberanceTurnOff".Translate());
            //this.encumberanceCap.DoSetting(listing_Standard);
            listing_Standard.Gap(18f);
            listing_Standard.Label("RV2R_Tick_Section".Translate());
            listing_Standard.Gap(12f);
            this.endoSicknessStrength.DoSetting(listing_Standard);
            this.endoPacify.DoSetting(listing_Standard);
            this.regressionStrength.DoSetting(listing_Standard);
            this.chronicCure.DoSetting(listing_Standard);
            listing_Standard.Gap(8f);
            this.endoThoughts.DoSetting(listing_Standard);
            this.endoPets.DoSetting(listing_Standard);
            this.endoPetsJoin.DoSetting(listing_Standard);
            listing_Standard.Gap(18f);
            listing_Standard.Label("RV2R_Lovin_Section".Translate());
            listing_Standard.Gap(12f);
            this.preyLovin.DoSetting(listing_Standard);
            this.gutLovinChance.DoSetting(listing_Standard);
            this.gutLovinSapients.DoSetting(listing_Standard);
            this.gutLovinStands.DoSetting(listing_Standard);
            this.gutLovinNonCon.DoSetting(listing_Standard);
            listing_Standard.Gap(18f);
            listing_Standard.Label("RV2R_Capture_Section".Translate());
            listing_Standard.Gap(12f);
            this.endoCapture.DoSetting(listing_Standard);
            this.insectoidCapture.DoSetting(listing_Standard);
            this.scariaCapture.DoSetting(listing_Standard);
            this.endoRecruitment.DoSetting(listing_Standard);
            listing_Standard.Gap(8f);
            this.devourFriendlies.DoSetting(listing_Standard);
            this.devourColonists.DoSetting(listing_Standard);
            this.devourColonistsFull.DoSetting(listing_Standard);
            listing_Standard.Gap(18f);
            listing_Standard.Label("RV2R_AnimalNom_Section".Translate());
            listing_Standard.Gap(12f);
            this.playVoreChance.DoSetting(listing_Standard);
            this.playVoreModifier.DoSetting(listing_Standard);
            this.playVoreColonistBias.DoSetting(listing_Standard);
            this.playVoreIndescriminate.DoSetting(listing_Standard);
            this.fatalPlayVore.DoSetting(listing_Standard);
            listing_Standard.Gap(18f);
            listing_Standard.Label("RV2R_Fodder_Section".Translate());
            listing_Standard.Gap(12f);
            this.fodderChance.DoSetting(listing_Standard);
            this.fodderNamedAllowed.DoSetting(listing_Standard);
            this.fodderPredatorsAllowed.DoSetting(listing_Standard);
            this.fodderAnimalsAllowed.DoSetting(listing_Standard);
            listing_Standard.Gap(10f);
            this.miscFodderChance.DoSetting(listing_Standard);
            this.fodderAnimalsFull.DoSetting(listing_Standard);
            this.fodderPenAnimals.DoSetting(listing_Standard);
            this.fodderAnimals.DoSetting(listing_Standard);
            this.fodderPredators.DoSetting(listing_Standard);
            this.fodderGuests.DoSetting(listing_Standard);
            this.fodderColonists.DoSetting(listing_Standard);
            listing_Standard.Gap(18f);
            listing_Standard.Label("RV2R_Bonds_Section".Translate());
            listing_Standard.Gap(12f);
            this.endoBondChance.DoSetting(listing_Standard);
            this.preyOnlyEndoBond.DoSetting(listing_Standard);
            this.willingOnlyEndoBond.DoSetting(listing_Standard);
            this.vornyBonds.DoSetting(listing_Standard);
            listing_Standard.Gap(18f);
            listing_Standard.Label("RV2R_Cheats_Section".Translate());
            listing_Standard.Gap(12f);
            this.preyJoy.DoSetting(listing_Standard);
            this.noBleedOut.DoSetting(listing_Standard);
            this.stopBleeding.DoSetting(listing_Standard);
            this.noBadTemp.DoSetting(listing_Standard);
            this.curagaVore.DoSetting(listing_Standard);
            //listing_Standard.Gap(126f);
            //listing_Standard.Label("RV2R_FuckUnity".Translate());
            listing_Standard.EndScrollView(ref this.height, ref this.heightStale);
        }
        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving || Scribe.mode == LoadSaveMode.LoadingVars)
                this.EnsureSmartSettingDefinition();

            Scribe_Deep.Look<BoolSmartSetting>(ref this.sizedEncumberance, "sizedEncumberance", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.encumberanceModifier, "encumberanceModifier", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.visibleEncumberance, "visibleEncumberance", new object[0]);
            //Scribe_Deep.Look<FloatSmartSetting>(ref this.encumberanceCap, "encumberanceCap", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.endoSicknessStrength, "endoSicknessStrength", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.endoPacify, "endoPacify", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.regressionStrength, "regressionStrength", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.chronicCure, "chronicCure", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.endoThoughts, "endoThoughts", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.endoPets, "endoPets", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.endoPetsJoin, "endoPetsJoin", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.endoCapture, "endoCapture", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.preyLovin, "preyLovin", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.gutLovinChance, "gutLovinChance", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.gutLovinSapients, "gutLovinSapients", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.gutLovinStands, "gutLovinStands", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.gutLovinNonCon, "gutLovinNonCon", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.insectoidCapture, "insectoidCapture", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.scariaCapture, "scariaCapture", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.endoRecruitment, "endoRecruitment", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.devourFriendlies, "devourFriendlies", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.devourColonists, "devourColonists", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.devourColonistsFull, "devourColonistsFull", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.playVoreChance, "playVoreChance", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.playVoreIndescriminate, "playVoreIndescriminate", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.playVoreColonistBias, "playVoreColonistBias", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fatalPlayVore, "fatalPlayVore", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.fodderChance, "fodderChance", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderNamedAllowed, "fodderNamedAllowed", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderPredatorsAllowed, "fodderPredatorsAllowed", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderAnimalsAllowed, "fodderAnimalsAllowed", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderAnimalsFull, "fodderAnimalsFull", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.miscFodderChance, "miscFodderChance", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderPenAnimals, "fodderPenAnimals", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderAnimals, "fodderAnimals", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderPredators, "fodderPredators", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderGuests, "fodderGuests", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.fodderColonists, "fodderColonists", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.endoBondChance, "endoBondChance", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.preyOnlyEndoBond, "preyOnlyEndoBond", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.willingOnlyEndoBond, "willingOnlyEndoBond", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.vornyBonds, "vornyBonds", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.preyJoy, "preyJoy", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.noBleedOut, "noBleedOut", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.stopBleeding, "stopBleeding", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.noBadTemp, "noBadTemp", new object[0]);
            Scribe_Deep.Look<BoolSmartSetting>(ref this.curagaVore, "curagaVore", new object[0]);
            this.PostExposeData();
        }
    }
}
