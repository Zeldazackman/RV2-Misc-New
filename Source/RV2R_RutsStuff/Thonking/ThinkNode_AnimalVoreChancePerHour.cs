using RimVore2;
using Verse;
using Verse.AI;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    internal class ThinkNode_AnimalVoreChancePerHour : ThinkNode_ChancePerHour_Constant
    {
        private readonly float mtbHours = RV2_Rut_Settings.rutsStuff.PlayVoreChance;
        protected override float MtbHours(Pawn Pawn)
        {
            float preyMod = 1f;

            if (Pawn.training != null && !Pawn.training.GetWanted(RV2R_Common.PlayVore))
                return -1f;
            //if (RV2_Rut_Settings.rutsStuff.PlayVoreNamedBoost && !Pawn.Name?.Numerical)
            //  mtbHours /= 2f;
            if (Pawn.IsActivePredator())
                preyMod += RV2R_Utilities.GetPreyCount(Pawn) * RV2_Rut_Settings.rutsStuff.PlayVoreModifier;

            return this.mtbHours * preyMod;
        }
    }
}
