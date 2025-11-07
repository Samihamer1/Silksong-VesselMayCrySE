using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.UIHandler
{
    internal class DevilMenuOptionsUI : DevilMenuPage
    {
        public override void CreateMenuOptions()
        {
            if (pageRoot == null) { return; }
            pageRoot.name = "Devil Menu Options";

            Vector3 origPos = new Vector3(-10.7f, 4.2f, -30);
            Vector3 addon = new Vector3(0, -2, 0);

            InventoryItemSelectableButtonEvent? skillListButton = DevilMenuUI.CreateTextButton("Skill List", "SKILL_LIST", origPos, pageRoot);
            skillListButton.ButtonActivated += ActivateSkillList;
            SetDefaultButton(skillListButton);

            origPos += addon;
            InventoryItemSelectableButtonEvent? backButton = DevilMenuUI.CreateTextButton("Back", "BACK", origPos, pageRoot);
            backButton.ButtonActivated += ReturnToPreviousPage;
        }

        private void ReturnToPreviousPage()
        {
            DevilMenuUI.ResetDevilMenu();
        }

        private void ActivateSkillList()
        {
            DevilSkillListUI skillListUI = gameObject.GetComponent<DevilSkillListUI>();
            if (skillListUI == null) { return; }
            DevilMenuUI.TraverseToPage(skillListUI);
        }

        public override void OnMenuOpened()
        {
            return;
        }
    }
}
