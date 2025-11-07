using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.EffectHandler.GameObjects
{
    internal class HunterStabObjectEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { return null; }

            GameObject? attacks = hornet.Child("Attacks");
            if (attacks == null) { return null; }

            GameObject? hunter = attacks.Child("Default");
            if (hunter == null) { return null; }

            return hunter.Child("Dash Stab");
        }
    }
}
