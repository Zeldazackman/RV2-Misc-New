using RimVore2;
using System;
using System.Collections.Generic;
using Verse;

namespace RV2R_RutsStuff
{
    internal class RollAction_ModHediff : RollAction
    {
        public override bool TryAction(VoreTrackerRecord record, float rollStrength)
        {
            base.TryAction(record, rollStrength);
            HediffDef hediffDef = HediffDef.Named(this.hediff);
            if (hediffDef != null)
            {
                Hediff hediff = base.TargetPawn.health.hediffSet.GetFirstHediffOfDef(hediffDef, false);
                if (hediff != null)
                {
                    hediff.Severity = Math.Min(hediff.Severity + rollStrength, hediffDef.maxSeverity);
                }
                else
                {
                    BodyPartRecord bodyPartByDef = base.TargetPawn.GetBodyPartByDef(this.partDef);
                    base.TargetPawn.health.AddHediff(hediffDef, bodyPartByDef, null, null);
                }
                return true;
            }
            return false;
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }
            if (this.hediff == null)
            {
                yield return "required field \"hediff\" is not set";
            }
            if (this.target == VoreRole.Invalid)
            {
                yield return "required field \"target\" is not set";
            }
            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<string>(ref this.hediff, "hediff", null, false);
            Scribe_Defs.Look<BodyPartDef>(ref this.partDef, "partDef");
        }

        public string hediff;

        protected BodyPartDef partDef;
    }
}
