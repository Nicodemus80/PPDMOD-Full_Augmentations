using HarmonyLib;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Common.Entities.Addons;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.Entities.GameTags;
using System.Reflection;
using PhoenixPoint.Geoscape.View.ViewControllers.AugmentationScreen;
using PhoenixPoint.Geoscape.View.ViewModules;
using System.Collections.Generic;
using FullAugmentations;
using UnityEngine.UI;
using Base.UI;
using PhoenixPoint.Modding;


namespace FullAugmentations.Patches
{
    [HarmonyPatch(typeof(UIModuleMutate), "InitCharacterInfo")]
    class InitCharacterInfo_Mutate
    {
        [HarmonyPrefix]
        static bool Prefix(UIModuleMutate __instance, ref int ____currentCharacterAugmentsAmount, Dictionary<AddonSlotDef, UIModuleMutationSection> ____augmentSections, GameTagDef ____bionicsTag,GameTagDef ____mutationTag)
        {
            ____currentCharacterAugmentsAmount = 0;
            ____currentCharacterAugmentsAmount = AugmentScreenUtilities.GetNumberOfAugments(__instance.CurrentCharacter);
            bool flag = ____currentCharacterAugmentsAmount < FullAugmentationsConfig.MaxAugmentations;

            foreach (KeyValuePair<AddonSlotDef, UIModuleMutationSection> keyValuePair in ____augmentSections)
            {
                AugumentSlotState slotState = AugumentSlotState.Available;
                string lockedReasonKey = null;
                ItemDef augmentAtSlot = AugmentScreenUtilities.GetAugmentAtSlot(__instance.CurrentCharacter, keyValuePair.Key);
                bool flag2 = augmentAtSlot != null && augmentAtSlot.Tags.Contains(____bionicsTag);
                bool flag3 = augmentAtSlot != null && augmentAtSlot.Tags.Contains(____mutationTag);
      
                if (flag2)
                {
                    lockedReasonKey = __instance.LockedDueToBionicsKey.LocalizationKey;
                    slotState = AugumentSlotState.BlockedByPermenantAugument;                   
                }
                else if (!flag && !flag3)
                {
                    lockedReasonKey = __instance.LockedDueToLimitKey.LocalizationKey;
                    slotState = AugumentSlotState.AugumentationLimitReached;
                }
                keyValuePair.Value.ResetContainer(slotState, lockedReasonKey);               
            }

            foreach (GeoItem geoItem in __instance.CurrentCharacter.ArmourItems)
            {
                if (geoItem.ItemDef.Tags.Contains(____mutationTag))
                {
                    foreach (AddonDef.RequiredSlotBind requiredSlotBind in geoItem.ItemDef.RequiredSlotBinds)
                    {
                        if (____augmentSections.ContainsKey(requiredSlotBind.RequiredSlot))
                        {
                            ____augmentSections[requiredSlotBind.RequiredSlot].SetMutationUsed(geoItem.ItemDef);
                        }
                    }
                }
            }

            string text = __instance.XoutOfY.Localize(null);
            text = text.Replace("{0}", ____currentCharacterAugmentsAmount.ToString());
            text = text.Replace("{1}", FullAugmentationsConfig.MaxAugmentations.ToString());
           
            __instance.MutationsAvailableValue.text = text;
            __instance.MutationsAvailableValue.GetComponent<UIColorController>().SetWarningActive(FullAugmentationsConfig.MaxAugmentations <= ____currentCharacterAugmentsAmount, false);


            return false;
        }

    }
}
