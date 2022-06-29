using System;
using System.Collections.Generic;
using HarmonyLib;
using flanne.Core;


namespace MTDUI.HarmonyPatches.Patches
{

    [HarmonyPatch(typeof(PauseState))]
    [HarmonyPatch("Exit", MethodType.Normal)]
    class ModOptionChangeIngamePatch
    {
        static List<Action> pauseActions = new List<Action>();

        static void Prefix()
        {
            pauseActions.ForEach(a => a.Invoke());
        }

        /// <summary>
        /// Add an action to be executed on pause exit, allow mods to update according to possible config modification
        /// For now, called for every config registered via RegisterWithPauseAction
        /// </summary>
        /// <param name="method">Action to invoke</param>
        static public void AddPatchActionToPause(Action method)
        {
            pauseActions.Add(method);
        }
    }
}
