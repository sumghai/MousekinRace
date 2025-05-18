using RimWorld;
using UnityEngine;
using Verse;

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
            
            Rect rect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            Widgets.BeginGroup(rect);

            // Add bills button
            var billsButton = new Rect(0f, 0f, 150f, 29f);
            if (Widgets.ButtonText(billsButton, "AddBill".Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(comp.MiningOptions));
            }

            Widgets.EndGroup();
        }
    }
}
