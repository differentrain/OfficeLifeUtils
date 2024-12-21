using BepInEx;
using BepInEx.Configuration;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Security.Cryptography;

using UnityEngine;

namespace TestSaveFolder
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private const string SAVE_FOLDER_NAME_ORG = "/Save";
        private const string SAVE_FOLDER_NAME_NEW = "/Test_Save";
        private static readonly DirectoryInfo s_orgSavePath = new(Application.persistentDataPath + SAVE_FOLDER_NAME_ORG);
        private static readonly DirectoryInfo s_newSavePath = new(Application.persistentDataPath + SAVE_FOLDER_NAME_NEW);

        public void Awake()
        {
            ConfigEntry<string> config = Config.Bind("General",
                   "Last_Release",
                   "",
                   "游戏最新的正式版本号");
            string gameVersion = GetGameVersion();
            if (string.IsNullOrWhiteSpace(config.Value))
            {
                Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}: Need Config Game Version. Current[{gameVersion}]");

            }
            else if (gameVersion.Contains(config.Value))
            {
                Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}: Not Patched. Current[{gameVersion}], Release[{config.Value}]");
            }
            else
            {
                Harmony.CreateAndPatchAll(typeof(Plugin));
                Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}: Patched. Current[{gameVersion}], Release[{config.Value}]");
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(SaveDataImpl), nameof(SaveDataImpl.Awake))]
        public static IEnumerable<CodeInstruction> PatchSaveDataFolder(IEnumerable<CodeInstruction> orgCodes)
        {
            foreach (CodeInstruction code in orgCodes)
            {
                if (code.opcode == OpCodes.Ldstr && (code.operand as string) == SAVE_FOLDER_NAME_ORG)
                {
                    code.operand = SAVE_FOLDER_NAME_NEW;
                    if (s_orgSavePath.Exists &&
                        !s_newSavePath.Exists)
                    {
                        s_newSavePath.Create();
                        CopyDirectory(s_orgSavePath, s_newSavePath);
                    }
                    break;
                }
            }
            return orgCodes;
        }

        private static void CopyDirectory(DirectoryInfo src, DirectoryInfo dest)
        {
            FileInfo[] files = src.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name.ToLower() != "steam_autocloud.vdf")
                {
                    file.CopyTo($"{dest.FullName}/{file.Name}");
                }
            }
            DirectoryInfo[] dirs = src.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                CopyDirectory(dir, dest.CreateSubdirectory(dir.Name));
            }
        }

        private  string GetGameVersion()
        {
            Singleton<ResManager>.Instance.Init();
            DataSys.SteamUpdateTitles = Singleton<ResManager>.Instance.LoadConfig<List<TbUpdateCfg>, XmlConfiger>(Singleton<ResManager>.Instance.LoadResource<TextAsset>(SysDefine.fillPath + SysDefine.UPDATELITLE_STEAM_PATH), ResType.Resources);
            if (DataSys.SteamUpdateTitles.IsNull<List<TbUpdateCfg>>() && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                DataSys.SteamUpdateTitles = Singleton<ResManager>.Instance.LoadConfig<List<TbUpdateCfg>, XmlConfiger>(Application.streamingAssetsPath + SysDefine.fillPath + SysDefine.UPDATELITLE_STEAM_PATH, ResType.File);
            }
            DataSys.SteamUpdateMessages = Singleton<ResManager>.Instance.LoadConfig<List<TbUpMessageCfg>, XmlConfiger>(Singleton<ResManager>.Instance.LoadResource<TextAsset>(SysDefine.fillPath + SysDefine.UPDATEMESSAGE_STEAM_PATH), ResType.Resources);
            if (DataSys.SteamUpdateMessages.IsNull<List<TbUpMessageCfg>>() && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                DataSys.SteamUpdateMessages = Singleton<ResManager>.Instance.LoadConfig<List<TbUpMessageCfg>, XmlConfiger>(Application.streamingAssetsPath + SysDefine.fillPath + SysDefine.UPDATEMESSAGE_STEAM_PATH, ResType.File);
            }
            DataSys.WegameUpdateTitles = Singleton<ResManager>.Instance.LoadConfig<List<TbUpdateCfg>, XmlConfiger>(Singleton<ResManager>.Instance.LoadResource<TextAsset>(SysDefine.fillPath + SysDefine.UPDATELITLE_WEGAME_PATH), ResType.Resources);
            if (DataSys.WegameUpdateTitles.IsNull<List<TbUpdateCfg>>() && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                DataSys.WegameUpdateTitles = Singleton<ResManager>.Instance.LoadConfig<List<TbUpdateCfg>, XmlConfiger>(Application.streamingAssetsPath + SysDefine.fillPath + SysDefine.UPDATELITLE_WEGAME_PATH, ResType.File);
            }
           return MySingleton<UpdateMessageImpl>.Instance.GetNewVersionName();
        }
    }


}
