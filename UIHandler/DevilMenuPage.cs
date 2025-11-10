using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace VesselMayCrySE.UIHandler
{
    internal abstract class DevilMenuPage : MonoBehaviour
    {
        internal GameObject pageRoot;
        internal GameObject scrollRoot;
        internal InventoryItemSelectableButtonEvent? defaultButton;

        private ScrollView scrollView;
        public InventoryItemGrid grid;

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
            if (DevilMenuUI.Instance == null) { return; }

            defaultButton = button;

            DevilMenuUI.Instance.AddButtonToDefault(defaultButton);
        }

        private void Awake()
        {
            if (DevilMenuUI.Instance== null) { return; }
            if (DevilMenuUI.Instance.buttonPrefab == null) { return; }
            if (DevilMenuUI.Instance.menuRoot == null) { return; }

            pageRoot = new GameObject("Devil Menu Page");
            pageRoot.transform.parent = DevilMenuUI.Instance.menuRoot.transform;
            pageRoot.transform.localPosition = Vector3.zero;
            pageRoot.SetActive(false);

            scrollRoot = new GameObject("Scroll Root");
            scrollRoot.transform.parent = pageRoot.transform;
            scrollRoot.transform.localPosition = Vector3.zero;

            scrollView = scrollRoot.AddComponent<ScrollView>();
            scrollView.useChildColliders = true;
            scrollView.wasOffBottom = true;
            scrollView.wasOffTop = true;
            scrollView.viewBounds = new Bounds(new Vector3(-12.5f, 2, 0), new Vector3(3.25f, 5.3f, 0));

            grid = scrollRoot.AddComponent<InventoryItemGrid>();
            grid.scrollView = scrollView;
            grid.selectables = new InventoryItemGrid.SelectableList[0];
            grid.ItemOffset = new Vector2(0, -2);
            grid.RowSplit = 1;

            CreateMenuOptions();
        }

        /// <summary>
        /// Sets the scroll view details for this page's children.
        /// </summary>
        /// <param name="xOffset">The offset of the entire list on the X axis</param>
        /// <param name="yAddon">The Y spacing between each button</param>
        public void SetScrollDetails(float xOffset, float yAddon)
        {
            scrollView.viewBounds = new Bounds(new Vector3(xOffset, 2, 0), new Vector3(3.25f, 5.3f, 0));
            grid.ItemOffset = new Vector2(0, yAddon);
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
        public virtual void OnMenuOpened() { }

        /// <summary>
        /// To be overridden. Create the menu page. You are provided pageRoot to work with.
        /// </summary>
        public virtual void CreateMenuOptions() { }
    }
}
