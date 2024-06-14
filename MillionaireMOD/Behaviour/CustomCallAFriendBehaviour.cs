using System.Collections;
using HarmonyLib;
using MillionaireMOD.Communication;
using MillionaireMOD.Communication.Outgoing;
using MillionaireMOD.Tweaks;
using UnityEngine;

namespace MillionaireMOD.Behaviour;

[HarmonyPatch]
internal static class CustomCallAFriendBehaviour
{
    public static bool CallOver { get; set; }
    public static string CallAnswer { get; set; }

    [HarmonyPatch(typeof(ScenarioLibrary), nameof(ScenarioLibrary.CallScenario))]
    [HarmonyPrefix]
    public static bool Prefix(ScenarioLibrary __instance, out float __result)
    {
        __instance.mCallOver = false;
        __instance.mCallAFriendRoutine = __instance.StartCoroutine(coroutine());
        __instance.mClipTimer = -1f;
        __result = 30;
        return false;

        IEnumerator coroutine()
        {
            UIController.sInstance.ShowCallAFriendClock();
            yield return new WaitForSeconds(0.2f);
            UIController.sInstance.StartClock();

            yield return new WaitUntil(() => UIController.sInstance.mCurrentRoutine == null || CallOver);

            if (!CallOver) WebSocketConnection.Send(new WsMessage("lifeline/phone_a_friend/request_end"));
            UIController.sInstance.SetLifelineAnswer(string.IsNullOrWhiteSpace(CallAnswer) ? "?" : CallAnswer);
            CallOver = false;
            CallAnswer = null;
            SendPhoneAFriendConfirm.IsThisTheTimeForMeToDoThis = false;
            PreventSkippingCustomLifelines.CanSkip = true;

            __instance.mCallOver = true;
            __instance.mCallAFriendRoutine = null;
        }
    }

    [HarmonyPatch(typeof(ScenarioLibrary), nameof(ScenarioLibrary.AnswerScenario))]
    [HarmonyPrefix]
    public static bool AnswerPrefix(ScenarioLibrary __instance, out float __result, int _id)
    {
        __instance.mIdCd = _id;
        __instance.mClipTimer = -1;
        __result = 0;
        return false;
    }

    [HarmonyPatch(typeof(LifelineStep), nameof(LifelineStep.MoveInPeopleChoice))]
    [HarmonyPrefix]
    public static bool AlwaysSelectFriendPrefix(LifelineStep __instance, out bool __result)
    {
        __result = true;
        return false;
    }
}
