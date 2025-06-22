using BoneLib.Notifications;
using LabFusion.Downloading.ModIO;
using LabFusion.Downloading;
using MelonLoader;

namespace PalletPaste
{
    public static class DownloadUtils
    {
        public static void DownloadFromModPageUrl(string modPageUrl, bool notify=false)
        {
            ModIOSettings.LoadToken(OnTokenLoaded);
            return;

            void OnTokenLoaded(string token)
            {
                var urlSplit = modPageUrl.Split('/');
                var modSlug = urlSplit[^1];

                ModIOManagerPublic.GetModFromSlug(modSlug, (info) => QueueDownload(info, modSlug, notify: notify));
            }
        }

        private static void QueueDownload(ModCallbackInfo info, string displayName, bool notify=false)
        {
            if (info.Result == ModResult.FAILED)
            {
                MelonLogger.Msg("Failed to get mod info");

                if (notify)
                {
                    Notifier.Send(new Notification()
                    {
                        Title = "Pallet Paste",
                        Message = $"Failed to fetch mod info for {displayName}",
                        Type = NotificationType.Error
                    });
                }

                return;
            }

            MelonLogger.Msg("Got mod info - downloading...");

            var transaction = new ModTransaction()
            {
                ModFile = new ModIOFile(info.Data.ID),
                Callback = OnDownloadComplete
            };

            if (notify)
            {
                Notifier.Send(new Notification()
                {
                    Title = "Pallet Paste",
                    Message = $"{displayName} was added to the download queue!"
                });
            }

            ModIODownloader.EnqueueDownload(transaction);
            return;

            static void OnDownloadComplete(DownloadCallbackInfo transaction)
            {
                PasteHistory.Append(new PasteHistory.Paste(transaction.pallet));
                
                MelonLogger.Msg(transaction.result == ModResult.FAILED ? "Download failed!" : "Download succeeded!");
                MelonLogger.Msg($"Pallet: {transaction.pallet.Title} ({transaction.pallet.Barcode.ID}) by @{transaction.pallet.Author}");
                MelonLogger.Msg("Crates:");

                foreach (var crate in transaction.pallet.Crates)
                {
                    MelonLogger.Msg($"- {crate.Title} ({crate.Barcode.ID})");
                    crate.Tags.Add("<color=#07f5f5>Pallet Paste</color>");
                }
            }
        }

        public static bool IsModPageUrl(string text)
        {
            var withoutHttp = text.Replace("http://", "").Replace("https://", "");
            return withoutHttp.Contains("bonelab/m/") && withoutHttp.StartsWith("mod.io");
        }
    }
}
