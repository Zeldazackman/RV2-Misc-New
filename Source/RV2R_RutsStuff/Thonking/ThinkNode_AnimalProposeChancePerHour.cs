using RimVore2;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    internal class ThinkNode_AnimalProposeChancePerHour : ThinkNode_ChancePerHour_Constant
    {
        protected override float MtbHours(Pawn Pawn)
        {
            if (!Pawn.CanParticipateInVore(out _))
                return -1f;
            if (!Pawn.RaceProps.predator)
                if (RV2_Rut_Settings.rutsStuff.WildPredatorProposals && !RV2_Rut_Settings.rutsStuff.WildPreyProposals)
                    return -1f;
                else
                if (!RV2_Rut_Settings.rutsStuff.WildPredatorProposals && RV2_Rut_Settings.rutsStuff.WildPreyProposals)
                    return -1f;

            return RV2Mod.Settings.fineTuning.MaxVoreProposalCooldown / 2500;
        }
    }
}
