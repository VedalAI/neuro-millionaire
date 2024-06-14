using MillionaireMOD.Tweaks;

namespace MillionaireMOD.Communication.Incoming;

internal static class ReceiveAskTheAudienceResults
{
    public static void UpdateUI(int a, int b, int c, int d)
    {
        UIController.sInstance.mPublicValues = [a, b, c, d];
        UIController.sInstance.StartStopPublicParticleAnim(false);
        UIController.sInstance.StartPublicCilinder();
        PreventSkippingCustomLifelines.CanSkip = true;
    }
}
