#nullable enable

using BepInEx.Configuration;
using MTDUI.Controllers;
using MTDUI.Data;
using MTDUI.HarmonyPatches.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MTDUI
{
    public enum ConfigEntryLocationType
    {
        Everywhere,
        PauseOnly,
        MainOnly
    }

    public static class ModOptions
    {
        private static List<object> AcceptableValuesFiller<T>(ConfigEntry<T> entry, List<T>? acceptableValues = null)
        {
            if (acceptableValues == null) acceptableValues = new List<T>();

            var entryBase = (entry as ConfigEntryBase);

            if (entry.SettingType.IsEnum)
            {
                var enumValues = Enum.GetValues(entryBase.SettingType);
                // add to acceptable values for enum
                foreach (var value in enumValues) if (!acceptableValues.Contains((T)value)) acceptableValues.Add((T)value);
            }
            else if (entry.SettingType == typeof(int) || entry.SettingType != typeof(float))
            {
                // really really need to do some better stuff here, but I don't care for now
                // if they're not providing anything, all that should be provided is the default value. no changin!
                if (!acceptableValues.Contains((T)entryBase.DefaultValue)) acceptableValues.Add((T)entryBase.DefaultValue);
            }

            if (entry.SettingType == typeof(bool))
            {
                foreach (var value in new List<object> { false, true }) if (!acceptableValues.Contains((T)value)) acceptableValues.Add((T)value);
            }

            return acceptableValues.Cast<object>().ToList();
        }

        public static void Register<T>(ConfigEntry<T> entry, List<T>? acceptableValues = null, ConfigEntryLocationType location = ConfigEntryLocationType.MainOnly, string subMenuName = "")
        {
            var modConfigEntry = new ModConfigEntry(entry, AcceptableValuesFiller(entry, acceptableValues), location);

            ModOptionsMenuController.ConfigEntries.Add(modConfigEntry);

            if (subMenuName == "") subMenuName= Assembly.GetCallingAssembly().GetName().Name;

            // TODO: add fallback for registering more than 9 config under the same submenuname (smtg like submenuname 2) automatically
            // Else the back button is not accessible
            if (!ModOptionsMenuController.SortedConfigEntries.ContainsKey(subMenuName)) ModOptionsMenuController.SortedConfigEntries.Add(subMenuName, new List<ModConfigEntry>());
            ModOptionsMenuController.SortedConfigEntries[subMenuName].Add(modConfigEntry);
        }
    }
}
