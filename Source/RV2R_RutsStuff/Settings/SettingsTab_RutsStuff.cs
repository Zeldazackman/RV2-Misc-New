using RimVore2;
using System;
using UnityEngine;
using static RV2R_RutsStuff.Patch_RV2R_Settings;

namespace RV2R_RutsStuff
{
    public class SettingsTab_RutsStuff : SettingsTab
    {
        public SettingsTab_RutsStuff(string label, Action clickedAction, bool selected)
            : base(label, clickedAction, selected)
        {
        }

        public SettingsTab_RutsStuff(string label, Action clickedAction, Func<bool> selected)
            : base(label, clickedAction, selected)
        {
        }

        public override SettingsContainer AssociatedContainer => RV2_Rut_Settings.rutsStuff;
        private SettingsContainer_RutsStuff RutsStuff => (SettingsContainer_RutsStuff)this.AssociatedContainer;

        public override void FillRect(Rect inRect)
        {
            this.RutsStuff.FillRect(inRect);
        }
    }
}