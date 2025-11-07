using HarmonyLib;
using Mono.Security.Authenticode;
using System;
using System.Collections.Generic;
using System.Reflection;
using TeamCherry.Localization;
using UnityEngine;
using VesselMayCrySE.AnimationHandler;
using VesselMayCrySE.Components;
using VesselMayCrySE.EffectHandler;
using VesselMayCrySE.UIHandler;

namespace VesselMayCrySE;

[HarmonyPatch]
public static class Patches
{
    //This patch is specifically because I use resources from runtime, mainly for HeroController.instance.
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.Start))]
    [HarmonyPostfix]
    private static void Postfix()
    {
        try
        {
            ResourceLoader.InitOnHeroLoad();
            AnimationManager.InitAnimations();
            EffectManager.Initialise();
            CrestManager.AddCrest();
        }
        catch (Exception ex)
        {
            VesselMayCrySEPlugin.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
        }
        
    }

    //Nail Art charge time patch
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.instance.CurrentNailChargeTime), MethodType.Getter)]
    [HarmonyPostfix]
    private static void ModifyChargeTime(ref float __result)
    {
        if (HeroController.instance.playerData.CurrentCrestID == "Devil")
        {
            __result = 0.45f;
        }        
    }

    //Nail Art charge begin time patch
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.instance.CurrentNailChargeBeginTime), MethodType.Getter)]
    [HarmonyPostfix]
    private static void ModifyBeginTime(ref float __result)
    {
        if (HeroController.instance.playerData.CurrentCrestID == "Devil")
        {
            __result = 0.25f;
        }
    }

    //Patching TakeDamage to know when enemies are hit
    [HarmonyPatch(typeof(HealthManager), nameof(HealthManager.TakeDamage))]
    [HarmonyPostfix]
    private static void SendHitToManager(ref HitInstance hitInstance)
    {
        if (HeroController.instance == null) { return; }

        DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
        if (handler == null) { return; }

        handler.HitLanded(hitInstance);

        if (!hitInstance.IsHeroDamage) { return; } //Must be a hero attack
        if (!handler.IsDevilEquipped()) { return; } // Must have devil crest equipped
        if (handler.GetTrigger().IsInTrigger()) { return; } // If in trigger, ignore the damage nerf
        

        hitInstance.Multiplier *= 0.75f; //Reduce player damage by 25% while using Devil. This is for balancing purposes, and for longer combos.
    }

    //Patching TakeDamage to know when player was hit
    [HarmonyPatch(typeof(HeroController), nameof(HeroController.instance.TakeDamage), typeof(GameObject), typeof(GlobalEnums.CollisionSide), typeof(int), typeof(GlobalEnums.HazardType), typeof(GlobalEnums.DamagePropertyFlags))]
    [HarmonyPostfix]
    private static void SendGotHitToManager()
    {
        if (HeroController.instance == null) { return; }

        DevilCrestHandler handler = HeroController.instance.GetComponent<DevilCrestHandler>();
        if (handler == null) { return; }

        handler.GotHit();
    }

    //for the skill icons
    [HarmonyPatch(typeof(HudCanvas), nameof(HudCanvas.Awake))]
    [HarmonyPostfix]
    private static void HudTest()
    {
        HudCanvas.instance.gameObject.AddComponent<CanvasUI>();        
    }

    //For inventory patches
    [HarmonyPatch(typeof(InventoryItemSelectableDirectional), nameof(InventoryItemSelectableDirectional.Awake))]
    [HarmonyPostfix]
    private static void InventoryTest(InventoryItemSelectableDirectional __instance)
    {
        if (__instance.name != "Change Crest Button") { return; }
        if (__instance is InventoryItemSelectableButtonEvent button)
        {
            try
            {
                DevilMenuUI.CreateMenu(button);
            }
            catch (Exception ex)
            {
                VesselMayCrySEPlugin.Instance.LogError(ex.Message + "\n" + ex.StackTrace); // goes to BepInEx/LogOutput.log
            }

            
        }
    }
}
