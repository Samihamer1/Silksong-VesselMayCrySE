using GlobalEnums;
using HutongGames.PlayMaker;
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
    /// <summary>
    /// Intended to be added to the InventoryItemSelectableButtonEvent button within 'Change Crest Action' to create the Devil Menu UI.
    /// </summary>
    internal class DevilMenuUI : MonoBehaviour
    {
        public static DevilMenuUI? Instance;

        //Misc references
        public InventoryItemToolManager inventoryItemToolManager;

        //Gameobject references
        public GameObject menuRoot;
        public GameObject buttonPrefab;
        private GameObject? changeCrestPrompt;
        private GameObject ToolsPane;
        private GameObject? cancelButtonPrefab;
        private GameObject? cancelButton;

        //Button references
        private InventoryItemSelectableButtonEvent changeCrestAction;
        private InventoryItemSelectableButtonEvent activationButton;

        //Menu page references
        public DevilSkillListUI skillListUI;
        public DevilMenuOptionsUI menuOptionsUI;
        public DevilSkillPageUI skillPageUI;
        public DevilDifficultyUI difficultyUI;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            //Getting references
            changeCrestAction = gameObject.GetComponent<InventoryItemSelectableButtonEvent>();
            buttonPrefab = gameObject;
            ToolsPane = transform.parent.transform.parent.gameObject; //Tools inventory pane
            inventoryItemToolManager = ToolsPane.GetComponent<InventoryItemToolManager>();
            changeCrestPrompt = ToolsPane.Child("Change Crest Prompt");
            cancelButtonPrefab = ToolsPane.Child("Cancel Action");


            //Creating menu
            menuRoot = new GameObject("Devil Menu");
            menuRoot.transform.parent = ToolsPane.transform;
            menuRoot.SetActive(false);

            //Creating the menu pages
            skillPageUI = menuRoot.AddComponent<DevilSkillPageUI>();
            skillListUI = menuRoot.AddComponent<DevilSkillListUI>();
            difficultyUI = menuRoot.AddComponent<DevilDifficultyUI>();

            menuOptionsUI = menuRoot.AddComponent<DevilMenuOptionsUI>(); //Must be last

            //Creating cancel button
            cancelButton = CreateMenuButtonPrompt(ToolsPane, Platform.MenuActions.Extra);
            if (cancelButton != null)
            {
                cancelButton.transform.localPosition = new Vector3(4f, -12.9f, -30f);
            }


            //Final touches
            CreateActivationButton();
            PatchMenuOpening(); //Allowing the menu to be activated
        }

        /// <summary>
        /// Patches the 'Inventory Proxy' fsm to allow opening and closing of the devil menu, and resetting of the menu when the InventoryPane is opened.
        /// </summary>
        private void PatchMenuOpening()
        {
            if (ToolsPane == null) { return; }

            PlayMakerFSM? proxyfsm = ToolsPane.LocateMyFSM("Inventory Proxy");
            if (proxyfsm == null) { return; }

            FsmState? extraState = proxyfsm.GetState("EXTRA");
            if (extraState == null) { return; }

            extraState.InsertMethod(0, _ => { ToggleDevilMenu(); });

            InventoryPane pane = proxyfsm.gameObject.GetComponent<InventoryPane>();
            pane.OnPaneStart += ResetDevilMenu;
        }

        /// <summary>
        /// Creates the activation button for the devil menu.
        /// </summary>
        private void CreateActivationButton()
        {
            InventoryItemSelectableButtonEvent prefab = changeCrestAction;
            if (prefab == null) { VesselMayCrySEPlugin.Instance.LogError("Change Crest Action prefab not found when creating Activation Button"); return; }

            GameObject? parent = changeCrestPrompt;
            if (parent == null) { VesselMayCrySEPlugin.Instance.LogError("Change Crest Prompt not found when creating Activation Button"); return; }

            GameObject buttonParent = new GameObject("Devil Crest Button Parent"); //To hold the activation button within it
            buttonParent.transform.parent = parent.transform;
            buttonParent.transform.localPosition = Vector3.zero;

            GameObject clone = GameObject.Instantiate(prefab.gameObject);
            clone.name = "Devil Crest Action";
            clone.transform.parent = buttonParent.transform;
            clone.transform.localPosition = new Vector3(6.515f, 0, 0);


            //Creating the activation button
            activationButton = clone.GetComponent<InventoryItemSelectableButtonEvent>();

            //Setting text
            activationButton.GetComponentInChildren<SetTextMeshProGameText>().text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", "DEVIL_CREST_BUTTON");
            activationButton.GetComponentInChildren<SetTextMeshProGameText>().UpdateText();

            //Setting keybind icon
            MenuButtonIcon icon = clone.GetComponentInChildren<MenuButtonIcon>();
            if (icon == null) { return; }

            icon.menuAction = Platform.MenuActions.Extra;
            icon.RefreshButtonIcon();

            //button parent component so that the activation button is only visible when devil crest is active
            buttonParent.AddComponent<CrestActiveCheckUI>();

            //Allowing activation of menu
            activationButton.ButtonActivated += ToggleDevilMenu;
        }

        /// <summary>
        /// Toggles the devil menu open or closed.
        /// </summary>
        private void ToggleDevilMenu()
        {
            if (menuRoot == null) { return; }
            SetStateDevilMenu(!menuRoot.activeSelf);
        }

        /// <summary>
        /// Traverses the menu to a given page
        /// </summary>
        /// <param name="page">Page to traverse to, may be null</param>
        public void TraverseToPage(DevilMenuPage? page)
        {
            if (page == null) { return; }
            DeactivateAllMenus();
            page.SetActive(true);
            page.OpenMenu();
        }

        /// <summary>
        /// Deactivates all devil menu pages, as well as all children of the menu root.
        /// </summary>
        private void DeactivateAllMenus()
        {
            if (menuRoot == null) { return; }
            menuRoot.SetActiveChildren(false);
        }

        /// <summary>
        /// Returns to the main menu page.
        /// </summary>
        public void ReturnToMenu()
        {
            DeactivateAllMenus();

            TraverseToPage(menuOptionsUI);
        }

        /// <summary>
        /// Adds a button to the default selectable buttons of the inventory tool manager.
        /// Used to allow auto navigation to work correctly.
        /// </summary>
        /// <param name="button">The button to add</param>
        public void AddButtonToDefault(InventoryItemSelectable button)
        {
            if (inventoryItemToolManager == null) { return; }
            List<InventoryItemSelectable> defaultList = inventoryItemToolManager.defaultSelectables.ToList();
            if (defaultList.Contains(button)) { return; }
            defaultList.Add(button);

            inventoryItemToolManager.defaultSelectables = defaultList.ToArray();
        }

        /// <summary>
        /// Closes the devil menu.
        /// </summary>
        public void ResetDevilMenu()
        {
            SetStateDevilMenu(false);
        }

        /// <summary>
        /// Sets the state of the devil menu.
        /// </summary>
        /// <param name="state">true: open, false: closed</param>
        private void SetStateDevilMenu(bool state)
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

        /// <summary>
        /// Creates a button with an icon and small font.
        /// </summary>
        /// <param name="name">Name of gameobject</param>
        /// <param name="localPosition">Position of gameobject</param>
        /// <param name="root">Parent of gameobject</param>
        /// <returns></returns>
        public InventoryItemSelectableButtonEvent? CreateButton(string name, Vector3 localPosition, GameObject? root)
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

        /// <summary>
        /// Creates a button with large text and an icon.
        /// </summary>
        /// <param name="name">Name of gameobjectf</param>
        /// <param name="key">Language key of text</param>
        /// <param name="localPosition">Position of gameobject</param>
        /// <param name="root">Parent of gameobject</param>
        /// <returns></returns>
        public InventoryItemSelectableButtonEvent? CreateTextButton(string name, string key, Vector3 localPosition, GameObject? root)
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

        /// <summary>
        /// Creates a combo button prompt for showing gameplay keybind prompts.  
        /// </summary>
        /// <returns>Created gameobject</returns>
        public static GameObject? CreateComboButtonPrompt()
        {
            if (GameCameras.instance == null) { return null; }
            GameObject? prefab = GameCameras.instance.gameObject.ChildChain("HudCamera", "In-game", "Inventory", "Inv", "Button Prompts", "Combo Button Prompt");

            if (prefab == null) { return null; }
            GameObject clone = GameObject.Instantiate(prefab);

            return clone;
        }

        /// <summary>
        /// Creates a menu button prompt, which is a keybind next to text.
        /// </summary>
        /// <param name="parent">Parent object</param>
        /// <param name="action">Action to display</param>
        /// <returns>Created gameobject</returns>
        public GameObject? CreateMenuButtonPrompt(GameObject parent, Platform.MenuActions action)
        {
            if (cancelButtonPrefab == null) { return null; }
            GameObject cloneButton = GameObject.Instantiate(cancelButtonPrefab);
            cloneButton.transform.parent = inventoryItemToolManager.gameObject.transform;

            MenuButtonIcon icon = cloneButton.GetComponentInChildren<MenuButtonIcon>();
            if (icon == null) { VesselMayCrySEPlugin.Instance.LogError($"MenuButtonIcon not found when calling CreateButtonPrompt({parent.name},{action.ToString()})"); return cloneButton; }

            icon.menuAction = Platform.MenuActions.Extra;
            icon.RefreshButtonIcon();

            return cloneButton;
        }

    }
}
