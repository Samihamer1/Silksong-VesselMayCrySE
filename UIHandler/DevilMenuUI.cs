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

        private static InventoryItemSelectableButtonEvent? activationButton;

        private static InventoryItemToolManager inventoryItemToolManager;
        public static DevilSkillListUI? skillListUI;
        public static DevilMenuOptionsUI? menuOptionsUI;
        public static DevilSkillPageUI? skillPageUI;

        public static void CreateMenu(InventoryItemSelectableButtonEvent button)
        {
            buttonPrefab = button.gameObject;

            GameObject clone = GameObject.Instantiate(button.gameObject);
            clone.name = "Devil Crest Action";
            clone.transform.parent = button.transform.parent;
            clone.transform.localPosition = new Vector3(6.515f, 0,0);


            //Creating the activation button
            activationButton = clone.GetComponent<InventoryItemSelectableButtonEvent>();
            button.Selectables[(int)InventoryItemManager.SelectionDirection.Right] = activationButton; //Allowing it to be reached


            MenuButtonIcon icon = clone.GetComponentInChildren<MenuButtonIcon>();
            if (icon == null) { return; }

            icon.menuAction = Platform.MenuActions.Extra;
            icon.RefreshButtonIcon();
                
            //Allowing dash to activate the menu
            PlayMakerFSM proxyfsm = clone.transform.parent.transform.parent.gameObject.LocateMyFSM("Inventory Proxy");
            proxyfsm.GetState("EXTRA").InsertMethod(0, _ =>
            {
                if (menuRoot == null) { return; }
                SetToggleDevilMenu(!menuRoot.activeSelf);
            });


            //Creating menu
            InventoryItemToolManager manager = proxyfsm.gameObject.GetComponent<InventoryItemToolManager>();
            inventoryItemToolManager = manager;
            GameObject tools = manager.gameObject;
            menuRoot = new GameObject("Devil Menu");
            menuRoot.transform.parent = tools.transform;
            menuRoot.SetActive(false);

            skillPageUI = menuRoot.AddComponent<DevilSkillPageUI>();
            skillListUI = menuRoot.AddComponent<DevilSkillListUI>();

            menuOptionsUI = menuRoot.AddComponent<DevilMenuOptionsUI>(); //Must be last



            InventoryPane pane = proxyfsm.gameObject.GetComponent<InventoryPane>();
            pane.OnPaneStart += ResetDevilMenu;

            void TestActivate()
            {
                if (menuRoot == null) { return; }
                SetToggleDevilMenu(!menuRoot.activeSelf);
            }

            activationButton.ButtonActivated += TestActivate;
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
            //Change Crest Prompt, when not active means we can't edit the devil menu.
            //This is because the button is a child of Change Crest Prompt.
            //When we can't see the button, it shouldn't be able to be activated.
            //But only for activating. Not deactivating.
            if (!activationButton.transform.parent.gameObject.activeSelf && state) { return; };

            menuRoot.SetActive(state);

            GameObject tools = menuRoot.transform.parent.gameObject;
            tools.gameObject.Child("Tool Group").SetActive(!state);
            tools.gameObject.ChildChain("Tool Group", "Tool List").SetActive(!state);
            tools.gameObject.Child("Crest List").SetActive(!state);
            tools.gameObject.ChildChain("Tool Group", "Floating Slots").SetActive(!state);

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
    }
}
