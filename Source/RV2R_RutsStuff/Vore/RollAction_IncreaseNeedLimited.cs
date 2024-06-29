using RimVore2;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RV2R_RutsStuff
{
    public class RollAction_IncreaseNeedLimited : RollAction
    {
        public override bool TryAction(VoreTrackerRecord record, float rollStrength)
        {
            base.TryAction(record, rollStrength);
            if (this.invert)
            {
                rollStrength *= -1f;
            }
            if (this.need == NeedDefOf.Food && record.Prey.needs != null && record.Prey.needs.food != null)
            {
                rollStrength *= base.TargetPawn.health.hediffSet.HungerRateFactor;
            }
            if (this.limit != null && this.need != null)
            {
                Need need = base.TargetPawn?.needs?.TryGetNeed(this.need);
                if (need != null)
                    if (need.CurLevelPercentage > this.limit)
                        rollStrength *= 0.15f;

            }
            return RV2PawnUtility.TryIncreaseNeed(base.TargetPawn, this.need, rollStrength);
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }
            bool flag = this.target == VoreRole.Invalid;
            if (flag)
            {
                yield return "required field \"target\" is not set";
            }
            bool flag2 = this.need == null;
            if (flag2)
            {
                yield return "required field \"need\" is not set";
            }
            yield break;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<NeedDef>(ref this.need, "need");
        }

        public NeedDef need;

        public float limit;
    }
}
