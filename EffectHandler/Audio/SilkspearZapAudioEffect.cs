using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE;

namespace VesselMayCrySE.EffectHandler.Audio
{
    internal class SilkspearZapAudioEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { return null; }

            PlayMakerFSM silkSpecialsFSM = hornet.LocateMyFSM("Silk Specials");
            if (silkSpecialsFSM == null) { return null; }

            PlayAudioEvent? action = silkSpecialsFSM.GetAction<PlayAudioEvent>("Silkspear Zap FX", 1);
            if (action == null) { return null; }

            return action.audioClip.value;
        }
    }
}
