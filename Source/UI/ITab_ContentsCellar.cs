using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class ITab_ContentsCellar : ITab_ContentsBase
    {
        public static readonly CachedTexture DropTex = new CachedTexture("UI/Buttons/Drop");

        public static readonly float buttonSize = 24f;

        public static readonly float rowHeight = 28f;

        public Building_StorageCellar StorageCellar => SelThing as Building_StorageCellar;
        
        public override IList<Thing> container => StorageCellar.GetDirectlyHeldThings().ToList();

        public override bool IsVisible
        {
            get
            {
                if (SelThing != null)
                {
                    return base.IsVisible;
                }
                return false;
            }
        }

        public override bool VisibleInBlueprintMode => false;

        public ITab_ContentsCellar()
        {
            labelKey = "TabCasketContents";
            containedItemsKey = "TabCasketContents";
        }

        public override void FillTab()
        {
            thingsToSelect.Clear();
            Rect outRect = new Rect(default(Vector2), size).ContractedBy(10f);
            outRect.yMin += 20f;
            Text.Font = GameFont.Small;
            float curY = 0f;
            DoItemsLists(outRect, ref curY);
            if (thingsToSelect.Any())
            {
                ITab_Pawn_FormingCaravan.SelectNow(thingsToSelect);
                thingsToSelect.Clear();
            }
        }

        public override void DoItemsLists(Rect inRect, ref float curY)
        {
            // Sort stored items by category, name, quality and hitpoints
            List<Thing> items = container.OrderBy(t => t.def.category).ThenBy(t => t.Label).ThenByDescending((Thing x) => 
            {
                QualityCategory c;
                x.TryGetQuality(out c);
                return (int)c;
            }).ThenByDescending((Thing x) => (x.HitPoints / x.MaxHitPoints)).ToList();
            ListContainedItems(inRect, items, ref curY);
        }

        public void ListContainedItems(Rect inRect, IList<Thing> items, ref float curY)
        {
            GUI.BeginGroup(inRect);

            // Tab heading and tooltip
            float num = curY;
            Widgets.ListSeparator(ref curY, inRect.width - 16f, containedItemsKey.Translate() + " (" + container.Count + " / " + StorageCellar.MaxStoredItems + " " + "ItemsLower".Translate() + ")");
            Rect headingRect = new Rect(0f, num, inRect.width - 16f, curY - num - 3f);
            if (Mouse.IsOver(headingRect))
            {
                Widgets.DrawHighlight(headingRect);
                TooltipHandler.TipRegionByKey(headingRect, "MousekinRace_CellarOutdoor_ContainedItemsDesc");
            }

            // Cellar contents - either empty, or a scrollable list of stored items
            if (items.NullOrEmpty())
            {
                Widgets.NoneLabel(ref curY, inRect.width - 16f);
            }
            else 
            {
                Rect scrollableAreaRect = new Rect(0f, curY, inRect.width, inRect.height - curY);
                Rect scrollableContentsRect = new Rect(0f, curY, inRect.width - 16f, rowHeight * items.Count);
                Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] is Thing thing)
                    {
                        DoRow(thing, inRect.width - 16f, i, ref curY);
                    }
                }
                Widgets.EndScrollView();
            }

            GUI.EndGroup();
        }

        public void DoRow(Thing item, float width, int i, ref float curY)
        {
            // Alternating row background, item infocard button, and mouseover highlight
            Rect curRowRect = new Rect(0f, curY, width, rowHeight);
            Widgets.InfoCardButton(0f, curY, item);
            if (Mouse.IsOver(curRowRect))
            {
                Widgets.DrawHighlightSelected(curRowRect);
            }
            else if (i % 2 == 1)
            {
                Widgets.DrawLightHighlight(curRowRect);
            }

            // Eject current item button
            Rect ejectItemButtonRect = new Rect(curRowRect.width - buttonSize, curY, buttonSize, buttonSize);
            if (Widgets.ButtonImage(ejectItemButtonRect, DropTex.Texture))
            {
                StorageCellar.GetDirectlyHeldThings().TryDrop(item, StorageCellar.InteractionCell, StorageCellar.Map, ThingPlaceMode.Near, item.stackCount, out var resultingThing);
                if (resultingThing.TryGetComp(out CompForbiddable comp))
                {
                    comp.Forbidden = true;
                }
            }
            TooltipHandler.TipRegionByKey(ejectItemButtonRect, "MousekinRace_CellarOutdoor_EjectItemTooltip");

            // Allow/forbid item button
            Rect forbidButtonRect = new Rect(curRowRect.width - 2 * buttonSize, curY, buttonSize, buttonSize);
            bool allowFlag = !item.IsForbidden(Faction.OfPlayer);
            bool tmpFlag = allowFlag;
            TooltipHandler.TipRegion(forbidButtonRect, allowFlag ? "CommandNotForbiddenDesc".Translate() : "CommandForbiddenDesc".Translate());
            Widgets.Checkbox(forbidButtonRect.x, forbidButtonRect.y, ref allowFlag, buttonSize, paintable: true);
            if (allowFlag != tmpFlag) 
            {
                ForbidUtility.SetForbidden(item, !allowFlag, false);
            }

            // Rottable status text
            Rect spoilDaysRect = new Rect(curRowRect.width - 5 * buttonSize, curY, 3 * buttonSize, buttonSize);
            if (item.TryGetComp<CompRottable>() is CompRottable compRottable)
            {
                Text.Anchor = TextAnchor.MiddleLeft;

                int ticksUntilRotAtCurrentTemp = compRottable.TicksUntilRotAtCurrentTemp;
                Widgets.Label(spoilDaysRect, ticksUntilRotAtCurrentTemp.ToStringTicksToPeriod());
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;

                float curTemp = GenTemperature.RotRateAtTemperature(Mathf.RoundToInt(StorageCellar.AmbientTemperature));
                TaggedString spoilDaysTooltipText = new();
                if (curTemp < 0.001f)
                {
                    spoilDaysTooltipText = "CurrentlyFrozen".Translate() + ".";
                }
                else if (curTemp < 0.999f)
                {
                    spoilDaysTooltipText = "CurrentlyRefrigerated".Translate(ticksUntilRotAtCurrentTemp.ToStringTicksToPeriod()) + ".";
                }
                else 
                {
                    spoilDaysTooltipText = "NotRefrigerated".Translate(ticksUntilRotAtCurrentTemp.ToStringTicksToPeriod()) + ".";
                }
                TooltipHandler.TipRegion(spoilDaysRect, spoilDaysTooltipText);
            }            

            // Item icon
            Widgets.ThingIcon(new Rect(24f, curY, curRowRect.height, curRowRect.height), item);

            // Item label
            Rect itemLabelRect = new Rect(60f, curY, curRowRect.width - 5 * buttonSize, curRowRect.height);
            itemLabelRect.xMax = ejectItemButtonRect.xMin;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(itemLabelRect, item.LabelCap.Truncate(itemLabelRect.width));
            Text.Anchor = TextAnchor.UpperLeft;

            // Item tooltip
            Rect tooltipHighlightRect = new Rect(0f, curY, width - 5 * buttonSize, curRowRect.height);
            if (Mouse.IsOver(tooltipHighlightRect)) 
            {
                TargetHighlighter.Highlight(item, arrow: true, colonistBar: false);
                TooltipHandler.TipRegion(curRowRect, item.DescriptionDetailed);
            }
            curY += curRowRect.height;
        }
    }
}
