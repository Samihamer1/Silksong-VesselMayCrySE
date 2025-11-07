using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.EffectHandler.GameObjects
{
    internal class ThunderExplosionObjectEffect : BaseEffect
    {
        public override UnityEngine.Object? TryInitialiseObject()
        {
            ToolItemManager manager = ToolItemManager.Instance;
            if (manager == null) { return null; }

            ToolItem rod = manager.toolItems.GetByName("Lightning Rod");
            if (rod == null) { return null; }
            ;

            if (!(rod is ToolItemToggleState)) { return null; }

            ToolItemToggleState togglerod = (ToolItemToggleState)rod;

            GameObject orbprefab = togglerod.onState.Usage.ThrowPrefab;
            if (orbprefab == null) { return null; }

            PlayMakerFSM control = orbprefab.LocateMyFSM("Control");
            if (control == null) { return null; }

            FsmState? wallState = control.GetState("Wall L");
            if (wallState == null) { return null; }

            FlingObjectsFromGlobalPool? fling = wallState.GetFirstActionOfType<FlingObjectsFromGlobalPool>();
            if (fling == null) { return null; }

            PlayMakerFSM bolacontrol = fling.gameObject.value.gameObject.LocateMyFSM("Control");
            if (bolacontrol == null) { return null; }

            FsmState? explodeState = bolacontrol.GetState("Explode");
            if (explodeState == null) { return null; }

            SpawnObjectFromGlobalPool? spawn = explodeState.GetFirstActionOfType<SpawnObjectFromGlobalPool>();
            if (spawn == null) { return null; }

            return spawn.gameObject.value;
        }
    }
}
