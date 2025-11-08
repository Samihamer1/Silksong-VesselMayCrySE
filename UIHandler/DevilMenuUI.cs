using GlobalEnums;
using Mono.WebBrowser;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

namespace VesselMayCrySE.UIHandler
{
    internal static class DevilMenuUI
    {
        public static GameObject? menuRoot;
        public static GameObject? buttonPrefab;
        private static GameObject? changeCrestPrompt;

        private static InventoryItemSelectableButtonEvent? activationButton;
        private static GameObject? cancelButton;

        public static InventoryItemToolManager inventoryItemToolManager;
        public static DevilSkillListUI? skillListUI;
        public static DevilMenuOptionsUI? menuOptionsUI;
        public static DevilSkillPageUI? skillPageUI;
        public static DevilDifficultyUI? difficultyUI;

        public static void CreateMenu(InventoryItemSelectableButtonEvent button)
        {
            buttonPrefab = button.gameObject;
            GameObject tools = button.transform.parent.transform.parent.gameObject;

            GameObject buttonParent = new GameObject("Devil Crest Button Parent");
            buttonParent.transform.parent = button.transform.parent;
            buttonParent.transform.localPosition = Vector3.zero;

            GameObject clone = GameObject.Instantiate(button.gameObject);
            clone.name = "Devil Crest Action";
            clone.transform.parent = buttonParent.transform;
            clone.transform.localPosition = new Vector3(6.515f, 0,0);


            //Creating the activation button
            activationButton = clone.GetComponent<InventoryItemSelectableButtonEvent>();
            button.Selectables[(int)InventoryItemManager.SelectionDirection.Right] = activationButton; //Allowing it to be reached

            activationButton.GetComponentInChildren<SetTextMeshProGameText>().text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", "DEVIL_CREST_BUTTON");
            activationButton.GetComponentInChildren<SetTextMeshProGameText>().UpdateText();

            MenuButtonIcon icon = clone.GetComponentInChildren<MenuButtonIcon>();
            if (icon == null) { return; }

            icon.menuAction = Platform.MenuActions.Extra;
            icon.RefreshButtonIcon();
                
            //Allowing dash to activate the menu
            PlayMakerFSM proxyfsm = tools.LocateMyFSM("Inventory Proxy");
            proxyfsm.GetState("EXTRA").InsertMethod(0, _ =>
            {
                if (menuRoot == null) { return; }
                SetToggleDevilMenu(!menuRoot.activeSelf);
            });


            //Creating menu
            InventoryItemToolManager manager = tools.GetComponent<InventoryItemToolManager>();
            inventoryItemToolManager = manager;
            menuRoot = new GameObject("Devil Menu");
            menuRoot.transform.parent = tools.transform;
            menuRoot.SetActive(false);

            changeCrestPrompt = manager.gameObject.Child("Change Crest Prompt");

            skillPageUI = menuRoot.AddComponent<DevilSkillPageUI>();
            skillListUI = menuRoot.AddComponent<DevilSkillListUI>();
            difficultyUI = menuRoot.AddComponent<DevilDifficultyUI>();

            menuOptionsUI = menuRoot.AddComponent<DevilMenuOptionsUI>(); //Must be last



            InventoryPane pane = proxyfsm.gameObject.GetComponent<InventoryPane>();
            pane.OnPaneStart += ResetDevilMenu;

            void TestActivate()
            {
                if (menuRoot == null) { return; }
                SetToggleDevilMenu(!menuRoot.activeSelf);
            }

            activationButton.ButtonActivated += TestActivate;

            //cancel button
            cancelButton = manager.gameObject.Child("Cancel Action");
            if (cancelButton == null) { return; }
            cancelButton = GameObject.Instantiate(cancelButton);
            cancelButton.transform.parent = manager.gameObject.transform;
            cancelButton.transform.localPosition = new Vector3(4f, -12.9f, -30f);
            cancelButton.GetComponentInChildren<MenuButtonIcon>().menuAction = Platform.MenuActions.Extra;
            cancelButton.GetComponentInChildren<MenuButtonIcon>().RefreshButtonIcon();


            //button parent component so that the activation button is only visible when devil crest is active
            buttonParent.AddComponent<CrestActiveCheckUI>();
        }

        public static void TraverseToPage(DevilMenuPage? page)
        {
            if (page == null) { return; }
            DeactivateAllMenus();
            page.SetActive(true);
            page.OpenMenu();
        }

        private static void DeactivateAllMenus()
        {
            if (menuRoot == null) { return; }
            menuRoot.SetActiveChildren(false);
        }

        public static void ReturnToMenu()
        {
            DeactivateAllMenus();

            TraverseToPage(menuOptionsUI);
        }

        public static void AddButtonToDefault(InventoryItemSelectable button)
        {
            if (inventoryItemToolManager == null) { return; }
            List<InventoryItemSelectable> defaultList = inventoryItemToolManager.defaultSelectables.ToList();
            if (defaultList.Contains(button)) { return; }
            defaultList.Add(button);

            inventoryItemToolManager.defaultSelectables = defaultList.ToArray();
        }

        public static void ResetDevilMenu()
        {
            SetToggleDevilMenu(false);
        }

        private static void SetToggleDevilMenu(bool state)
        {
            if (menuRoot == null) { return; }
            if (activationButton == null) { return; }

            //When we can't see the button, it shouldn't be able to be activated.
            //But only for activating. Not deactivating.
            if (changeCrestPrompt == null) { return; }
            if (!activationButton.gameObject.activeInHierarchy && state) { return; };

            menuRoot.SetActive(state);
            cancelButton.SetActive(state);

            GameObject tools = menuRoot.transform.parent.gameObject;
            tools.gameObject.Child("Tool Group").SetActive(!state);
            tools.gameObject.ChildChain("Tool Group", "Tool List").SetActive(!state);
            tools.gameObject.Child("Crest List").SetActive(!state);
            tools.gameObject.ChildChain("Tool Group", "Floating Slots").SetActive(!state);
            changeCrestPrompt.SetActiveChildren(!state); //Change Crest Prompt 


            ReturnToMenu();


            //Return to activation button if closing menu
            if (!state)
            {
                inventoryItemToolManager.SetSelected(activationButton, InventoryItemManager.SelectionDirection.Down, false);
            }
        }

        public static InventoryItemSelectableButtonEvent? CreateButton(string name, Vector3 localPosition, GameObject? root)
        {
            if (root == null) { return null; }
            if (buttonPrefab == null) { return null; }
            GameObject parent = new GameObject(name + " Parent");
            parent.transform.parent = root.transform;
            parent.transform.localPosition = localPosition;

            GameObject button1 = GameObject.Instantiate(buttonPrefab.gameObject);
            button1.name = name;
            button1.transform.parent = parent.transform;
            button1.transform.localPosition = Vector3.zero;

            return button1.GetComponentInChildren<InventoryItemSelectableButtonEvent>();
        }

        public static InventoryItemSelectableButtonEvent? CreateTextButton(string name, string key, Vector3 localPosition, GameObject? root)
        {
            InventoryItemSelectableButtonEvent? button = CreateButton(name, localPosition, root);
            if (button == null) { return null; }

            GameObject? changeAction = button.gameObject.Child("Change Crest Action");
            if (changeAction == null) { return null; }
            GameObject? actionButtonIcon = changeAction.Child("ActionButtonIcon");
            GameObject? textobject = changeAction.Child("Text");
            if (textobject == null || actionButtonIcon == null)
            {
                return null;
            }

            //actual changes
            actionButtonIcon.SetActive(false);
            textobject.transform.localPosition = new Vector3(1.32f, -2.184f, 0);
            textobject.GetComponent<ChangeFontByLanguage>().startFontSize = 8;

            textobject.GetComponent<SetTextMeshProGameText>().text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", key);

            return button;
        }

        public static GameObject? CreateComboButtonPrompt()
        {
            if (GameCameras.instance == null) { return null; }
            GameObject? prefab = GameCameras.instance.gameObject.ChildChain("HudCamera", "In-game", "Inventory", "Inv", "Button Prompts", "Combo Button Prompt");

            if (prefab == null) { return null; }
            GameObject clone = GameObject.Instantiate(prefab);

            return clone;
        }

        public static GameObject? CreateActionButton(HeroActionButton action)
        {
            if (menuRoot == null) { return null; }
            GameObject manager = menuRoot.transform.parent.gameObject;

            GameObject? button = manager.Child("Cancel Action");
            if (button == null) { return null; }
            button = GameObject.Instantiate(button);
            button.transform.localScale = new Vector3(1, 1, 1);
            GameObject.Destroy(button.GetComponentInChildren<MenuButtonIcon>());

            ActionButtonIcon actionButton = button.AddComponent<ActionButtonIcon>();
            actionButton.label = button.GetComponentInChildren<TextMeshPro>();
            actionButton.textContainer = button.GetComponentInChildren<TextContainer>();
            actionButton.initialAutoSize = false;
            actionButton.initialScale = new Vector3(1, 1, 1);
            actionButton.liveUpdate = true;

            actionButton.action = action;
            actionButton.RefreshButtonIcon();

            actionButton.label.color = new Color(1, 1, 1, 1);
            button.Child("ActionButtonIcon").transform.localPosition = new Vector3(0,0, 0);
            button.Child("ActionButtonIcon").transform.localScale = new Vector3(1, 1, 1);

            return button;
        }
    }
}
