using System.Collections;
using HarmonyLib;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class SkipPressAnyButton
{
    [HarmonyPatch(typeof(MenuToStart), nameof(MenuToStart.StepRoutine))]
    [HarmonyPrefix]
    public static bool SkipPatch(MenuToStart __instance, out IEnumerator __result)
    {
        __result = coroutine();
        return false;

        IEnumerator coroutine()
        {
            Shell.sInstance.ResetSimulateClick("ANY");
            AudioDirector.PlayUISound("AK_Event_UI_Generic_Select");
            __instance.FinishStep();
            yield break;
        }
    }
}
