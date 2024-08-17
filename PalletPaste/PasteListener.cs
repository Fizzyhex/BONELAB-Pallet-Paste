using LabFusion.Downloading;
using LabFusion.Downloading.ModIO;
using LabFusion.Utilities;
using MelonLoader;
using UnityEngine;

namespace PalletPaste
{
    [RegisterTypeInIl2Cpp]
    class PasteListener : MonoBehaviour
    {
        // interop...
        public PasteListener(IntPtr ptr) : base(ptr) { }

        public delegate void ModIdCallback(int modId);

        private bool isPasting;

#pragma warning disable IDE0051 // Remove unused private members
        void Update()
#pragma warning restore IDE0051 // Remove unused private members
        {
            bool nowPasting = (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V));

            if (nowPasting != isPasting)
            {
                isPasting = nowPasting;
                MelonLogger.Msg("paste detected");
                OnPaste();
            }
        }

        void OnPaste()
        {
            var clipboardContent = GUIUtility.systemCopyBuffer;
            MelonLogger.Msg($"'{clipboardContent}' was pasted");

            if (!IsModPageUrl(clipboardContent))
            {
                return;
            }

            MelonLogger.Msg($"Mod link detected: {clipboardContent.Trim()}");
            DownloadFromModPageUrl(clipboardContent.Trim());
        }

        void DownloadFromModPageUrl(string modPageUrl)
        {
            ModIOSettings.LoadToken(OnTokenLoaded);

            void OnTokenLoaded(string token)
            {
                var urlSplit = modPageUrl.Split('/');
                var modSlug = urlSplit[^1];

                ModIOManagerPublic.GetModFromSlug(modSlug, (info) => QueueDownload(info, modSlug));
            }
        }

        void QueueDownload(ModCallbackInfo info, string displayName)
        {
            if (info.result == ModResult.FAILED)
            {
                MelonLogger.Msg("Failed to get mod info");
                return;
            }

            MelonLogger.Msg("Got mod info - downloading...");

            var transaction = new ModTransaction()
            {
                ModFile = new(info.data.Id),
                Callback = OnDownloadComplete
            };

            FusionNotifier.Send(new FusionNotification()
            {
                title = "Pallet Paster",
                message = $"{displayName} was added to the download queue!"
            });

            static void OnDownloadComplete(DownloadCallbackInfo transaction)
            {
                if (transaction.result == ModResult.FAILED)
                {
                    MelonLogger.Msg("Download failed!");
                }
                else
                {
                    MelonLogger.Msg("Download succeeded!");
                }
            }

            ModIODownloader.EnqueueDownload(transaction);
        }

        public static bool IsModPageUrl(string text)
        {
            string withoutHttp = text.Replace("http://", "").Replace("https://", "");
            return withoutHttp.Contains("bonelab/m/") && withoutHttp.StartsWith("mod.io");
        }
    }
}