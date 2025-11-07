using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.Components.ObjectComponents;

namespace VesselMayCrySE.EffectHandler.GameObjects
{
    internal class RoundTripObjectEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            GameObject sword = new GameObject(AttackNames.ROUNDTRIP);
            sword.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator = sword.AddComponent<tk2dSpriteAnimator>();
            animator.library = AnimationManager.GetDevilSwordAnimator();
            animator.playAutomaticallyOnEnable = true;
            animator.defaultClipId = AnimationManager.GetDevilSwordAnimator().GetClipIdByName("RoundTripEffect");
            BoxCollider2D collider = sword.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            DamageEnemies dmg = sword.AddComponent<DamageEnemies>();
            dmg.useNailDamage = true;
            dmg.nailDamageMultiplier = 0.15f;
            dmg.multiHitter = true;
            dmg.multiHitFirstEffects = EnemyHitEffectsProfile.EffectsTypes.Minimal;
            dmg.multiHitEffects = EnemyHitEffectsProfile.EffectsTypes.Minimal;
            dmg.magnitudeMult = 0f;
            dmg.stepsPerHit = 8;
            dmg.damageMultPerHit = new float[0];
            dmg.sourceIsHero = true;
            dmg.isNailAttack = true;
            dmg.attackType = AttackTypes.Nail;
            dmg.corpseDirection = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.corpseMagnitudeMult = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.currencyMagnitudeMult = new TeamCherry.SharedUtils.OverrideFloat();
            dmg.slashEffectOverrides = new GameObject[0];
            dmg.DealtDamage = new UnityEngine.Events.UnityEvent();
            dmg.damageFSMEvent = "";
            dmg.dealtDamageFSMEvent = "";
            dmg.stunDamage = 0.1f;
            dmg.isHeroDamage = true;
            dmg.silkGeneration = HitSilkGeneration.None;

            Rigidbody2D rigidbody = sword.AddComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            rigidbody.SetVelocity(0, 0);

            sword.transform.parent = HeroController.instance.transform;

            sword.AddComponent<RoundTripSword>();

            sword.SetActive(false);

            return sword;
        }
    }
}
