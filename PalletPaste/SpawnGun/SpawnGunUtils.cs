using BoneLib.Notifications;
using Il2CppSLZ.Marrow.Warehouse;
using LabFusion.Marrow;
using LabFusion.Utilities;

namespace PalletPaste.SpawnGun;
using Il2CppSLZ.Bonelab;

public static class SpawnGunUtils
{
    public static bool GetSpawnGun(out SpawnGun spawnGun)
    {
        spawnGun = BoneLib.Player.GetComponentInHand<SpawnGun>(BoneLib.Player.RightHand);
        if (!spawnGun) spawnGun = BoneLib.Player.GetComponentInHand<SpawnGun>(BoneLib.Player.LeftHand);
        return spawnGun != null;
    }

    public static bool CopyBarcodeToSpawnGun(SpawnGun spawnGun, Barcode barcode, bool notify=false)
    {
        var crate = CrateFilterer.GetCrate<SpawnableCrate>(barcode);
        if (crate) spawnGun.OnSpawnableSelected(crate);

        if (notify)
        {
            Notifier.Send(new Notification()
            {
                Title = "Pallet Paste",
                Message = $"Copied to Spawn Gun!",
                Type = NotificationType.Success,
            });
        }
        
        return crate != null;
    }
}