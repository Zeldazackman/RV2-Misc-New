using RimVore2;
using Verse;

namespace RV2R_RutsStuff
{
    public class Hediff_LGSP : Hediff
    {
        public override void Tick()
        {
            base.Tick();
            if (ageTicks % 10000 == 0)
            {
                this.SanityCheck();
            }
        }
        public override bool Visible
        {
            get
            {
                return false;
            }
        }

        private void SanityCheck()
        {
            HediffSet hediffSet = this.pawn.health.hediffSet;
            if (hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("LightGenitals_Anus")) != null)
            {
                if (hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("LightGenitals_Anus")).Part.def != DefDatabase<BodyPartDef>.GetNamedSilentFail("Anus"))
                {
                    Log.Warning("RV-2R: Light gentials sanity check for " + pawn.LabelShort + " triggered");
                    Hediff bum = hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("LightGenitals_Anus")) ?? null;
                    Hediff booba = hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("LightGenitals_Breasts")) ?? null;
                    Hediff bonger = hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("LightGenitals_Penis")) ?? null;
                    Hediff buussy = hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("LightGenitals_Vagina")) ?? null;

                    if (bum != null)
                    {
                        pawn.health.RemoveHediff(bum);
                        pawn.health.AddHediff(bum, pawn.GetBodyPartByDef(DefDatabase<BodyPartDef>.GetNamedSilentFail("Anus")));
                    }
                    if (booba != null)
                    {
                        pawn.health.RemoveHediff(booba);
                        pawn.health.AddHediff(booba, pawn.GetBodyPartByDef(DefDatabase<BodyPartDef>.GetNamedSilentFail("Chest")));
                    }
                    if (bonger != null)
                    {
                        pawn.health.RemoveHediff(bonger);
                        pawn.health.AddHediff(bonger, pawn.GetBodyPartByDef(DefDatabase<BodyPartDef>.GetNamedSilentFail("Genitals")));
                    }
                    if (buussy != null)
                    {
                        pawn.health.RemoveHediff(buussy);
                        pawn.health.AddHediff(buussy, pawn.GetBodyPartByDef(DefDatabase<BodyPartDef>.GetNamedSilentFail("Genitals")));
                    }
                    Log.Warning("RV-2R: " + pawn.LabelShort + " light gentials reset, curse you Pawnmorpher");
                }
            }
        }
    }
}
