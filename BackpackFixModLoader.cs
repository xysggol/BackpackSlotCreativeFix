using System;
using Engine;
using HarmonyLib;

namespace Game {
    public class BackpackFixModLoader : ModLoader {
        public const string PackageName = "scmod.backpackslotcreativefix";

        public override void __ModInitialize() {
            try {
                new Harmony(PackageName).PatchAll();
                Log.Information("[BackpackSlotCreativeFix] Harmony patches applied.");
            }
            catch (Exception e) {
                Log.Error($"[BackpackSlotCreativeFix] Failed to apply Harmony patches: {e}");
            }
        }
    }
}
