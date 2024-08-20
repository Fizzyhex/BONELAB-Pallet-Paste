using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(PalletPaste.Core), "PalletPaste", "1.0.0", "Fizzyhex", null)]
[assembly: MelonColor(20, 194, 215, 255)]
[assembly: MelonAuthorColor(108, 116, 230, 255)]
[assembly: MelonAdditionalDependencies("LabFusion")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace PalletPaste
{
    public class Core : MelonMod
    {
        public override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
            {
                OnPaste();
            }
        }

        static void OnPaste()
        {
            var clipboardContent = GUIUtility.systemCopyBuffer;

            if (!Downloader.IsModPageUrl(clipboardContent))
            {
                return;
            }

            MelonLogger.Msg($"Mod.io link detected: {clipboardContent.Trim()}");
            Downloader.DownloadFromModPageUrl(clipboardContent.Trim());
        }
    }
}