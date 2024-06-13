using System.Collections.Generic;
using HarmonyLib;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendAnswerCorrect
{
    [HarmonyPatch(typeof(StageResultStep), nameof(StageResultStep.StartVictorySingleResult))]
    [HarmonyPostfix]
    private static void Patch(StageResultStep __instance)
    {
        Dictionary<string, object> data = new()
        {
            // {"money_earned", __instance.mUIController.mBaseValues.GetPyramidValue(__instance.mUIController.mPyramidPalierIndex)}
        };
        WebSocketConnection.Send(new WsMessage("answer/correct", data));
    }
}
