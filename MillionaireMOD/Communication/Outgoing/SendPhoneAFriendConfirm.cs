﻿using HarmonyLib;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
public static class SendPhoneAFriendConfirm
{
    public static bool IsThisTheTimeForMeToDoThis;

    [HarmonyPatch(typeof(MusicDirector), nameof(MusicDirector.PlayMusicTrack), typeof(string), typeof(bool))]
    [HarmonyPatch(typeof(MusicDirector), nameof(MusicDirector.PlayMusicTrack), typeof(string), typeof(float))]
    [HarmonyPrefix, HarmonyPriority(Priority.First)]
    // ReSharper disable once InconsistentNaming
    public static void Prefix(string MusicTrackName)
    {
        if (MusicTrackName != "AK_Event_Mus_Bonus_FC_Call_Start") return;

        IsThisTheTimeForMeToDoThis = true;
        PreventSkippingCustomLifelines.CanSkip = false;
        WebSocketConnection.Send(new WsMessage("lifeline/phone_a_friend/confirm_start"));
    }
}