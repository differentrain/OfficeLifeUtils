# OfficeLifeUtils
职场浮生记Office Life 小工具类MOD。

此项目依赖于 [BepInEx](https://github.com/BepInEx/BepInEx).

目前项目分为两个独立的插件。

- `NonUpgradableCardFilter` 用于优化对于卡牌的排序。
- `TestSaveFolder` 用于修改游戏的存档目录。

## 关于 NonUpgradableCardFilter

使用此插件后，在进行卡牌排序时，总是会将不可升级的卡牌放到列表的最后面。

游戏原本的排序功能，会把不可升级的卡牌“参杂”在可升级卡牌中。

此功能主要是为了方便修改党获得大量碎片后，升级卡牌时使用自动化脚本。

## 关于 TestSaveFolder

此插件用于迁移游戏存档位置。

插件会读取游戏的当前版本号，一旦发现是测试版本，

则游戏会将存档位置迁移到 `C:\Users\<username>\AppData\LocalLow\XiaMenLeiYun\OfficeLife\Test_Save` 目录下。

由于目前无法判断是否是测试版，所以需要手动设置游戏最新的正式版本号。

因此将此插件放入 [BepInEx](https://github.com/BepInEx/BepInEx) 目录后，**需要** 先运行一次游戏，以生成插件的配置文件。

`BepInEx\config\YY.OfficeLifePlugin.TestSaveFolder.cfg` 是此项目的配置文件。

将配置文件中的 `Last_Release` 字段设为当前游戏最新的正式版本号 (例如 `"1.0.0"`)，再次运行游戏即可。

之后，一旦游戏正式版本再次更新，可以在此配置文件中更新。

此插件会将所有“不是最新的正式版本号”的游戏，一律视为测试版。



