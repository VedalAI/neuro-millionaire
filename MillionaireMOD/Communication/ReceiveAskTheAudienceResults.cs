using System.Collections.Generic;

namespace MillionaireMOD.Communication;

public static class ReceiveAskTheAudienceResults
{
    public static void UpdateUI(int a, int b, int c, int d)
    {
        UIController.sInstance.mPublicValues = new List<int> { a, b, c, d };
        UIController.sInstance.StartStopPublicParticleAnim(false);
        UIController.sInstance.StartPublicCilinder();
        LoopAudienceCamera.CanContinue = true;
        PreventSkippingCustomLifelines.CanSkip = true;
    }
}
