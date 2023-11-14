using HarmonyLib;

namespace MillionaireMOD;

[HarmonyPatch]
internal static class DisableAudienceAmbientSounds
{
    [HarmonyPatch(typeof(AudioDirector), nameof(AudioDirector.PlayUISound), typeof(string), typeof(bool))]
    [HarmonyPrefix]
    private static bool Prefix(string eventName)
    {
        return eventName != "AK_Event_Amb_Public_Presence_Start";
    }
}
