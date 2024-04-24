using HarmonyLib;
using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;

using System.Linq;
using Verse;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    [HarmonyPatch(typeof(PreVoreUtility), "PopulateRecord")]
    internal class Patch_ImprisonHostile
    {
        [HarmonyPostfix]
        public static void EndoJail(ref VoreTrackerRecord record)
        {

            if (RV2R_Utilities.ShouldBandaid(record.Predator, record.Prey))
                return;

            try
            {
                Hediff firstHediffOfDef = record.Predator.health.hediffSet.GetFirstHediffOfDef(RV2R_Common.Encumbrance, false) ?? null;
                if (firstHediffOfDef == null)
                    record.Predator.health.AddHediff(RV2R_Common.Encumbrance, null, null, null);

                Hediff LGAnus = record.Predator.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("LightGenitals_Anus")) ?? null;
                if (LGAnus != null)
                    record.Predator.health.AddHediff(RV2R_Common.LGSanity, null, null, null);

                Hediff grappleHediff = record.Predator.health.hediffSet.GetFirstHediffOfDef(RV2_Common.GrappledHediff, false) ?? null;

                if (record.VoreGoal.IsLethal)
                    return;

                if (!RV2_Rut_Settings.rutsStuff.EndoCapture)
                    return;

                if (record.Predator.Faction == null || !record.Predator.Faction.IsPlayer)
                    return;

                if (record.Prey.IsAnimal() && record.VoreGoal == VoreGoalDefOf.Heal && RV2_Rut_Settings.rutsStuff.ScariaCapture)
                    HandleRabids(record);

                if ((grappleHediff != null && !record.Prey.health.InPainShock) || !record.Prey.Downed) // So grappled pawns won't be captured
                    return;
                
                if (record.Prey.IsInsectoid() && !record.Prey.IsHumanoid()) // Needs to be set up like this because of Apini; they're made of insect meat
                    HandleInsectoids(record);
                else if (RV2R_Utilities.IsColonyHostile(record.Predator, record.Prey) && record.Prey.IsHumanoid())
                    record.Prey.guest.SetGuestStatus(record.Predator.Faction, GuestStatus.Prisoner);
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when trying to handle record creation : " + e);
                return;
            }
        }
        private static void HandleInsectoids(VoreTrackerRecord record)
        {
            List<Faction> bugFactions = new List<Faction> { Faction.OfInsects };
            if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.PackageId.ToLower() == "sarg.alphaanimals"))
                bugFactions.Add(Find.FactionManager.FirstFactionOfDef(FactionDef.Named("AA_BlackHive")) ?? null);
            if (bugFactions.Contains(record.Prey.Faction))
                record.Prey.SetFaction(null, null);
        }
        private static void HandleRabids(VoreTrackerRecord record)
        {
            Hediff rabies = record.Prey.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Scaria, false) ?? null;
            if (record.Prey.Faction == null
             && rabies != null)
                record.Prey.health.RemoveHediff(record.Prey.health.hediffSet.GetFirstHediffOfDef(rabies.def, false));
        }
    }
}
