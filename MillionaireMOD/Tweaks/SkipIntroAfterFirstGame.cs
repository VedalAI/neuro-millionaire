using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class SkipIntroAfterFirstGame
{
    private static int _availableNonSkips;//= 2;
    private static bool _press;

    [HarmonyPatch(typeof(IntroStep), nameof(IntroStep.StartStep))]
    [HarmonyPatch(typeof(IntroStep), nameof(IntroStep.StartStepTwo))]
    [HarmonyPrefix]
    private static void Prefix(IntroStep __instance)
    {
        _availableNonSkips--;
        if (_availableNonSkips >= 0) return;

        __instance.StartCoroutine(coroutine());
        return;

        IEnumerator coroutine()
        {
            yield return new WaitForSeconds(1);

            _press = true;
        }
    }

    [HarmonyPatch(typeof(SoloGameplay), nameof(SoloGameplay.SkippedPressed))]
    [HarmonyPrefix]
    private static bool PressSkip(ref bool __result)
    {
        if (!_press) return true;
        _press = false;
        __result = true;
        return false;
    }
}
