using GlobalEnums;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.UIHandler
{
    internal class UIComboPrompt : MonoBehaviour
    {
        private GameObject? withModifer;
        private GameObject? withoutModifier;
        private GameObject? doubleModifier;
        private GameObject? promptText;
        private GameObject? actionIcon;

        private GameObject? upArrow;
        private GameObject? downArrow;
        private GameObject? horizontalArrow;

        private GameObject? holdPrompt;

        private void Awake()
        {
            withModifer = gameObject.Child("Layout With Modifier");
            withoutModifier = gameObject.Child("Layout Without Modifier");
            promptText = gameObject.Child("Prompt Text");

            if (withModifer == null || withoutModifier == null || promptText == null)
            {
                VesselMayCrySEPlugin.Instance.LogError("UIComboPrompt: Failed to find required child objects.");
                return;
            }

            upArrow = withModifer.Child("Up Modifier");
            downArrow = withModifer.Child("Down Modifier");
            holdPrompt = withModifer.Child("Hold Prompt");
            actionIcon = withModifer.Child("Use ActionButtonIcon");

            if (upArrow == null || downArrow == null || holdPrompt == null || actionIcon == null)
            {
                VesselMayCrySEPlugin.Instance.LogError("UIComboPrompt: Failed to find required modifier icon objects.");
                return;
            }

            horizontalArrow = Instantiate(upArrow);
            horizontalArrow.name = "Horizontal Modifier";
            horizontalArrow.transform.parent = withModifer.transform;
            horizontalArrow.transform.localPosition = new Vector3(-0.7f, 0, 0);
            horizontalArrow.transform.localRotation = Quaternion.Euler(0, 0, 90);


            CreateDoubleModifier();
        }

        private void CreateDoubleModifier()
        {
            if (withModifer == null) { return; }
            //double modifier for sprint slash
            doubleModifier = Instantiate(withModifer);
            doubleModifier.name = "Layout Double Modifier";
            doubleModifier.transform.parent = gameObject.transform;
            doubleModifier.transform.localPosition = withModifer.transform.localPosition;

            GameObject? icon = doubleModifier.Child("Use ActionButtonIcon");
            if (icon == null) { return; }

            GameObject icon2 = Instantiate(icon);
            icon2.name = "Use ActionButtonIcon 2";
            icon2.transform.parent = icon.transform.parent;
            icon2.transform.localPosition = new Vector3(-0.75f, 0, 0);
        }

        private void SetDoubleCombo(HeroActionButton heroAction, HeroActionButton heroAction2, bool hold)
        {
            if (doubleModifier == null) { return; }
            gameObject.SetActiveChildren(false);
            doubleModifier.SetActive(true);
            doubleModifier.SetActiveChildren(false);


            GameObject? icon1 = doubleModifier.Child("Use ActionButtonIcon");
            GameObject? icon2 = doubleModifier.Child("Use ActionButtonIcon 2");

            if (icon1 == null || icon2 == null) { return; }

            ActionButtonIcon action1 = icon1.GetComponent<ActionButtonIcon>();
            ActionButtonIcon action2 = icon2.GetComponent<ActionButtonIcon>();

            if (action1 == null || action2 == null) { return; }

            action1.action = heroAction;
            action1.RefreshButtonIcon();
            action1.gameObject.SetActive(true);

            action2.action = heroAction2;
            action2.RefreshButtonIcon();
            action2.gameObject.SetActive(true);

            GameObject? plus = doubleModifier.Child("Plus Text");
            if (plus != null)
            {
                plus.SetActive(true);
            }

            GameObject? holdprompt = doubleModifier.Child("Hold Prompt");
            if (holdprompt != null)
            {
                holdprompt.SetActive(hold);
            }
        }

        public void SetToSkill(DevilSkillPage.SkillData skill)
        {
            switch (skill.type)
            {
                case DevilSkillPage.SkillType.UpSpell:
                    SetComboUp(HeroActionButton.QUICK_CAST, false);
                    break;
                case DevilSkillPage.SkillType.DownSpell:
                    SetComboDown(HeroActionButton.QUICK_CAST, false);
                    break;
                case DevilSkillPage.SkillType.HorizontalSpell:
                    SetComboHorizontal(HeroActionButton.QUICK_CAST, false);
                    break;
                case DevilSkillPage.SkillType.NeutralSpell:
                    SetNoCombo(HeroActionButton.QUICK_CAST);
                    break;
                case DevilSkillPage.SkillType.Passive:
                    SetNoModifier();
                    break;
                case DevilSkillPage.SkillType.Slash:
                    SetNoCombo(HeroActionButton.ATTACK);
                    break;
                case DevilSkillPage.SkillType.GreatSlash:
                    SetComboHorizontal(HeroActionButton.ATTACK, true);
                    break;
                case DevilSkillPage.SkillType.CycloneSlash:
                    SetComboUp(HeroActionButton.ATTACK, true);
                    break;
                case DevilSkillPage.SkillType.SprintSlash:
                    SetDoubleCombo(HeroActionButton.ATTACK, HeroActionButton.DASH, true);
                    break;
                case DevilSkillPage.SkillType.DownSlash:
                    SetComboDown(HeroActionButton.ATTACK, false);
                    break;
                case DevilSkillPage.SkillType.DashStab:
                    SetDoubleCombo(HeroActionButton.ATTACK, HeroActionButton.DASH, false);
                    break;
                default:
                    SetNoModifier();
                    break;
            }
        }

        private void SetNoCombo(HeroActionButton heroAction)
        {
            if (withoutModifier == null) { return; }
            gameObject.SetActiveChildren(false);
            withoutModifier.SetActive(true);
            ActionButtonIcon action = withoutModifier.GetComponentInChildren<ActionButtonIcon>();
            if (action == null) { return; }
            action.action = heroAction;
            action.RefreshButtonIcon();
        }

        private void SetNoModifier()
        {
            gameObject.SetActiveChildren(false);
        }

        private void SetComboUp(HeroActionButton heroAction, bool hold)
        {
            if (withModifer == null || upArrow == null || downArrow == null || holdPrompt == null) { return; }
            ResetWithModifierObject();
            upArrow.SetActive(true);

            holdPrompt.SetActive(hold);

            SetWithModifierAction(heroAction);
        }

        private void SetComboDown(HeroActionButton heroAction, bool hold)
        {
            if (withModifer == null || upArrow == null || downArrow == null || holdPrompt == null) { return; }
            ResetWithModifierObject();
            downArrow.SetActive(true);

            holdPrompt.SetActive(hold);

            SetWithModifierAction(heroAction);
        }

        private void SetComboHorizontal(HeroActionButton heroAction, bool hold)
        {
            if (withModifer == null || horizontalArrow == null || holdPrompt == null) { return; }
            ResetWithModifierObject();
            horizontalArrow.SetActive(true);

            holdPrompt.SetActive(hold);

            SetWithModifierAction(heroAction);
        }

        private void SetWithModifierAction(HeroActionButton heroAction)
        {
            if (actionIcon == null) { return; }
            ActionButtonIcon action = actionIcon.GetComponent<ActionButtonIcon>();
            if (action == null) { return; }
            action.action = heroAction;
            action.RefreshButtonIcon();
            action.gameObject.SetActive(true);
        }

        private void ResetWithModifierObject()
        {
            if (withModifer == null) { return; }
            gameObject.SetActiveChildren(false);
            withModifer.SetActive(true);
            withModifer.SetActiveChildren(false);

            GameObject plus = withModifer.Child("Plus Text");
            if (plus != null)
            {
                plus.SetActive(true);
            }
        }
    }
}
