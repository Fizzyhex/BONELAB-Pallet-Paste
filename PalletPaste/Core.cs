using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(PalletPaste.Core), "PalletPaste", "0.0.1", "Fizzyhex", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace PalletPaste
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            ClassInjector.RegisterTypeInIl2Cpp<PasteListener>();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            var gameObject = new GameObject("Pallet Paste Input Listener");
            gameObject.AddComponent<PasteListener>();
        }
    }
}