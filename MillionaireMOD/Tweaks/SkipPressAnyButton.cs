using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class SkipPressAnyButton
{
    [HarmonyPatch(typeof(MenuToStart), nameof(MenuToStart.StepRoutine))]
    [HarmonyPrefix]
    private static bool SkipPatch(MenuToStart __instance, out IEnumerator __result)
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

    [HarmonyPatch]
    private static class LoadScreenSkip
    {
        private static readonly Type _loadScreenDisplayLoadScreen = AccessTools.Inner(typeof(LoadScreen), "<DisplayLoadScreenCoroutine>d__30");
        private static readonly FieldInfo _loadScreenAutoSkip = AccessTools.Field(_loadScreenDisplayLoadScreen, "<autoSkip>5__2");

        [HarmonyTargetMethod]
        [UsedImplicitly]
        private static MethodBase TargetMethod() => AccessTools.Method(_loadScreenDisplayLoadScreen, "MoveNext");

        [HarmonyPrefix]
        [UsedImplicitly]
        private static void Prefix(object __instance)
        {
            _loadScreenAutoSkip.SetValue(__instance, true);
        }
    }
}
