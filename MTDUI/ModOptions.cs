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

        public static void Register<T>(string subMenuName, ConfigEntry<T> entry, List<T>? acceptableValues = null, bool isInPause = false)
        {
            var modConfigEntry = new ModConfigEntry(entry, AcceptableValuesFiller(entry, acceptableValues), isInPause);

            ModOptionsMenuController.ConfigEntries.Add(modConfigEntry);

            if (!ModOptionsMenuController.SortedConfigEntries.ContainsKey(subMenuName)) ModOptionsMenuController.SortedConfigEntries.Add(subMenuName, new List<ModConfigEntry>());
            ModOptionsMenuController.SortedConfigEntries[subMenuName].Add(modConfigEntry);
        }

        public static void RegisterWithPauseAction<T>(string subMenuName, ConfigEntry<T> entry, Action changeMethod, List<T>? acceptableValues = null)
        {
            ModOptionChangeIngamePatch.AddPatchActionToPause(changeMethod);
            Register(subMenuName, entry, acceptableValues, true);
        }
    }
}
