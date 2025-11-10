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
            if (DevilMenuUI.Instance == null) { return; }
            pageRoot.name = "Devil Menu Options";

            SetScrollDetails(-10.7f, -2);
            Vector3 origPos = new Vector3(-10.7f, 0, -30);

            InventoryItemSelectableButtonEvent? skillListButton = DevilMenuUI.Instance.CreateTextButton("Skill List", "SKILL_LIST", origPos, scrollRoot);
            skillListButton.ButtonActivated += ActivateSkillList;
            SetDefaultButton(skillListButton);

            InventoryItemSelectableButtonEvent? difficultyButton = DevilMenuUI.Instance.CreateTextButton("Difficulty", "DIFFICULTY", origPos, scrollRoot);
            difficultyButton.ButtonActivated += ActivateDifficulty;

            InventoryItemSelectableButtonEvent? backButton = DevilMenuUI.Instance.CreateTextButton("Back", "BACK", origPos, scrollRoot);
            backButton.ButtonActivated += ReturnToPreviousPage;
        }

        private void ActivateDifficulty()
        {
            if (DevilMenuUI.Instance == null) { return; }

            DevilDifficultyUI difficultyUI = gameObject.GetComponent<DevilDifficultyUI>();
            if (difficultyUI == null) { return; }
            DevilMenuUI.Instance.TraverseToPage(difficultyUI);
        }

        private void ReturnToPreviousPage()
        {
            if (DevilMenuUI.Instance == null) { return; }

            DevilMenuUI.Instance.ResetDevilMenu();
        }

        private void ActivateSkillList()
        {
            if (DevilMenuUI.Instance == null) { return; }

            DevilSkillListUI skillListUI = gameObject.GetComponent<DevilSkillListUI>();
            if (skillListUI == null) { return; }
            DevilMenuUI.Instance.TraverseToPage(skillListUI);
        }

        public override void OnMenuOpened()
        {
            return;
        }
    }
}
