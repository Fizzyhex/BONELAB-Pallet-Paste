using BoneLib;
using BoneLib.BoneMenu;
using MelonLoader;
using UnityEngine;

namespace PalletPaste;

internal static class Root
{
    private static MelonPreferences_Category _category;
    private static MelonPreferences_Entry<bool> _boneMenuEnabled;
    
    public static void Create()
    {
        _category = MelonPreferences.CreateCategory("PalletPaste", "Pallet Paste");
        _boneMenuEnabled = _category.CreateEntry("BoneMenuEnabled", true, "BoneMenu Enabled");

        if (!_boneMenuEnabled.Value)
            return;
        
        var page = Page.Root.CreatePage("Pallet Paste", Color.cyan);
        page.CreateFunction("Paste!", Color.cyan, () => Core.OnPaste());

        page.Add(page.CreateFunction("Pastes", Color.cyan, () =>
        {
            var pastePage = new Page("Pastes", Color.white);
            PasteHistory.DrawPasteList(pastePage);
            Menu.OpenPage(pastePage);
        }));
        
        if (HelperMethods.IsAndroid()) return;
        var settingsPage = page.CreatePage("Settings (PC)", Color.white);

        DrawSettings(settingsPage);
    }
    
    private static void DrawSettings(Page settingsPage)
    {
        settingsPage.RemoveAll();
            
        settingsPage.CreateFunction(_boneMenuEnabled.Value ? "Hide from BoneMenu" : "Undo hide from BoneMenu", Color.red, () =>
        {
            if (_boneMenuEnabled.Value)
            {
                Menu.DisplayDialog("Disable this page?",
                    "You will need to manually edit your MelonPreferences file to re-enable this page after game restart. Are you sure?",
                    confirmAction:
                    () =>
                    {
                        _boneMenuEnabled.Value = false;
                        _category.SaveToFile();
                        Menu.CurrentPage = Page.Root;
                        DrawSettings(settingsPage);
                    });
            }
            else
            {
                _boneMenuEnabled.Value = true;
                _category.SaveToFile();
                DrawSettings(settingsPage);
            }
        });
    }
}