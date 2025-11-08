using Mono.WebBrowser;
using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

namespace VesselMayCrySE.UIHandler
{
    internal class DevilSkillListUI : DevilPanelPage
    {
        private List<InventoryItemSelectableButtonEvent> buttons;

        public override void CreateMenuOptions()
        {
            base.CreateMenuOptions();
            if (pageRoot == null) { return; }
            pageRoot.name = "Devil Skill List";

            SetScrollDetails(-10.7f, -2);
            Vector3 origPos = new Vector3(-10.7f, 0, -30);

            List<string> nameKeys = new List<string>();

            foreach (string key in DevilSkillPage.SkillPages.Keys)
            {
                DevilSkillPage.SkillPageData pageData = DevilSkillPage.SkillPages[key];
                nameKeys.Add(pageData.name);
            }

            buttons = new List<InventoryItemSelectableButtonEvent>();

            foreach (string weapon in nameKeys)
            {
                InventoryItemSelectableButtonEvent? button = DevilMenuUI.CreateTextButton(weapon, weapon+"_NAME", origPos, scrollRoot);
                buttons.Add(button);
                button.OnSelected += SetPanelName;
                button.ButtonActivated += SetSkillInfo;
            }

            SetDefaultButton(buttons[0]);


            InventoryItemSelectableButtonEvent? backButton = DevilMenuUI.CreateTextButton("Back", "BACK", origPos, scrollRoot);
            backButton.ButtonActivated += ReturnToPreviousPage;
            backButton.OnSelected += SetNoName;

        }

        private void SetNoName(InventoryItemSelectable selectable)
        {
            SetNameAndDesc("BACK", "BLANK");
        }

        private void SetSkillInfo()
        {
            InventoryItemToolManager manager = gameObject.transform.parent.GetComponent<InventoryItemToolManager>();
            if (manager == null) { return; }

            InventoryItemSelectable current = manager.CurrentSelected;
            if (current == null) { return; }

            string key = current.name;

            if (DevilMenuUI.skillPageUI == null) { return; }

            DevilMenuUI.skillPageUI.SetPageTo(key);
            DevilMenuUI.TraverseToPage(DevilMenuUI.skillPageUI);
            
        }

        private void SetPanelName(InventoryItemSelectable selectable)
        {
            string key = selectable.gameObject.name;
            SetNameAndDesc(key + "_NAME", key + "_DESC");
        }

        private void ReturnToPreviousPage()
        {
            DevilMenuUI.TraverseToPage(DevilMenuUI.menuOptionsUI);
        }

        public override void OnMenuOpened()
        {
            //hotfix :)
            if (defaultButton == null) { return; }
            
            SetPanelName(defaultButton);
        }
    }
}
