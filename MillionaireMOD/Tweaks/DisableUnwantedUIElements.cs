using HarmonyLib;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class DisableUnwantedUIElements
{
    [HarmonyPatch(typeof(UIController), nameof(UIController.Awake))]
    [HarmonyPostfix]
    public static void Postfix(UIController __instance)
    {
        __instance.transform.Find("Canvas_Question/HighLights").gameObject.SetActive(false);
        __instance.transform.Find("Lifeline_Panel/Call_A_Friend/Canvas_Choice/1").gameObject.SetActive(false);
        __instance.transform.Find("Lifeline_Panel/Call_A_Friend/Canvas_Choice/2").gameObject.SetActive(false);
        __instance.transform.Find("Lifeline_Panel/Call_A_Friend/Canvas_Choice/3").gameObject.SetActive(false);
        __instance.transform.Find("Lifeline_Panel/Call_A_Friend/Canvas_Choice/4").gameObject.SetActive(false);
    }
}
