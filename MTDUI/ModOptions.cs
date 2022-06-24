#nullable enable

using BepInEx.Configuration;
using MTDUI.Controllers;
using MTDUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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
            else if(entry.SettingType == typeof(int))
            {
                // really really need to do some better stuff here, but I don't care for now
                // if they're not providing anything, all that should be provided is the default value. no changin!
                if(!acceptableValues.Contains((T)entryBase.DefaultValue)) acceptableValues.Add((T)entryBase.DefaultValue);
            }

            ModOptionsMenuController.ConfigEntries.Add(new ModConfigEntry(entryBase, acceptableValues.Cast<object>().ToList()));
        }
    }
}
