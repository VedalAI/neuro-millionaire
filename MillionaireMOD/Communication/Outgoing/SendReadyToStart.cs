using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MillionaireMOD.Communication.Outgoing;

[HarmonyPatch]
internal static class SendReadyToStart
{
    private static readonly Type _mainMenuStepRoutine = AccessTools.Inner(typeof(MainMenu), "<StepRoutine>d__15");
    private static readonly FieldInfo _mainMenuStepRoutineState = AccessTools.Field(_mainMenuStepRoutine, "<>1__state");

    [HarmonyTargetMethod]
    [UsedImplicitly]
    private static MethodBase TargetMethod() => AccessTools.Method(_mainMenuStepRoutine, "MoveNext");

    [HarmonyPrefix]
    [UsedImplicitly]
    private static void Prefix(object __instance)
    {
        int state = (int)_mainMenuStepRoutineState.GetValue(__instance);
        if (state == 1 && !Input.GetKey(KeyCode.LeftShift))
        {
            Dictionary<string, object> data = new()
            {
                { "availableDifficulties", new[] { "easy", "normal" } },
                { "availableCategories", MenuManager.sInstance.mMenuGameplay.mPackSelection.mAllPacks.Select(p => p.mDataValue.ToString()) },
            };

            WebSocketConnection.Send(new WsMessage("ready", data));
        }
    }
}
