using HarmonyLib;
using MillionaireMOD.Tweaks;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendAskTheAudienceConfirm
{
    [HarmonyPatch(typeof(MusicDirector), nameof(MusicDirector.PlayMusicTrack), typeof(string), typeof(bool))]
    [HarmonyPatch(typeof(MusicDirector), nameof(MusicDirector.PlayMusicTrack), typeof(string), typeof(float))]
    [HarmonyPrefix, HarmonyPriority(Priority.First)]
    // ReSharper disable once InconsistentNaming
    public static void Prefix(string MusicTrackName)
    {
        if (MusicTrackName != "AK_Event_Mus_Bonus_Public_Keyboard") return;

        PreventSkippingCustomLifelines.CanSkip = false;
        WebSocketConnection.Send(new WsMessage("lifeline/ask_the_audience/confirm_start"));
    }
}
