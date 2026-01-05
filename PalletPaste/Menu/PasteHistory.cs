using BoneLib.BoneMenu;
using BoneLib.Notifications;
using Il2CppInterop.Runtime;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using LabFusion.Network;
using LabFusion.Senders;
using MelonLoader;
using PalletPaste.SpawnGun;
using UnityEngine;

namespace PalletPaste;

public static class PasteHistory
{
    public class MockPallet
    {
        public readonly Barcode Barcode;
        public readonly string Title;

        public Pallet Pallet
        {
            get
            {
                if (_pallet != null) return _pallet;
                if (!AssetWarehouse.Instance.TryGetPallet(Barcode, out var result)) return _pallet;
                _pallet = result;
                return result;

            }
        }

        private Pallet _pallet;
        
        public MockPallet(Pallet pallet)
        {
            Barcode = pallet.Barcode;
            Title = pallet.Title;
        }
    }
    
    public struct Paste(Pallet pallet)
    {
        public readonly MockPallet MockPallet = new MockPallet(pallet);
    }
    
    private static List<Paste> pasteHistory = [];

    public static void Append(Paste paste)
    {
        pasteHistory.Insert(0, paste);
        if (pasteHistory.Count > 200) pasteHistory.RemoveAt(pasteHistory.Count - 1);
    }
    
    private static void DrawCrate(Page page, Crate crate)
    {
        page.Add(new FunctionElement(crate.Title, Color.white, () =>
        {
            MelonLogger.Msg($"{crate.Barcode} selected in menu");

            if (crate.GetIl2CppType() == Il2CppType.Of<SpawnableCrate>() && SpawnGunUtils.GetSpawnGun(out var spawnGun))
            {
                SpawnGunUtils.CopyBarcodeToSpawnGun(spawnGun, crate.Barcode, notify: true);
            }
            else if (crate.GetIl2CppType() == Il2CppType.Of<SpawnableCrate>())
            {
                Notifier.Send(new Notification
                {
                    Title = "Pallet Paste",
                    Message = $"Hold a spawn gun to copy this spawnable.",
                    Type = NotificationType.Information,
                });
            }
            else if (crate.GetIl2CppType() == Il2CppType.Of<LevelCrate>())
            {
                var context = NetworkInfo.IsClient
                    ? "Click YES to send a level request to the lobby host."
                    : "Click YES to load the selected level";

                Menu.DisplayDialog($"Swap maps to {crate.Title}?", context, confirmAction:
                    () =>
                    {
                        if (NetworkInfo.IsClient)
                        {
                            LoadSender.SendLevelRequest(crate.Cast<LevelCrate>());

                            Notifier.Send(new Notification()
                            {
                                Title = "Requested Level",
                                Message = $"Sent a level request for {crate.Title}!",
                            });
                        }
                        else
                        {
                            SceneStreamer.Load(crate.Barcode);
                        }
                    });
            }
            else
            {
                Menu.DisplayDialog($"Swap avatar to {crate.Title}?", "Click YES to swap your current avatar.",
                    confirmAction:
                    () => { BoneLib.Player.RigManager.SwapAvatarCrate(crate.Barcode); });
            }
        }));
    }
    
    private static void DrawPaste(Page page, Paste paste)
    {
        page.Add(new FunctionElement($"Crates ({paste.MockPallet.Pallet.Crates.Count}):", Color.cyan, () => {}));
        foreach (var crate in paste.MockPallet.Pallet.Crates) DrawCrate(page, crate);
    }
    
    public static void DrawPasteList(Page page)
    {
        foreach (var paste in pasteHistory)
        {
            page.Add(new FunctionElement(paste.MockPallet.Pallet.name, Color.white, () =>
            {
                var subPage = new Page(paste.MockPallet.Pallet.name, Color.white);
                DrawPaste(subPage, paste);
                Menu.OpenPage(subPage);
            }));
        }
    }
}