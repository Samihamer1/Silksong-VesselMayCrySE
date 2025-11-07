using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.EffectHandler.Audio
{
    internal class FrostOverlayAudioEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            //Hornet/Frost_Region_Enter(Clone)
            //Get component AnimationAudioEvents.audioClips[0]

            GameObject hornet = HeroController.instance.gameObject;
            if (hornet == null) { return null; }

            GameObject? region = hornet.Child("Frost_Region_Enter(Clone)");
            if (region == null) { return null; }

            AudioEventAnimationEvents events = region.GetComponent<AudioEventAnimationEvents>();
            if (events == null) { return null; };

            return events.audioEvents[0].Clips[0];
        }
    }
}
