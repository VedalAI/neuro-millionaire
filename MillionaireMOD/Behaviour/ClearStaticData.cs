using HarmonyLib;
using MillionaireMOD.Communication.Outgoing;

namespace MillionaireMOD.Behaviour;

[HarmonyPatch]
internal static class ClearStaticData
{
    [HarmonyPatch(typeof(PackSelection), nameof(PackSelection.PacksRoutine))]
    private static void Prefix()
    {
        CustomCallAFriendBehaviour.CallOver = false;
        CustomCallAFriendBehaviour.CallAnswer = null;
        SendCharacter.LastSentName = null;
        SendPhoneAFriendConfirm.IsThisTheTimeForMeToDoThis = false;
        SendQuestion.LastSentQuestion = null;
    }
}
