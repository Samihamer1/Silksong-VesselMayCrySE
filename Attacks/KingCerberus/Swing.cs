using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.Attacks.Slashes;
using VesselMayCrySE.Attacks.Slashes.SlashComponents;
using VesselMayCrySE.Components;
using VesselMayCrySE.EffectHandler;

namespace VesselMayCrySE.Attacks.KingCerberus
{
    internal class Swing : BaseSpell
    {
        private NailSlash swingObject;
        private float COOLDOWNTIME = 1f;
        public Swing(DevilCrestHandler handler) : base(handler)
        {
            this.EVENTNAME = EventNames.SWING;
            ICON = ResourceLoader.LoadAsset<Sprite>("SwingIcon");
            ICONGLOW = ResourceLoader.LoadAsset<Sprite>("SwingIconGlow");
        }

        private void CreateSwing()
        {
            GameObject swing = new GameObject(AttackNames.SWING);
            swing.transform.parent = CrestManager.devilroot.transform;
            swing.transform.localPosition = new Vector3(0f, 0, -0.001f);
            StandardSlash slash1 = swing.AddComponent<StandardSlash>();
            slash1.SetScale(new Vector3(1f, 1, 1));
            slash1.SetAnimation(AnimationManager.GetKingCerberusAnimator(), "SwingEffect");
            slash1.SetColliderSize("CerberusSwing");
            slash1.SetAudioClip((AudioClip?)EffectManager.Get(EffectManager.AudioClipNames.WITCHSLASH));
            slash1.SetColor(ColorConstants.CerberusBlue);
            slash1.SetDownAttack();
            slash1.SetDamageDirection(270);            

            DamageEnemies dmg = swing.GetComponent<DamageEnemies>();
            dmg.nailDamageMultiplier = 1.4f;
            dmg.silkGeneration = HitSilkGeneration.None;

            dmg.dealtDamageFSM = fsm;
            dmg.dealtDamageFSMEvent = "END SWING";

            //red slash here to avoid ice

            slash1.CreateRedSlash(new Vector3(-0.3f, -0.3f, 0));

            swing.AddComponent<IceSlash>();

            swingObject = swing.GetComponent<NailSlash>();

        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            CreateSwing();

            FsmState SwingAntic = fsm.AddState("Swing Antic");
            SwingAntic.AddMethod(_ =>
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
            });
            SwingAntic.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("Swing Antic"));
            SwingAntic.AddWatchAnimationAction("FINISHED");

            FsmState Swing = fsm.AddState("Swing");
            Swing.AddMethod(_ =>
            {
                swingObject.StartSlash();
                StartCooldownTimer(COOLDOWNTIME);
                float scale = HeroController.instance.transform.GetScaleX();
                HeroController.instance.GetComponent<Rigidbody2D>().SetVelocity(scale * -30, -30);
            });
            Swing.AddAction(new DecelerateV2 { gameObject = Helper.GetHornetOwnerDefault(), brakeOnExit = false, deceleration = 0.7f });
            Swing.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("Swing"));
            Swing.AddWatchAnimationAction("END SWING");

            //cancel cleanup
            FsmState CancelAllState = fsm.GetState("Cancel All");
            CancelAllState.AddMethod(_ =>
            {
                swingObject.CancelAttack();
            });


            SetStateAsInit(SwingAntic.name);
            SwingAntic.AddTransition("FINISHED", Swing.name);
            Swing.AddTransition("END SWING", "Special End");
        }

        public override bool OnManualCooldown()
        {
            return false;
        }
    }
}
