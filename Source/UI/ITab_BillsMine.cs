using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MousekinRace
{
    public class ITab_BillsMine : ITab
    {
        public float viewHeight = 1000f;

        public Vector2 scrollPosition;

        public static readonly Vector2 WinSize = ITab_Bills.WinSize;

        public ITab_BillsMine()
        { 
            size = WinSize;
            labelKey = "TabBills";
        }

        public override void FillTab()
        {
            CompUndergroundMineDeposits comp = SelThing.TryGetComp<CompUndergroundMineDeposits>();
            List<MiningBill> miningBills = comp.MiningBillStack.MiningBills;

            // Add paste mining bill button
            Rect pasteRect = new(WinSize.x - ITab_Bills.PasteX, ITab_Bills.PasteY, ITab_Bills.PasteSize, ITab_Bills.PasteSize);
            if (UndergroundMineSys_Utils.Clipboard != null)
            {
                if (miningBills.Count >= 15)
                {
                    GUI.color = Color.gray;
                    Widgets.DrawTextureFitted(pasteRect, TexButton.Paste, 1f);
                    GUI.color = Color.white;
                    if (Mouse.IsOver(pasteRect))
                    {
                        TooltipHandler.TipRegion(pasteRect, "PasteBillTip".Translate() + " (" + "PasteBillTip_LimitReached".Translate() + "): " + UndergroundMineSys_Utils.Clipboard.Label);
                    }
                }
                else
                {
                    if (Widgets.ButtonImageFitted(pasteRect, TexButton.Paste, Color.white))
                    {
                        MiningBill bill = UndergroundMineSys_Utils.Clipboard.Clone();
                        bill.InitializeAfterClone();
                        comp.miningBillStack.AddMiningBill(bill, (ThingWithComps)SelThing);
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                    }
                    if (Mouse.IsOver(pasteRect))
                    {
                        TooltipHandler.TipRegion(pasteRect, "PasteBillTip".Translate() + ": " + UndergroundMineSys_Utils.Clipboard.Label);
                    }
                }
            }

            Rect rect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            Widgets.BeginGroup(rect);

            // Add bills button
            Rect billsButton = new(0f, 0f, 150f, 29f);
            if (Widgets.ButtonText(billsButton, "AddBill".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(comp.MiningOptions));
            }

            // Draw mining bills
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
            Rect outRect = new(0f, 35f, rect.width, rect.height - 35f - 24f - 6f - 6f);
            Rect viewRect = new(0f, 0f, outRect.width - 16f, viewHeight);
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            float num = 0f;
            for (int i = 0; i < miningBills.Count; i++)
            {
                MiningBill miningBill = miningBills[i];
                Rect miningBillRect = miningBill.DoInterface(0f, num, viewRect.width, i);
                num += miningBillRect.height + 6f;
            }
            if (Event.current.type == EventType.Layout)
            {
                viewHeight = num + 60f;
            }
            Widgets.EndScrollView();
            Widgets.EndGroup();
        }
    }
}
