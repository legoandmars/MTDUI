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
        public static Dictionary<string, List<ModConfigEntry>> TitleConfigEntries { get; } = new Dictionary<string, List<ModConfigEntry>>();
        public static Dictionary<string, List<ModConfigEntry>> PauseConfigEntries { get; } = new Dictionary<string, List<ModConfigEntry>>();

        // Mod Option in Title Screen
        public static bool isTitleScreenModMenuCreated
        {
            get { return TitleModOptionsButton != null && TitleMenu != null; }
        }
        private static TitleScreenController? _titleScreenController = null;
        public static flanne.UIExtensions.Menu? TitleMenu { get; private set; }
        public static flanne.UIExtensions.Menu? TitleSubmenu { get; private set; }
        public static Button? TitleModOptionsButton { get; private set; }
        public static Button? TitleMenuBackButton { get; private set; }
        public static Button? TitleSubmenuBackButton { get; private set; }
        public static List<ModOptionComponent> TitleScreenModOptionComponents = new List<ModOptionComponent>();

        // Mod Option in Pause Menu
        public static bool isPauseMenuModMenuCreated
        {
            get { return PauseModOptionsButton != null && PauseMenu != null; }
        }
        private static GameController? _gameController = null;
        public static flanne.UIExtensions.Menu? PauseMenu { get; private set; }
        public static flanne.UIExtensions.Menu? PauseSubmenu { get; private set; }
        public static Button? PauseModOptionsButton { get; private set; }
        public static Button? PauseMenuBackButton { get; private set; }
        public static Button? PauseSubmenuBackButton { get; private set; }
        public static List<ModOptionComponent> PauseMenuModOptionComponents = new List<ModOptionComponent>();

        private static void OnModClick(string modName)
        {
            ModOptionsSubmenuState.CurrentSubmenu = modName;
            if (_titleScreenController != null) _titleScreenController.ChangeState<ModOptionsSubmenuState>();
        }

        private static void OnPauseModClick(string modName)
        {
            ModOptionsPauseSubmenuState.CurrentSubmenu = modName;
            if (_gameController != null) _gameController.ChangeState<ModOptionsPauseSubmenuState>();
        }

        private static bool AreEntryAndMenuCompatible(OptionsMenuType menu, ConfigEntryLocationType location)
        {
            if (location == ConfigEntryLocationType.Everywhere) return true;
            if (menu == OptionsMenuType.PauseMenu) return location == ConfigEntryLocationType.PauseOnly;
            else return location == ConfigEntryLocationType.MainOnly;
        }

        private static ModOptionComponent? AddButtonFromConfigEntry(GameObject template, ModConfigEntry configEntry, OptionsMenuType menuType, string mod)
        {
            // Check if configEntry needs to be implemented in the current menu
            if (!AreEntryAndMenuCompatible(menuType, configEntry.Location)) return null;

            var newButton = Object.Instantiate(template, template.transform.parent);
            newButton.GetComponentInChildren<TextLocalizerUI>().enabled = false;

            var optionComponent = newButton.AddComponent<ModOptionComponent>();
            optionComponent.Initialize(configEntry, mod);

            newButton.SetActive(false);

            return optionComponent;
        }

        private static void AddButtonFromModName(GameObject template, OptionsMenuType menuType, string mod)
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

        private static Button CreateModOptionsButton(GameObject template)
        {
            var newButton = Object.Instantiate(template, template.transform.parent);

            var newLocalization = new LocalizedString("menu_modoptions");
            var localizer = newButton.GetComponentInChildren<TextLocalizerUI>();
            Traverse.Create(localizer).Field("localizedString").SetValue(newLocalization); // force set to new localization

            newButton.transform.SetSiblingIndex(template.transform.GetSiblingIndex() + 1);
            return newButton.GetComponent<Button>();
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

            var configEntries = menuType == OptionsMenuType.MainMenu ? TitleConfigEntries : PauseConfigEntries;
            foreach (var modConfigEntries in configEntries)
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
        public static void CreateTitleScreenModsMenu(TitleScreenController titleScreenController)
        {
            if (isTitleScreenModMenuCreated) return;
            _titleScreenController = titleScreenController;

            TitleModOptionsButton = CreateModOptionsButton(_titleScreenController.mainMenu.transform.GetChild(2).gameObject);
            var genericMenuResult = CreateGenericModsMenu(_titleScreenController.optionsMenu.gameObject, OptionsMenuType.MainMenu);
            TitleMenu = genericMenuResult.Item1;
            TitleMenuBackButton = genericMenuResult.Item2;
            TitleSubmenu = genericMenuResult.Item3;
            TitleSubmenuBackButton = genericMenuResult.Item4;
            TitleScreenModOptionComponents = genericMenuResult.Item5;
        }

        public static void CreatePauseMenuModsMenu(GameController gameController)
        {
            if (isPauseMenuModMenuCreated) return;
            _gameController = gameController;

            PauseModOptionsButton = CreateModOptionsButton(_gameController.optionsButton.gameObject);
            var genericMenuResult = CreateGenericModsMenu(_gameController.optionsMenu.gameObject, OptionsMenuType.PauseMenu);
            PauseMenu = genericMenuResult.Item1;
            PauseMenuBackButton = genericMenuResult.Item2;
            PauseSubmenu = genericMenuResult.Item3;
            PauseSubmenuBackButton = genericMenuResult.Item4;
            PauseMenuModOptionComponents = genericMenuResult.Item5;

            // Resize ingame main pause menu
            var rect = _gameController.pauseMenu.GetComponent<RectTransform>();
            rect.sizeDelta = rect.sizeDelta + new Vector2(0, PauseModOptionsButton?.GetComponent<RectTransform>().sizeDelta.y ?? 24f);

            // Resize custom mods list menu
            var optionsMenuRect = PauseMenu.GetComponent<RectTransform>();
            var entryCount = PauseMenu.GetComponentsInChildren<Button>().Length;
            var buttonSize = PauseMenuBackButton.GetComponent<RectTransform>().sizeDelta.y;
            var verticalPadding = 40;
            optionsMenuRect.sizeDelta = new Vector2(300, verticalPadding + entryCount * buttonSize);

            // Align buttons in menu
            var buttonListRect = PauseMenu.transform.GetChild(0).GetComponent<RectTransform>();
            buttonListRect.position = Vector3.zero;
            buttonListRect.sizeDelta = new Vector2(100, entryCount * buttonSize);
        }
    }
}
