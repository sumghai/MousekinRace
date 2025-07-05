using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MousekinRace
{
    public class MiningBill : IExposable, ILoadReferenceable
    {
        [Unsaved(false)]
        public MiningBillStack miningBillStack;

        public int loadID = -1;                 // mining bill unique ID
        
        public ThingWithComps parent;           // parent thing
        public ThingDef mineableThing;          // mineable resource thingDef
        public int minedPortionSize;            // mined portion size

        public bool suspended;                  // is process suspended?
        public int targetCount = 1;             // number of times this mining bill should repeat
        public int currRepeatCount = 0;         // number of times this mining bill has been repeated                          
        public BillRepeatModeDef repeatMode = BillRepeatModeDefOf.RepeatCount;  // bill repeat mode (default to "repeat X times")

        public bool paused;
        public bool pauseWhenSatisfied;
        public int unpauseWhenYouHave = 5;

        private List<FloatMenuOption> billRepeatOptions;

        public BillStoreModeDef storeMode = BillStoreModeDefOf.BestStockpile;
        public ISlotGroup storeGroup;
        public ISlotGroup includeGroup;

        public Map Map => parent.Map;

        public string Label => "MousekinRace_MineEntrance_Mine".Translate(mineableThing.LabelCap, "x" + minedPortionSize).CapitalizeFirst();

        public string StatusString
        {
            get
            {
                if (paused)
                {
                    return " " + "Paused".Translate().CapitalizeFirst();
                }
                return "";
            }
        }

        public float StatusLineMinHeight
        {
            get
            {
                if (!CanUnpause())
                {
                    return 0f;
                }
                return 24f;
            }
        }

        private Building_MineEntrance MineEntrance => parent as Building_MineEntrance;

        private CompUndergroundMineDeposits CompUMD => parent.TryGetComp<CompUndergroundMineDeposits>();

        public bool DepositIsDepleted => MineEntrance.mapComp_UMD.DepositIsDepleted(mineableThing);

        private Color BaseColor
        {
            get
            {
                if (ShouldDoNow() && !DepositIsDepleted)
                {
                    return Color.white;
                }
                return new Color(1f, 0.7f, 0.7f, 0.7f);
            }
        }

        public string RepeatInfoText
        {
            get
            {
                if (repeatMode == BillRepeatModeDefOf.Forever)
                {
                    return "Forever".Translate();
                }
                if (repeatMode == BillRepeatModeDefOf.RepeatCount)
                {
                    return targetCount + "x";
                }
                if (repeatMode == BillRepeatModeDefOf.TargetCount)
                {
                    return UndergroundMineSys_Utils.CountMinedProductsOnMap(this) + "/" + targetCount;
                }
                throw new InvalidOperationException();
            }
        }

        public List<FloatMenuOption> Options
        {
            get
            {
                billRepeatOptions ??=
                [
                    new(BillRepeatModeDefOf.RepeatCount.LabelCap, delegate
                    {
                        repeatMode = BillRepeatModeDefOf.RepeatCount;
                        MineEntrance.UpdateMiningJobSlots();
                    }),
                    new(BillRepeatModeDefOf.TargetCount.LabelCap, delegate
                    {
                        repeatMode = BillRepeatModeDefOf.TargetCount;
                        MineEntrance.UpdateMiningJobSlots();
                    }),
                    new(BillRepeatModeDefOf.Forever.LabelCap, delegate
                    {
                        repeatMode = BillRepeatModeDefOf.Forever;
                        MineEntrance.UpdateMiningJobSlots();
                    })
                ];
                return billRepeatOptions;
            }
        }

        public MiningBill()
        { 
        }

        public MiningBill(ThingDef mineableThingInput, ThingWithComps parent)
        {
            this.parent = parent;
            mineableThing = mineableThingInput;
            minedPortionSize = CompUMD.Props.mineables.Find(x => x.mineableThing == mineableThing).minedPortionSize;
            InitializeAfterClone();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref loadID, "loadID", 0, true);
            Scribe_Defs.Look(ref mineableThing, "miningBillThing");
            Scribe_Values.Look(ref minedPortionSize, "minedPortionSize");
            Scribe_Defs.Look(ref repeatMode, "repeatMode");
            Scribe_Values.Look(ref suspended, "suspended");
            Scribe_Values.Look(ref targetCount, "targetCount");
            Scribe_Values.Look(ref currRepeatCount, "currRepeatCount");

            Scribe_References.Look(ref parent, "parent");

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                SaveSlotReferencable(storeGroup, "storeGroup");
                SaveSlotReferencable(includeGroup, "includeGroup");
            }
            else if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs || Scribe.mode == LoadSaveMode.LoadingVars)
            {
                LoadSlotReferencable(ref storeGroup, "storeGroup");
                LoadSlotReferencable(ref includeGroup, "includeGroup");
            }

            Scribe_Values.Look(ref pauseWhenSatisfied, "pauseWhenSatisfied", defaultValue: false);
            Scribe_Values.Look(ref unpauseWhenYouHave, "unpauseWhenYouHave", 0);
            Scribe_Values.Look(ref paused, "paused", defaultValue: false);

            if (repeatMode == null)
            {
                repeatMode = BillRepeatModeDefOf.RepeatCount;
            }
            if (storeMode == null)
            {
                storeMode = BillStoreModeDefOf.BestStockpile;
            }
        }

        public static void SaveSlotReferencable(ISlotGroup slot, string key)
        {
            ILoadReferenceable refee = null;
            if (slot is ILoadReferenceable loadReferenceable)
            {
                refee = loadReferenceable;
            }
            else if (slot is SlotGroup { parent: ILoadReferenceable parent })
            {
                refee = parent;
            }
            Scribe_References.Look(ref refee, key);
        }

        public static void LoadSlotReferencable(ref ISlotGroup slot, string key)
        {
            ILoadReferenceable refee = null;
            Scribe_References.Look(ref refee, key);
            if (refee is ISlotGroup slotGroup)
            {
                slot = slotGroup;
            }
            else if (refee is ISlotGroupParent slotGroupParent)
            {
                slot = slotGroupParent.GetSlotGroup();
            }
        }

        public BillStoreModeDef GetStoreMode()
        {
            return storeMode;
        }

        public ISlotGroup GetSlotGroup()
        {
            return storeGroup;
        }

        public void SetIncludeGroup(ISlotGroup group)
        {
            includeGroup = group;
        }

        public ISlotGroup GetIncludeSlotGroup()
        {
            return includeGroup;
        }

        public void SetStoreMode(BillStoreModeDef mode, ISlotGroup group = null)
        {
            storeGroup = group;
            storeMode = mode;
            if (storeMode == BillStoreModeDefOf.SpecificStockpile != (group != null))
            {
                Log.ErrorOnce("Inconsistent bill StoreMode data set", 75645354);
            }
        }

        public string GetUniqueLoadID() => "MousekinRace_MiningBill_" + mineableThing.defName + "_" + loadID;

        public override string ToString() => GetUniqueLoadID();

        public bool ShouldDoNow()
        {           
            if (repeatMode != BillRepeatModeDefOf.TargetCount)
            {
                paused = false;
            }
            if (suspended || DepositIsDepleted)
            {
                return false;
            }
            if (repeatMode == BillRepeatModeDefOf.Forever)
            {
                return true;
            }
            if (repeatMode == BillRepeatModeDefOf.RepeatCount)
            {
                return currRepeatCount < targetCount;
            }
            if (repeatMode == BillRepeatModeDefOf.TargetCount)
            {
                int num = UndergroundMineSys_Utils.CountMinedProductsOnMap(this);
                if (pauseWhenSatisfied && num >= targetCount)
                {
                    paused = true;
                }
                if (num <= unpauseWhenYouHave || !pauseWhenSatisfied)
                {
                    if (paused)
                    { 
                        MineEntrance.triggerAutoSlotUpdate = true; // oneshot
                    }
                    paused = false;
                }
                if (paused)
                {
                    return false;
                }
                return num < targetCount;
            }
            throw new InvalidOperationException();
        }

        public void Setup()
        {
            targetCount = 1;
        }

        public void ResetMiningBill(bool finished = true)
        {
            if (finished)
            {
                if (targetCount - currRepeatCount == 0)
                {
                    targetCount++;  // Prevent going into negative
                }
                currRepeatCount++;
            }
        }

        public Rect DoInterface(float x, float y, float width, int index)
        {
            Rect rect = new(x, y, width, 53f);
            Color color = (GUI.color = BaseColor);
            Text.Font = GameFont.Small;

            // Optionally expand rect to make room for status line
            float statusLineOffset = 0f;
            if (!StatusString.NullOrEmpty())
            {
                statusLineOffset = Mathf.Max(Text.TinyFontSupported ? 17f : 21f, StatusLineMinHeight);
            }
            rect.height += statusLineOffset;

            // Draw row background
            if (index % 2 == 0) 
            {
                Widgets.DrawAltRect(rect);
            }

            Widgets.BeginGroup(rect);
            MiningBillStack stack = MineEntrance.MiningBillStack;

            // If the current mining bill isn't the first, we can move it up
            var miningBillIndex = stack.IndexOf(this);
            if (miningBillIndex > 0)
            {
                var upRect = new Rect(0f, 0f, 24f, 24f);
                if (Widgets.ButtonImage(upRect, TexButton.ReorderUp, color))
                {
                    stack.Reorder(this, -1);
                    MineEntrance.UpdateMiningJobSlots();
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegionByKey(upRect, "ReorderBillUpTip");
            }

            // If the current mining bill isn't the last, we can move it down
            if (miningBillIndex < stack.MiningBills.Count - 1)
            {
                var downRect = new Rect(0f, 24f, 24f, 24f);
                if (Widgets.ButtonImage(downRect, TexButton.ReorderDown, color))
                {
                    stack.Reorder(this, 1);
                    MineEntrance.UpdateMiningJobSlots();
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegionByKey(downRect, "ReorderBillDownTip");
            }
            GUI.color = color;

            // Mining bill label
            Widgets.Label(new Rect(28f, 0f, rect.width - 48f - 40f, rect.height + 5f), Label);

            // Config
            var baseRect = rect.AtZero();
            GUI.color = new Color(1f, 1f, 1f, 0.65f);
            Widgets.Label(new Rect(28f, 32f, 100f, 30f), RepeatInfoText);
            GUI.color = color;

            var widgetRow = new WidgetRow(baseRect.xMax, baseRect.y + 29f, UIDirection.LeftThenUp);
            if (widgetRow.ButtonText("Details".Translate() + "..."))
            {
                Find.WindowStack.Add(GetMiningBillDialog());
            }
            if (widgetRow.ButtonText(repeatMode.LabelCap.Resolve().PadRight(20)))
            {
                Find.WindowStack.Add(new FloatMenu(Options));
            }
            if (widgetRow.ButtonIcon(TexButton.Plus))
            {
                // Forever, click plus put it in count
                if (targetCount == -1)
                {
                    currRepeatCount = 0;
                    targetCount = 1;
                }
                else if (targetCount > -1)
                {
                    targetCount += GenUI.CurrentAdjustmentMultiplier();
                }
                MineEntrance.UpdateMiningJobSlots();
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
            }
            if (widgetRow.ButtonIcon(TexButton.Minus))
            {
                // Forever, click minus put it in count
                if (targetCount == -1)
                {
                    currRepeatCount = 0;
                    targetCount = 1;
                }
                else if (targetCount > -1)
                {
                    targetCount = Mathf.Max(0, targetCount - GenUI.CurrentAdjustmentMultiplier());
                }
                MineEntrance.UpdateMiningJobSlots();
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
            }

            // Delete bill
            Rect deleteRect = new(rect.width - 24f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(deleteRect, TexButton.Delete, color, color * GenUI.SubtleMouseoverColor))
            {
                stack.Delete(this);
                MineEntrance.UpdateMiningJobSlots();
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
            TooltipHandler.TipRegionByKey(deleteRect, "DeleteBillTip");

            // Copy bill
            Rect copyRect = new(rect.width - (2f * 24f), 0f, 24f, 24f);
            if (Widgets.ButtonImageFitted(copyRect, TexButton.Copy, color))
            {
                UndergroundMineSys_Utils.Clipboard = Clone();
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            }

            // Suspend
            Rect suspendRect = new(rect.width - (3f * 24f), 0f, 24f, 24f);
            if (Widgets.ButtonImage(suspendRect, TexButton.Suspend, color))
            {
                suspended = !suspended;
                MineEntrance.UpdateMiningJobSlots();
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
            TooltipHandler.TipRegionByKey(suspendRect, "SuspendBillTip");

            // Status line (i.e. "paused")
            if (!StatusString.NullOrEmpty())
            {
                Text.Font = GameFont.Tiny;
                Rect statusLineRect = new(24f, rect.height - statusLineOffset, rect.width - 24f, statusLineOffset);
                Widgets.Label(statusLineRect, StatusString);
                DoStatusLineInterface(statusLineRect);
            }

            Widgets.EndGroup();

            // Draw suspended or deposit depleted overlays
            if (suspended || DepositIsDepleted)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                float overlayWidth = 250f;
                float overlayHeight = 40f;
                Rect overlayRect = new(rect.x + rect.width / 2f - overlayWidth / 2f, rect.y + rect.height / 2f - overlayHeight / 2f, overlayWidth, overlayHeight);
                GUI.DrawTexture(overlayRect, TexUI.GrayTextBG);
                Widgets.Label(overlayRect, suspended ? "SuspendedCaps".Translate() : (TaggedString)"MousekinRace_MineEntrance_DepositDepleted".Translate().ToString().ToUpper());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }

            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            return rect;
        }

        public Window GetMiningBillDialog()
        {
            return new Dialog_MiningBillConfig(this, parent.Position);
        }

        public bool CanUnpause()
        {
            if (repeatMode == BillRepeatModeDefOf.TargetCount && paused && pauseWhenSatisfied)
            {
                return UndergroundMineSys_Utils.CountMinedProductsOnMap(this) < targetCount;
            }
            return false;
        }

        public void DoStatusLineInterface(Rect rect)
        {
            if (paused && new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenUp).ButtonText("Unpause".Translate()))
            {
                paused = false;
                MineEntrance.UpdateMiningJobSlots();
            }
        }

        public MiningBill Clone()
        { 
            MiningBill obj = (MiningBill)Activator.CreateInstance(GetType());
            obj.repeatMode = repeatMode;
            obj.mineableThing = mineableThing;
            obj.minedPortionSize = minedPortionSize;
            obj.suspended = suspended;
            obj.targetCount = targetCount;
            obj.currRepeatCount = currRepeatCount;
            obj.parent = parent;
            obj.pauseWhenSatisfied = pauseWhenSatisfied;
            obj.unpauseWhenYouHave = unpauseWhenYouHave;
            obj.paused = paused;

            return obj;
        }

        public void InitializeAfterClone()
        {
            loadID = Map.GetComponent<MapComponent_UndergroundMineDeposits>().GetNextMiningBillID(Map);
        }

        public void Notify_IterationCompleted()
        {
            if (repeatMode == BillRepeatModeDefOf.RepeatCount)
            {
                if (targetCount > 0)
                {
                    targetCount--;
                }
                if (targetCount == 0)
                {
                    Messages.Message("MessageBillComplete".Translate(Label), parent, MessageTypeDefOf.TaskCompletion);
                }
            }
        }
    }
}
