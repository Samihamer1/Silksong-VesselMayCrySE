using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;
using UnityEngine.UIElements.Layout;

namespace VesselMayCrySE.UIHandler
{
    internal class DevilPanelPage : DevilMenuPage
    {
        public GameObject? panel;

        private TextMeshPro? displayHeader;
        private TextMeshPro? displayDesc;

        private SpriteRenderer? displayIcon;


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

            panel = infoPanel;
        }


        public void SetNameAndDesc(string name, string desc)
        {
            if (displayHeader == null || displayDesc == null) { return; }
            displayHeader.text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", name);
            displayDesc.text = new LocalisedString($"Mods.{VesselMayCrySEPlugin.Id}", desc);
        }

        public override void CreateMenuOptions()
        {
            base.CreateMenuOptions();

            if (pageRoot == null) { return; }
            CreatePanel();
        }

        public override void OnMenuOpened()
        {
            return;
        }
    }
}
