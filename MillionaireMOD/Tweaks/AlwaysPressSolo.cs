using System.Collections;
using HarmonyLib;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class AlwaysPressSolo
{
    private static bool _runOriginal;

    [HarmonyPatch(typeof(SimpleChoice), nameof(SimpleChoice.StepRoutine))]
    private static bool Prefix(SimpleChoice __instance, ref IEnumerator __result)
    {
        if (__instance != MenuManager.sInstance.mMenuGameplay.mMode) return true;

        if (_runOriginal)
        {
            _runOriginal = false;
            return true;
        }

        __result = coroutine();
        return false;

        IEnumerator coroutine()
        {
            __instance.mCurrentSelected = 0;
            __instance.mInsideDone = true;
            _runOriginal = true;
            yield return __instance.StepRoutine();
        }
    }
}
