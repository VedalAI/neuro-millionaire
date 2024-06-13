using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace MillionaireMOD.Communication.Incoming;

[HarmonyPatch]
public static class ReceiveStart
{
    public static bool LastDifficultyWasNormal { get; private set; }

    public static void StartGame(string difficulty, string[] categories)
    {
        MenuManager.sInstance.mMainMenu.mFirstHighlighted = 0;
        StepRoutinePatch.EndStep = true;

        LastDifficultyWasNormal = difficulty switch
        {
            "easy" => false,
            "normal" => true,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };
    }

    [HarmonyPatch]
    private static class StepRoutinePatch
    {
        public static bool EndStep { get; set; }

        private static readonly Type _mainMenuStepRoutine = AccessTools.Inner(typeof(MainMenu), "<StepRoutine>d__15");
        private static readonly FieldInfo _mainMenuValidateInput = AccessTools.Field(_mainMenuStepRoutine, "<validateInput>5__2");

        [HarmonyTargetMethod]
        [UsedImplicitly]
        private static MethodBase TargetMethod() => AccessTools.Method(_mainMenuStepRoutine, "MoveNext");

        [HarmonyPrefix]
        [UsedImplicitly]
        private static void Prefix(object __instance)
        {
            if (EndStep)
            {
                _mainMenuValidateInput.SetValue(__instance, true);
                EndStep = false;
            }
        }
    }
}
