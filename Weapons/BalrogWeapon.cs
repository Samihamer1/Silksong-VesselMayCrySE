using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.Attacks.Balrog;
using VesselMayCrySE.Attacks.KingCerberus;
using VesselMayCrySE.Attacks.Slashes;
using VesselMayCrySE.Attacks.Slashes.SlashComponents;

namespace VesselMayCrySE.Weapons
{
    internal class BalrogWeapon : WeaponBase
    {
        public BalrogWeapon() {
            try
            {
                CreateWeaponObjects();
            }
            catch (Exception ex)
            {
                VesselMayCrySEPlugin.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
            }

            name = WeaponNames.BALROG;
            weaponEvent = EventNames.BALROG;
            //dashSlash = new KingSlayer();
            //cycloneSlash = new Revolver();
            //greatSlash = new HotStuff();
            specialDownSlash = new BalrogDownslash(objectInfo.slashDownObject);
            specialDashStab = new BalrogDashstab(handler);

            //downSpell = new Swing(handler);
            //horizontalSpell = new ThunderClap(handler);
            //upSpell = new IceAge(handler);

            try
            {
                Initialise();
            }
            catch (Exception ex)
            {
                VesselMayCrySEPlugin.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
            }

            configGroup = objectInfo.getConfigGroup();

            HeroControllerConfig configRef = config.GetConfig();
            configRef.heroAnimOverrideLib = AnimationManager.GetBalrogAnimator();
            configRef.attackCooldownTime = 0f;
            configRef.attackDuration = 0.2f;
            configRef.attackRecoveryTime = 0f;

            configRef.dashStabTime = 0.175f;
            configRef.dashStabSpeed = -40f;
            configRef.dashStabBounceJumpSpeed = 18f;

            configRef.downSlashType = HeroControllerConfig.DownSlashTypes.Custom;
            configRef.downSlashEvent = "BALROG DOWNSLASH";
        }

        private void CreateWeaponObjects()
        {
            objectInfo = new CrestManagement.CrestObjectInfo();
            GameObject devilroot = CrestManager.devilroot;

            GameObject hornet = HeroController.instance.gameObject;
            GameObject attacks = hornet.Child("Attacks");
            GameObject hunter = attacks.Child("Default");

            AudioClip slashAudio = HeroController.instance.gameObject.Child("Attacks").Child("Wanderer").Child("Slash").GetComponent<AudioSource>().clip;

            //Regular slash
            GameObject balrogSlash = new GameObject("Balrog Slash");
            balrogSlash.transform.parent = devilroot.transform;
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
            slash1.CreateNailTravel(new Vector2(-2.5f, 0), 0.15f);
            slash1.SetOffsetVariance(new Vector2(0.2f, 0.75f));
            slash1.CreateRedSlash(new Vector3(0.3f, 0, 0));

            objectInfo.slashObject = balrogSlash;

            //Altslash
            GameObject balrogSlashAlt = new GameObject("Balrog SlashAlt");
            balrogSlashAlt.transform.parent = devilroot.transform;
            balrogSlashAlt.transform.localPosition = new Vector3(-1.3f, -0.05f, -0.001f);
            BalrogSlash slash2 = balrogSlashAlt.AddComponent<BalrogSlash>();
            slash2.SetScale(new Vector3(1f, 1f, 1));
            slash2.SetAnimation(AnimationManager.GetBalrogAnimator(), "SlashEffectAlt");
            slash2.SetNailDamageMultiplier(0.35f);
            slash2.SetStunDamage(0.1f);
            slash2.SetMagnitude(0);
            slash2.SetColliderSize("CerberusSlashAlt");
            slash2.SetAudioClip(slashAudio);
            slash2.RemoveRecoil();
            slash2.CreateNailTravel(new Vector2(-2.5f, 0), 0.15f);
            slash2.SetOffsetVariance(new Vector2(0.2f, 0.75f));
            slash2.CreateRedSlash(new Vector3(0.3f, 0, 0));

            objectInfo.slashAltObject = balrogSlashAlt;

            //Upslash
            GameObject balrogSlashUp = new GameObject("Balrog SlashUp");
            balrogSlashUp.transform.parent = devilroot.transform;
            balrogSlashUp.transform.localPosition = new Vector3(-0.75f, 0.6f, -0.001f);
            balrogSlashUp.transform.SetRotation2D(330);
            BalrogSlash slash3 = balrogSlashUp.AddComponent<BalrogSlash>();
            slash3.SetScale(new Vector3(1f, 1f, 1));
            slash3.SetAnimation(AnimationManager.GetBalrogAnimator(), "SlashEffectAlt");
            slash3.SetNailDamageMultiplier(0.35f);
            slash3.SetStunDamage(0.1f);
            slash3.SetMagnitude(0);
            slash3.SetColliderSize("CerberusSlashAlt");
            slash3.SetAudioClip(slashAudio);
            slash3.RemoveRecoil();
            slash3.CreateNailTravel(new Vector2(-2.55f, 1.5f), 0.15f);
            slash3.SetOffsetVariance(new Vector2(0.3f, 0.5f));
            slash3.CreateRedSlash(new Vector3(0.3f, 0, 0));

            objectInfo.slashUpObject = balrogSlashUp;

            //Downslash
            Vector2 downTravelVec = new Vector2(
                -0.7f, -0.7f
            ) * 4;

            GameObject balrogSlashDown = new GameObject("Balrog SlashDown");
            balrogSlashDown.transform.parent = devilroot.transform;
            balrogSlashDown.transform.localPosition = new Vector3(-0.3f, -1.4f, -0.001f);
            balrogSlashDown.transform.SetRotation2D(45);
            BalrogSlash slash4 = balrogSlashDown.AddComponent<BalrogSlash>();
            slash4.SetScale(new Vector3(1f, 1f, 1));
            slash4.SetAnimation(AnimationManager.GetBalrogAnimator(), "SlashEffectAlt");
            slash4.SetNailDamageMultiplier(0.35f);
            slash4.SetStunDamage(0.1f);
            slash4.SetMagnitude(0);
            slash4.SetDownAttack();
            slash4.SetDamageDirection(270f);
            slash4.SetColliderSize("CerberusSlashAlt");
            slash4.SetAudioClip(slashAudio);
            slash4.CreateNailTravel(downTravelVec, 0.15f);
            slash4.SetOffsetVariance(new Vector2(0.2f, 0.25f));
            slash4.CreateRedSlash(new Vector3(0.3f, 0, 0));

            objectInfo.slashDownObject = balrogSlashDown;
        }
    }
}
