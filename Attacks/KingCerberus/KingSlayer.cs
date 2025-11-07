using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.Components;
using VesselMayCrySE.EffectHandler;
using static AudioSourceMovingClips;

namespace VesselMayCrySE.Attacks.KingCerberus
{
    internal class KingSlayer : BaseNailArt
    {
        private GameObject explodePrefab;

        private GameObject spawnedExplodePrefab;
        public KingSlayer() : base()
        {
            name = "KingSlayer";
        }

        private FsmState? GetRodExplodeState()
        {
            PlayMakerFSM toolAttacks = HeroController.instance.gameObject.LocateMyFSM("Tool Attacks");
            if (toolAttacks == null) { VesselMayCrySEPlugin.Instance.LogError("Tool Attacks FSM not found"); return null; }

            FsmState? throwstate = toolAttacks.GetState("LR Throw");
            if (throwstate == null) { VesselMayCrySEPlugin.Instance.LogError("LR Throw state not found"); return null; }

            SpawnProjectileV2? spawnproj = throwstate.GetFirstActionOfType<SpawnProjectileV2>();
            if (spawnproj == null) { VesselMayCrySEPlugin.Instance.LogError("SpawnProjectileV2 not found in LR Throw state"); return null; }

            GameObject rodprefab = spawnproj.Prefab.value;

            if (rodprefab == null) { VesselMayCrySEPlugin.Instance.LogError("Lightning Rod not found in SpawnProjectileV2"); return null; }

            PlayMakerFSM control = rodprefab.LocateMyFSM("Control");
            if (control == null) { VesselMayCrySEPlugin.Instance.LogError("Control FSM not found in lightning rod"); return null; }

            FsmState? explodeState = control.GetState("Explode");
            if (explodeState == null) { VesselMayCrySEPlugin.Instance.LogError("Explode state not found in Control state"); return null ; }

            return explodeState;
        }

        private void CreateObjects()
        {
            FsmState? explodeState = GetRodExplodeState();
            if (explodeState == null) { return; }

            SpawnObjectFromGlobalPool action = explodeState.GetAction<SpawnObjectFromGlobalPool>(0);

            explodePrefab = action.gameObject.value;
        }

        private void ModifyExplosion(GameObject explosion)
        {
            explosion.transform.SetScale2D(new Vector2(0.35f, 0.6f));

            GameObject subexplosion = explosion.Child("lightning_rod_explode");

            //Pt cast (the lightning particle effects)
            GameObject ptCast = subexplosion.Child("Pt Cast");
            ptCast.SetActive(false);

            //Glow
            GameObject glow = subexplosion.Child("glow");
            glow.SetActive(false);

            //Bolt top (lightning effect)
            GameObject boltTop = subexplosion.Child("bolt_top");
            boltTop.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

            GameObject boltTop1 = subexplosion.Child("bolt_top (1)");
            boltTop1.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

            //camera shake removal
            GameObject.Destroy(subexplosion.GetComponent<CameraControlAnimationEvents>());

            GameObject.Destroy(explosion.GetComponent<AutoRecycleSelf>());

            //Name
            subexplosion.name = AttackNames.KINGSLAYER;

            //damager
            GameObject damager = subexplosion.Child("damager");
            DamageEnemies dmg = damager.GetComponent<DamageEnemies>();
            dmg.multiHitter = false;
            dmg.useNailDamage = true;
            dmg.nailDamageMultiplier = 0.4f;
            dmg.stunDamage = 0.3f;
            dmg.ignoreInvuln = true;
            dmg.isHeroDamage = true;
            dmg.sourceIsHero = true;
            dmg.doesNotTink = true;

            //Audio
            AudioSource audio = subexplosion.AddComponent<AudioSource>();
            audio.outputAudioMixerGroup = HeroController.instance.gameObject.GetComponent<AudioSource>().outputAudioMixerGroup;
            audio.playOnAwake = true;
            audio.clip = (AudioClip?)EffectManager.Get(EffectManager.AudioClipNames.SILKSPEARZAP);
            audio.pitch = UnityEngine.Random.Range(1.1f, 1.4f);
        }

        private GameObject CreateExplosion(Vector3 location)
        {
            if (spawnedExplodePrefab == null) { return new GameObject(); }
            GameObject explosion = GameObject.Instantiate(spawnedExplodePrefab);
            ModifyExplosion(explosion);
            explosion.transform.position = location;
            //explosion.transform.position += new Vector3(0, 5.5f, 0);
            GameManager.instance.subbedCamShake.DoShake(GlobalSettings.Camera.SmallShake, explosion, false);

            explosion.SetActive(true);
            return explosion;
        }

        private GameObject CreateRedExplosion(Vector3 location)
        {
            GameObject explosion = CreateExplosion(location);
            explosion.name = AttackNames.REDSLASH;

            TintRendererGroup tint = explosion.AddComponent<TintRendererGroup>();
            tint.GetComponentsInChildrenRecursively(explosion.transform);
            tint.color = ColorConstants.DanteRed;
            tint.UpdateTint();

            DamageEnemies dmg = explosion.GetComponentInChildren<DamageEnemies>();
            if (dmg != null)
            {
                dmg.nailDamageMultiplier = 0.2f;
                dmg.stunDamage = 0.1f;
            }

            return explosion;
        }

        private IEnumerator ReleaseChaserAttack()
        {
            yield break;
            DevilCrestHandler handler = HeroController.instance.gameObject.GetComponent<DevilCrestHandler>();
            if (handler == null) { yield break; }
            if (!handler.ConsumeChaserBlade()) { yield break; }

           
        }

        private IEnumerator ReleaseAttack()
        {
            float offset = 2.5f * -HeroController.instance.gameObject.transform.GetScaleX();
            Vector3 loc = HeroController.instance.transform.position + new Vector3(offset,0,0);

            GameObject e1 = CreateExplosion(loc);
            e1.transform.position += new Vector3(offset, 0, 0);
            yield return new WaitForSeconds(0.3f);

            GameObject e2 = CreateExplosion(loc);
            e2.transform.position += new Vector3(2*offset, 0, 0);
            yield return new WaitForSeconds(0.3f);

            GameObject e3 = CreateExplosion(loc);
            e3.transform.position += new Vector3(3*offset, 0, 0);

            yield return new WaitForSeconds(0.5f);
            GameObject.Destroy(e1);
            GameObject.Destroy(e2);
            GameObject.Destroy(e3);
        }

        public override void CreateAttack()
        {
            if (fsm == null) { return; }

            CreateObjects();

            FsmGameObject storedExplosion = fsm.GetGameObjectVariable("King Slayer Stored Explosion");

            FsmState setupState = fsm.AddState("King Slayer Setup");
            setupState.AddMethod(_ =>
            {
                if (spawnedExplodePrefab == null)
                {
                    fsm.SendEvent("SETUP");
                }
            });

            FsmState getExplosionState = fsm.AddState("King Slayer Get Explosion");
            getExplosionState.AddAction(new SpawnObjectFromGlobalPool { gameObject = explodePrefab, spawnPoint = HeroController.instance.gameObject, position = new Vector3(), rotation = new Vector3(), storeObject = storedExplosion });
            getExplosionState.AddMethod(_ =>
            {
                storedExplosion.value.SetActive(false);
                GameObject.DontDestroyOnLoad(storedExplosion.value);
                spawnedExplodePrefab = storedExplosion.value;
                spawnedExplodePrefab.transform.parent = null;
            });

            FsmState anticState = fsm.AddState("King Slayer Antic");
            anticState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.GetComponent<SpriteFlash>().flashFocusHeal();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.AffectedByGravity(false);
                HeroController.instance.RelinquishControl();
            });
            anticState.AddAnimationAction(AnimationManager.GetKingCerberusAnimator().GetClipByName("King Slayer Antic"));
           

            FsmState explodeState = fsm.AddState("King Slayer Explode");
            explodeState.AddMethod(_ =>
            {
                AudioClip? clip = (AudioClip?)EffectManager.Get(EffectManager.AudioClipNames.WITCHSLASH);
                HeroController.instance.GetComponent<AudioSource>().PlayOneShot(clip);
                GameManager.instance.StartCoroutine(ReleaseAttack());
                GameManager.instance.StartCoroutine(ReleaseChaserAttack());
                
            });
            explodeState.AddAction(new DecelerateV2 { deceleration = 0.85f, brakeOnExit = true, gameObject = Helper.GetHornetOwnerDefault() });
            explodeState.AddWatchAnimationAction("FINISHED");


            setupState.AddTransition("SETUP", getExplosionState.name);
            setupState.AddTransition("FINISHED", anticState.name);
            getExplosionState.AddTransition("FINISHED", anticState.name);
            anticState.AddTransition("FINISHED", explodeState.name);
            explodeState.AddTransition("FINISHED", "Set Finished");
            SetStateAsInit(setupState.name);
        }
    }
}
