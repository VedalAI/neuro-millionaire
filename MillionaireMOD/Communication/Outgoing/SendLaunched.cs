using HarmonyLib;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendLaunched
{
    [HarmonyPatch(typeof(LanguageStep), nameof(LanguageStep.StartStep))]
    [HarmonyPostfix]
    private static void Postfix(LanguageStep __instance)
    {
        WebSocketConnection.Send(new WsMessage("launched"));
    }
}
