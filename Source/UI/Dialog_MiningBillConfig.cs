using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MousekinRace
{
    public class Dialog_MiningBillConfig : Window
    {
        public IntVec3 billGiverPos;

        public MiningBill miningBill;

        public string repeatCountEditBuffer;

        public string targetCountEditBuffer;

        public string unpauseCountEditBuffer;

        public static readonly Dictionary<string, List<ISlotGroup>> tmpGroups = [];

        public override Vector2 InitialSize => new(540, 395f);

        public Dialog_MiningBillConfig(MiningBill miningBill, IntVec3 billGiverPos) 
        {
            this.billGiverPos = billGiverPos;
            this.miningBill = miningBill;
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
        }

        public void AdjustCount(int offset)
        {
            SoundDefOf.DragSlider.PlayOneShotOnCamera();
            miningBill.targetCount += offset;
            if (miningBill.targetCount < 1)
            {
                miningBill.targetCount = 1;
            }
        }

        public override void LateWindowOnGUI(Rect inRect)
        {
            Rect rect = new(inRect.x, inRect.y, 34f, 34f);
            Widgets.DefIcon(rect, miningBill.mineableThing);
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;

            // Mining bill label as title
            Widgets.Label(new Rect(40f, 0f, 400f, Dialog_BillConfig.RecipeIconSize), miningBill.Label);
            float width = (int)((inRect.width - (Dialog_BillConfig.RecipeIconSize / 2f)) / 2f);
            Rect rect = new(0f, 80f, width, inRect.height - 80f);
            Rect rect2 = new(rect.xMax + 17f, Dialog_BillConfig.IngredientRadiusSubdialogHeight, width, inRect.height - Dialog_BillConfig.IngredientRadiusSubdialogHeight - Window.CloseButSize.y);
            Text.Font = GameFont.Small;

            Listing_Standard listing_Standard = new();
            listing_Standard.Begin(rect2);

            // Mining bill repeat mode and existing stockpile lookup pane
            Listing_Standard listing_Standard_BillRepeatMode = listing_Standard.BeginSection(200f);
            if (listing_Standard_BillRepeatMode.ButtonText(miningBill.repeatMode.LabelCap))
            {
                Find.WindowStack.Add(new FloatMenu(miningBill.Options));
            }
            listing_Standard_BillRepeatMode.Gap();
            if (miningBill.repeatMode == BillRepeatModeDefOf.RepeatCount)
            {
                listing_Standard_BillRepeatMode.Label("RepeatCount".Translate(miningBill.targetCount));
                listing_Standard_BillRepeatMode.IntEntry(ref miningBill.targetCount, ref repeatCountEditBuffer);
            }
            else if (miningBill.repeatMode == BillRepeatModeDefOf.TargetCount)
            {
                string text = "CurrentlyHave".Translate() + ": ";
                text += UndergroundMineSys_Utils.CountMinedProductsOnMap(miningBill);
                text += " / ";
                text += (miningBill.targetCount < 999999) ? miningBill.targetCount.ToString() : "Infinite".Translate().ToLower().ToString();
                listing_Standard_BillRepeatMode.Label(text);
                int targetCount = miningBill.targetCount;
                listing_Standard_BillRepeatMode.IntEntry(ref miningBill.targetCount, ref targetCountEditBuffer);
                miningBill.unpauseWhenYouHave = Mathf.Max(0, miningBill.unpauseWhenYouHave + (miningBill.targetCount - targetCount));
                ThingDef producedThingDef = miningBill.mineableThing;
                if (producedThingDef != null)
                {
                    TaggedString taggedString = (miningBill.GetIncludeSlotGroup() == null) ? "IncludeFromAll".Translate() : "IncludeSpecific".Translate(SlotGroup.GetGroupLabel(miningBill.GetIncludeSlotGroup()));
                    if (listing_Standard_BillRepeatMode.ButtonText(taggedString))
                    {
                        Text.Font = GameFont.Small;
                        List<FloatMenuOption> opts =
                        [
                            new FloatMenuOption("IncludeFromAll".Translate(), delegate
                            {
                                miningBill.SetIncludeGroup(null);
                            }),
                        ];
                        foreach (BillStoreModeDef item in DefDatabase<BillStoreModeDef>.AllDefs.OrderBy((BillStoreModeDef bsm) => bsm.listOrder))
                        {
                            if (item == BillStoreModeDefOf.SpecificStockpile)
                            {
                                FillOutputDropdownOptions(ref opts, "IncludeSpecific".Translate(), delegate (ISlotGroup slot)
                                {
                                    miningBill.SetIncludeGroup(slot);
                                });
                            }
                        }
                        Find.WindowStack.Add(new FloatMenu(opts));
                    }
                }
            }
            if (miningBill.repeatMode == BillRepeatModeDefOf.TargetCount)
            {
                listing_Standard_BillRepeatMode.CheckboxLabeled("PauseWhenSatisfied".Translate(), ref miningBill.pauseWhenSatisfied);
                if (miningBill.pauseWhenSatisfied)
                {
                    listing_Standard_BillRepeatMode.Label("UnpauseWhenYouHave".Translate() + ": " + miningBill.unpauseWhenYouHave.ToString("F0"));
                    listing_Standard_BillRepeatMode.IntEntry(ref miningBill.unpauseWhenYouHave, ref unpauseCountEditBuffer);
                    if (miningBill.unpauseWhenYouHave >= miningBill.targetCount)
                    {
                        miningBill.unpauseWhenYouHave = miningBill.targetCount - 1;
                        unpauseCountEditBuffer = miningBill.unpauseWhenYouHave.ToStringCached();
                    }
                }
            }
            listing_Standard.EndSection(listing_Standard_BillRepeatMode);
            listing_Standard.Gap();

            // Store mode pane
            Listing_Standard listing_Standard_StoreMode = listing_Standard.BeginSection(Dialog_BillConfig.StoreModeSubdialogHeight);
            string text2 = string.Format(miningBill.GetStoreMode().LabelCap, (miningBill.GetSlotGroup() != null) ? SlotGroup.GetGroupLabel(miningBill.GetSlotGroup()) : "");
            if (miningBill.GetSlotGroup() is SlotGroup group && !group.Settings.AllowedToAccept(miningBill.mineableThing))
            {
                text2 += string.Format(" ({0})", "IncompatibleLower".Translate());
                Text.Font = GameFont.Tiny;
            }
            if (listing_Standard_StoreMode.ButtonText(text2))
            {
                Text.Font = GameFont.Small;
                List<FloatMenuOption> opts2 = [];
                foreach (BillStoreModeDef item2 in DefDatabase<BillStoreModeDef>.AllDefs.OrderBy((BillStoreModeDef bsm) => bsm.listOrder))
                {
                    if (item2 == BillStoreModeDefOf.SpecificStockpile)
                    {
                        FillOutputDropdownOptions(ref opts2, BillStoreModeDefOf.SpecificStockpile.LabelCap, delegate (ISlotGroup slot)
                        {
                            miningBill.SetStoreMode(BillStoreModeDefOf.SpecificStockpile, slot);
                        });
                        continue;
                    }
                    BillStoreModeDef smLocal = item2;
                    opts2.Add(new FloatMenuOption(smLocal.LabelCap, delegate
                    {
                        miningBill.SetStoreMode(smLocal);
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(opts2));
            }
            Text.Font = GameFont.Small;
            listing_Standard.EndSection(listing_Standard_StoreMode);
            // No worker selection pane, as Mine Entrances allow multiple miners
            listing_Standard.End();

            float y = rect2.y;
            // No ingredient search radius or config pane, as mining bills don't use ingredient filters

            // Info pane
            Listing_Standard listing_Standard_MiningBillInfo = new();
            listing_Standard_MiningBillInfo.Begin(rect);
            if (miningBill.suspended)
            {
                if (listing_Standard_MiningBillInfo.ButtonText("Suspended".Translate()))
                {
                    miningBill.suspended = false;
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
            }
            else if (listing_Standard_MiningBillInfo.ButtonText("NotSuspended".Translate()))
            {
                miningBill.suspended = true;
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("MousekinRace_MineEntrance_MineDescription".Translate(miningBill.mineableThing.label, miningBill.parent.LabelCap));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("WorkAmount".Translate() + ": " + ((float)UndergroundMineSys_Utils.GetWorkRequiredToMineResource(miningBill.mineableThing)).ToStringWorkAmount());
            Text.Font = GameFont.Small;
            string text4 = stringBuilder.ToString();
            if (Text.CalcHeight(text4, rect.width) > rect.height)
            {
                Text.Font = GameFont.Tiny;
            }
            listing_Standard_MiningBillInfo.Label(text4);
            Text.Font = GameFont.Small;
            listing_Standard_MiningBillInfo.End();

            Widgets.InfoCardButton(rect.x, rect2.y, miningBill.mineableThing);
        }

        public override void PostClose()
        {
            (miningBill.parent as Building_MineEntrance).UpdateMiningJobSlots();
        }

        public void FillOutputDropdownOptions(ref List<FloatMenuOption> opts, string prefix, Action<ISlotGroup> selected)
        {
            List<SlotGroup> allGroupsListInPriorityOrder = miningBill.Map.haulDestinationManager.AllGroupsListInPriorityOrder;
            tmpGroups.ClearValueLists();
            for (int i = 0; i < allGroupsListInPriorityOrder.Count; i++)
            {
                SlotGroup slotGroup = allGroupsListInPriorityOrder[i];
                if (slotGroup.StorageGroup != null)
                {
                    StorageGroup storageGroup = slotGroup.StorageGroup;
                    if (!tmpGroups.ContainsKey(storageGroup.GroupingLabel))
                    {
                        tmpGroups.Add(storageGroup.GroupingLabel, []);
                    }
                    if (!tmpGroups[storageGroup.GroupingLabel].Contains(slotGroup.StorageGroup))
                    {
                        tmpGroups[storageGroup.GroupingLabel].Add(slotGroup.StorageGroup);
                    }
                }
                else if (!(slotGroup.parent is Building_Storage building_Storage) || building_Storage is IRenameable)
                {
                    if (!tmpGroups.ContainsKey(slotGroup.GroupingLabel))
                    {
                        tmpGroups.Add(slotGroup.GroupingLabel, new List<ISlotGroup>());
                    }
                    tmpGroups[slotGroup.GroupingLabel].Add(slotGroup);
                }
            }
            foreach (KeyValuePair<string, List<ISlotGroup>> kvp2 in tmpGroups.OrderBy((KeyValuePair<string, List<ISlotGroup>> kvp) => (kvp.Value.Count > 0) ? kvp.Value[0].GroupingOrder : 0))
            {
                if (ShouldCollapseGroup(kvp2.Key))
                {
                    opts.Add(new FloatMenuOption(kvp2.Key, delegate
                    {
                        List<FloatMenuOption> list = new List<FloatMenuOption>();
                        FillSlotGroupOptions(kvp2.Value, list, prefix, selected);
                        Find.WindowStack.Add(new FloatMenu(list));
                    }));
                }
            }
            foreach (KeyValuePair<string, List<ISlotGroup>> item in tmpGroups.OrderBy((KeyValuePair<string, List<ISlotGroup>> kvp) => (kvp.Value.Count > 0) ? kvp.Value[0].GroupingOrder : 0))
            {
                if (!ShouldCollapseGroup(item.Key))
                {
                    FillSlotGroupOptions(item.Value, opts, prefix, selected);
                }
            }
        }

        public bool ShouldCollapseGroup(string key)
        {
            bool flag = false;
            foreach (KeyValuePair<string, List<ISlotGroup>> tmpGroup in tmpGroups)
            {
                if (tmpGroup.Key != key && tmpGroup.Value.Count > 0)
                {
                    flag = true;
                    break;
                }
            }
            return tmpGroups[key].Count > 2 && flag;
        }

        public void FillSlotGroupOptions(List<ISlotGroup> groups, List<FloatMenuOption> opts, string prefix, Action<ISlotGroup> selected)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                ISlotGroup group = groups[i];
                if (!group.Settings.AllowedToAccept(miningBill.mineableThing))
                {
                    opts.Add(new FloatMenuOption(string.Format("{0} ({1})", string.Format(prefix, SlotGroup.GetGroupLabel(group)), "IncompatibleLower".Translate()), null));
                    continue;
                }
                opts.Add(new FloatMenuOption(string.Format(prefix, SlotGroup.GetGroupLabel(group)), delegate
                {
                    selected(group);
                }));
            }
        }
    }
}
