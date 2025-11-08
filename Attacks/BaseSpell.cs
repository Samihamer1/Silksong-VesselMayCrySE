using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VesselMayCrySE.Components;

namespace VesselMayCrySE.Attacks
{
    public abstract class BaseSpell : BaseAttack
    {
        public DevilCrestHandler handler;
        public string EVENTNAME;
        private FsmEvent GLOBALEVENT;
        private float cooldown = 0;
        private float lastUsedTime = 0;
        public Sprite? ICON;
        public Sprite? ICONGLOW;

        public BaseSpell(DevilCrestHandler handler) {
            if (HeroController.instance == null) { return; }
            PlayMakerFSM spellfsm = HeroController.instance.gameObject.LocateMyFSM("Silk Specials");
            fsm = spellfsm;
            EVENTNAME = "";

            this.handler = handler;
        }

        public abstract bool OnManualCooldown();

        public void StartCooldownTimer(float cooldown)
        {
            lastUsedTime = Time.time;
            this.cooldown = cooldown;
        }

        public bool OnCooldown()
        {
            //Addon for if you don't have the silk for the skill in non Easy mode
            if (handler.GetCurrentDifficulty() != Difficulties.DevilDifficulty.EASY && HeroController.instance.playerData.silk == 0)
            {
                return true;
            }

            //Time check
            float currentTime = Time.time;

            if (currentTime-lastUsedTime >= cooldown)
            {
                return false;
            }

            return true;
        }

        public FsmEvent GetGlobalEvent()
        {
            return GLOBALEVENT;
        }

        /// <summary>
        /// Creates a global transition to the given state name using the EVENTNAME variable.
        /// </summary>
        /// <param name="statename"></param>
        public void SetStateAsInit(string statename)
        {
            GLOBALEVENT = fsm.AddGlobalTransition(EVENTNAME, statename);            
        }

        /// <summary>
        /// Adds a transition to 'Special End' from the given state name.
        /// </summary>
        /// <param name="statename"></param>
        public void SetStateAsEnd(string statename)
        {
            fsm.AddTransition(statename, "FINISHED", "Special End");
        }
    }
}
