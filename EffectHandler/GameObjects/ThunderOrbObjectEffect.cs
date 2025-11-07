using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.EffectHandler.GameObjects
{
    internal class ThunderOrbObjectEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            ToolItemManager manager = ToolItemManager.Instance;
            if (manager == null) { return null; }

            ToolItem rod = manager.toolItems.GetByName("Lightning Rod");
            if (rod == null) { return null; };

            if (!(rod is ToolItemToggleState)) { return null; }

            ToolItemToggleState togglerod = (ToolItemToggleState)rod;
            
            GameObject orbprefab = togglerod.onState.Usage.ThrowPrefab;
            if (orbprefab == null) { return null; }

            PlayMakerFSM control = orbprefab.LocateMyFSM("Control");
            if (control == null) { return null; }

            FsmState? wallState = control.GetState("Wall L");
            if (wallState == null) { return null; }

            FlingObjectsFromGlobalPool? fling = wallState.GetFirstActionOfType<FlingObjectsFromGlobalPool>();
            if (fling == null) { return null; }

            GameObject clone = GameObject.Instantiate(fling.gameObject.value);
            if (clone == null) { return null; }

            clone.SetActive(false);
            GameObject.Destroy(clone.Child("Terrain Box"));
            GameObject.Destroy(clone.GetComponent<AutoRecycleSelf>());
            GameObject.Destroy(clone.GetComponent<TinkEffect>());
            GameObject.Destroy(clone.GetComponent<PlayMakerFSM>());
            GameObject.Destroy(clone.GetComponent<PlayMakerFSM>()); //There's two.
            GameObject.Destroy(clone.GetComponent<PlayMakerFSM>()); //There's three?

            GameObject? ring = clone.Child("Antic Ring");
            if (ring != null)
            {
                ring.SetActive(true);
                float val = 1.5f * 0.5f;
                ring.transform.localScale = new Vector3(val, val, 1);
            }

            GameObject? sprite = clone.Child("Ball Sprite");
            if (sprite != null)
            {
                sprite.SetActive(false);
            }

            GameObject.DontDestroyOnLoad(clone);
            clone.transform.parent = HeroController.instance.transform;

            clone.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            clone.GetComponent<Rigidbody2D>().useFullKinematicContacts = true;

            return clone;
        }
    }
}
