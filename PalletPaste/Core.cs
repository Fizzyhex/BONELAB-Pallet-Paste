using LabFusion.Utilities;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(PalletPaste.Core), "PalletPaste", "2.0.0", "Fizzyhex", null)]
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

        internal static void OnPaste(bool verbose = false)
        {
            var clipboardContent = GUIUtility.systemCopyBuffer.Trim();

            if (!Downloader.IsModPageUrl(clipboardContent))
            {
                if (verbose)
                {
                    FusionNotifier.Send(new FusionNotification()
                    {
                        Title = "Pallet Paste",
                        Message = $"No mod.io link detected on clipboard!",
                        Type = NotificationType.ERROR
                    });
                }
                
                return;
            }

            MelonLogger.Msg($"Mod.io link detected: {clipboardContent.Trim()}");
            Downloader.DownloadFromModPageUrl(clipboardContent.Trim());
        }

        public override void OnInitializeMelon()
        {
            PalletPastePage.Create();
        }
    }
}