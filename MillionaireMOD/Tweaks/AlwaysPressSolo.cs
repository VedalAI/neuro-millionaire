using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class AlwaysPressSolo
{
    [HarmonyPatch(typeof(SimpleChoice), nameof(SimpleChoice.StepRoutine))]
    private static void Prefix(SimpleChoice __instance)
    {
        if (__instance != MenuManager.sInstance.mMenuGameplay.mMode) return;
        if (Input.GetKey(KeyCode.LeftShift)) return;

        __instance.StartCoroutine(coroutine());
        return;

        IEnumerator coroutine()
        {
            yield return new WaitForSeconds(__instance.mStartingTween.mDuration);

            __instance.mCurrentSelected = 0;
            __instance.mInsideDone = true;
            AudioDirector.PlayUISound("AK_Event_UI_Generic_Select");
        }
    }
}
