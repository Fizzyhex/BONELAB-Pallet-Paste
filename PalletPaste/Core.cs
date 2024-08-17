using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(PalletPaste.Core), "PalletPaste", "1.0.0", "Fizzyhex", null)]
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