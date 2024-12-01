using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(PalletPaste.Core), "PalletPaste", "1.0.2", "Fizzyhex", null)]
[assembly: MelonColor(alpha: 255, red: 20, green: 194, blue: 215)]
[assembly: MelonAuthorColor(alpha: 255, red: 108, green: 116, blue: 230)]
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