using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.EffectHandler.Audio
{
    internal class SharpDashAudioEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { return null; }

            PlayMakerFSM silkSpecialsFSM = hornet.LocateMyFSM("Silk Specials");
            if (silkSpecialsFSM == null) { return null; }

            PlayAudioEvent? action = silkSpecialsFSM.GetAction<PlayAudioEvent>("Silk Charge Start", 12);
            if (action == null) { return null; }

            return action.audioClip.value;
        }
    }
}
