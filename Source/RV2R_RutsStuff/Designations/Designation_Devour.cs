using RimVore2;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RV2R_RutsStuff
{
    public class Designator_Devour : Designator
    {
        public override int DraggableDimensions
        {
            get
            {
                return 2;
            }
        }

        protected override DesignationDef Designation
        {
            get
            {
                return RV2R_Common.Devour;
            }
        }

        public Designator_Devour()
        {
            this.defaultLabel = "RV2R_DesginateDevour".Translate();
            this.defaultDesc = "RV2R_DesginateDevourDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Devour", true);
            this.soundDragSustain = SoundDefOf.Designate_DragStandard;
            this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            this.useMouseIcon = true;
            this.soundSucceeded = SoundDefOf.Designate_Hunt;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            bool flag = !c.InBounds(base.Map);
            AcceptanceReport acceptanceReport;
            if (flag)
            {
                acceptanceReport = false;
            }
            else
            {
                bool flag2 = !this.EdiblesInCell(c).Any<Pawn>();
                if (flag2)
                {
                    acceptanceReport = "RV2R_DesginateDevourErr".Translate();
                }
                else
                {
                    acceptanceReport = true;
                }
            }
            return acceptanceReport;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            foreach (Pawn pawn in this.EdiblesInCell(loc))
            {
                this.DesignateThing(pawn);
            }
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            bool flag = false;
            SettingsContainer_RutsStuff settings = Patch_RV2R_Settings.RV2_Rut_Settings.rutsStuff;
            AcceptanceReport acceptanceReport = false;
            if (t is Pawn)
            {
                Pawn pawn = t as Pawn;
                bool factionFlag = pawn.Faction == null
                                || pawn.Faction.HostileTo(Faction.OfPlayer)
                               || (pawn.Faction != Faction.OfPlayer && settings.DevourFriendlies)
                               || (pawn.Faction == Faction.OfPlayer && (settings.DevourColonists || settings.DevourColonistsFull));

                flag = base.Map.designationManager.DesignationOn(pawn, this.Designation) == null
                   && (pawn.Downed || (pawn.Faction == Faction.OfPlayer && settings.DevourColonistsFull))
                   && !pawn.IsMechanoid()
                   && !pawn.IsPrisonerInPrisonCell()
                   && factionFlag;

                if (!pawn.CanBeFatalPrey(out string _))
                {
                    if (pawn.Faction.HostileTo(Faction.OfPlayer) && (pawn.IsHumanoid() || !settings.EndoCapture))
                    {
                        flag = false;
                        acceptanceReport = "RV2R_DesginateDevourErrHuman".Translate();
                    }
                    if (pawn.IsInsectoid() || !settings.InsectoidCapture)
                    {
                        flag = false;
                        acceptanceReport = "RV2R_DesginateDevourErrScaria".Translate();
                    }
                    if (pawn.health.hediffSet.HasHediff(HediffDefOf.Scaria) || !settings.ScariaCapture)
                    {
                        flag = false;
                        acceptanceReport = "RV2R_DesginateDevourErrInsect".Translate();
                    }

                }
            }
            if (flag)
            {
                acceptanceReport = true;
            }
            return acceptanceReport;
        }

        public override void DesignateThing(Thing t)
        {
            base.Map.designationManager.RemoveAllDesignationsOn(t, false);
            base.Map.designationManager.AddDesignation(new Designation(t, this.Designation));
            this.justDesignated.Add((Pawn)t);
        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();
            this.justDesignated.Clear();
        }

        private IEnumerable<Pawn> EdiblesInCell(IntVec3 c)
        {
            bool flag = c.Fogged(base.Map);
            if (flag)
            {
                yield break;
            }
            List<Thing> thingList = c.GetThingList(base.Map);
            int num;
            for (int i = 0; i < thingList.Count; i = num + 1)
            {
                bool accepted = this.CanDesignateThing(thingList[i]).Accepted;
                if (accepted)
                {
                    yield return (Pawn)thingList[i];
                }
                num = i;
            }
            yield break;
        }

        protected private List<Pawn> justDesignated = new List<Pawn>();
    }
}
