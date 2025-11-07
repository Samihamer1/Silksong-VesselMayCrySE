using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.Attacks.Slashes.SlashComponents
{
    internal class FireSlash : MonoBehaviour
    {
        private PlayParticleEffects? flameParticles;
        private NailAttackBase? slash;

        private void Start()
        {
            slash = GetComponent<NailAttackBase>();
            if (slash == null) { VesselMayCrySEPlugin.Instance.LogError($"NailAttackBase or inheriting class not found in {gameObject.name}. Can't create FireSlash."); return; }

            slash.AttackStarting += ActivateFlame;
            slash.EndedDamage += DeactivateFlame;

            DamageEnemies dmg = GetComponent<DamageEnemies>();
            if (dmg == null) { return; }

            float multiplier = HeroController.instance.NailImbuement.nailConfigs[(int)NailElements.Fire].NailDamageMultiplier;

            dmg.nailDamageMultiplier = dmg.nailDamageMultiplier / multiplier; // So that damage is kept the same despite the multiplier.
        }

        private void OnDestroy()
        {
            if (slash == null) { return; }

            slash.AttackStarting -= ActivateFlame;
            slash.EndedDamage -= DeactivateFlame;
        }

        private void DeactivateFlame(bool obj)
        {
            if (flameParticles != null)
            {
                flameParticles.StopParticleSystems();
            }

            //light group
            HeroController.instance.NailImbuement.imbuedHeroLightGroup.FadeToZero(0.2f);
        }

        private void ActivateFlame()
        {
            //Color
            NailImbuementConfig config = HeroController.instance.NailImbuement.nailConfigs[(int)NailElements.Fire];

            gameObject.GetComponent<NailAttackBase>().SetNailImbuement(config, NailElements.Fire);


            //Effects
            flameParticles = config.HeroParticles.Spawn(HeroController.instance.transform.position);
            flameParticles.PlayParticleSystems();

            //light group
            HeroController.instance.NailImbuement.imbuedHeroLightGroup.FadeToOne(0.2f);
            HeroController.instance.NailImbuement.imbuedHeroLightRenderer.color = config.ExtraHeroLightColor;

        }
    }
}
