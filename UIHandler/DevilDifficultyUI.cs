using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;
using VesselMayCrySE.Components;
using static HutongGames.PlayMaker.Actions.Vector2RandomValue;

namespace VesselMayCrySE.UIHandler
{
    internal class DevilDifficultyUI : DevilPanelPage
    {

        private GameObject? selectButton;
        public override void CreateMenuOptions()
        {
            if (DevilMenuUI.Instance == null) { return; }   

            base.CreateMenuOptions();

            if (pageRoot == null) { return; }
            pageRoot.name = "Devil Difficulty Select";

            SetScrollDetails(-10.7f, -2);

            string[] difficulties = new string[Difficulties.DifficultyList.Keys.Count];
            Difficulties.DifficultyList.Keys.CopyTo(difficulties, 0);

            Vector3 origPos = new Vector3(-10.7f, 0, -30);

            foreach (string difficulty in difficulties)
            {

                InventoryItemSelectableButtonEvent? button = DevilMenuUI.Instance.CreateTextButton(difficulty, difficulty, origPos, scrollRoot);
                button.ButtonActivated += SelectDifficulty;
                button.OnSelected += SetPanelName;

                if (difficulty == difficulties[0])
                {
                    SetDefaultButton(button);
                }
            }

            InventoryItemSelectableButtonEvent? backButton = DevilMenuUI.Instance.CreateTextButton("Back", "BACK", origPos, scrollRoot);
            backButton.ButtonActivated += ReturnToPreviousPage;
            backButton.OnSelected += SetNoName;

            //ModifyIconSpacer();

            CreateSelectButton();
        }

        private void SetNoName(InventoryItemSelectable selectable)
        {
            SetNameAndDesc("BACK", "BLANK");
        }

        private void ReturnToPreviousPage()
        {
            if (DevilMenuUI.Instance == null) { return; }
            DevilMenuUI.Instance.TraverseToPage(DevilMenuUI.Instance.menuOptionsUI);
        }

        private void SetPanelName(InventoryItemSelectable selectable)
        {
            if (DevilCrestHandler.Instance == null) { return; }
            string difficultyName = selectable.gameObject.name;

            string title = "NOT_SELECTED";

            Difficulties.DevilDifficulty newDifficulty = Difficulties.DifficultyList[difficultyName];

            if (DevilCrestHandler.Instance.GetCurrentDifficulty() == newDifficulty)
            {
                title = "SELECTED";
            }

            SetNameAndDesc(title, difficultyName + "_DESC");
        }

        private void CreateSelectButton()
        {
            if (DevilMenuUI.Instance == null) { return; }

            //cancel button
            InventoryItemToolManager manager = DevilMenuUI.Instance.inventoryItemToolManager;
            if (manager == null) { return; }
            GameObject? cancelbutton = manager.gameObject.Child("Cancel Action");
            if (cancelbutton == null) { return; }
            selectButton = GameObject.Instantiate(cancelbutton);
            selectButton.transform.parent = pageRoot.transform;
            selectButton.transform.localPosition = new Vector3(-0.05f, -4.6f, 0f);
            selectButton.GetComponentInChildren<MenuButtonIcon>().menuAction = Platform.MenuActions.Submit;
            selectButton.GetComponentInChildren<MenuButtonIcon>().RefreshButtonIcon();
            selectButton.SetActive(true);

            foreach (TextMeshPro text in selectButton.GetComponentsInChildren<TextMeshPro>())
            {
                text.color = new Color(1, 1, 1, 1);
            }

            selectButton.GetComponentInChildren<SetTextMeshProGameText>().text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", "SELECT_BUTTON");
            selectButton.GetComponentInChildren<SetTextMeshProGameText>().UpdateText();

            UpdateSelectButton();
        }

        private void UpdateSelectButton()
        {
            if (selectButton == null) { return; }
            foreach (TextMeshPro text in selectButton.GetComponentsInChildren<TextMeshPro>())
            {
                text.color = new Color(1, 1, 1, 1);
            }

        }

        private void SelectDifficulty()
        {
            if (DevilCrestHandler.Instance== null) { return; }
            if (DevilMenuUI.Instance == null) { return; }

            GameObject currentButton = DevilMenuUI.Instance.inventoryItemToolManager.CurrentSelected.gameObject;

            string difficultyName = currentButton.name;

            Difficulties.DevilDifficulty newDifficulty = Difficulties.DifficultyList[difficultyName];

            DevilCrestHandler.Instance.ChangeDifficulty(newDifficulty);

            SetPanelName(DevilMenuUI.Instance.inventoryItemToolManager.CurrentSelected);
        }

        public override void OnMenuOpened()
        {
            base.OnMenuOpened();
            UpdateSelectButton();
        }
    }
}
