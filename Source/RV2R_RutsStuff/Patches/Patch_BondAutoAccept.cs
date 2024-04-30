using HarmonyLib;
using RimVore2;
using RimWorld;
using Verse;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    [HarmonyPatch(typeof(PreferenceUtility), "AutoAccepts")]
    internal class Patch_BondAutoAccept
    {
        internal static bool Prefix(Pawn target, Pawn initiator, ref bool __result)
        {
            if (RV2_Rut_Settings.rutsStuff.VornyBonds)
            {
                if (target.relations.DirectRelationExists(PawnRelationDefOf.Bond, initiator))
                {
                    RV2Log.Message("Recipiant " + target.LabelShort + " auto accepting proposal due to bond", "Proposals");
                    __result = true;
                    return false;
                }

                if (target.relations.DirectRelationExists(RV2R_Common.PetPrey, initiator) || target.relations.DirectRelationExists(RV2R_Common.PetPred, initiator))
                {
                    RV2Log.Message("Recipiant " + target.LabelShort + " auto accepting proposal due to being endo pet/pred", "Proposals");
                    __result = true;
                    return false;
                }
            }

            return true;
        }
    }
}