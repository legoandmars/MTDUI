# MTDUI
A simple UI library for [20 Minutes Till Dawn](https://store.steampowered.com/app/1966900/20_Minutes_Till_Dawn/).

## Installation
If your game isn't modded with BepinEx, DO THAT FIRST! Simply go to the [latest BepinEx release](https://github.com/BepInEx/BepInEx/releases) and extract BepinEx_x64_VERSION.zip directly into your game's folder, then run the game once to install BepinEx properly.

Next, go to the [latest release of this mod](https://github.com/legoandmars/MTDUI/releases/latest) and extract it directly into your game's folder. Make sure it's extracted directly into your game's folder and not into a subfolder!


## For Developers
MTDUI is primarily meant to be used in conjunction with the BepInEx configuration system.

**NOTE:** MTDUI is still in early development, and as such only currently supports a few types of config options:
| ConfigEntry Type  | Supported |
| ------------- | ------------- |
| Enum  | ✅  |
| Int  | ✅  |
| Float  | ✅ |
| Bool  | ✅  |
| String  | ❌  |
| Others?  | ❌  |

Support for new ConfigEntry types will be added as the library matures.

### Example Usage

```cs
using MTDUI;
// Create a new configuration file using the BepInEx config system
var customFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "GameSpeed.cfg"), true);

// Add a config option
var gameSpeed = customFile.Bind("General", "Game Speed", 1, "The speed at which the game runs.");

// Register the config option with MTDUI's in-game mod settings
// 1, 2, 3, 4, 5, and 10 are valid options that can be selected
// NOTE: Enums and Bools will have this list automatically filled, but you need to manually add values for ints/floats
// This is visible on the title screen and pause menu by default
ModOptions.Register(gameSpeed, new List<int>(){ 1, 2, 3, 4, 5, 10 });
// gameSpeed should now be updated when changed in the ingame UI (or when the cfg file is manually modified)
// Use gameSpeed.Value when you want to actually use the value

// Other options:
// You can control which menu a setting will appear on by using ConfigEntryLocationType
// This can be useful if you have settings you don't want people to change mid-run
ModOptions.Register(gameSpeed, new List<int>(){ 1, 2, 3, 4, 5, 10 }, ConfigEntryLocationType.PauseOnly); // only appears in the in-game pause menu
ModOptions.Register(gameSpeed, new List<int>(){ 1, 2, 3, 4, 5, 10 }, ConfigEntryLocationType.MainOnly); // only appears on the title screen
// You can make the option appear on a custom sub-menu by passing a string
ModOptions.Register(gameSpeed, new List<int>(){ 1, 2, 3, 4, 5, 10 }, ConfigEntryLocationType.Everywhere, "Custom SubMenu"); // appears in a "Custom SubMenu" tab


// You can use RegisterOptionInModList to add configs to a "Mod List" subpanel
// This section is mostly intended to be used for activation/deactivation, as to not clutter up the submenu with inactive mods
// The expected usage is to disable the rest of the plugin if enabled
activateMod = Config.Bind("General", "Activation", true, "If false, the mod does not load");
ModOptions.RegisterOptionInModList(activateMod);
if (!activateMod.Value) return;
```

For more information on how the BepInEx config system works, check out [the official documentation](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/4_configuration.html).