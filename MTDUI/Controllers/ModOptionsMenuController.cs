#nullable enable

using flanne;
using flanne.Core;
using flanne.TitleScreen;
using flanne.UI;
using flanne.UIExtensions;
using HarmonyLib;
using MTDUI.Data;
using MTDUI.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace MTDUI.Controllers
{
    public static class ModOptionsMenuController
    {
        // things are a bit repetitive here
        public static List<ModConfigEntry> ConfigEntries = new List<ModConfigEntry>();
        public static Dictionary<string, List<ModConfigEntry>> SortedConfigEntries = new Dictionary<string, List<ModConfigEntry>>();

        // Mod Option in Title Screen
        public static TitleScreenController? TitleScreenController = null;
        public static Button? ModOptionsButton = null;
        public static Button? ModOptionsBackButton = null;
        public static Button? ModOptionsSubMenuBackButton = null;
        public static flanne.UIExtensions.Menu? ModOptionsMenu = null;
        public static flanne.UIExtensions.Menu? ModOptionsSubmenuMenu = null;

        // Mod Option in Pause Menu
        public static GameController? GameController = null;
        public static Button? PauseModOptionsButton = null;
        public static Button? PauseModOptionsBackButton = null;
        public static flanne.UIExtensions.Panel? PauseModOptionsPanel = null;
        public static flanne.UIExtensions.Panel? PauseModOptionsSubPanel = null;
        public static Button? PauseModOptionsSubMenuBackButton = null;

        public static List<ModOptionComponent> PauseMenuModOptionComponents = new List<ModOptionComponent>();
        public static List<ModOptionComponent> TitleScreenModOptionComponents = new List<ModOptionComponent>();

        // Listeners Title screen
        public static void OnModOptionsClick()
        {
            if (TitleScreenController != null) TitleScreenController.ChangeState<ModOptionsMenuState>();
        }

        public static void OnModClick(string modName)
        {
            ModOptionsSubmenuState.CurrentSubmenu = modName;
            if (TitleScreenController != null) TitleScreenController.ChangeState<ModOptionsSubmenuState>();
        }

        // Listeners Pause menu
        public static void OnPauseModOptionsClick()
        {
            if (GameController != null) GameController.ChangeState<ModOptionsPauseState>();
        }

        public static void OnPauseModClick(string modName)
        {
            ModOptionsPauseSubmenuState.CurrentSubmenu = modName;
            if (GameController != null) GameController.ChangeState<ModOptionsPauseSubmenuState>();
        }

        private static bool AreEntryAndMenuCompatible(OptionsMenuType menu, ConfigEntryLocationType location)
        {
            if (location == ConfigEntryLocationType.Everywhere) return true;
            if (menu == OptionsMenuType.PauseMenu) return location == ConfigEntryLocationType.PauseOnly;
            else return location == ConfigEntryLocationType.MainOnly;
        }

        public static ModOptionComponent? AddButtonFromConfigEntry(GameObject templateButton, ModConfigEntry configEntry, OptionsMenuType menuType, string mod)
        {
            // Check if configEntry needs to be implemented in the current menu
            if (!AreEntryAndMenuCompatible(menuType, configEntry.Location)) return null;

            var newButton = Object.Instantiate(templateButton, templateButton.transform.parent);
            newButton.GetComponentInChildren<TextLocalizerUI>().enabled = false;

            var optionComponent = newButton.AddComponent<ModOptionComponent>();
            optionComponent.Initialize(configEntry, mod);

            newButton.SetActive(false);

            return optionComponent;
        }

        public static void AddButtonFromModName(GameObject template, OptionsMenuType menuType, string mod)
        {
            var newButton = Object.Instantiate(template, template.transform.parent);

            newButton.GetComponent<Button>().onClick.AddListener(
                menuType == OptionsMenuType.MainMenu ?
                new UnityAction(() => { OnModClick(mod); }) :
                new UnityAction(() => { OnPauseModClick(mod); })
            );

            newButton.SetActive(true);
            newButton.GetComponentInChildren<TextLocalizerUI>().enabled = false;
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = mod;
        }

        public static void CreateModOptionsButton(OptionsMenuType menuType)
        {
            if (menuType == OptionsMenuType.MainMenu ? ModOptionsButton != null : PauseModOptionsButton != null) return;

            var optionsButtonObject = menuType == OptionsMenuType.MainMenu ? TitleScreenController?.mainMenu.transform.GetChild(2).gameObject : GameController?.optionsButton.gameObject;
            if (optionsButtonObject == null) return; // Should never happend

            var newButton = Object.Instantiate(optionsButtonObject, optionsButtonObject.transform.parent);

            var newLocalization = new LocalizedString("menu_modoptions");
            var localizer = newButton.GetComponentInChildren<TextLocalizerUI>();
            Traverse.Create(localizer).Field("localizedString").SetValue(newLocalization); // force set to new localization

            // now set it to the proper spot in the hierarchy
            newButton.transform.SetSiblingIndex(optionsButtonObject.transform.GetSiblingIndex() + 1);

            if (menuType == OptionsMenuType.MainMenu) ModOptionsButton = newButton.GetComponent<Button>();
            else PauseModOptionsButton = newButton.GetComponent<Button>();
        }

        private static void ClearMenuEntriesList(flanne.UIExtensions.Menu menu, Button? button)
        {
            var entries = Traverse.Create(menu).Field("entries");
            var list = new List<ButtonExtension>();
            if (button != null) list.Add(button.GetComponent<ButtonExtension>());
            entries.SetValue(list);
        }

        private static (flanne.UIExtensions.Menu, Button) CloneAndCleanMenu(GameObject objectMenu)
        {
            var newMenuObject = Object.Instantiate(objectMenu, objectMenu.transform.parent);
            foreach (var button in newMenuObject.GetComponentsInChildren<Button>())
            {
                if (button.name != "Back") Object.DestroyImmediate(button.gameObject);
            }
            Button backButton = newMenuObject.GetComponentInChildren<Button>();
            flanne.UIExtensions.Menu menu = newMenuObject.GetComponent<flanne.UIExtensions.Menu>();
            Object.DestroyImmediate(newMenuObject.GetComponent<OptionsSetter>());
            ClearMenuEntriesList(menu, backButton);
            return (menu, backButton);
        }

        private static (flanne.UIExtensions.Menu, Button, flanne.UIExtensions.Menu, Button, List<ModOptionComponent>) CreateGenericModsMenu(GameObject templateMenu, OptionsMenuType menuType)
        {
            var (mainMenu, mainBackButton) = CloneAndCleanMenu(templateMenu);
            var (subMenu, subMenuBackButton) = CloneAndCleanMenu(templateMenu);

            var modOptionCompList = new List<ModOptionComponent>();
            var modListHasEntries = false;

            foreach (var modConfigEntries in SortedConfigEntries)
            {
                var entryName = modConfigEntries.Key;
                bool isNotEmptyEntry = false;
                foreach (var configEntry in modConfigEntries.Value)
                {
                    var button = AddButtonFromConfigEntry(subMenuBackButton.gameObject, configEntry, menuType, entryName);
                    if (button == null) continue; // if not in correct menu
                    isNotEmptyEntry = true;
                    modOptionCompList.Add(button);
                    button.gameObject.SetActive(false);
                }

                subMenuBackButton.transform.SetAsLastSibling();

                if (isNotEmptyEntry)
                {
                    if (entryName == ModOptions.ModListButtonName) modListHasEntries = true;
                    else AddButtonFromModName(mainBackButton.gameObject, menuType, entryName);
                }
            }

            if (modListHasEntries) AddButtonFromModName(mainBackButton.gameObject, OptionsMenuType.MainMenu, ModOptions.ModListButtonName);
            mainBackButton.transform.SetAsLastSibling();

            return (mainMenu, mainBackButton, subMenu, subMenuBackButton, modOptionCompList);
        }
        public static void CreateTitleScreenModsMenu()
        {
            if (TitleScreenController == null || ModOptionsMenu != null) return;
            var ingameOptionsMenu = TitleScreenController.optionsMenu.gameObject;

            var genericMenuResult = CreateGenericModsMenu(ingameOptionsMenu, OptionsMenuType.MainMenu);
            ModOptionsMenu = genericMenuResult.Item1;
            ModOptionsBackButton = genericMenuResult.Item2;
            ModOptionsSubmenuMenu = genericMenuResult.Item3;
            ModOptionsSubMenuBackButton = genericMenuResult.Item4;
            TitleScreenModOptionComponents = genericMenuResult.Item5;
        }

        public static void CreatePauseMenuModsMenu()
        {
            if (GameController == null || PauseModOptionsPanel != null) return;
            var ingameOptionsMenu = GameController.optionsMenu.gameObject;

            var genericMenuResult = CreateGenericModsMenu(ingameOptionsMenu, OptionsMenuType.PauseMenu);
            PauseModOptionsPanel = genericMenuResult.Item1;
            PauseModOptionsBackButton = genericMenuResult.Item2;
            PauseModOptionsSubPanel = genericMenuResult.Item3;
            PauseModOptionsSubMenuBackButton = genericMenuResult.Item4;
            PauseMenuModOptionComponents = genericMenuResult.Item5;

            // Resize ingame main pause menu
            var rect = GameController.pauseMenu.GetComponent<RectTransform>();
            rect.sizeDelta = rect.sizeDelta + new Vector2(0, PauseModOptionsButton?.GetComponent<RectTransform>().sizeDelta.y ?? 24f);

            // Resize main mods menu
            var optionsMenuRect = PauseModOptionsPanel.GetComponent<RectTransform>();
            var entryCount = PauseModOptionsPanel.GetComponentsInChildren<Button>().Length;
            var buttonSize = PauseModOptionsBackButton.GetComponent<RectTransform>().sizeDelta.y;
            var verticalPadding = 40;
            optionsMenuRect.sizeDelta = new Vector2(300, verticalPadding + entryCount * buttonSize);

            // Align buttons in menu
            var buttonListRect = PauseModOptionsPanel.transform.GetChild(0).GetComponent<RectTransform>();
            buttonListRect.position = Vector3.zero;
            buttonListRect.sizeDelta = new Vector2(100, entryCount * buttonSize);
        }
    }
}
