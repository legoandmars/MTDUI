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
        public static readonly string ModListButtonName = "Mod List";
        private static readonly int maxItemInSubmenu = 8;
        private static readonly int maxSubmenusWithSameName = 4;

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

        public static void Register<T>(ConfigEntry<T> entry, List<T>? acceptableValues = null, ConfigEntryLocationType location = ConfigEntryLocationType.Everywhere, string subMenuName = "")
        {
            var modConfigEntry = new ModConfigEntry(entry, AcceptableValuesFiller(entry, acceptableValues), location);

            if (subMenuName == "") subMenuName = Assembly.GetCallingAssembly().GetName().Name;

            if (location == ConfigEntryLocationType.Everywhere || location == ConfigEntryLocationType.MainOnly)
                CleanRegister(subMenuName, ModOptionsMenuController.TitleConfigEntries, modConfigEntry);
            if (location == ConfigEntryLocationType.Everywhere || location == ConfigEntryLocationType.PauseOnly)
                CleanRegister(subMenuName, ModOptionsMenuController.PauseConfigEntries, modConfigEntry);
        }

        private static void CleanRegister(string submenuName, Dictionary<string, List<ModConfigEntry>> configEntries, ModConfigEntry modConfigEntry)
        {
            var _submenuName = submenuName;
            var isSubmenuFull = false;
            var counter = 1;
            do
            {
                if (!configEntries.ContainsKey(_submenuName)) configEntries.Add(_submenuName, new List<ModConfigEntry>());
                if (configEntries[_submenuName].Count >= maxItemInSubmenu)
                {
                    counter++;
                    _submenuName = submenuName + " " + counter;
                    isSubmenuFull = true;
                }
                else isSubmenuFull = false;
            } while (isSubmenuFull || counter >= maxSubmenusWithSameName);
            if (isSubmenuFull)
            {
                Debug.LogError("Too many registrations with the same name");
                return;
            }
            configEntries[_submenuName].Add(modConfigEntry);
        }

        public static void RegisterOptionInModList<T>(ConfigEntry<T> entry, List<T>? acceptableValues = null)
        {
            Register(entry, acceptableValues, ConfigEntryLocationType.MainOnly, ModListButtonName);
        }
    }
}
