using System.Collections.Generic;
using HarmonyLib;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendCharacter
{
    public static string LastSentName { get; set; }

    [HarmonyPatch(typeof(CandidateSelect), nameof(CandidateSelect.UpdateInfo))]
    [HarmonyPostfix]
    private static void Postfix(CandidateSelect __instance)
    {
        string name = __instance.mCandidateInfoContainer.mName.text;

        if (name == LastSentName) return;
        LastSentName = name;

        Dictionary<string, object> data = new()
        {
            { "name", __instance.mCandidateInfoContainer.mName.text },
            { "age", __instance.mCandidateInfoContainer.mAge.text },
            { "profession", __instance.mCandidateInfoContainer.mJob.text },
            { "passion", __instance.mCandidateInfoContainer.mHobby.text },
        };

        WebSocketConnection.Send(new WsMessage("character", data));
    }
}
