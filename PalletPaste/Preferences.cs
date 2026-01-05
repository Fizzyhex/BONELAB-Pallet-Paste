using BoneLib;
using BoneLib.BoneMenu;
using MelonLoader;
using UnityEngine;

namespace PalletPaste
{
    internal class PageExtras(MelonPreferences_Category category, Page page)
    {
        public BoolElement CreateBoolPref(ref MelonPreferences_Entry<bool> pref,  string name, Color color, bool startValue, Action<bool> callback = null, string prefName = null)
        {
            prefName ??= name;
            
            if(!category.HasEntry(prefName))
                pref = category.CreateEntry(prefName, startValue);
            
            var entry = pref;
            
            return page.CreateBool(pref.DisplayName, color, entry.Value, (value) =>
            {
                callback?.InvokeActionSafe(value);
                entry.Value = value;
                category.SaveToFile();
            });
        }
    }
}