using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VesselMayCrySE.Attacks
{
    public abstract class BaseCrestAttack : BaseAttack
    {
        public GameObject? attackObject;
        public string? FSMEVENT;
        public BaseCrestAttack(GameObject attackObject)
        {
            PlayMakerFSM crestAttackFSM = HeroController.instance.gameObject.LocateMyFSM("Crest Attacks");
            if (crestAttackFSM == null) { VesselMayCrySEPlugin.Instance.LogError("Crest Attacks FSM not found"); return; }
            fsm = crestAttackFSM;

            this.attackObject = attackObject;
        }

        /// <summary>
        /// Adds a transition to 'End' from the given state name.
        /// </summary>
        /// <param name="statename"></param>
        public void SetStateAsEnd(string statename)
        {
            fsm.AddTransition(statename, "FINISHED", "End");
        }

        /// <summary>
        /// Adds a transition from 'Idle' to the given state name.
        /// </summary>
        /// <param name="statename"></param>
        public void SetStateAsInit(string statename)
        {
            if (FSMEVENT == null) { return; }
            fsm.AddTransition("Idle", FSMEVENT, statename);
        }
    }
}