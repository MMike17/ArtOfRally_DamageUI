using HarmonyLib;
using System;

using static RepairsManagerUI;

namespace DamageUI
{
    // Patch model
    // [HarmonyPatch(typeof(), nameof())]
    // static class type_method_Patch
    // {
    // 	static void Prefix()
    // 	{
    // 		//
    // 	}

    // 	static void Postfix()
    // 	{
    // 		//
    // 	}
    // }

    // TODO : Patch event when we start race to update repairs (Refresh)

    // spawn ui at the start of stage
    [HarmonyPatch(typeof(StageSceneManager), MethodType.Constructor)]
    static class StageSceneManager_Patch
    {
        static void Postfix()
        {
            if (!Main.enabled)
                return;

            Main.Try(() => Main.SpawnUI());
        }
    }

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