using RimWorld;
using UnityEngine;

namespace RV2R_RutsStuff
{
    internal class Thought_VoreRelation : Thought_Situational
    {
        protected override float BaseMoodOffset
        {
            get
            {
                float num = 1f;
                switch (this.def.defName)
                {
                    case "RV2R_LoverPred":
                    case "RV2R_LoverPrey":
                        if (LovePartnerRelationUtility.ExistingMostLikedLovePartner(this.pawn, false) != null)
                            num = 0.05f * (float)this.pawn.relations.OpinionOf(LovePartnerRelationUtility.ExistingMostLikedLovePartner(this.pawn, false));
                        return Mathf.Max(num, 1f);
                    case "RV2R_PetPred":
                        if (this.pawn.relations.GetFirstDirectRelationPawn(RV2R_Common.PetPred) != null)
                            num = 0.05f * (float)this.pawn.relations.OpinionOf(this.pawn.relations.GetFirstDirectRelationPawn(RV2R_Common.PetPred));
                        return Mathf.Max(num, 1f);
                    case "RV2R_PetPrey":
                        if (this.pawn.relations.GetFirstDirectRelationPawn(RV2R_Common.PetPrey) != null)
                            num = 0.1f * (float)this.pawn.relations.OpinionOf(this.pawn.relations.GetFirstDirectRelationPawn(RV2R_Common.PetPrey));
                        return Mathf.Max(num, 1f);
                    default:
                        return num;
                }
            }
        }
    }
}
