using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.UIHandler
{
    internal abstract class DevilMenuPage : MonoBehaviour
    {
        internal GameObject? pageRoot;
        private InventoryItemSelectableButtonEvent? defaultButton;

        /// <summary>
        /// Sets the page's state as active or not.
        /// </summary>
        /// <param name="state">The state to change to</param>
        public void SetActive(bool state)
        {
            pageRoot?.SetActive(state);
        }

        /// <summary>
        /// Sets the page's default button. Required for auto navigation when the page is activated.
        /// </summary>
        /// <param name="button"></param>
        public void SetDefaultButton(InventoryItemSelectableButtonEvent button)
        {
            defaultButton = button;

            DevilMenuUI.AddButtonToDefault(defaultButton);
        }

        private void Awake()
        {
            if (DevilMenuUI.buttonPrefab == null) { return; }
            if (DevilMenuUI.menuRoot == null) { return; }

            pageRoot = new GameObject("Devil Menu Page");
            pageRoot.transform.parent = DevilMenuUI.menuRoot.transform;
            pageRoot.transform.localPosition = Vector3.zero;
            pageRoot.SetActive(false);

            CreateMenuOptions();
        }

        public void OpenMenu()
        {
            if (defaultButton == null) { VesselMayCrySEPlugin.Instance.LogError("There is no default button in " + pageRoot?.name); return; }
            gameObject.transform.parent.GetComponent<InventoryItemToolManager>().SetSelected(defaultButton, null, false);
            OnMenuOpened();
        }

        /// <summary>
        /// To be overridden. Called when the menu is open.
        /// </summary>
        public abstract void OnMenuOpened();

        /// <summary>
        /// To be overridden. Create the menu page. You are provided pageRoot to work with.
        /// </summary>
        public abstract void CreateMenuOptions();
    }
}
