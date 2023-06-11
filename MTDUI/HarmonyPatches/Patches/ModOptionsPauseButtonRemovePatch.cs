using flanne.Core;
using HarmonyLib;
using MTDUI.Controllers;

namespace MTDUI.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(PauseState))]
    [HarmonyPatch("Exit", MethodType.Normal)]
    internal class ModOptionsPauseButtonRemovePatch
    {
        private static void Postfix(PauseState __instance)
        {
            ModOptionsMenuController.PauseMenuRemoveListeners();
        }
    }
}
