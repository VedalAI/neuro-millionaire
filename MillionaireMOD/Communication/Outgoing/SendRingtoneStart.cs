using HarmonyLib;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
public static class SendRingtoneStart
{
    [HarmonyPatch(typeof(LifelineStep), nameof(LifelineStep.CallAFriendPartTwoRoutine))]
    [HarmonyPrefix]
    private static void Prefix(LifelineStep __instance)
    {
        WebSocketConnection.Send(new WsMessage("lifeline/phone_a_friend/ringtone_start"));
    }
}
