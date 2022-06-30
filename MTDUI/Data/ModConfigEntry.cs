using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace MTDUI.Data
{
    public class ModConfigEntry
    {
        public ConfigEntryLocationType Location { get; }

        public ConfigEntryBase EntryConfigBase { get; }

        // unused. this really needs to be expanded once proper sliders for ints and floats are added.
        // nothing I can use in the game right now though, so that might be put on hold
        public int IncrementValue { get; } = 0;

        public List<object> AcceptableValues { get; }

        public ModConfigEntry(ConfigEntryBase entryConfigBase, List<object> acceptableValues, ConfigEntryLocationType location)
        {
            EntryConfigBase = entryConfigBase;
            AcceptableValues = acceptableValues;
            Location = location;
        }
    }
}
