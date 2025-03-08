using RimWorld;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class ITab_FuelFilter : ITab
    {
        public ThingFilterUI.UIState fuelFilterState = new();

        public float topAreaHeight = 20;

        public CompRefuelable compRefuelable => (SelThing as ThingWithComps)?.GetComp<CompRefuelable>();

        public CompAllowedFuelTypes compAllowedFuelTypes => (SelThing as ThingWithComps)?.GetComp<CompAllowedFuelTypes>();

        public override bool IsVisible
        {
            get
            {
                if (SelThing != null && !SelThing.def.IsFrame && SelThing.Faction.IsPlayer && !IsDisabledByOtherFuelFilterMods())
                {
                    return base.IsVisible;
                }
                return false;
            }
        }

        public static bool IsDisabledByOtherFuelFilterMods() => ModCompatibility.MedievalOverhaulIsActive || ModCompatibility.LwmFuelFilterIsActive;

        public override bool VisibleInBlueprintMode => false;

        public ITab_FuelFilter() 
        {
            size = ITab_Storage.WinSize;
            labelKey = "Fuel";
        }

        public override void OnOpen()
        {
            base.OnOpen();
            fuelFilterState.quickSearch.Reset();
        }

        public override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, size.x, size.y).ContractedBy(10f);
            Widgets.BeginGroup(rect);
            Rect rect3 = new Rect(0f, topAreaHeight, rect.width, rect.height - topAreaHeight);

            ThingFilterUI.DoThingFilterConfigWindow(rect3, fuelFilterState, compAllowedFuelTypes.allowedFuelTypesFilter, compRefuelable.Props.fuelFilter, 8);

            Widgets.EndGroup();
        }
    }
}