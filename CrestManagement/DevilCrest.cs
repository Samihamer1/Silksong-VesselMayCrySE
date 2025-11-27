using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using UnityEngine;
using VesselMayCrySE.Attacks.DevilSword;
using GlobalEnums;
using VesselMayCrySE.Weapons;
using VesselMayCrySE.Components;
using System;

namespace VesselMayCrySE.CrestManagement
{
    public class DevilCrest : CustomCrest
    {
        private bool wasSprinting = false;

        public void SetupWeapon()
        {
            PlayMakerFSM nailartFSM = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
            if (nailartFSM == null) { VesselMayCrySEPlugin.Instance.LogError("VMCSE - Nail Arts not found"); return; }

            PlayMakerFSM crestAttackFSM = HeroController.instance.gameObject.LocateMyFSM("Crest Attacks");
            if (crestAttackFSM == null) { VesselMayCrySEPlugin.Instance.LogError("VMCSE - Crest Attacks not found"); return; }

            PlayMakerFSM sprintFSM = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            if (sprintFSM == null) { VesselMayCrySEPlugin.Instance.LogError("VMCSE - Sprint not found."); return; }

            ReviveNailArts(nailartFSM);
            PatchSprintFSM(sprintFSM);

            HeroController.instance.gameObject.AddComponent<DevilCrestHandler>();
        }

        private void PatchSprintFSM(PlayMakerFSM sprintFSM)
        {
            FsmState StartAttack = sprintFSM.GetState("Start Attack");
            StartAttack.InsertMethod(0, _ =>
            {
                //I have problems with using return here sometimes, so I'll go with the less readable variant for functionality's sake.
                if (DevilCrestHandler.Instance != null) { 
                    if (DevilCrestHandler.Instance.IsDevilEquipped())
                    {
                        sprintFSM.SendEvent("DEVIL");
                    }
                }
            });

            FsmState DevilAttack = sprintFSM.AddState("Devil Attack Select");
            DevilAttack.AddMethod(_ =>
            {
                sprintFSM.GetBoolVariable("Did First Slash").value = false;
                sprintFSM.GetBoolVariable("In Attack").value = true;
                HeroController.instance.IncrementAttackCounter();
                HeroController.instance.SetCState("isSprinting", false);

                SendWeaponEvent(sprintFSM);
            });

            StartAttack.AddTransition("DEVIL", DevilAttack.name);
            DevilAttack.AddTransition("FINISHED", "Set Attack Single"); //Just in case of failure
        }

        private void ReviveNailArts(PlayMakerFSM nailartFSM)
        {
            FsmState TakeControlState = nailartFSM.GetState("Can Nail Art?");
            TakeControlState.AddMethod(_ =>
            {
                wasSprinting = HeroController.instance.cState.isSprinting;
            });

            FsmEventTarget nailartTarget = new FsmEventTarget() { target = FsmEventTarget.EventTarget.FSMComponent, fsmComponent = nailartFSM };

            FsmState AnticTypeState = nailartFSM.GetState("Antic Type");
            AnticTypeState.AddMethod(_ =>
            {
                if (HeroController.instance.playerData.CurrentCrestID == "Devil")
                {
                    nailartFSM.SendEvent("DEVIL");
                }
            });

            FsmState SprintCheckState = nailartFSM.AddState("Sprinting?");
            SprintCheckState.AddMethod(_ =>
            {
                if (wasSprinting || nailartFSM.GetFsmBoolIfExists("dashing") || GameManager.instance.inputHandler.inputActions.Dash.IsPressed)
                {
                    nailartFSM.SendEvent("DASHSLASH");
                }
            });


            FsmState CycloneCheckState = nailartFSM.AddState("Holding Up?");
            CycloneCheckState.AddMethod(_ =>
            {
                if (GameManager.instance.inputHandler.inputActions.Up.IsPressed || GameManager.instance.inputHandler.inputActions.Down.IsPressed)
                {
                    nailartFSM.SendEvent("CYCLONESLASH");
                }
            });
            

            FsmState GreatSlashChoiceState = nailartFSM.AddState("Great Slash Choice");
            GreatSlashChoiceState.AddMethod(_ =>
            {
                SendWeaponEvent(nailartFSM);
            });
            FsmState CycloneSlashChoiceState = nailartFSM.AddState("Cyclone Slash Choice");
            CycloneSlashChoiceState.AddMethod(_ =>
            {
                SendWeaponEvent(nailartFSM);
            });
            FsmState DashSlashChoiceState = nailartFSM.AddState("Dash Slash Choice");
            DashSlashChoiceState.AddMethod(_ =>
            {
                SendWeaponEvent(nailartFSM);
            });

            //transitions
            AnticTypeState.AddTransition("DEVIL", SprintCheckState.name);
            SprintCheckState.AddTransition("DASHSLASH", DashSlashChoiceState.name);
            SprintCheckState.AddTransition("FINISHED", CycloneCheckState.name);
            CycloneCheckState.AddTransition("CYCLONESLASH", CycloneSlashChoiceState.name);
            CycloneCheckState.AddTransition("FINISHED", GreatSlashChoiceState.name);
            GreatSlashChoiceState.AddTransition("FINISHED", "Antic");
            CycloneSlashChoiceState.AddTransition("FINISHED", "Antic");
            DashSlashChoiceState.AddTransition("FINISHED", "Antic");
        }

        private void SendWeaponEvent(PlayMakerFSM playmakerFSM)
        {
            if (HeroController.instance == null) { return; }
            DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
            if (handler == null) { return; }
            playmakerFSM.SendEvent(handler.getEquippedWeapon().weaponEvent);
        }
    }
}
