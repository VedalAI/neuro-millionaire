using HarmonyLib;
using UnityEngine;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class DisableLogSpam
{
    [HarmonyPatch(typeof(Debug), nameof(Debug.Log), typeof(object))]
    [HarmonyPrefix]
    private static bool Prefix(object message)
    {
        return message.ToString() != "AddOnContentIsPresent";
    }
}
