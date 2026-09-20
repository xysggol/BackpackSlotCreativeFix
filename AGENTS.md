# BackpackSlotCreativeFix_API193 Agent 指南

本仓库是《生存战争》插件版（SCAPI）模组 **背包槽创造修复 / Backpack Slot Creative Fix** 的源码与发布仓库。

模组作用：修复「增加背包槽」类模组在创造模式下的通病——被扩大的背包槽会被取之不尽的创造栏物品占用。

## 基本信息

| 项 | 值 |
| --- | --- |
| 模组名 | 背包槽创造修复 Backpack Slot Creative Fix |
| PackageName | `scmod.backpackslotcreativefix` |
| 版本 | 1.0.0 |
| 目标 API | Survivalcraft API `1.9.3.1`（兼容 1.9.x） |
| 目标框架 | `net10.0` |
| 平台 | Windows / Linux / Android（单个 `.scmod` 通用） |
| 许可 | LGPL-3.0（`LICENSE`，另附 GPL-3.0 全文 `LICENSE.GPL-3.0`） |

## GitHub 信息

| 项 | 值 |
| --- | --- |
| GitHub 账号 | `xysggol` |
| 仓库 | https://github.com/xysggol/BackpackSlotCreativeFix |
| 默认分支 | `main`（本地分支名为 `master`，推送用 `git push origin master:main`） |
| 提交身份 | `xysgg` `<225452402+xysggol@users.noreply.github.com>` |
| Releases | https://github.com/xysggol/BackpackSlotCreativeFix/releases |

### 邮箱与隐私（重要）

- 账号开启了「阻止命令行推送暴露我的邮箱」（GH007），**禁止**用私有邮箱 `xysgg_new@outlook.com` 提交/推送。
- 提交必须使用 GitHub noreply 邮箱：`225452402+xysggol@users.noreply.github.com`（即 `用户ID+用户名@users.noreply.github.com`）。
- 本仓库已设置 `user.name=xysgg`、`user.email=225452402+xysggol@users.noreply.github.com`。

### SSH 认证（本机已配置）

- 密钥：`~/.ssh/id_ed25519`（公钥已注册到 GitHub 账号）。
- 本机 22 端口被拒，`~/.ssh/config` 已把 GitHub 指向 443：

  ```
  Host github.com
      HostName ssh.github.com
      Port 443
      User git
      IdentityFile ~/.ssh/id_ed25519
      IdentitiesOnly yes
  ```

- 验证：`ssh -T git@github.com` 应返回 `Hi xysggol! ...`。
- 全局地址改写（让 `https://github.com/...` 自动走 SSH）：

  ```
  git config --global url."git@github.com:".insteadOf "https://github.com/"
  ```

### GPG 签名（本机已配置）

- 签名密钥：`ed25519 D63479F50BA4B12A`
- 指纹：`8215CF979D9A483449BA5982D63479F50BA4B12A`
- UID：`xysgg <225452402+xysggol@users.noreply.github.com>`，**无口令**
- 全局已开启：`commit.gpgsign=true`、`tag.gpgsign=true`、`user.signingkey=<指纹>`、`gpg.program=gpg`
- 只有把该公钥加入 GitHub `Settings → SSH and GPG keys → New GPG key` 后，提交/标签才会显示 **Verified**。
- 验证签名：`git log --show-signature`；打签名标签：`git tag -s v1.0.1 -m "..."`。

### GitHub CLI

- 本机 `gh` 位于 `~/.local/bin/gh`，已登录账号 `xysggol`（token 存于系统 keyring，scope 含 `repo`）。
- 常用：
  - 刷新/重传发行附件：`gh release upload <tag> <files...> --clobber`
  - 更新发行说明：`gh release edit <tag> --notes-file <notes.md>`
  - 改标题：`gh release edit <tag> --title "..."`
- **不要把 token、私钥、口令写入任何仓库文件。**

## 目录结构

```
BackpackSlotCreativeFix_API193.csproj   构建与打包（PostBuild 生成 .scmod）
BackpackFixModLoader.cs                  ModLoader，__ModInitialize 里 Harmony.PatchAll
CreativeInventoryOpenSlotsPatch.cs       Harmony Prefix：核心修复逻辑
modinfo.json                             模组元数据（Name/Version/ApiVersion/PackageName 等）
nuget.config                             NuGet 源（nuget.org + SurvivorcraftAPI 私有源）
AGENTS.md                                本文件
README.md / README.zh-CN.md              英文 / 中文说明
LICENSE / LICENSE.GPL-3.0                LGPL-3.0 / GPL-3.0 全文
.github/workflows/release.yml            打 tag 自动构建与发布
```

## 核心实现（改代码前先读这里）

修复思路：生存模式用 `ComponentInventory`（槽数由 `Database.xml` 的 `SlotsCount` 决定，原版 26）；创造模式改用 `ComponentCreativeInventory`，用 `OpenSlotsCount` 划分「归玩家的个人槽」与「无限创造物品」。背包模组通常只放大了前者，导致新增槽位索引落进创造物品区。

`CreativeInventoryOpenSlotsPatch.cs` 对 `ComponentCreativeInventory.Load` 加 Harmony **Prefix**：在原始 `Load` 执行前，若玩家 `ComponentInventory` 的实际槽数（优先取已加载的 `SlotsCount`，否则取其 `ValuesDictionary["SlotsCount"]` 模板值）大于原 `OpenSlotsCount`，就克隆一份 `ValuesDictionary` 并把 `OpenSlotsCount` 提到该值。

要点：
- `Load` 与 `Save` 都按同一个 `OpenSlotsCount` 读写存档，因此新增个人槽的物品不会丢失。
- 用**克隆**而非原地修改 VD，避免污染共享的模板 `ValuesDictionary`。
- 只增不减（`Math.Max`），未装背包模组时保持原版 26，行为不变。
- 对非玩家实体（无 `ComponentInventory`）返回 0，不生效。

## 构建

需要 **.NET 10 SDK**。

```bash
dotnet build BackpackSlotCreativeFix_API193.csproj -c Release
```

- 产物：`bin/Release/BackpackSlotCreativeFix_API193.scmod`（内部仅含 `BackpackSlotCreativeFix_API193.dll` 与 `modinfo.json`，**不含 README**）。
- 还原依赖需要本目录的 `nuget.config`；私有源 `https://nuget.fury.io/survivalcraftapi` 可匿名只读拉取，无需凭据。
- `bin/`、`obj/` 已被 `.gitignore` 忽略，**不要提交构建产物**。

## 发布流程

在 GitHub 上由 Actions 构建并发布，本机无需上传：

```bash
git tag -s v1.0.1 -m "背包槽创造修复 v1.0.1"
git push origin v1.0.1
```

推送 `v*` tag 会触发 `.github/workflows/release.yml`：在 `ubuntu-latest` 上 `dotnet build`，生成 `.scmod` 与 `.sha256`，再 `gh release create` 建 release 并挂附件（若 release 已存在则 `gh release upload --clobber`）。也可在仓库 Actions 页手动 `Run workflow` 仅测试构建。

注意：改 README 不会改变 `.scmod` 内容；如需刷新已有 release 的附件/说明，用上文 `gh release upload` / `gh release edit`。

## 代码风格

遵循 `SurvivalcraftApi/.editorconfig`：4 空格缩进、K&R 大括号、最大行宽 150、`Nullable disable`、`LangVersion preview`、避免 `var`、公有成员 PascalCase。异常要捕获并 `Log.Error`，不要让游戏崩溃。

## 需要向用户确认的情形

- 任何会改写远端历史的操作（force push、删 tag/release）。
- 版本号、modinfo 字段、许可协议的变更。
- 是否需要 Android 实机验证（本机无设备，只能编译）。
