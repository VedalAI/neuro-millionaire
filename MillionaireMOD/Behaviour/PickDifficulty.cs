using System.Collections;
using HarmonyLib;
using MillionaireMOD.Communication.Incoming;
using UnityEngine;

namespace MillionaireMOD.Behaviour;

[HarmonyPatch]
internal static class PickDifficulty
{
    [HarmonyPatch(typeof(SimpleChoice), nameof(SimpleChoice.StepRoutine))]
    private static void Prefix(SimpleChoice __instance)
    {
        if (__instance != MenuManager.sInstance.mMenuGameplay.mDifficulty) return;
        if (Input.GetKey(KeyCode.LeftShift)) return;

        __instance.StartCoroutine(coroutine());
        return;

        IEnumerator coroutine()
        {
            yield return new WaitForSeconds(__instance.mStartingTween.mDuration);

            __instance.mCurrentSelected = ReceiveStart.LastDifficultyWasNormal ? 0 : 1;
            __instance.mInsideDone = true;
            AudioDirector.PlayUISound("AK_Event_UI_Generic_Select");
        }
    }
}
