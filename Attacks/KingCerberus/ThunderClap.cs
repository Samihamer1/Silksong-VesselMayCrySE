using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.Attacks.DevilSword;
using VesselMayCrySE.Components;
using VesselMayCrySE.Components.ObjectComponents;
using VesselMayCrySE.EffectHandler;

namespace VesselMayCrySE.Attacks.KingCerberus
{
    internal class ThunderClap : BaseSpell
    {
        private float COOLDOWN = 3f;
        public ThunderClap(DevilCrestHandler handler) : base(handler)
        {
            EVENTNAME = EventNames.THUNDERCLAP;
            ICON = ResourceLoader.LoadAsset<Sprite>("ThunderClapIcon");
            ICONGLOW = ResourceLoader.LoadAsset<Sprite>("ThunderClapIconGlow");
        }

        private void CreateOrbs()
        {
            GameObject? orb = (GameObject?)EffectManager.Get(EffectManager.GameObjectNames.THUNDERORB);
            if (orb == null) { return; }

            GameObject clone = GameObject.Instantiate(orb);
            clone.transform.parent = null;
            clone.transform.position = HeroController.instance.transform.position;

            clone.AddComponent<ThunderOrb>();
            DestroyAfter da = clone.AddComponent<DestroyAfter>();
            da.SetTimer(3f);

            clone.SetActive(true);

            if (!handler.ConsumeChaserBlade()) { return; }

            CreateOrbitingSwords(clone);
        }

        private void CreateOrbitingSwords(GameObject orb)
        {
            GameObject? sword1 = CreateSword();
            GameObject? sword2 = CreateSword();

            if (sword1 == null || sword2 == null) { return; }

            OrbitingRoundTripSword sword1orbit = CreateOrbitingSword(sword1, orb);
            OrbitingRoundTripSword sword2orbit = CreateOrbitingSword(sword2, orb);
            sword2orbit.SetTime(0.5f); //so it is on the other side of orbit
        }

        private GameObject? CreateSword()
        {
            GameObject? prefab = (GameObject?)EffectManager.Get(EffectManager.GameObjectNames.ROUNDTRIP);
            if (prefab == null) { return null; }

            GameObject roundtrip = GameObject.Instantiate(prefab);
            roundtrip.name = AttackNames.ROUNDTRIPRED;

            roundtrip.transform.parent = null;
            roundtrip.transform.position = HeroController.instance.transform.position;

            roundtrip.SetActive(true);

            roundtrip.transform.SetScale2D(new Vector2(1f, 1f));
            roundtrip.GetComponent<tk2dSpriteAnimator>().Play("RoundTripEffect");
            roundtrip.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;

            return roundtrip;
        }

        private OrbitingRoundTripSword CreateOrbitingSword(GameObject sword1, GameObject root)
        {
            sword1.name = AttackNames.ROUNDTRIPMINI;
            sword1.transform.parent = root.transform;
            GameObject.Destroy(sword1.GetComponent<RoundTripSword>());
            sword1.GetComponent<DamageEnemies>().nailDamageMultiplier = 0.1f;
            sword1.GetComponent<DamageEnemies>().stepsPerHit = 14;
            OrbitingRoundTripSword orbit = sword1.AddComponent<OrbitingRoundTripSword>();
            sword1.SetActive(true);
            sword1.GetComponent<tk2dSpriteAnimator>().Play("RoundTripEffect");
            sword1.GetComponent<tk2dSprite>().color = ColorConstants.DanteRed;

            return orbit;
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }


            FsmState Antic = fsm.AddState("ThunderClap Antic");
            Antic.AddMethod(_ =>
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
            });
            Antic.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("HotStuff"));
            Antic.AddWatchAnimationActionTrigger("FINISHED");

            FsmState Release = fsm.AddState("ThunderClap Release");
            Release.AddWatchAnimationAction("FINISHED");
            Release.AddMethod(_ =>
            {
                AudioClip? clip = (AudioClip?)EffectManager.Get(EffectManager.AudioClipNames.WITCHSLASH);
                HeroController.instance.GetComponent<AudioSource>().PlayOneShot(clip);

                CreateOrbs();
                StartCooldownTimer(COOLDOWN);
            });


            SetStateAsInit(Antic.name);
            Antic.AddTransition("FINISHED", Release.name);
            SetStateAsEnd(Release.name);
        }

        public override bool OnManualCooldown()
        {
            return false;
        }
    }
}
