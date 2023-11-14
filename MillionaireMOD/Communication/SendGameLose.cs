using System.Collections.Generic;
using HarmonyLib;

namespace MillionaireMOD.Communication;

[HarmonyPatch]
internal static class SendGameLose
{
    [HarmonyPatch(typeof(StageResultStep), nameof(StageResultStep.StartDefeatResult))]
    [HarmonyPostfix]
    private static void Patch(StageResultStep __instance)
    {
        Dictionary<string, object> data = new()
        {
            {"won", false}
        };
        WebSocketConnection.Send(new WSMessage("millionaire/finish", data));
    }
}
