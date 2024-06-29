using RimVore2;
using RimWorld;
using System;
using Verse;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    public class Hediff_VoreEncumberance : Hediff
    {
        public override void Tick()
        {
            base.Tick();
            if (ageTicks % 250 == 0)
            {
                Severity = TickSeverity(pawn);
                pawn.health.Notify_HediffChanged(this);
            }
        }

        private float TickSeverity(Pawn pawn)
        {
            try
            {
                if (pawn.kindDef.defName.Contains("NCP_")) // Carnivorous Plants
                    return 0f;
                if (RV2_Rut_Settings.rutsStuff.EncumberanceModifier <= 0.0f)
                    return 0f;
                if (RV2_Rut_Settings.rutsStuff.EncumberanceCap < 0.05f)
                    return 0f;
                if (!pawn.IsActivePredator())
                    return 0f;
                if (pawn.PawnData(false) == null)
                    return 0f;

                float quirkMod = pawn.QuirkManager(false)?.CapModOffsetModifierFor(PawnCapacityDefOf.Moving, null) ?? 1f;

                Hediff accHediff = pawn.health.hediffSet.GetFirstHediffOfDef(RV2R_Common.MovingAcclimation, false);

                float accMod = accHediff == null ? 1f : 1 + accHediff.Severity;

                float severity = 0f;

                if (RV2_Rut_Settings.rutsStuff.SizedEncumberance)
                {
                    float totalWeight = RV2R_Utilities.GetPreySize(pawn);
                    severity = Math.Min(totalWeight / Math.Max(pawn.BodySize, 0.01f) / 4f * RV2_Rut_Settings.rutsStuff.EncumberanceModifier * quirkMod / accMod, RV2_Rut_Settings.rutsStuff.EncumberanceCap);
                }
                else
                {
                    int totalPrey = RV2R_Utilities.GetPreyCount(pawn);
                    severity = Math.Min(totalPrey / 4f * RV2_Rut_Settings.rutsStuff.EncumberanceModifier * quirkMod / accMod, RV2_Rut_Settings.rutsStuff.EncumberanceCap);
                }
                if (RV2_Rut_Settings.rutsStuff.MovingCapacityAclimation > 0f && severity > RV2_Rut_Settings.rutsStuff.MovingCapacityAclimationLimit)
                {
                    if (accHediff != null)
                    {
                        float mod = Math.Min(1f, 1f - (severity / RV2_Rut_Settings.rutsStuff.MovingCapacityAclimationLimit));
                        accHediff.Severity += 0.0005f * mod * RV2_Rut_Settings.rutsStuff.MovingCapacityAclimation;
                    }
                    else
                        pawn.health.AddHediff(RV2R_Common.MovingAcclimation, null, null, null);


                }
                return severity;
            }
            catch (Exception e)
            {
                Log.Warning("RV-2R: Something went wrong when getting " + pawn.LabelShort + "'s encumberance: " + e);
                return 0f;
            }
        }

        public override bool Visible
        {
            get
            {
                if (Severity > 0.05f && RV2_Rut_Settings.rutsStuff.VisibleEncumberance)
                    return true;
                return false;
            }
        }

    }
}
