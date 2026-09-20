# Backpack Slot Creative Fix

A generic compatibility fix mod for [Survivalcraft](https://gitee.com/SC-SPM/SurvivalcraftApi) API 1.9.x.

[中文说明 / Chinese](README.zh-CN.md)

---

## The problem this mod solves

Many “more backpack slots” mods enlarge the player's inventory. They work fine in survival mode, but in **creative mode** they share a common bug:

> **The extra backpack slots get occupied by the infinite creative inventory items.**

The game uses two different inventory components:

- Survival mode uses `ComponentInventory`. Its total slot count comes from `SlotsCount` in `Database.xml` (26 in vanilla: the first 10 are the hotbar and the last 16 are the backpack).
- Creative mode uses `ComponentCreativeInventory` instead. Its `OpenSlotsCount` is the number of slots that belong to the player (also 26 in vanilla); only the slots after `OpenSlotsCount` hold the infinite creative items.

Backpack-slot mods usually increase `ComponentInventory.SlotsCount` but forget to update the creative-mode `OpenSlotsCount`. As a result, the added slot indices fall into the creative-item region and show infinite blocks instead of being usable backpack slots.

## How it works

Before `ComponentCreativeInventory.Load` runs, this mod syncs the `OpenSlotsCount` that the method is about to read to the player's actual inventory size:

```
OpenSlotsCount = max(original OpenSlotsCount, player's ComponentInventory slot count)
```

Both `Load` and `Save` use the same `OpenSlotsCount`, so:

- The creative “backpack” category gains `N - 10` **empty, normal personal slots** that can be freely used.
- Saving and loading stay consistent; items are never lost.
- With no backpack mod installed the value stays at the vanilla 26 and nothing changes.

## Compatibility & limitations

- Built and tested against [Survivalcraft](https://gitee.com/SC-SPM/SurvivalcraftApi) API 1.9.3.1.
- Targets backpack mods that enlarge the player's `ComponentInventory` slot count, whether via `Database.xml`/templates or by changing `SlotsCount` around the player inventory load; both are synced automatically.
- This mod does not add backpack slots itself. It only aligns the creative personal-slot count with the player's actual inventory size.
- If a backpack mod uses a completely separate inventory component (not the player's `ComponentInventory`), it cannot be detected automatically.

## Installation

1. Download `BackpackSlotCreativeFix_API193.scmod` from Releases.
2. **PC (Windows / Linux)**: put the `.scmod` into the game's `Mods/` folder.
3. **Android**: push it to `/storage/emulated/0/Survivalcraft2.4_API1.9/Mods`, or simply open the file on the device to install it.
4. Enable it together with your backpack-slot mod. It is recommended to load this mod after (a higher load order value than) the backpack mod.

## Build from source

Requires the .NET 10 SDK.

```bash
dotnet build BackpackSlotCreativeFix_API193.csproj -c Release
```

On success, `BackpackSlotCreativeFix_API193.scmod` is generated in `bin/Release/`.

## Releasing (maintainers)

Pushing a tag matching `v*` triggers the workflow in `.github/workflows/release.yml`. It builds the `.scmod` on GitHub's runners and attaches it (plus a `.sha256` checksum) to the GitHub release, so nothing has to be uploaded from your machine:

```bash
git tag v1.0.0
git push origin v1.0.0
```

## License

Released under the **GNU Lesser General Public License v3.0 (LGPL-3.0)**. See [LICENSE](LICENSE). LGPL-3.0 incorporates the terms of the GNU GPL v3.0, whose full text is in [LICENSE.GPL-3.0](LICENSE.GPL-3.0).
