using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public static class UndergroundMineSys_Utils
    {
        public static MiningBill Clipboard = null;
        
        private static readonly RecipeTooltipLayout miningOptionTooltip = new();

        public static void DoOptionInfoWindow(this MineableCountRange mineable, int mineableOptionIndex, Rect rect)
        {
            MiningOptionTooltip(mineable, false);
            if (!miningOptionTooltip.Empty) {
                Rect windowRect = Find.WindowStack.currentlyDrawnWindow.windowRect;
                Rect immRect = new(windowRect.x + rect.xMax + 10f, windowRect.y + rect.y, miningOptionTooltip.Size.x, miningOptionTooltip.Size.y);
                immRect.x = Mathf.Min(immRect.x, (float)UI.screenWidth - immRect.width);
                immRect.y = Mathf.Min(immRect.y, (float)UI.screenHeight - immRect.height);
                Find.WindowStack.ImmediateWindow(123 * (mineableOptionIndex + 1), immRect, WindowLayer.Super, delegate
                {
                    GameFont font = Text.Font;
                    Text.Font = GameFont.Small;
                    GUI.BeginGroup(immRect.AtZero());
                    MiningOptionTooltip(mineable, draw: true);
                    GUI.EndGroup();
                    Text.Font = font;
                });
            }
        }

        private static void MiningOptionTooltip(MineableCountRange mineable, bool draw)
        {
            miningOptionTooltip.Reset(6f);
            miningOptionTooltip.Label(("Products".Translate() + ": ").AsTipTitle(), draw);
            miningOptionTooltip.Newline();
            DisplayIngredientIconRow([ToTextureAndColor(mineable.mineableThing)], draw, mineable.minedPortionSize);
            if (!miningOptionTooltip.Empty)
            {
                miningOptionTooltip.Expand(6f, 6f);
            }
        }

        private static void DisplayIngredientIconRow(List<TextureAndColor> icons, bool draw, float count)
        {
            int iconCount = icons.Count;
            if (iconCount > 0)
            {
                int num = Mathf.Min(9, iconCount);
                miningOptionTooltip.Gap(8f, 0f);
                miningOptionTooltip.Label(count + "x ", draw);
                // Draw 9 or less icons
                for (int i = 0; i < num; i++)
                {
                    var textureAndColor = icons[i];
                    miningOptionTooltip.Icon(textureAndColor.Texture, textureAndColor.Color, Text.LineHeightOf(GameFont.Small), draw);
                    miningOptionTooltip.Gap(4f, 0f);
                }
                // Add etc it more than 9 icons
                if (iconCount > 9)
                {
                    Text.Anchor = TextAnchor.MiddleLeft;
                    miningOptionTooltip.Label(" " + "Etc".Translate() + "...", draw);
                    Text.Anchor = TextAnchor.UpperLeft;
                }
            }
        }

        private static TextureAndColor ToTextureAndColor(ThingDef td) => new(Widgets.GetIconFor(td), td.uiIconColor);

        public static int CountMinedProductsOnMap(MiningBill bill)
        {
            int num = 0;
            ISlotGroup includeSlotGroup = bill.GetIncludeSlotGroup();

            // Standard behaviour: count matching items in (cached) storage across the map and things carried by pawns on the same map
            if (includeSlotGroup == null) 
            {
                num += bill.Map.resourceCounter.GetCount(bill.mineableThing);

                foreach (Pawn pawn in bill.Map.mapPawns.FreeColonistsSpawned)
                {
                    Thing carriedThing = pawn.carryTracker.CarriedThing;
                    if (carriedThing != null && carriedThing.def == bill.mineableThing)
                    {
                        int carriedStackCount = carriedThing.stackCount;
                        carriedThing = carriedThing.GetInnerIfMinified();
                        if (carriedThing.SpawnedOrAnyParentSpawned && !carriedThing.PositionHeld.Fogged(carriedThing.MapHeld))
                        {
                            num += carriedStackCount;
                        }
                    }
                }
            }
            // Alternative: count only items in the specified storage slot group
            else
            {
                foreach (Thing heldThing in includeSlotGroup.HeldThings)
                {
                    Thing innerIfMinified = heldThing.GetInnerIfMinified();
                    if (innerIfMinified.def == bill.mineableThing)
                    {
                        num += innerIfMinified.stackCount;
                    }
                }
            }

            return num;
        }

        public static int GetWorkRequiredToMineResource(ThingDef mineable)
        {
            return MousekinDefOf.Mousekin_MineEntrance.GetCompProperties<CompProperties_UndergroundMineDeposits>().mineables.First(x => x.mineableThing == mineable).workRequiredPerPortionMined;
        }
    }
}
