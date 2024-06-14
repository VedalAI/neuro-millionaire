using MillionaireMOD.Behaviour;
using MillionaireMOD.Communication.Outgoing;

namespace MillionaireMOD.Communication.Incoming;

internal static class ReceivePhoneAFriendResults
{
    public static void EndCall(string answer)
    {
        if (!SendPhoneAFriendConfirm.IsThisTheTimeForMeToDoThis)
        {
            LOGGER.LogFatal("Received phone a friend results at the wrong time, wtf are you doing?");
            return;
        }

        CustomCallAFriendBehaviour.CallOver = true;
        CustomCallAFriendBehaviour.CallAnswer = answer;
    }
}
