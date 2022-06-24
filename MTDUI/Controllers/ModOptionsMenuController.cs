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

namespace MTDUI.Controllers
{
    public static class ModOptionsMenuController
    {
        // things are a bit repetitive here
        public static List<ModConfigEntry> ConfigEntries = new List<ModConfigEntry>();
        public static Button? ModOptionsButton = null;
        public static Button? ModOptionsBackButton = null;
        public static Panel? ModOptionsPanel = null;
        public static TitleScreenController? TitleScreenController = null;

        public static GameController? GameController = null;
        public static Button? PauseModOptionsButton = null;
        public static Button? PauseModOptionsBackButton = null;
        public static Panel? PauseModOptionsPanel = null;

        public static GameObject? ButtonTemplate = null;
        public static GameObject? PauseButtonTemplate = null;

        public static void OnModOptionsClick()
        {
            if (TitleScreenController != null) TitleScreenController.ChangeState<ModOptionsMenuState>();
        }

        public static void OnPauseModOptionsClick()
        {
            if (GameController != null) GameController.ChangeState<ModOptionsPauseState>();
        }

        public static void AddButtonFromConfigEntry(ModConfigEntry configEntry, OptionsMenuType menuType)
        {
            var template = menuType == OptionsMenuType.MainMenu ? ButtonTemplate : PauseButtonTemplate;
            if (template != null)
            {
                var newButton = UnityEngine.Object.Instantiate(template, template.transform.parent);
                var optionComponent = newButton.AddComponent<ModOptionComponent>();
                optionComponent.Initialize(configEntry);

                newButton.SetActive(true);
            }
        }

        public static void CreateModOptionsButton(OptionsMenuType menuType)
        {
            if (menuType == OptionsMenuType.MainMenu ? ModOptionsButton == null : PauseModOptionsButton == null)
            {
                var optionsButtonObject = menuType == OptionsMenuType.MainMenu ? TitleScreenController?.optionsButton.gameObject : GameController?.optionsButton.gameObject;
                if (optionsButtonObject != null)
                {
                    var newButton = UnityEngine.Object.Instantiate(optionsButtonObject, optionsButtonObject.transform.parent);

                    var newLocalization = new LocalizedString("menu_modoptions");
                    var localizer = newButton.GetComponentInChildren<TextLocalizerUI>();
                    Traverse.Create(localizer).Field("localizedString").SetValue(newLocalization); // force set to new localization

                    // now set it to the proper spot in the hierarchy
                    newButton.transform.SetSiblingIndex(optionsButtonObject.transform.GetSiblingIndex() + 1);

                    if (menuType == OptionsMenuType.MainMenu) ModOptionsButton = newButton.GetComponent<Button>();
                    else PauseModOptionsButton = newButton.GetComponent<Button>();
                }
            }
        }

        public static void CreateModOptionsPanel(OptionsMenuType menuType)
        {
            if (menuType == OptionsMenuType.MainMenu ? ModOptionsPanel == null : PauseModOptionsPanel == null)
            {
                var optionsPanelObject = menuType == OptionsMenuType.MainMenu ? TitleScreenController?.optionsMenuPanel.gameObject : GameController?.optionsMenu.gameObject;
                if (optionsPanelObject != null)
                {
                    var newPanel = UnityEngine.Object.Instantiate(optionsPanelObject, optionsPanelObject.transform.parent);

                    // do a little bit of disabling :)
                    foreach (var button in newPanel.GetComponentsInChildren<Button>())
                    {
                        if (button.name == "Back")
                        {
                            if (menuType == OptionsMenuType.MainMenu) ModOptionsBackButton = button;
                            else PauseModOptionsBackButton = button;
                        }
                        else if (button.name == "SoundVolume")
                        {
                            // a normal template that shouldn't change much on UI changes
                            button.gameObject.SetActive(false);

                            if (menuType == OptionsMenuType.MainMenu) ButtonTemplate = button.gameObject;
                            else PauseButtonTemplate = button.gameObject;
                        }
                        else button.gameObject.SetActive(false);
                    }

                    UnityEngine.Object.DestroyImmediate(newPanel.GetComponent<OptionsSetter>());

                    if (menuType == OptionsMenuType.MainMenu) ModOptionsPanel = newPanel.GetComponent<Panel>();
                    else PauseModOptionsPanel = newPanel.GetComponent<Panel>();

                    // now properly add the options that already exist by this point
                    foreach (var option in ConfigEntries)
                    {
                        AddButtonFromConfigEntry(option, menuType);
                    }

                    var backButton = menuType == OptionsMenuType.MainMenu ? ModOptionsBackButton : PauseModOptionsBackButton;
                    if (backButton != null) backButton.transform.SetAsLastSibling(); // back button should always be at the bottom

                    // do some minor panel trickery to make the pause menu not look horrible
                    if(menuType == OptionsMenuType.PauseMenu)
                    {
                        var rect = GameController?.pauseMenu.GetComponent<RectTransform>();
                        if(rect != null)
                        {
                            rect.sizeDelta = rect.sizeDelta + new Vector2(0, 24);
                            // rect.localPosition = rect.localPosition + new Vector3(0, -12, 0);
                        }

                        var optionsMenuRect = PauseModOptionsPanel?.GetComponent<RectTransform>();
                        if(optionsMenuRect != null)
                        {
                            // assuming 6 options. this could break on future updates. this really should be dynamically checked
                            var currentY = optionsMenuRect.sizeDelta.y;
                            var entryCount = ConfigEntries.Count + 1;
                            if (entryCount < 6) currentY -= (6 - entryCount) * 24;
                            else if(entryCount > 6) currentY += (entryCount - 6) * 24;

                            optionsMenuRect.sizeDelta = new Vector2(300, currentY);

                            // todo: dynamic vertical scaling based on text height
                        }
                    }
                }
            }
        }
    }
}
