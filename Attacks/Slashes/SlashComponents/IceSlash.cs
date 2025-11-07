using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;
using VesselMayCrySE.EffectHandler;

namespace VesselMayCrySE.Attacks.Slashes.SlashComponents
{
    internal class IceSlash : MonoBehaviour
    {
        private PlayParticleEffects? frostParticles;
        private PlayParticleEffects? spawnedFrostParticles;
        private NailAttackBase? slash;
        private NailImbuementConfig? frostConfig;

        private void CreateFrost()
        {
            GameObject? effects = HeroController.instance.gameObject.Child("Effects");

            if (effects == null) { return; }

            NailImbuementConfig config = Instantiate(HeroController.instance.NailImbuement.nailConfigs[(int)NailElements.Fire]);
            config.name = "Ice Nail";
            config.ToolSource = null;

            //Color
            config.ExtraHeroLightColor = new Color(0.65f, 0.75f, 1, 1);
            config.NailTintColor = new Color(0.65f, 0.75f, 1, 1);

            
            //Particles
            GameObject? FrostAnticPt = (GameObject?)EffectManager.Get(EffectManager.GameObjectNames.FROSTANTIC);
            if (FrostAnticPt != null)
            {
                GameObject frost = GameObject.Instantiate(FrostAnticPt);
                frost.transform.parent = effects.transform.parent;
                frost.transform.localPosition = Vector3.zero;
                frost.name = "SlashFrost";

                frostParticles = frost.GetComponent<PlayParticleEffects>();
            }


            //Audio
            AudioClip? clip = (AudioClip?)EffectManager.Get(EffectManager.AudioClipNames.FROSTOVERLAY);
            if (clip != null)
            {                
                config.ExtraSlashAudio = new AudioEvent
                {
                    Clip = clip,
                    PitchMax = 1.05f,
                    PitchMin = 0.95f,
                    Volume = 1,
                    vibrationDataAsset = null
                };
            }


            //Slash effect
            GameObject? slasheffect = (GameObject?)EffectManager.Get(EffectManager.GameObjectNames.FROSTSLASHEFFECT);
            if (slasheffect != null)
            {
                config.SlashEffect = slasheffect;
            }

            //Hit effect
            GameObject? hiteffect = (GameObject?)EffectManager.Get(EffectManager.GameObjectNames.FROSTHITEFFECT);
            if (hiteffect != null)
            {
                config.StartHitEffect.FullEffects.SpawnEffectPrefabs = new GameObject[] { hiteffect };
                config.StartHitEffect.MinimalEffects.SpawnEffectPrefabs = new GameObject[] { hiteffect };
                config.InertHitEffect.FullEffects.SpawnEffectPrefabs = new GameObject[] { hiteffect };
                config.InertHitEffect.MinimalEffects.SpawnEffectPrefabs = new GameObject[] { hiteffect };
            }

            //Tag
            config.LuckyHitChance = 0;
            config.HitsToTag = new MinMaxInt { End = 99, Start = 99 };

            frostConfig = config;

        }


        private void Start()
        {
            slash = GetComponent<NailAttackBase>();
            if (slash == null) { VesselMayCrySEPlugin.Instance.LogError($"NailAttackBase or inheriting class not found in {gameObject.name}. Can't create FireSlash."); return; }

            CreateFrost();

            if (frostConfig == null)
            {
                return;
            }

            slash.AttackStarting += ActivateFrost;
            slash.EndedDamage += DeactivateFrost;

            DamageEnemies dmg = GetComponent<DamageEnemies>();
            if (dmg == null) { return; }

            float multiplier = frostConfig.NailDamageMultiplier;

            dmg.nailDamageMultiplier = dmg.nailDamageMultiplier / multiplier; // So that damage is kept the same despite the multiplier.
        }

        private void OnDestroy()
        {
            if (slash == null) { return; }
            if (frostConfig == null) { return; }

            slash.AttackStarting -= ActivateFrost;
            slash.EndedDamage -= DeactivateFrost;
        }

        private IEnumerator DestroyFrost(PlayParticleEffects? fx)
        {
            if (fx == null) { yield break; }
            yield return new WaitForSeconds(0.5f);
            fx.StopParticleSystems();
        }
 
        private void DeactivateFrost(bool obj)
        {
            GameManager.instance.StartCoroutine(DestroyFrost(frostParticles));

            //light group
            HeroController.instance.NailImbuement.imbuedHeroLightGroup.FadeToZero(0.2f);
        }

        private void ActivateFrost()
        {
            if (frostConfig == null) { return; }

            //Color
            NailImbuementConfig config = frostConfig;

            gameObject.GetComponent<NailAttackBase>().SetNailImbuement(config, NailElements.None);


            //Effects
            if (frostParticles != null)
            {
                frostParticles.PlayParticleSystems();
            }

            //light group
            HeroController.instance.NailImbuement.imbuedHeroLightGroup.FadeToOne(0.2f);
            HeroController.instance.NailImbuement.imbuedHeroLightRenderer.color = config.ExtraHeroLightColor;

        }
    }
}
