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
        public BillRepeatModeDef repeatMode = BillRepeatModeDefOf.RepeatCount;  // bill repeat mode (default to "repeat X times")
        
        private ThingWithComps parent;                      // parent thing
        public ThingDef mineableThing;                     // mineable resource thingDef
        private int minedPortionSize;                       // mined portion size
        public int tickLeft;                                // ticks left before spawning product
        private float progress;                             // progress percentage

        public bool suspended;                              // is process suspended?
        public int targetCount = 1;                         // number of times this mining bill should repeat
        private int currRepeatCount = 0;                    // number of times this mining bill has been repeated
        public int ticksRequired;

        private string id;                                  // mining bill unique ID

        private List<FloatMenuOption> billRepeatOptions;

        public Map Map => parent.Map;

        private CompUndergroundMineDeposits CompUMD => parent.TryGetComp<CompUndergroundMineDeposits>();

        public bool IsRunning => ShouldDoNow();

        private Color BaseColor
        {
            get
            {
                if (ShouldDoNow())
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
                        CompUMD.MiningBillStack.Notify_MiningBillChange();
                    }),
                    new(BillRepeatModeDefOf.TargetCount.LabelCap, delegate
                    {
                        repeatMode = BillRepeatModeDefOf.TargetCount;
                        CompUMD.MiningBillStack.Notify_MiningBillChange();
                    }),
                    new(BillRepeatModeDefOf.Forever.LabelCap, delegate
                    {
                        repeatMode = BillRepeatModeDefOf.Forever;
                        CompUMD.MiningBillStack.Notify_MiningBillChange();
                    })
                ];
                return billRepeatOptions;
            }
        }

        public float Progress
        {
            get
            {

                return progress;
            }
            set
            {
                progress = value;
            }
        }

        public int TickLeft => tickLeft;

        public MiningBill()
        { 
        }

        public MiningBill(ThingDef mineableThingInput, ThingWithComps parent)
        {
            this.parent = parent;
            mineableThing = mineableThingInput;

            ticksRequired = CompUMD.Props.mineables.Find(x => x.mineableThing == mineableThing).ticksPerPortionMined;
            minedPortionSize = CompUMD.Props.mineables.Find(x => x.mineableThing == mineableThing).minedPortionSize;
            tickLeft = ticksRequired;
            progress = 0f;

            Map map = parent.Map;

            id = $"UndergroundMiningBill_{map.uniqueID}_{mineableThing.defName}_{map.GetComponent<MapComponent_UndergroundMineDeposits>().GetNextMiningBillID(map)}";
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref repeatMode, "repeatMode");
            Scribe_Defs.Look(ref mineableThing, "miningBillThing");
            Scribe_Values.Look(ref id, "id");
            Scribe_Values.Look(ref minedPortionSize, "minedPortionSize");
            Scribe_Values.Look(ref tickLeft, "tickLeft");
            Scribe_Values.Look(ref progress, "progress");
            Scribe_Values.Look(ref suspended, "suspended");
            Scribe_Values.Look(ref targetCount, "targetCount");
            Scribe_Values.Look(ref currRepeatCount, "currRepeatCount");
            Scribe_Values.Look(ref ticksRequired, "ticksRequired");
            Scribe_References.Look(ref parent, "parent");
        }

        public string GetUniqueLoadID() => id;

        public override string ToString() => GetUniqueLoadID();

        public bool ShouldDoNow()
        {
            if (suspended)
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
                return UndergroundMineSys_Utils.CountMinedProductsOnMap(this) < targetCount;
            }
            throw new InvalidOperationException();
        }

        public void Setup()
        {
            targetCount = 1;
        }

        public void Tick(int ticks)
        {
            // todo - continue only if there is corresponding deposit is not empty
            /*
             * if (deposit for current mineable is empty)
             * {
             *      return;
             * }
             */ 
             
            // We are active for ticks
            if (tickLeft > 0)
            {
                tickLeft -= ticks;
            }
            // Set progress (for the bar)

            progress = Math.Min(1f - (tickLeft / (float)ticksRequired), 1);

            // Check if processor should produce this tick
            if (tickLeft <= 0)
            {
                // todo - spawn the appropriate portion-sized mined resource
            }
        }

        public void ResetMiningBill(bool finished = true)
        {
            tickLeft = ticksRequired;
            progress = 0;

            if (finished)
            {
                if (targetCount - currRepeatCount == 0)
                {
                    targetCount++;  // Prevent going into negative
                }
                currRepeatCount++;
            }

            CompUMD.MiningBillStack.Notify_MiningBillEnded();
        }

        public Rect DoInterface(float x, float y, float width, int index)
        {
            Rect rect = new(x, y, width, 53f);
            Color color = (GUI.color = BaseColor);
            Text.Font = GameFont.Small;

            // Draw row background
            if (index % 2 == 0) 
            {
                Widgets.DrawAltRect(rect);
            }

            Widgets.BeginGroup(rect);
            MiningBillStack stack = CompUMD.MiningBillStack;

            // If the current mining bill isn't the first, we can move it up
            var miningBillIndex = stack.IndexOf(this);
            if (miningBillIndex > 0)
            {
                var upRect = new Rect(0f, 0f, 24f, 24f);
                if (Widgets.ButtonImage(upRect, TexButton.ReorderUp, color))
                {
                    stack.Reorder(this, -1);
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
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegionByKey(downRect, "ReorderBillDownTip");
            }
            GUI.color = color;

            // Mining bill label
            Widgets.Label(new Rect(28f, 0f, rect.width - 48f - 40f, rect.height + 5f), "MousekinRace_MineEntrance_Mine".Translate(mineableThing.LabelCap, "x" + minedPortionSize).CapitalizeFirst() + " (" + ticksRequired.ToStringTicksToDays() + ")");

            // Config
            var baseRect = rect.AtZero();
            GUI.color = new Color(1f, 1f, 1f, 0.65f);
            Widgets.Label(new Rect(28f, 32f, 100f, 30f), RepeatInfoText);
            GUI.color = color;

            var widgetRow = new WidgetRow(baseRect.xMax, baseRect.y + 29f, UIDirection.LeftThenUp);
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
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                CompUMD.MiningBillStack.Notify_MiningBillChange();
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
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                CompUMD.MiningBillStack.Notify_MiningBillChange();
            }

            // Delete bill
            var deleteRect = new Rect(rect.width - 24f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(deleteRect, TexButton.Delete, color, color * GenUI.SubtleMouseoverColor))
            {
                stack.Delete(this);
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
            TooltipHandler.TipRegionByKey(deleteRect, "DeleteBillTip");

            // Suspend
            var suspendRect = new Rect(rect.width - 24f - 24f, 0f, 24f, 24f);
            if (Widgets.ButtonImage(suspendRect, TexButton.Suspend, color))
            {
                suspended = !suspended;
                SoundDefOf.Click.PlayOneShotOnCamera();
                CompUMD.MiningBillStack.Notify_MiningBillChange();
            }
            TooltipHandler.TipRegionByKey(suspendRect, "SuspendBillTip");

            Widgets.EndGroup();

            // Draw suspended
            if (suspended)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                var suspendedRect = new Rect(rect.x + rect.width / 2f - 70f, rect.y + rect.height / 2f - 20f, 140f, 40f);
                GUI.DrawTexture(suspendedRect, TexUI.GrayTextBG);
                Widgets.Label(suspendedRect, "SuspendedCaps".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }
            Text.Font = GameFont.Small;
            GUI.color = Color.white;

            return rect;
        }
    }
}
