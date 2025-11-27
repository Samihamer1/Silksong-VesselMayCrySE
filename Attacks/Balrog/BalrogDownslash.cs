using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.AnimationHandler;

namespace VesselMayCrySE.Attacks.Balrog
{
    internal class BalrogDownslash : BaseCrestAttack
    {
        public BalrogDownslash(GameObject attackObject) : base(attackObject)
        {
            FSMEVENT = "BALROG DOWNSLASH";
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }
            if (attackObject == null) { return; }

            DamageEnemies dmg = attackObject.GetComponent<DamageEnemies>();
            if (dmg == null) { return; }

            dmg.dealtDamageFSM = fsm;
            dmg.dealtDamageFSMEvent = "ATTACK LANDED";
            dmg.contactFSMEvent = "ATTACK LANDED";


            FsmState AnticState = fsm.AddState("Balrog Downslash Antic");
            AnticState.AddMethod(_=>
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
            });
            AnticState.AddAnimationAction(AnimationManager.GetBalrogAnimator().GetClipByName("DownSlash Antic"));
            AnticState.AddWatchAnimationAction("FINISHED");

            FsmState AttackState = fsm.AddState("Balrog Downslash Attack");
            AttackState.AddMethod(_ =>
            {
                NailSlash slash = attackObject.GetComponent<NailSlash>();
                if (slash == null) { return; }
                HeroController.instance.SetCState("downAttacking", true);
                slash.StartSlash();
            });
            AttackState.AddAnimationAction(AnimationManager.GetBalrogAnimator().GetClipByName("DownSlash"));
            AttackState.AddWatchAnimationAction("FINISHED");

            //Bounce
            FsmState BounceState = fsm.AddState("Balrog Bounce");
            BounceState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                HeroController.instance.SetCState("downAttacking", false);
                HeroController.instance.SetStartWithDownSpikeBounce();

                HeroController.instance.animCtrl.StartControlToIdle();
            });

            SetStateAsInit(AnticState.name);
            AnticState.AddTransition("FINISHED", AttackState.name);
            AttackState.AddTransition("ATTACK LANDED", BounceState.name);
            SetStateAsEnd(AttackState.name);
            SetStateAsEnd(BounceState.name);
            
        }
    }
}
