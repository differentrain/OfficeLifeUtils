using BepInEx;
using HarmonyLib;
using System.Collections.Generic;

namespace NonUpgradableCardFilter
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}: Patched.");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardImpl), nameof(CardImpl.GetFuseByMyCardList))]
        public static void PatchNonUpgradableCardSorter(ref List<TbCardSto> __result, CardImpl __instance, string parameter, SortEnum sortEnum)
        {
            int count = __result.Count;
            __result.Sort((a, b) =>
            {
                FragmentImpl.ButtonUpgradeEnum x = MySingleton<FragmentImpl>.Instance.IsUpgrade(a.cardId);
                FragmentImpl.ButtonUpgradeEnum y = MySingleton<FragmentImpl>.Instance.IsUpgrade(b.cardId);
                if (x == FragmentImpl.ButtonUpgradeEnum.none)
                {
                    if (y != FragmentImpl.ButtonUpgradeEnum.none)
                    {
                        return 1;
                    }
                }
                else if (y == FragmentImpl.ButtonUpgradeEnum.none)
                {
                    return -1;
                }
                switch (parameter)
                {
                    case "0":
                        if (a.getTime > b.getTime)
                        {
                            return sortEnum == SortEnum.asc ? 1 : -1;
                        }
                        else if (a.getTime < b.getTime)
                        {
                            return sortEnum == SortEnum.asc ? -1 : 1;
                        }
                        return 0;
                    case "1":
                        if (__instance.GetCardCfgById(a.cardId).actionV> __instance.GetCardCfgById(b.cardId).actionV)
                        {
                            return sortEnum == SortEnum.asc ? 1 : -1;
                        }
                        else if(__instance.GetCardCfgById(a.cardId).actionV < __instance.GetCardCfgById(b.cardId).actionV)
                        {
                            return sortEnum == SortEnum.asc ? -1 : 1;
                        }
                        return 0;
                    case "2":
                        if (__instance.GetCardCfgById(a.cardId).pressureV > __instance.GetCardCfgById(b.cardId).pressureV)
                        {
                            return sortEnum == SortEnum.asc ? 1 : -1;
                        }
                        else if (__instance.GetCardCfgById(a.cardId).pressureV < __instance.GetCardCfgById(b.cardId).pressureV)
                        {
                            return sortEnum == SortEnum.asc ? -1 : 1;
                        }
                        return 0;
                    default:// 3
                        if (__instance.GetCardCfgById(a.cardId).cardrarityEnum > __instance.GetCardCfgById(b.cardId).cardrarityEnum)
                        {
                            return sortEnum == SortEnum.asc ? 1 : -1;
                        }
                        else if (__instance.GetCardCfgById(a.cardId).cardrarityEnum < __instance.GetCardCfgById(b.cardId).cardrarityEnum)
                        {
                            return sortEnum == SortEnum.asc ? -1 : 1;
                        }
                        return 0;
                }
            });
        }
    }



}
