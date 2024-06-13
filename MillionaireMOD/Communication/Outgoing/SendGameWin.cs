using System.Collections.Generic;
using HarmonyLib;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendGameWin
{
    [HarmonyPatch(typeof(StageResultStep), nameof(StageResultStep.StartFinalVictoryResult))]
    [HarmonyPostfix]
    private static void Patch(StageResultStep __instance)
    {
        Dictionary<string, object> data = new()
        {
            {"won", true}
        };
        WebSocketConnection.Send(new WsMessage("finish", data));
    }
}
