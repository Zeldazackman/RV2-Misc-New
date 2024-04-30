﻿using HarmonyLib;
using RimVore2;
using RimVore2.Tabs;
using RimWorld;
using Verse;

namespace RV2R_RutsStuff
{
    [HarmonyPatch(typeof(VoreButton), "CanPlayerGiveCommand")]
    internal class Patch_CanGiveCommand
    {
        [HarmonyPostfix]
        public static void TamedCommands(ref bool __result, Pawn pawn)
        {
            if (!pawn.Dead && !pawn.InMentalState && !pawn.Downed && !pawn.IsBurning())
            {
                if (RV2R_Utilities.IsSapient(pawn))
                {
                    __result = true;
                    return;
                }
                if (pawn.IsAnimal() && pawn.IsActivePredator())
                {
                    if (RV2R_Utilities.GetHighestPreySkillLevel(pawn, SkillDefOf.Animals) >= pawn.GetStatValue(StatDefOf.MinimumHandlingSkill) * 2)
                    {
                        __result = true;
                        return;
                    }
                    if (pawn.training != null)
                    {
                        Pawn_PlayerSettings playerSettings = pawn.playerSettings;
                        if (playerSettings != null)
                            if (playerSettings.RespectedMaster != null && (pawn.playerSettings.RespectedMaster.Position.DistanceTo(pawn.Position) <= 8f || pawn.playerSettings.RespectedMaster.IsPreyOf(pawn)))
                                __result = true;
                    }
                }
            }
            return;
        }
    }
}
