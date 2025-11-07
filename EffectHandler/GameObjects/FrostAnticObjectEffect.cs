using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.EffectHandler.GameObjects
{
    internal class FrostAnticObjectEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { return null; }

            return hornet.ChildChain("Effects", "Frost Antic Pt");
        }
    }
}
