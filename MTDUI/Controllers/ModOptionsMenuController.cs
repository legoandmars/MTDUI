#nullable enable

using flanne;
using flanne.Core;
using flanne.TitleScreen;
using flanne.UI;
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
        public static Panel? ModOptionsPanel = null;
        public static Panel? ModOptionsSubPanel = null;
        public static Button? ModOptionsSubMenuBackButton = null;

        // Mod Option in Pause Menu
        public static GameController? GameController = null;
        public static Button? PauseModOptionsButton = null;
        public static Button? PauseModOptionsBackButton = null;
        public static Panel? PauseModOptionsPanel = null;
        public static Panel? PauseModOptionsSubPanel = null;
        public static Button? PauseModOptionsSubMenuBackButton = null;

        public static List<ModOptionComponent> ModOptionComponents = new List<ModOptionComponent>();

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

        public static ModOptionComponent? AddButtonFromConfigEntry(ModConfigEntry configEntry, OptionsMenuType menuType, string mod)
        {
            // Check if configEntry needs to be in pausemenu
            if (menuType == OptionsMenuType.PauseMenu && !configEntry.IsInPauseMenu) return null;

            var template = (menuType == OptionsMenuType.MainMenu ? ModOptionsSubMenuBackButton : PauseModOptionsSubMenuBackButton)?.gameObject;
            if (template == null) return null; // Should never happed

            var newButton = Object.Instantiate(template, template.transform.parent);
            newButton.GetComponentInChildren<TextLocalizerUI>().enabled = false;

            var optionComponent = newButton.AddComponent<ModOptionComponent>();
            optionComponent.Initialize(configEntry, mod);

            newButton.SetActive(false);

            return optionComponent;
        }

        public static void AddButtonFromModName(OptionsMenuType menuType, string mod)
        {
            var template = (menuType == OptionsMenuType.MainMenu ? ModOptionsBackButton : PauseModOptionsBackButton)?.gameObject;
            if (template == null) return; //Should never happend

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

            var optionsButtonObject = menuType == OptionsMenuType.MainMenu ? TitleScreenController?.optionsButton.gameObject : GameController?.optionsButton.gameObject;
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

        public static void CreateModOptionsPanel(OptionsMenuType menuType)
        {
            if (menuType == OptionsMenuType.MainMenu ? ModOptionsPanel != null : PauseModOptionsPanel != null) return;

            var optionsPanelObject = menuType == OptionsMenuType.MainMenu ? TitleScreenController?.optionsMenuPanel.gameObject : GameController?.optionsMenu.gameObject;
            if (optionsPanelObject == null) return; // Should never happend

            var newPanel = Object.Instantiate(optionsPanelObject, optionsPanelObject.transform.parent);

            // do a little bit of disabling :)
            foreach (var button in newPanel.GetComponentsInChildren<Button>())
            {
                if (button.name == "Back")
                {
                    if (menuType == OptionsMenuType.MainMenu) ModOptionsBackButton = button;
                    else PauseModOptionsBackButton = button;
                }
                else button.gameObject.SetActive(false); // should be destroyed
            }

            Object.DestroyImmediate(newPanel.GetComponent<OptionsSetter>());

            if (menuType == OptionsMenuType.MainMenu) ModOptionsPanel = newPanel.GetComponent<Panel>();
            else PauseModOptionsPanel = newPanel.GetComponent<Panel>();

            // make submenu 
            var subPanel = Object.Instantiate(newPanel, newPanel.transform.parent);
            if (menuType == OptionsMenuType.MainMenu) ModOptionsSubPanel = subPanel.GetComponent<Panel>();
            else PauseModOptionsSubPanel = subPanel.GetComponent<Panel>();

            if (menuType == OptionsMenuType.MainMenu) ModOptionsSubMenuBackButton = subPanel.transform.Find("Back").GetComponent<Button>();
            else PauseModOptionsSubMenuBackButton = subPanel.transform.Find("Back").GetComponent<Button>();

            // get all config files so we can make per-mod submenus

            foreach (var component in ModOptionComponents)
            {
                if (component != null)
                {
                    component.gameObject.SetActive(false);
                    Object.Destroy(component);
                }
            }
            ModOptionComponents = new List<ModOptionComponent>();
            var entryCount = 1;

            foreach (var modConfigEntries in SortedConfigEntries)
            {
                var name = modConfigEntries.Key;
                bool hasConfigEntries = false;
                foreach (var configEntry in modConfigEntries.Value)
                {
                    // create button for each entry
                    var button = AddButtonFromConfigEntry(configEntry, menuType, name);
                    if (button != null) // Prevent button that should not be in pause menu
                    {
                        hasConfigEntries = true;
                        ModOptionComponents.Add(button);
                        button.gameObject.SetActive(false);
                    }
                }
                // submenu back button should always be last
                var submenuBackbutton = menuType == OptionsMenuType.MainMenu ? ModOptionsSubMenuBackButton : PauseModOptionsSubMenuBackButton;
                if (submenuBackbutton != null) submenuBackbutton.transform.SetAsLastSibling(); // back button should always be at the bottom

                // create buttons for each mod, if necessary
                if (hasConfigEntries)
                {
                    AddButtonFromModName(menuType, name);
                    entryCount++;
                }
            }

            var backButton = menuType == OptionsMenuType.MainMenu ? ModOptionsBackButton : PauseModOptionsBackButton;
            if (backButton != null) backButton.transform.SetAsLastSibling(); // back button should always be at the bottom

            // do some minor panel trickery to make the pause menu not look horrible
            if (menuType != OptionsMenuType.PauseMenu) return;

            // Here only for Pause Menu
            var rect = GameController?.pauseMenu.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = rect.sizeDelta + new Vector2(0, 24);
                // rect.localPosition = rect.localPosition + new Vector3(0, -12, 0);
            }

            var optionsMenuRect = PauseModOptionsPanel?.GetComponent<RectTransform>();
            if (optionsMenuRect != null)
            {
                // assuming 6 options. this could break on future updates. this really should be dynamically checked
                var currentY = optionsMenuRect.sizeDelta.y;

                currentY += (entryCount - 6) * 24;

                optionsMenuRect.sizeDelta = new Vector2(300, currentY);
            }
        }
    }
}
