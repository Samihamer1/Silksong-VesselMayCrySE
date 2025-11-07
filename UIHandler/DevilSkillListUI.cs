using Mono.WebBrowser;
using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

namespace VesselMayCrySE.UIHandler
{
    internal class DevilSkillListUI : DevilMenuPage
    {
        private List<InventoryItemSelectableButtonEvent> buttons;

        private TextMeshPro? displayHeader;
        private TextMeshPro? displayDesc;

        private SpriteRenderer? displayIcon;

        public override void CreateMenuOptions()
        {
            if (pageRoot == null) { return; }
            pageRoot.name = "Devil Skill List";

            Vector3 origPos = new Vector3(-10.7f, 4.2f, -30);
            Vector3 addon = new Vector3(0, -2, 0);

            List<string> weapons = new List<string>() { "DEVILSWORD", "CERBERUS" };
            buttons = new List<InventoryItemSelectableButtonEvent>();

            foreach (string weapon in weapons)
            {
                InventoryItemSelectableButtonEvent? button = DevilMenuUI.CreateTextButton(weapon, weapon+"_NAME", origPos, pageRoot);
                buttons.Add(button);
                button.OnSelected += SetPanelName;
                button.ButtonActivated += SetSkillInfo;
                origPos += addon;
            }

            SetDefaultButton(buttons[0]);


            InventoryItemSelectableButtonEvent? backButton = DevilMenuUI.CreateTextButton("Back", "BACK", origPos, pageRoot);
            backButton.ButtonActivated += ReturnToPreviousPage;

            CreatePanel();

        }

        private void SetSkillInfo()
        {
            InventoryItemToolManager manager = gameObject.transform.parent.GetComponent<InventoryItemToolManager>();
            if (manager == null) { return; }

            InventoryItemSelectable current = manager.CurrentSelected;
            if (current == null) { return; }

            string key = current.name;

            Dictionary<string, string[]> keypairs = new Dictionary<string, string[]>()
            {
                ["DEVILSWORD"] = ["DEVILSWORD_MASTERY", "DEVILSWORD_COMBO", "DEVILSWORD_DOWNSLASH", "DEVILSWORD_DRIVE", "DEVILSWORD_HIGHTIME", "DEVILSWORD_MILLIONSTAB", "DEVILSWORD_REACTOR", "DEVILSWORD_ROUNDTRIP", "DEVILSWORD_FORMATION"]
            };

            if (DevilMenuUI.skillPageUI == null) { return; }

            DevilMenuUI.TraverseToPage(DevilMenuUI.skillPageUI);
            DevilMenuUI.skillPageUI.SetPageTo(key);
        }

        private void SetPanelName(InventoryItemSelectable selectable)
        {
            string key = selectable.gameObject.name;
            SetNameAndDesc(key + "_NAME", key + "_DESC");
        }

        private void CreatePanel()
        {
            if (pageRoot == null) { return; }
            GameObject? thingToClone = pageRoot.transform.parent.transform.parent.gameObject.ChildChain("Tool Group", "Tool Description Panel");
            //WOW THAT ONE HURT TO TYPE
            if (thingToClone == null) { return; }

            GameObject infoPanel = GameObject.Instantiate(thingToClone);
            infoPanel.name = "Weapon Info Panel";
            infoPanel.transform.parent = pageRoot.transform;
            infoPanel.transform.localPosition = new Vector3(-3.3f, 7.1f, 0);

            infoPanel.SetActiveChildren(false);

            GameObject? divider = infoPanel.Child("Divider");
            GameObject? layoutStack = infoPanel.Child("Layout Stack");
            if (divider == null || layoutStack == null) { return; }

            GameObject? textAmount = layoutStack.ChildChain("Icon Spacer", "Text Amount");
            if (textAmount == null) { return; }

            textAmount.SetActive(false);

            layoutStack.SetActive(true);
            divider.SetActive(true);

            displayHeader = layoutStack.Child("Text Name").gameObject.GetComponent<TextMeshPro>();
            displayDesc = layoutStack.Child("Text Desc").gameObject.GetComponent<TextMeshPro>();

            GameObject? toolIcon = layoutStack.ChildChain("Icon Spacer", "Tool Icon");
            displayIcon = toolIcon.GetComponent<SpriteRenderer>();
        }

        private void SetNameAndDesc(string name, string desc)
        {
            if (displayHeader == null || displayDesc == null) { return; }
            displayHeader.text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", name);
            displayDesc.text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", desc);
        }

        private void ReturnToPreviousPage()
        {
            DevilMenuUI.TraverseToPage(DevilMenuUI.menuOptionsUI);
        }

        public override void OnMenuOpened()
        {
            //hotfix :)
            SetNameAndDesc("DEVILSWORD_NAME", "DEVILSWORD_DESC");
        }
    }
}
