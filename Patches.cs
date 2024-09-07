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
    // TODO : Patch event when a wheel gets a puncture

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
}