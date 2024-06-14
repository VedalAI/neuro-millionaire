using HarmonyLib;
using UnityEngine;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class PreventUnfocusedInput
{
    [HarmonyPatch(typeof(InputController), nameof(InputController.Update))]
    [HarmonyPrefix]
    private static void DisableInput(InputController __instance)
    {
        __instance.mDisableControls = !Application.isFocused;
    }
}
