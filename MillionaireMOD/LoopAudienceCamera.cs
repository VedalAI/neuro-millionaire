using System.Collections;
using System.Collections.Generic;
using HarmonyLib;

namespace MillionaireMOD;

[HarmonyPatch]
public static class LoopAudienceCamera
{
    public static bool CanContinue;

    // [HarmonyPatch(typeof(LifelineStep), nameof(LifelineStep.AskPublicRoutine))]
    // [HarmonyPrefix]
    public static bool PrefixPatch(LifelineStep __instance, out IEnumerator __result)
    {
        __result = OurCoroutine();
        return false;

        IEnumerator OurCoroutine()
        {
            CanContinue = false;
            __instance.mJustEntered = true;
            int[] availableCilinder = __instance.mUIController.GetAvailableCilinder();
            List<int> list = __instance.AllValuesPublicRightOrder(availableCilinder);
            __instance.mUIController.SetPublicFinalValue(list);
            yield return __instance.NextStep("Ask Public stuck");

            while (!CanContinue)
            {
                // AnimatorStateInfo info = CinemachineController.sInstance.mCinemachineAnimator.GetCurrentAnimatorStateInfo(0);
                // info.m_Loop

                CinemachineController.sInstance.mCinemachineAnimator.SetTrigger("Next");
                yield return null;
                CinemachineController.sInstance.mCinemachineAnimator.SetTrigger("Lifeline");
                yield return null;
                CinemachineController.sInstance.mCinemachineAnimator.SetInteger("LifelineChoice", (int) UIController.eLifeline_Name.PUBLIC);
                CinemachineController.sInstance.mCinemachineAnimator.ResetTrigger("AnswerPr");
                CinemachineController.sInstance.mCinemachineAnimator.ResetTrigger("AnswerCd");
                CinemachineController.sInstance.mCinemachineAnimator.ResetTrigger("AnswerBoth");
                yield return null;
                yield return __instance.WaitEndOfTimeline("");
            }

            __instance.mUIController.LifelineDone(2);
            __instance.mPublicStarted = false;
            __instance.mRoutine = null;
        }
    }
}
