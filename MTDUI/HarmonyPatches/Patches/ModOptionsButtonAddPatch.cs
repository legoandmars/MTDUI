using flanne.TitleScreen;
using HarmonyLib;
using MTDUI.Controllers;
using MTDUI.Data;
using UnityEngine.Events;

namespace MTDUI.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(TitleMainMenuState))]
    [HarmonyPatch("Enter", MethodType.Normal)]
    internal class ModOptionsButtonAddPatch
    {
        private static void Postfix(ref TitleScreenController ___owner)
        {
            if (ModOptionsMenuController.TitleScreenController == null) ModOptionsMenuController.TitleScreenController = ___owner;

            ModOptionsMenuController.CreateModOptionsButton(OptionsMenuType.MainMenu);
            ModOptionsMenuController.CreateTitleScreenModsMenu();

            ModOptionsMenuController.ModOptionsButton?.onClick.AddListener(new UnityAction(ModOptionsMenuController.OnModOptionsClick));
        }
    }
}
