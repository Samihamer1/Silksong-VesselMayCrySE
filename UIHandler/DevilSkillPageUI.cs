using Mono.WebBrowser;
using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.EffectHandler;

namespace VesselMayCrySE.UIHandler
{
    internal class DevilSkillPageUI : DevilPanelPage
    {
        private List<GameObject> baseLists = new List<GameObject>();
        private Dictionary<string, InventoryItemSelectableButtonEvent> defaultButtons = new Dictionary<string, InventoryItemSelectableButtonEvent>();

        private GameObject? currentPage;
        private UIComboPrompt? comboPrompt;
        private GameObject? thoughtsButton;

        private bool hornetMode = false; //For toggling Hornet's thoughts

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
            scrollRoot.SetActiveChildren(false);
            scrollRoot.SetActive(true);
            page.SetActive(true);
            currentPage = page;

            hornetMode = false;

            if (defaultButtons.ContainsKey(key))
            {
                InventoryItemSelectableButtonEvent defaultbutton = defaultButtons[key];
                SetDefaultButton(defaultbutton);
            }

            grid.Start();
        }

        public override void CreateMenuOptions()
        {
            base.CreateMenuOptions();

            if (pageRoot == null) { return; }
            pageRoot.name = "Devil Skill Page";

            SetScrollDetails(-10.7f, -2);

            foreach (string key in DevilSkillPage.SkillPages.Keys)
            {
                DevilSkillPage.SkillPageData pageData = DevilSkillPage.SkillPages[key];
                List<string> skillNames = new List<string>();

                foreach (DevilSkillPage.SkillData skill in pageData.skills)
                {
                    skillNames.Add(skill.key);
                }

                GameObject baselist = CreateBaseList(pageData.name, skillNames.ToArray());
                baseLists.Add(baselist);
            }

            ModifyIconSpacer();

            CreateThoughtsButton();
        }

        private void CreateThoughtsButton()
        {
            if (DevilMenuUI.Instance == null) { return; }

            //cancel button
            InventoryItemToolManager manager = DevilMenuUI.Instance.inventoryItemToolManager;
            if (manager == null) { return; }
            GameObject? cancelbutton = manager.gameObject.Child("Cancel Action");
            if (cancelbutton == null) { return; }
            thoughtsButton = GameObject.Instantiate(cancelbutton);
            thoughtsButton.transform.parent = pageRoot.transform;
            thoughtsButton.transform.localPosition = new Vector3(-0.05f, -4.6f, 0f);
            thoughtsButton.GetComponentInChildren<MenuButtonIcon>().menuAction = Platform.MenuActions.Submit;
            thoughtsButton.GetComponentInChildren<MenuButtonIcon>().RefreshButtonIcon();
            thoughtsButton.SetActive(true);
            
            foreach (TextMeshPro text in thoughtsButton.GetComponentsInChildren<TextMeshPro>())
            {
                text.color = new Color(1, 1, 1, 1);
            }

            thoughtsButton.GetComponentInChildren<SetTextMeshProGameText>().text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", "THOUGHTS_BUTTON");
            UpdateThoughtsButton();
        }

        private void UpdateThoughtsButton()
        {
            if (thoughtsButton == null) { return; }
            foreach (TextMeshPro text in thoughtsButton.GetComponentsInChildren<TextMeshPro>())
            {
                text.color = new Color(1, 1, 1, 1);
            }
            thoughtsButton.GetComponentInChildren<SetTextMeshProGameText>().text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", "THOUGHTS");
            if (hornetMode)
            {
                thoughtsButton.GetComponentInChildren<SetTextMeshProGameText>().text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", "DESCRIPTION");
            }
            thoughtsButton.GetComponentInChildren<SetTextMeshProGameText>().UpdateText();
        }

        private void ModifyIconSpacer()
        {
            if (panel == null) { return; }
            GameObject? spacer = panel.ChildChain("Layout Stack", "Icon Spacer");

            if (spacer == null) { return; }

            spacer.Child("Tool Icon").SetActive(false);

            GameObject? button = DevilMenuUI.CreateComboButtonPrompt();
            if (button == null) { return; }
            button.transform.parent = spacer.transform;
            button.transform.localPosition = new Vector3(0, -1.5f, 0);
            button.SetActive(true);
            comboPrompt = button.AddComponent<UIComboPrompt>();
        }

        private GameObject CreateBaseList(string name, string[] moves)
        {
            if (DevilMenuUI.Instance == null) { return null; }

            GameObject root = new GameObject(name);
            root.transform.parent = scrollRoot.transform;

            Vector3 origPos = new Vector3(-10.7f, 0, -30);

            int count = 0;

            foreach (string move in moves)
            {
                count++;
                InventoryItemSelectableButtonEvent? button = DevilMenuUI.Instance.CreateTextButton(move, move + "_NAME", origPos, root);
                button.OnSelected += SetPanelName;
                button.ButtonActivated += ToggleThoughts;

                //default
                if (move == moves[0])
                {
                    SetDefaultButton(button);
                    defaultButtons.Add(name, button);
                }
            }

            InventoryItemSelectableButtonEvent? backButton = DevilMenuUI.Instance.CreateTextButton("BACK", "BACK", origPos, root);
            backButton.ButtonActivated += ReturnToPreviousPage;
            backButton.OnSelected += SetNoName;

            return root;
        }

        private void SetNoName(InventoryItemSelectable selectable)
        {
            SetNameAndDesc("BACK", "BLANK");
            if (comboPrompt != null)
            {
                comboPrompt.gameObject.SetActiveChildren(false);
            }
        }

        private void ToggleThoughts()
        {
            if (DevilMenuUI.Instance == null) { return; }
            if (currentPage == null) { return; }
            if (thoughtsButton == null) { return; }
            GameObject currentButton = DevilMenuUI.Instance.inventoryItemToolManager.CurrentSelected.gameObject;
            if (currentButton == null) { return; }
            string movekey = currentButton.name;
            string namekey = currentPage.gameObject.name;

            hornetMode = !hornetMode;

            UpdateThoughtsButton();

            SetNameAndDesc(movekey + "_NAME", movekey + "_DESC");
            if (hornetMode)
            {
                SetNameAndDesc(movekey + "_NAME", movekey + "_HORNET");
            }
        }

        private void ReturnToPreviousPage()
        {
            if (DevilMenuUI.Instance == null) { return; }
            DevilMenuUI.Instance.TraverseToPage(DevilMenuUI.Instance.skillListUI);
        }

        private void SetPanelName(InventoryItemSelectable selectable)
        {
            if (currentPage == null) { return; }
            string movekey = selectable.gameObject.name;
            string namekey = currentPage.gameObject.name;

            DevilSkillPage.SkillData skillData = DevilSkillPage.SkillPages[namekey].skills.Find(s => s.key == movekey);
            if (skillData == null) { return; }

            if (comboPrompt != null)
            {
                hornetMode = false;
                UpdateThoughtsButton();

                comboPrompt.SetToSkill(skillData);
            }

            SetNameAndDesc(movekey + "_NAME", movekey + "_DESC");

            //Hotfix for desc size
            if (panel == null) { return; }
            GameObject? desc = panel.ChildChain("Layout Stack", "Text Desc");
            if (desc != null)
            {
                desc.GetComponent<RectTransform>().sizeDelta = new Vector2(7, 9);
            }
        }
        public override void OnMenuOpened()
        {
            if (currentPage == null) { return; }
            if (defaultButtons.ContainsKey(currentPage.name))
            {
                gameObject.transform.parent.GetComponent<InventoryItemToolManager>().SetSelected(defaultButtons[currentPage.name], null, false);
            }

            if (panel == null) { return; }
            panel.SetActive(true);

            scrollRoot.SetActive(true);
        }
    }
}
