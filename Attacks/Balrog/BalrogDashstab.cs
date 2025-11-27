using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.Attacks.Slashes;
using VesselMayCrySE.Components;
using VesselMayCrySE.CrestManagement;

namespace VesselMayCrySE.Attacks.Balrog
{
    internal class BalrogDashstab : BaseDashStab
    {
        private GameObject dashAttackObject1;
        public BalrogDashstab(DevilCrestHandler crestHandler) : base(crestHandler)
        {
            FSMEVENT = EventNames.BALROG;
        }

        private void CreateObjects()
        {
            AudioClip slashAudio = HeroController.instance.gameObject.Child("Attacks").Child("Wanderer").Child("Slash").GetComponent<AudioSource>().clip;

            //Regular slash
            GameObject balrogSlash = new GameObject("Balrog Dash Slash 1");
            balrogSlash.transform.parent = CrestManager.devilroot.transform;
            balrogSlash.transform.localPosition = new Vector3(-0.7f, -0.2f, -0.001f);
            BalrogSlash slash1 = balrogSlash.AddComponent<BalrogSlash>();
            slash1.SetScale(new Vector3(1f, 0.85f, 1));
            slash1.SetAnimation(AnimationManager.GetBalrogAnimator(), "SlashEffect");
            slash1.SetNailDamageMultiplier(0.33f);
            slash1.SetStunDamage(0.1f);
            slash1.SetMagnitude(0);
            slash1.SetColliderSize("CerberusSlash");
            slash1.SetAudioClip(slashAudio);
            slash1.RemoveRecoil();
            slash1.CreateNailTravel(new Vector2(-4.5f, 0), 0.15f);
            slash1.SetOffsetVariance(new Vector2(0.2f, 0.75f));
            slash1.CreateRedSlash(new Vector3(0.3f, 0, 0));

            DamageEnemies dmg = balrogSlash.GetComponent<DamageEnemies>();
            dmg.contactFSMEvent = "ATTACK LANDED";
            dmg.dealtDamageFSMEvent = "ATTACK LANDED";
            dmg.dealtDamageFSM = fsm;

            dashAttackObject1 = balrogSlash;
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }
            CreateObjects();

            FsmState AnticState = fsm.AddState("Balrog Antic");
            AnticState.AddAnimationAction(AnimationManager.GetBalrogAnimator().GetClipByName("Dash Attack Antic"));
            AnticState.AddWatchAnimationAction("FINISHED");

            FsmState SlashState = fsm.AddState("Balrog Slash");
            SlashState.AddAnimationAction(AnimationManager.GetBalrogAnimator().GetClipByName("Dash Attack"));
            SlashState.AddWatchAnimationAction("FINISHED");
            SlashState.AddMethod(_ =>
            {
                if (dashAttackObject1 == null) { return; }
                NailSlash slash = dashAttackObject1.GetComponent<NailSlash>();
                if (slash == null) { return; }
                slash.StartSlash();
            });

            FsmState RecoverState = fsm.AddState("Balrog Recover");
            RecoverState.AddAnimationAction(AnimationManager.GetBalrogAnimator().GetClipByName("Dash Attack Recover"));
            RecoverState.AddWatchAnimationAction("FINISHED");

            FsmState BounceState = fsm.AddState("Balrog Bounce");
            BounceState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                HeroController.instance.SetStartWithDownSpikeBounce();
            });


            SetStateAsInit(AnticState.name);
            AnticState.AddTransition("FINISHED", SlashState.name);
            SlashState.AddTransition("FINISHED", RecoverState.name);
            SlashState.AddTransition("ATTACK LANDED", BounceState.name);
            RecoverState.AddTransition("FINISHED", "Continue Sprint?");
            SetStateAsEnd(BounceState.name);
        }
    }
}
