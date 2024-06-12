using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace MillionaireMOD;

[HarmonyPatch]
public static class LoopAudienceCamera
{
    public static bool CanContinue;

    private static readonly int _next = Animator.StringToHash("Next");
    private static readonly int _lifeline = Animator.StringToHash("Lifeline");
    private static readonly int _lifelineChoice = Animator.StringToHash("LifelineChoice");
    private static readonly int _answerPr = Animator.StringToHash("AnswerPr");
    private static readonly int _answerCd = Animator.StringToHash("AnswerCd");
    private static readonly int _answerBoth = Animator.StringToHash("AnswerBoth");

    // [HarmonyPatch(typeof(LifelineStep), nameof(LifelineStep.AskPublicRoutine))]
    // [HarmonyPrefix]
    public static bool PrefixPatch(LifelineStep __instance, out IEnumerator __result)
    {
        __result = ourCoroutine();
        return false;

        IEnumerator ourCoroutine()
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

                CinemachineController.sInstance.mCinemachineAnimator.SetTrigger(_next);
                yield return null;
                CinemachineController.sInstance.mCinemachineAnimator.SetTrigger(_lifeline);
                yield return null;
                CinemachineController.sInstance.mCinemachineAnimator.SetInteger(_lifelineChoice, (int) UIController.eLifeline_Name.PUBLIC);
                CinemachineController.sInstance.mCinemachineAnimator.ResetTrigger(_answerPr);
                CinemachineController.sInstance.mCinemachineAnimator.ResetTrigger(_answerCd);
                CinemachineController.sInstance.mCinemachineAnimator.ResetTrigger(_answerBoth);
                yield return null;
                yield return __instance.WaitEndOfTimeline("");
            }

            __instance.mUIController.LifelineDone(2);
            __instance.mPublicStarted = false;
            __instance.mRoutine = null;
        }
    }
}
