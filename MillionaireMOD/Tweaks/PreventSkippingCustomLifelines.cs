using HarmonyLib;
using UnityEngine;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class PreventSkippingCustomLifelines
{
    public static bool CanSkip
    {
        get => !GameObject.Find("DO NOT SKIP");
        set
        {
            if (value == CanSkip) return;

            // ReSharper disable once ObjectCreationAsStatement
            if (!value) new GameObject("DO NOT SKIP");
            else Object.Destroy(GameObject.Find("DO NOT SKIP"));
        }
    }

    [HarmonyPatch(typeof(SoloGameplay), nameof(SoloGameplay.SkippedPressed))]
    [HarmonyPrefix]
    public static bool Prefix(out bool __result)
    {
        return __result = CanSkip;
    }
}
