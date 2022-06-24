using flanne;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace MTDUI.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(LocalizationSystem))]
    [HarmonyPatch("Init", MethodType.Normal)]
    internal class LocalizerInjectionPatch
    {
        private static void Postfix(LocalizationSystem __instance)
        {
            // inject specific things into all dictionary values
            // traverse doesn't quite work with this for some reason. look more into that later
            // var values = Traverse.Create<LocalizationSystem>().Fields();
            var values = typeof(LocalizationSystem).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var field in values)
            {
                var val = field.GetValue(null);

                if(val.GetType() == typeof(Dictionary<string, string>))
                {
                    // property is a dictionary, we need to do stuff with it
                    var valueDictionary = (Dictionary<string, string>)val;
                    valueDictionary.Add("menu_modoptions", "Mod Options");
                    // todo more dynamic way of adding lots of things. localization should be a nice easy thing for modders.
                }
            }
        }
    }
}
