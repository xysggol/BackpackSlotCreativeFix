using System;
using System.Collections.Generic;
using GameEntitySystem;
using HarmonyLib;
using TemplatesDatabase;

namespace Game {
    [HarmonyPatch(typeof(ComponentCreativeInventory), nameof(ComponentCreativeInventory.Load))]
    internal static class CreativeInventoryOpenSlotsPatch {
        static void Prefix(ComponentCreativeInventory __instance, ref ValuesDictionary valuesDictionary) {
            int playerSlotsCount = GetPlayerSlotsCount(__instance);
            if (playerSlotsCount <= valuesDictionary.GetValue<int>("OpenSlotsCount", 0)) {
                return;
            }
            valuesDictionary = CloneWithOpenSlotsCount(valuesDictionary, playerSlotsCount);
        }

        static int GetPlayerSlotsCount(ComponentCreativeInventory creativeInventory) {
            Entity entity = creativeInventory.Entity;
            if (entity == null) {
                return 0;
            }
            ComponentInventory inventory = entity.FindComponent<ComponentInventory>();
            if (inventory == null) {
                return 0;
            }
            ValuesDictionary valuesDictionary = inventory.ValuesDictionary;
            int templateSlotsCount = valuesDictionary?.GetValue<int>("SlotsCount", 0) ?? 0;
            return Math.Max(inventory.SlotsCount, templateSlotsCount);
        }

        static ValuesDictionary CloneWithOpenSlotsCount(ValuesDictionary source, int openSlotsCount) {
            ValuesDictionary clone = [];
            clone.DatabaseObject = source.DatabaseObject;
            foreach (KeyValuePair<string, object> pair in source) {
                clone.SetValue(pair.Key, pair.Value);
            }
            clone.SetValue("OpenSlotsCount", openSlotsCount);
            return clone;
        }
    }
}
