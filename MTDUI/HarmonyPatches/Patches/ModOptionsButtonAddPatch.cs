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
        private static void Postfix(TitleMainMenuState __instance)
        {
            if(ModOptionsMenuController.TitleScreenController == null)
            {
                ModOptionsMenuController.TitleScreenController = Traverse.Create(__instance).Field("owner").GetValue() as TitleScreenController;
            }

            ModOptionsMenuController.CreateModOptionsButton(OptionsMenuType.MainMenu);
            ModOptionsMenuController.CreateModOptionsPanel(OptionsMenuType.MainMenu);

            ModOptionsMenuController.ModOptionsButton?.onClick.AddListener(new UnityAction(ModOptionsMenuController.OnModOptionsClick));
        }
    }
}
