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
            return;

            void OnTokenLoaded(string token)
            {
                var urlSplit = modPageUrl.Split('/');
                var modSlug = urlSplit[^1];

                ModIOManagerPublic.GetModFromSlug(modSlug, (info) => QueueDownload(info, modSlug));
            }
        }

        private static void QueueDownload(ModCallbackInfo info, string displayName)
        {
            if (info.result == ModResult.FAILED)
            {
                MelonLogger.Msg("Failed to get mod info");

                FusionNotifier.Send(new FusionNotification()
                {
                    Title = "Pallet Paste",
                    Message = $"Failed to fetch mod info for {displayName}",
                    Type = NotificationType.ERROR
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
                Title = "Pallet Paste",
                Message = $"{displayName} was added to the download queue!"
            });

            ModIODownloader.EnqueueDownload(transaction);
            return;

            static void OnDownloadComplete(DownloadCallbackInfo transaction)
            {
                MelonLogger.Msg(transaction.result == ModResult.FAILED ? "Download failed!" : "Download succeeded!");
            }
        }

        public static bool IsModPageUrl(string text)
        {
            var withoutHttp = text.Replace("http://", "").Replace("https://", "");
            return withoutHttp.Contains("bonelab/m/") && withoutHttp.StartsWith("mod.io");
        }
    }
}
