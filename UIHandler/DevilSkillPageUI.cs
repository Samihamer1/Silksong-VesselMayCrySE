using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.Localization;
using TMPro;
using UnityEngine;

namespace VesselMayCrySE.UIHandler
{
    internal class DevilSkillPageUI : DevilMenuPage
    {
        private List<GameObject> baseLists = new List<GameObject>();
        private Dictionary<string, InventoryItemSelectableButtonEvent> defaultButtons = new Dictionary<string, InventoryItemSelectableButtonEvent>();

        private TextMeshPro? displayHeader;
        private TextMeshPro? displayDesc;

        private SpriteRenderer? displayIcon;

        public void SetPageTo(string key)
        {
            GameObject? page = null;
            foreach (GameObject list in baseLists)
            {
                if (list.name == key)
                {
                    page = list; break;
                }
            }

            if (page == null) { return; }
            if (pageRoot == null) { return; }
            pageRoot.SetActiveChildren(false);
            page.SetActive(true);

            if (defaultButtons.ContainsKey(key))
            {
                gameObject.transform.parent.GetComponent<InventoryItemToolManager>().SetSelected(defaultButtons[key], null, false);
            }

        }

        public override void CreateMenuOptions()
        {
            if (pageRoot == null) { return; }
            pageRoot.name = "Devil Skill Page";

            Dictionary<string, string[]> keypairs = new Dictionary<string, string[]>()
            {
                ["DEVILSWORD"] = ["DEVILSWORD_MASTERY", "DEVILSWORD_COMBO", "DEVILSWORD_DOWNSLASH", "DEVILSWORD_DRIVE", "DEVILSWORD_HIGHTIME", "DEVILSWORD_MILLIONSTAB", "DEVILSWORD_REACTOR", "DEVILSWORD_ROUNDTRIP", "DEVILSWORD_FORMATION"]
            };

            foreach (string key in keypairs.Keys)
            {
                string[] vals = keypairs[key];

                GameObject baselist = CreateBaseList(key, vals);
                baseLists.Add(baselist);
            }

            CreatePanel();
        }

        private GameObject CreateBaseList(string name, string[] moves)
        {
            GameObject root = new GameObject(name);
            root.transform.parent = pageRoot.transform;

            Vector3 origPos = new Vector3(-12.5f, 4.2f, -30);
            Vector3 addon = new Vector3(0, -2, 0);

            foreach (string move in moves)
            {
                InventoryItemSelectableButtonEvent? button = DevilMenuUI.CreateTextButton(move, move+"_NAME", origPos, root);
                button.OnSelected += SetPanelName;
                origPos += addon;

                //default
                if (move == moves[0])
                {
                    SetDefaultButton(button);
                    defaultButtons.Add(name,button);
                }
            }

            return root;
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

        private void SetPanelName(InventoryItemSelectable selectable)
        {
            string key = selectable.gameObject.name;
            SetNameAndDesc(key + "_NAME", key + "_DESC");
        }

        private void SetNameAndDesc(string name, string desc)
        {
            if (displayHeader == null || displayDesc == null) { return; }
            displayHeader.text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", name);
            displayDesc.text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", desc);
        }

        public override void OnMenuOpened()
        {
            throw new NotImplementedException();
        }
    }
}
