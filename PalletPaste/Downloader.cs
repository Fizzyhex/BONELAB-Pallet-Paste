using LabFusion.Downloading.ModIO;
using LabFusion.Downloading;
using LabFusion.Utilities;
using MelonLoader;

namespace PalletPaste
{
    public static class Downloader
    {
        public static void DownloadFromModPageUrl(string modPageUrl)
        {
            ModIOSettings.LoadToken(OnTokenLoaded);

            void OnTokenLoaded(string token)
            {
                var urlSplit = modPageUrl.Split('/');
                var modSlug = urlSplit[^1];

                ModIOManagerPublic.GetModFromSlug(modSlug, (info) => QueueDownload(info, modSlug));
            }
        }

        public static void QueueDownload(ModCallbackInfo info, string displayName)
        {
            if (info.result == ModResult.FAILED)
            {
                MelonLogger.Msg("Failed to get mod info");

                FusionNotifier.Send(new FusionNotification()
                {
                    title = "Pallet Paster",
                    message = $"Failed to fetch mod info for {displayName}",
                    type = NotificationType.ERROR
                });

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
