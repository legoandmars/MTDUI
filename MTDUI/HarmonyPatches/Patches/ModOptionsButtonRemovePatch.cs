using flanne.TitleScreen;
using HarmonyLib;
using MTDUI.Controllers;

namespace MTDUI.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(TitleMainMenuState))]
    [HarmonyPatch("Exit", MethodType.Normal)]
    internal class ModOptionsButtonRemovePatch
    {
        private static void Postfix(TitleMainMenuState __instance)
        {
            ModOptionsMenuController.TitleScreenMenuRemoveListeners();
        }
    }
}
