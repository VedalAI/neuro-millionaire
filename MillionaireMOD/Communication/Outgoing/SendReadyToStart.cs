using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendReadyToStart
{
    private static readonly Type _mainMenuStepRoutine = AccessTools.Inner(typeof(MainMenu), "<StepRoutine>d__15");
    private static readonly FieldInfo _mainMenuStepRoutineState = AccessTools.Field(_mainMenuStepRoutine, "<>1__state");

    [HarmonyTargetMethod]
    [UsedImplicitly]
    private static MethodBase TargetMethod() => AccessTools.Method("MainMenu.<StepRoutine>d__15:MoveNext");

    [HarmonyPrefix]
    [UsedImplicitly]
    private static void Prefix(object __instance)
    {
        int state = (int)_mainMenuStepRoutineState.GetValue(__instance);
        if (state == 1)
        {
            Dictionary<string, object> data = new()
            {
                { "availableDifficulties", new[] { "easy", "normal" } },
                { "availableCategories", new[] { "" } },
            };

            WebSocketConnection.Send(new WsMessage("ready", data));
        }
    }
}
