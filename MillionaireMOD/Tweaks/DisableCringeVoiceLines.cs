using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MillionaireMOD.Tweaks;

[HarmonyPatch]
internal static class DisableCringeVoiceLines
{
    [HarmonyPatch(typeof(ScenarioLibrary), nameof(ScenarioLibrary.PlayPresenterScenario))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static bool PresenterLines(ScenarioLibrary.ePresenterAction _prAction)
    {
        if (_prAction is ScenarioLibrary.ePresenterAction.vous_avez_choisi or ScenarioLibrary.ePresenterAction.dernier_mot) return false;

        LOGGER.LogWarning("Playing presenter scenario: " + _prAction);
        return true;
    }

    private static readonly MethodInfo _scenarioLibrarySetRandom = AccessTools.Method(typeof(ScenarioLibrary), nameof(ScenarioLibrary.SetRandom),
        [typeof(ScenarioLibrary.eCharacters), typeof(ScenarioLibrary.ePresenterAction), typeof(ScenarioLibrary.eCandidateAction)]);

    private static readonly FieldInfo _scenarioBehaviourMCandidateAction = AccessTools.Field(typeof(ScenarioBehavior), nameof(ScenarioBehavior.mCandidateAction));

    [HarmonyPatch(typeof(ScenarioBehavior), nameof(ScenarioBehavior.OnBehaviourPlay))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ContestantLines(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.Start();
        matcher.SearchForward(c => c.Calls(_scenarioLibrarySetRandom));
        matcher.Advance(-6);
        int insertPos = matcher.Pos;

        Label? label = null;
        matcher.SearchForward(c => c.Branches(out label));

        matcher.Advance(-999);
        matcher.Advance(insertPos + 1);

        matcher.InsertAndAdvance
        (
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, _scenarioBehaviourMCandidateAction),
            new CodeInstruction(OpCodes.Call, new Func<ScenarioLibrary.eCandidateAction, bool>(shouldPlay).Method),
            new CodeInstruction(OpCodes.Brfalse, label!.Value)
        );

        return matcher.InstructionEnumeration();

        static bool shouldPlay(ScenarioLibrary.eCandidateAction candidateAction)
        {
            return false;

            /*if (candidateAction is ScenarioLibrary.eCandidateAction.dernier_mot) return false;

            LOGGER.LogWarning("Playing candidate scenario: " + candidateAction);
            return true;*/
        }
    }

    [HarmonyPatch(typeof(AnswerStep), nameof(AnswerStep.StartStep))]
    [HarmonyPostfix]
    private static void AnswerLines(AnswerStep __instance)
    {
        __instance.mLastWord = true;
        __instance.mSkipReflexion = true;
    }
}
