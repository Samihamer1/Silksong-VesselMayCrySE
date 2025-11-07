using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.Components;
using VesselMayCrySE.EffectHandler;

namespace VesselMayCrySE.Attacks.KingCerberus
{
    internal class IceAge : BaseSpell
    {
        private float COOLDOWN = 14;

        public IceAge(DevilCrestHandler handler) : base(handler)
        {
            EVENTNAME = EventNames.ICEAGE;
            ICON = ResourceLoader.LoadAsset<Sprite>("IceAgeIcon");
            ICONGLOW = ResourceLoader.LoadAsset<Sprite>("IceAgeIconGlow");
        }

        private IEnumerator DelayFinished()
        {
            yield return new WaitForSeconds(0.6f);
            fsm.SendEvent("END ICEAGE");
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            FsmState Antic = fsm.AddState("Ice Age Antic");
            Antic.AddMethod(_ =>
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
            });
            Antic.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("Ice Age Antic"));
            Antic.AddWatchAnimationAction("FINISHED");

            FsmState IceAge = fsm.AddState("Ice Age");
            IceAge.AddMethod(_ =>
            {
                HeroController.instance.StartInvulnerable(2f);
                GameObject? effect = (GameObject?)EffectManager.Get(EffectManager.GameObjectNames.FROSTHITEFFECT);

                if (effect != null)
                {
                    GameObject clone = GameObject.Instantiate(effect);
                    clone.GetComponent<TintRendererGroup>().color = new Color(1, 1, 1, 0.5f);
                    clone.GetComponent<TintRendererGroup>().UpdateTint();

                    clone.transform.parent = HeroController.instance.transform;
                    clone.transform.localPosition = Vector3.zero;

                    clone.SetActive(true);
                }

                AudioClip? clip = (AudioClip?)EffectManager.Get(EffectManager.AudioClipNames.SHARPDASH);
                HeroController.instance.GetComponent<AudioSource>().PlayOneShot(clip);

                GameManager.instance.StartCoroutine(DelayFinished());

                StartCooldownTimer(COOLDOWN);
            });
            IceAge.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("Ice Age"));
            IceAge.AddWatchAnimationAction("FINISHED");

            FsmState End = fsm.AddState("Ice Age End");            

            SetStateAsInit(Antic.name);
            Antic.AddTransition("FINISHED", IceAge.name);
            IceAge.AddTransition("END ICEAGE", End.name);
            SetStateAsEnd(End.name);
        }

        public override bool OnManualCooldown()
        {
            return false;
        }
    }
}
