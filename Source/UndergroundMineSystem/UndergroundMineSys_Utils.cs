using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public static class UndergroundMineSys_Utils
    {
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

            // Count items spawned in storage on the map 
            num += bill.Map.resourceCounter.GetCount(bill.mineableThing);

            // Count items being carried by pawns on the same map
            foreach (Pawn pawn in bill.Map.mapPawns.FreeColonistsSpawned)
            { 
                Thing carriedThing = pawn.carryTracker.CarriedThing;
                if (carriedThing != null && carriedThing.def == bill.mineableThing) 
                { 
                    int carriedStackCount = carriedThing.stackCount;
                    carriedThing = carriedThing.GetInnerIfMinified();
                    if (carriedThing.SpawnedOrAnyParentSpawned && !carriedThing.PositionHeld.Fogged(carriedThing.MapHeld))
                    {
                        Log.Warning($"Also counting {carriedStackCount} {carriedThing} carried by {pawn}");
                        num += carriedStackCount;
                    }
                }
            }
            
            return num;
        }
    }
}
