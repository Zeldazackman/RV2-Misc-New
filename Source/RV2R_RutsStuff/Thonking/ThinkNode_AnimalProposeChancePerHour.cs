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

            if (RV2_Rut_Settings.rutsStuff.WildProposalMod <= 0f)
                return -1f;

            if (!Pawn.RaceProps.predator)
            {
                if (!RV2_Rut_Settings.rutsStuff.WildPreyProposals)
                    return -1f;
            }
            else
            {
                if (!RV2_Rut_Settings.rutsStuff.WildPredatorProposals)
                    return -1f;
            }

            int mod = 1;

            if (GlobalVoreTrackerUtility.ActiveVoreTrackers.Count > RV2_Rut_Settings.rutsStuff.WildTotalVoreLimit)
                mod = (GlobalVoreTrackerUtility.ActiveVoreTrackers.Count - RV2_Rut_Settings.rutsStuff.WildTotalVoreLimit);

            return (float)((RV2Mod.Settings.fineTuning.MinVoreProposalCooldown + RV2Mod.Settings.fineTuning.MaxVoreProposalCooldown) * mod) / 2500f / RV2_Rut_Settings.rutsStuff.WildProposalMod;
        }
    }
}
