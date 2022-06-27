#nullable enable

using BepInEx.Configuration;
using MTDUI.Controllers;
using MTDUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MTDUI
{
    public static class ModOptions
    {
        public static void Register<T>(ConfigEntry<T> entry, List<T>? acceptableValues = null)
        {
            if(acceptableValues == null) acceptableValues = new List<T>();

            var entryBase = (entry as ConfigEntryBase);

            if (entry.SettingType.IsEnum)
            {
                var enumValues = Enum.GetValues(entryBase.SettingType);
                // add to acceptable values for enum
                foreach (var value in enumValues) if (!acceptableValues.Contains((T)value)) acceptableValues.Add((T)value);
            }
            else if(entry.SettingType == typeof(int) || entry.SettingType != typeof(float))
            {
                // really really need to do some better stuff here, but I don't care for now
                // if they're not providing anything, all that should be provided is the default value. no changin!
                if(!acceptableValues.Contains((T)entryBase.DefaultValue)) acceptableValues.Add((T)entryBase.DefaultValue);
            }
            
            if(entry.SettingType == typeof(bool))
            {
                foreach (var value in new List<object> { false, true }) if (!acceptableValues.Contains((T)value)) acceptableValues.Add((T)value);
            }

            // this is moderately insane and not how this should work
            var asmName = Assembly.GetCallingAssembly().GetName().Name;

            var modConfigEntry = new ModConfigEntry(entryBase, acceptableValues.Cast<object>().ToList());

            ModOptionsMenuController.ConfigEntries.Add(modConfigEntry);

            if (!ModOptionsMenuController.SortedConfigEntries.ContainsKey(asmName)) ModOptionsMenuController.SortedConfigEntries.Add(asmName, new List<ModConfigEntry>());
            ModOptionsMenuController.SortedConfigEntries[asmName].Add(modConfigEntry);
        }
    }
}
