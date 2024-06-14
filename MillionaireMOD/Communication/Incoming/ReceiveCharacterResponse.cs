using MillionaireMOD.Communication.Outgoing;

namespace MillionaireMOD.Communication.Incoming;

internal static class ReceiveCharacterResponse
{
    public static void ReceiveResponse(bool accept)
    {
        if (!accept)
        {
            MenuManager.sInstance.mMenuGameplay.mCandidateSelect.mJustMoved = true;
            MenuManager.sInstance.mMenuGameplay.mCandidateSelect.mMoveRight = true;
        }
        else
        {
            SendCharacter.LastSentName = null;
            MenuManager.sInstance.mMenuGameplay.mCandidateSelect.mInsideDone = true;
        }
    }
}
