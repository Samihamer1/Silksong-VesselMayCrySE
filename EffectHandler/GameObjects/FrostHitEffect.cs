using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;
using VesselMayCrySE.Components;

namespace VesselMayCrySE.EffectHandler.GameObjects
{
    internal class FrostHitEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { return null; };


            GameObject? effect = hornet.ChildChain("Effects", "Damage Effect", "Frost Dmg Pt");
            if (effect == null) { return null; }

            

            GameObject newfx = GameObject.Instantiate(effect);
            GameObject.DontDestroyOnLoad(newfx);

            TintRendererGroup tint = newfx.AddComponent<TintRendererGroup>();
            tint.GetComponentsInChildrenRecursively(newfx.transform);
            tint.color = new Color(1, 1, 1, 0.035f);
            tint.UpdateTint();

            DestroyAfter da = newfx.AddComponent<DestroyAfter>();
            da.SetTimer(3);

            newfx.SetActive(false);

            return newfx;
        }
    }
}
