using LabFusion.Downloading.ModIO;
using MelonLoader;

namespace PalletPaste
{
    public static class ModIOManagerPublic
    {
        // This is a modified version of Fusion's ModIOManager.GetMod method that accepts a
        // slug instead of mod id
        public static void GetModFromSlug(string modSlug, ModCallback modCallback)
        {
            var url = $"{ModIOSettings.GameApiPath}@{modSlug}";

            MelonLogger.Msg($"Downloading mod file from '{url}'...");

            ModIOSettings.LoadToken(OnTokenLoaded);

            void OnTokenLoaded(string token)
            {
                // If the token is null, it likely didn't load
                if (string.IsNullOrWhiteSpace(token))
                {
                    modCallback?.Invoke(ModCallbackInfo.FailedCallback);

                    return;
                }

                System.Reflection.MethodInfo CoGetMod = typeof(ModIOManager).GetMethod(
                    "CoGetMod",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                MelonCoroutines.Start(
                    (System.Collections.IEnumerator)CoGetMod.Invoke(null, new object[] { url, token, modCallback }));
            }
        }
    }
}
