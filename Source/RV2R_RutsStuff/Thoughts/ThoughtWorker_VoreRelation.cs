using RimVore2;
using RimWorld;
using System;
using Verse;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    internal class ThoughtWorker_VoreRelation : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!RV2_Rut_Settings.rutsStuff.EndoThoughts)
                return false;

            if (p.IsActivePredator())
            {
                Pawn prey;
                switch (this.def.defName)
                {
                    case "RV2R_LoverPred":
                        prey = LovePartnerRelationUtility.ExistingMostLikedLovePartner(p, false);
                        if (prey != null && prey.GetVoreRecord()?.Predator == p)
                            return !prey.GetVoreRecord().VoreGoal.IsLethal && p.relations.OpinionOf(prey) > 0;
                        break;

                    case "RV2R_PetPred":
                        for (int i = 0; i < p.relations.DirectRelations.Count; i++)
                        {
                            if (p.relations.DirectRelations[i].def == RV2R_Common.PetPred
                             && p.relations.DirectRelations[i].otherPawn.GetVoreRecord()?.Predator == p
                             && !p.relations.DirectRelations[i].otherPawn.GetVoreRecord().VoreGoal.IsLethal)
                                return p.relations.OpinionOf(p.relations.DirectRelations[i].otherPawn) > 0;
                        }
                        break;

                    case "RV2R_BondPred":
                        int strenght = -1;
                        for (int i = 0; i < p.relations.DirectRelations.Count; i++)
                        {
                            if (p.relations.DirectRelations[i].def == PawnRelationDefOf.Bond
                             && p.relations.DirectRelations[i].otherPawn.GetVoreRecord()?.Predator == p
                             && !p.relations.DirectRelations[i].otherPawn.GetVoreRecord().VoreGoal.IsLethal)
                                strenght += 1;
                        }
                        return ThoughtState.ActiveAtStage(Math.Min(1, strenght));

                }
            }
            if (p.GetVoreRecord() != null && !p.GetVoreRecord().VoreGoal.IsLethal)
            {
                Pawn predator = p.GetVoreRecord().Predator;
                switch (this.def.defName)
                {
                    case "RV2R_LoverPrey":
                        if (predator == LovePartnerRelationUtility.ExistingMostLikedLovePartner(p, false))
                            return p.relations.OpinionOf(predator) > 0;
                        break;
                    case "RV2R_PetPrey":
                        if (p.relations.GetDirectRelation(RV2R_Common.PetPrey, predator) != null)
                            return p.relations.OpinionOf(predator) > 0;
                        break;
                    case "RV2R_BondPrey":
                        if (p.relations.GetDirectRelation(PawnRelationDefOf.Bond, predator) != null)
                            return true;
                        break;
                }
            }
            return false;
        }
    }
}
