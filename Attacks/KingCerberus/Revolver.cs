using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections;
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
    internal class Revolver : BaseNailArt
    {
        private GameObject revolverObject;
        private GameObject redRevolverObject;

        private GameObject CreateRedRevolver()
        {
            GameObject revolver = CreateRevolver();
            revolver.name = AttackNames.REDSLASH;

            StandardSlash slash = revolver.GetComponent<StandardSlash>();
            slash.SetColor(ColorConstants.DanteRed);
            slash.SetNailDamageMultiplier(0.3f);
            slash.SetMagnitude(0);
            slash.SetStunDamage(0.3f);

            revolver.transform.localPosition = new Vector3(-3f, 0, 0);

            return revolver;
        }

        private GameObject CreateIceRevolver()
        {
            GameObject revolver = CreateRevolver();
            revolver.AddComponent<IceSlash>();
            return revolver;
        }

        private GameObject CreateRevolver()
        {
            GameObject revolverObject = new GameObject(AttackNames.REVOLVER);
            revolverObject.transform.parent = CrestManager.devilroot.transform;
            revolverObject.transform.localPosition = new Vector3(0,0, -0.01f);

            StandardSlash slash1 = revolverObject.AddComponent<StandardSlash>();
            slash1.SetScale(new Vector3(-1f, 1, 1));
            slash1.SetAnimation(AnimationManager.GetKingCerberusAnimator(), "RevolverEffect");
            slash1.SetColliderSize("CerberusRevolver");

            DamageEnemies dmg = revolverObject.GetComponent<DamageEnemies>();
            dmg.multiHitter = true;
            dmg.nailDamageMultiplier = 0.6f;
            dmg.stunDamage = 0.5f;
            dmg.silkGeneration = HitSilkGeneration.FirstHit;
            dmg.stepsPerHit = 7;
            dmg.magnitudeMult = 0;

            //audio
            AudioSource audio = revolverObject.GetComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.clip = (AudioClip?)EffectManager.Get(EffectManager.AudioClipNames.WITCHSLASH);
            audio.volume = 0.5f;

            return revolverObject;
        }

        private void CheckChaser()
        {
            DevilCrestHandler handler = HeroController.instance.gameObject.GetComponent<DevilCrestHandler>();
            if (handler == null) { return; }
            if (!handler.ConsumeChaserBlade()) { return; }

            if (redRevolverObject == null) { return; }
            redRevolverObject.GetComponent<NailSlash>().StartSlash();
        }

        private IEnumerator ReleaseAttack()
        {
            if (revolverObject == null ) { yield break; }

            revolverObject.GetComponent<NailSlash>().StartSlash();

            CheckChaser();

            AudioSource audio = revolverObject.GetComponent<AudioSource>();

            float time = 0.5f * 2/3;

            for (int i = 0; i < 2; i++)
            {
                if (revolverObject == null) { yield break; }

                audio.pitch = UnityEngine.Random.Range(1f, 1.2f);
                audio.Play();
                yield return new WaitForSeconds(time / 2);
            }

            fsm.SendEvent("REVOLVER END");
            revolverObject.GetComponent<NailSlash>().CancelAttack();

            if (redRevolverObject != null)
            {
                redRevolverObject.GetComponent<NailSlash>().CancelAttack();
            }

            yield return new WaitForSeconds(0.3f); //For the audio to finish
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            revolverObject = CreateIceRevolver();
            redRevolverObject = CreateRedRevolver();

            FsmState revolverState = fsm.AddState("Revolver");
            revolverState.AddMethod(_ => {
                Vector2 velocity = HeroController.instance.GetComponent<Rigidbody2D>().linearVelocity;
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.GetComponent<Rigidbody2D>().SetVelocity(velocity.x, velocity.y);

                try
                {
                    GameManager.instance.StartCoroutine(ReleaseAttack());
                }
                catch (Exception ex)
                {
                    VesselMayCrySEPlugin.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
                }
            });
            revolverState.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("Revolver"));
            revolverState.AddAction(new DecelerateXY { decelerationX = 0.85f, decelerationY = 0.8f, gameObject = Helper.GetHornetOwnerDefault() });

            FsmState? CancelAllState = fsm.GetState("Cancel All");
            if (CancelAllState != null)
            {
                CancelAllState.AddMethod(_ =>
                {
                    if (revolverObject == null) { return; }
                    revolverObject.GetComponent<NailSlash>().CancelAttack();
                    if (redRevolverObject == null) { return; }
                    redRevolverObject.GetComponent<NailSlash>().CancelAttack();
                });
            }


            SetStateAsInit(revolverState.name);
            revolverState.AddTransition("REVOLVER END", "Set Finished");
        }
    }
}
