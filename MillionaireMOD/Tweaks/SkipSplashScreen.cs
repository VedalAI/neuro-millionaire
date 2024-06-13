using HarmonyLib;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class SkipSplashScreen
{
    [HarmonyPatch(typeof(SplashScreenManager), nameof(SplashScreenManager.Start))]
    [HarmonyPrefix]
    private static bool Patch(SplashScreenManager __instance)
    {
        __instance.StartCoroutine(__instance.LaunchFirstScene());
        return false;
    }
}
