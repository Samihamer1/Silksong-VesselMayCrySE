using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;
using VesselMayCrySE.Components;

namespace VesselMayCrySE.Attacks
{
    public abstract class BaseDashStab : BaseAttack
    {
        public DevilCrestHandler handler;
        public BaseDashStab(DevilCrestHandler crestHandler)
        {
            handler = crestHandler;
            if (HeroController.instance == null) { return; }
            PlayMakerFSM sprintfsm = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            fsm = sprintfsm;

        }
    }
}
