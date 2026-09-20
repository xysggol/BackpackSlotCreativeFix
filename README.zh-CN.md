# 背包槽创造修复

一个用于[生存战争插件版](https://gitee.com/SC-SPM/SurvivalcraftApi)（SCAPI 1.9.x）的通用兼容修复模组。

[English](README.md)

---

## 这个模组解决什么问题

许多“增加背包槽”模组会扩大玩家的背包容量。它们在生存模式下工作正常，但在**创造模式**下会出现一个通病：

> **新增的背包槽会被取之不尽的创造栏物品占用。**

原因是游戏为两种模式准备了不同的库存组件：

- 生存模式使用 `ComponentInventory`，由 `Database.xml` 里的 `SlotsCount` 决定总槽数（原版为 26，其中前 10 格是快捷栏，后 16 格是背包）。
- 创造模式改用 `ComponentCreativeInventory`，它的 `OpenSlotsCount` 表示“归玩家自己所有的槽位数”（原版同样是 26），`OpenSlotsCount` 之后的槽位才是无限创造物品。

“增加背包槽”模组通常只扩大了 `ComponentInventory` 的 `SlotsCount`，却漏掉了创造模式的 `OpenSlotsCount`。于是创造模式里新增的槽位索引落进了创造物品区，显示为无限方块，无法作为普通背包槽使用。

## 修复原理

本模组在 `ComponentCreativeInventory.Load` 执行前，把它要读取的 `OpenSlotsCount` 同步为玩家背包的实际槽数：

```
OpenSlotsCount = max(原 OpenSlotsCount, 玩家 ComponentInventory 实际槽数)
```

`Load` 与 `Save` 都使用同一个 `OpenSlotsCount`，因此：

- 创造模式“背包”分类会多出 `N - 10` 个**空的普通个人槽**，可自由存取。
- 存档读写一致，背包里的物品不会丢失。
- 未安装任何背包扩展模组时，数值保持原版 26，行为完全不变。

## 兼容性与限制

- 通过与[生存战争插件版 1.9.3.1](https://gitee.com/SC-SPM/SurvivalcraftApi/releases/tag/API_1.9.3.1) 共同测试/编译。
- 目标为“扩大玩家 `ComponentInventory` 槽数”的背包模组，无论是通过修改 `Database.xml`/模板，还是在玩家库存组件加载前后修改 `SlotsCount`，均可自动同步。
- 本模组不会自行增加背包槽，只负责把创造模式的个人槽数量对齐到玩家背包实际槽数。
- 若某个背包模组使用完全独立的库存组件（不改变玩家 `ComponentInventory`），本模组无法自动识别。

## 安装

1. 从 Releases 下载 `BackpackSlotCreativeFix_API193.scmod`。
2. **PC（Windows / Linux）**：把 `.scmod` 放入游戏目录下的 `Mods/` 文件夹。
3. **Android**：把 `.scmod` 推送到 `/storage/emulated/0/Survivalcraft2.4_API1.9/Mods`，或在设备上直接打开该文件安装。
4. 与对应的“增加背包槽”模组一同启用即可。建议本模组的加载顺序晚于（LoadOrder数值大于）背包模组。

## 从源码构建

需要 .NET 10 SDK。

```bash
dotnet build BackpackSlotCreativeFix_API193.csproj -c Release
```

构建成功后会在 `bin/Release/` 下生成 `BackpackSlotCreativeFix_API193.scmod`。

## 发布新版本（维护者）

推送一个匹配 `v*` 的 tag 会触发 `.github/workflows/release.yml`：由 GitHub 的 runner 构建 `.scmod`，并自动把模组文件与 `.sha256` 校验文件挂到对应的 GitHub Release 上，本机无需上传任何东西：

```bash
git tag v1.0.0
git push origin v1.0.0
```

## 许可证

本项目以 **GNU Lesser General Public License v3.0（LGPL-3.0）** 发布，详见 [LICENSE](LICENSE)。LGPL-3.0 内含对 GNU GPL v3.0 的引用，其完整文本见 [LICENSE.GPL-3.0](LICENSE.GPL-3.0)。
