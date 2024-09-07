using HarmonyLib;
using System;

using static RepairsManagerUI;

namespace DamageUI
{
    // spawn ui at the start of stage
    [HarmonyPatch(typeof(StageSceneManager), MethodType.Constructor)]
    static class StageSceneManager_Patch
    {
        static void Postfix() => Main.Try(() => Main.SpawnUI());
    }

    // refresh UI when we start race
    [HarmonyPatch(typeof(StageSceneManager), nameof(StageSceneManager.StartEvent))]
    static class StageSceneManager_StartEvent_Patch
    {
        static void Postfix()
        {
            if (!Main.enabled)
                return;

            Main.Try(() => DamagePanel.Refresh());
        }
    }

    // start UI show anim
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.ShowStageHud))]
    static class HudManager_ShowStageHud_Patch
    {
        static void Postfix()
        {
            if (!Main.enabled)
                return;

            Main.Try(() => Main.damagePanel.PlayAnimation(true));
        }
    }

    // start UI hide anim
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.HideStageHud))]
    static class HudManager_HideStageHud_Patch
    {
        static void Postfix()
        {
            if (!Main.enabled)
                return;

            Main.Try(() => Main.damagePanel.PlayAnimation(false));
        }
    }

    // update wheels damage on UI
    [HarmonyPatch(typeof(Wheel), nameof(Wheel.DoTirePuncture))]
    static class Wheel_DoTirePuncture_Patch
    {
        static void Postfix(Wheel __instance)
        {
            if (!Main.enabled)
                return;

            Main.Try(() => Main.damagePanel.OnTirePuncture(__instance));
        }
    }

    // update wheels repair on UI
    [HarmonyPatch(typeof(Wheel), nameof(Wheel.RepairTirePunctureOrOffRim))]
    static class Wheel_RepairTirePunctureOrOffRim_Patch
    {
        static void Postfix(Wheel __instance)
        {
            if (!Main.enabled)
                return;

            Main.Try(() => Main.damagePanel.OnTireRepair(__instance));
        }
    }

    // updates damages on UI
    [HarmonyPatch(typeof(PerformanceDamage), nameof(PerformanceDamage.UpdatePerformanceDamage))]
    static class PerformanceDamage_UpdatePerformanceDamage_Patch
    {
        static void Postfix(PerformanceDamage __instance)
        {
            if (!Main.enabled)
                return;

            Main.Try(() => Main.damagePanel.OnPartDamage(GetPart(__instance)));
        }

        static SystemToRepair GetPart(PerformanceDamage part)
        {
            switch (part)
            {
                case AerodynamicsPerformanceDamage body:
                    return SystemToRepair.CLEANCAR;

                case SteeringPerfomanceDamage suspension:
                    return SystemToRepair.SUSPENSION;

                case RadiatorPerformanceDamage radiator:
                    return SystemToRepair.RADIATOR;

                case EnginePerformanceDamage engine:
                    return SystemToRepair.ENGINE;

                case TurboPerformanceDamage turbo:
                    return SystemToRepair.TURBO;

                case TransmissionPerformanceDamage gearbox:
                    return SystemToRepair.GEARBOX;

                default:
                    throw new Exception("Didn't recognize the part");
            }
        }
    }
}