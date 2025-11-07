using HutongGames.PlayMaker;
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
    internal class HotStuff : BaseNailArt
    {
        private NailSlash hotStuff;

        private const float COOLDOWNTIME = 1f;

        private GameObject? roundtrip;

        private void CreateHotStuff()
        {
            GameObject hotstuff = new GameObject(AttackNames.HOTSTUFF);
            hotstuff.transform.parent = CrestManager.devilroot.transform;
            hotstuff.transform.localPosition = new Vector3(-2f, 0, -0.001f);
            StandardSlash slash1 = hotstuff.AddComponent<StandardSlash>();
            slash1.SetScale(new Vector3(-1.35f, 1, 1));
            slash1.SetAnimation(AnimationManager.GetKingCerberusAnimator(), "HotStuffEffect");
            slash1.SetColliderSize("CerberusHotStuff");
            slash1.SetDamageDirection(180);
            slash1.SetAudioClip((AudioClip?)EffectManager.Get(EffectManager.AudioClipNames.WITCHSLASH));

            DamageEnemies dmg = hotstuff.GetComponent<DamageEnemies>();
            dmg.nailDamageMultiplier = 1.4f;
            dmg.silkGeneration = HitSilkGeneration.None;

            hotstuff.AddComponent<FireSlash>();

            hotStuff = hotstuff.GetComponent<NailSlash>();

        }

        private void ChaserCheck()
        {
            if (roundtrip != null) { return; }

            DevilCrestHandler handler = HeroController.instance.gameObject.GetComponent<DevilCrestHandler>();
            if (handler == null) { return; }
            if (!handler.ConsumeChaserBlade()) { return; }

            GameObject? prefab = (GameObject?)EffectManager.Get(EffectManager.GameObjectNames.ROUNDTRIP);
            if (prefab == null) { return; }

            roundtrip = GameObject.Instantiate(prefab);
            roundtrip.name = AttackNames.ROUNDTRIPRED;

            roundtrip.transform.parent = null;
            roundtrip.transform.position = HeroController.instance.transform.position;

            roundtrip.SetActive(true);

            roundtrip.transform.SetScale2D(new Vector2(0.7f, 0.7f));
            roundtrip.GetComponent<tk2dSpriteAnimator>().Play("RoundTripEffect");
            roundtrip.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            CreateHotStuff();

            FsmState Antic = fsm.AddState("Hot Stuff Antic");
            Antic.AddMethod(_ =>
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
            });
            Antic.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("HotStuff"));
            Antic.AddWatchAnimationActionTrigger("FINISHED");

            FsmState Slash = fsm.AddState("Hot Stuff Slash");
            Slash.AddWatchAnimationAction("FINISHED");
            Slash.AddMethod(_ =>
            {
                hotStuff.StartSlash();
                ChaserCheck();
            });

            //cancel cleanup
            FsmState CancelAllState = fsm.GetState("Cancel All");
            CancelAllState.AddMethod(_ =>
            {
                hotStuff.FullCancelAttack();
            });


            SetStateAsInit(Antic.name);
            Antic.AddTransition("FINISHED", Slash.name);
            SetStateAsEnd(Slash.name);
        }
    }
}
