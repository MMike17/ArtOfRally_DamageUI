using HarmonyLib;

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

    // TODO : Patch event when a part takes damage / send through the state of the part
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

    // 
}