using System;
using System.Collections;
using System.Reflection;
using System.Web.SessionState;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MillionaireMOD.Communication.Incoming;

[HarmonyPatch]
internal static class ReceiveLanguage
{
    public static void SetLanguage(string languageStr)
    {
        LanguageStep languageStep = GameObject.FindObjectOfType<LanguageStep>();
        if (!languageStep) return;

        eLanguage language = (eLanguage) Enum.Parse(typeof(eLanguage), languageStr);
        MenuManager.sInstance.mLanguage.MovingTo(language);
        StepRoutinePatch.EndStep = true;
    }

    [HarmonyPatch(typeof(MenuManager), nameof(MenuManager.ShowCountryStep))]
    [HarmonyPrefix]
    private static bool Prefix(MenuManager __instance)
    {
        __instance.AddPage(MenuManager.eMenuState.COUNTRY);
        __instance.mGaussian.SetBlur(0f);
        __instance.mCurrentStep = __instance.mLanguage;
        __instance.next = false;
        return false;
    }

    [HarmonyPatch]
    private static class StepRoutinePatch
    {
        public static bool EndStep { get; set; }

        private static readonly Type _languageStepStepRoutine = AccessTools.Inner(typeof(LanguageStep), "<StepRoutine>d__30");
        private static readonly FieldInfo _languageStepRunning = AccessTools.Field(_languageStepStepRoutine, "<running>5__2");

        [HarmonyTargetMethod]
        [UsedImplicitly]
        private static MethodBase TargetMethod() => AccessTools.Method(_languageStepStepRoutine, "MoveNext");

        [HarmonyPrefix]
        [UsedImplicitly]
        private static void Prefix(object __instance)
        {
            if (EndStep)
            {
                _languageStepRunning.SetValue(__instance, false);
                EndStep = false;
            }
        }
    }
}
