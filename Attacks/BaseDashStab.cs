using HutongGames.PlayMaker;
using Silksong.FsmUtil;
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
        public string? FSMEVENT;
        public BaseDashStab(DevilCrestHandler crestHandler)
        {
            handler = crestHandler;
            if (HeroController.instance == null) { return; }
            PlayMakerFSM sprintfsm = HeroController.instance.gameObject.LocateMyFSM("Sprint");
            fsm = sprintfsm;
        }

        /// <summary>
        /// Adds a transition to 'Regain Control Normal' from the given state name.
        /// </summary>
        /// <param name="statename"></param>
        public void SetStateAsEnd(string statename)
        {
            fsm.AddTransition(statename, "FINISHED", "Regain Control Normal");
        }

        /// <summary>
        /// Adds a transition from 'Devil Attack Select' to the given state name using the weapon event
        /// </summary>
        /// <param name="statename"></param>
        public void SetStateAsInit(string statename)
        {
            if (DevilCrestHandler.Instance == null) { return; }
            if (FSMEVENT == null) { return; }
            fsm.AddTransition("Devil Attack Select", FSMEVENT, statename);
        }
    }
}
