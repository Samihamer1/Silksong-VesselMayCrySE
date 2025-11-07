using GlobalSettings;
using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using VesselMayCrySE.Components;

namespace VesselMayCrySE.Attacks.KingCerberus
{
    internal class CerberusDashStab : BaseDashStab
    {
        private GameObject dashStabObject;
        private PlayParticleEffects flameParticles;
        public CerberusDashStab(DevilCrestHandler crestHandler, GameObject slashObject) : base(crestHandler)
        {
            dashStabObject = slashObject;

            DashStabNailAttack dashstab = dashStabObject.GetComponent<DashStabNailAttack>();

        }

        public override void CreateAttack()
        {
            if (fsm == null) { VesselMayCrySEPlugin.Instance.LogError("Sprint FSM not found when creating CerberusDashStab"); return; }

            #region Start Attack state
            FsmState? SetAttackSingleState = fsm.GetState("Set Attack Single");
            if (SetAttackSingleState == null) { VesselMayCrySEPlugin.Instance.LogError("Set Attack Single state not found in SprintFSM. Incompatibility with other mod?"); return; }
            FsmGameObject objectVariable = fsm.GetGameObjectVariable("Dash Stab Crt");

            SetAttackSingleState.AddMethod(_ =>
            {
                if (!handler.IsDevilEquipped()) { return; }
                if (!handler.isWeaponEquipped(WeaponNames.KINGCERBERUS)) { return; }

                objectVariable.value = dashStabObject;
            });
            #endregion
        }
    }
}
