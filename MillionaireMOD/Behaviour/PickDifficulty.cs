using System.Collections;
using HarmonyLib;
using MillionaireMOD.Communication.Incoming;
using UnityEngine;

namespace MillionaireMOD.Behaviour;

[HarmonyPatch]
internal static class PickDifficulty
{
    private static bool _runOriginal;

    [HarmonyPatch(typeof(SimpleChoice), nameof(SimpleChoice.StepRoutine))]
    private static bool Prefix(SimpleChoice __instance, ref IEnumerator __result)
    {
        if (__instance != MenuManager.sInstance.mMenuGameplay.mDifficulty) return true;
        if (Input.GetKey(KeyCode.LeftShift)) return true;

        if (_runOriginal)
        {
            _runOriginal = false;
            return true;
        }

        __result = coroutine();
        return false;

        IEnumerator coroutine()
        {
            __instance.mCurrentSelected = ReceiveStart.LastDifficultyWasNormal ? 0 : 1;
            __instance.mInsideDone = true;
            _runOriginal = true;
            yield return __instance.StepRoutine();
        }
    }
}
