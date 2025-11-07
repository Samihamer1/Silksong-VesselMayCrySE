using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;
using VesselMayCrySE.Components;

namespace VesselMayCrySE.EffectHandler.GameObjects
{
    internal class FrostSlashEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { return null; };

            NailImbuementConfig config = HeroController.instance.NailImbuement.nailConfigs[(int)NailElements.Fire];
            if (config == null) { return null; };

            GameObject flintstoneSlash = config.SlashEffect;
            if (flintstoneSlash == null) { return null; };

            GameObject? embers4 = flintstoneSlash.Child("embers (4)");
            GameObject? embers5 = flintstoneSlash.Child("embers (5)");

            if (embers4 == null || embers5 == null) { return null; };

            GameObject frostslash = new GameObject("Frost Slash Effect");
            GameObject.DontDestroyOnLoad(frostslash);
            frostslash.transform.parent = HeroController.instance.gameObject.transform;
            frostslash.transform.localPosition = Vector3.zero;
            GameObject e4copy = GameObject.Instantiate(embers4);
            GameObject e5copy = GameObject.Instantiate(embers5);

            e4copy.transform.parent = frostslash.transform;
            e4copy.transform.localPosition = Vector3.zero;

            e5copy.transform.parent = frostslash.transform;
            e5copy.transform.localPosition = Vector3.zero;

            TintRendererGroup tint = frostslash.AddComponent<TintRendererGroup>();
            tint.GetComponentsInChildrenRecursively(frostslash.transform);
            tint.color = new Color(0.2f, 0.6f, 0.75f, 0.15f);
            tint.UpdateTint();

            DestroyAfter da = frostslash.AddComponent<DestroyAfter>();
            da.SetTimer(2);

            frostslash.SetActive(false);

            return frostslash;
        }
    }
}
