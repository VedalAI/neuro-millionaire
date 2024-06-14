using System.Collections.Generic;
using HarmonyLib;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendCharacter
{
    [HarmonyPatch(typeof(CandidateSelect), nameof(CandidateSelect.UpdateInfo))]
    [HarmonyPostfix]
    private static void Postfix(CandidateSelect __instance)
    {
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
