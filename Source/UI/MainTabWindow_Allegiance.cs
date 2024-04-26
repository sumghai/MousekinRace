using RimWorld;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class MainTabWindow_Allegiance : MainTabWindow
    {
        public override void DoWindowContents(Rect inRect)
        {
            float y = 0;
            DrawTitleBlock(inRect, ref y);

            if (GameComponent_Allegiance.Instance.alignedFaction != null)
            { 
            
            } 
            else 
            {
                DrawFactionChooser(inRect, ref y);
            }
        }

        public void DrawTitleBlock(Rect rect, ref float y)
        {
            Faction playerFaction = Find.FactionManager.OfPlayer;
            string playerFactionName = playerFaction.Name;
            string playerFactionSubtitle = playerFaction.def.LabelCap;
            Texture2D factionIcon = playerFaction.def.FactionIcon;
            Color factionIconColor = playerFaction.Color;

            // todo - modify above if player has picked allegiance

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;

            Text.Font = GameFont.Medium;
            Vector2 playerFactionNameRect = Text.CalcSize(playerFactionName);
            Text.Font = GameFont.Small;
            Vector2 playerFactionSubtitleRect = Text.CalcSize(playerFactionSubtitle);

            float spacing = 10f;
            float titleBlockHeight = playerFactionNameRect.y + playerFactionSubtitleRect.y;
            float titleBlockTextWidth = Math.Max(playerFactionNameRect.x, playerFactionSubtitleRect.x) + spacing;
            float titleBlockWidth = titleBlockHeight + spacing + titleBlockTextWidth;

            float centeredTitleRectStartingX = (rect.width - titleBlockWidth) / 2;
            float centeredTitleRectStartingY = y;

            Rect centeredTitleRect = new Rect(centeredTitleRectStartingX, centeredTitleRectStartingY, titleBlockWidth, titleBlockHeight);

            GUI.color = factionIconColor;
            GUI.DrawTexture(new Rect(centeredTitleRect.xMin, centeredTitleRect.yMin, titleBlockHeight, titleBlockHeight), factionIcon);
            GUI.color = Color.white;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredTitleRect.xMin + titleBlockHeight + spacing, centeredTitleRect.yMin, titleBlockTextWidth, playerFactionNameRect.y), playerFactionName);
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredTitleRect.xMin + titleBlockHeight + spacing, centeredTitleRect.yMin + playerFactionNameRect.y, titleBlockTextWidth, playerFactionSubtitleRect.y), playerFactionSubtitle);

            Text.Anchor = anchor;

            y += titleBlockHeight;
        }

        public void DrawFactionChooser(Rect rect, ref float y)
        {
            List<Faction> factionOptions = GameComponent_Allegiance.Instance.alliableFactions;
            float columnSpacing = StandardMargin;
            float numColumns = factionOptions.Count;
            float columnWidth = (rect.width - columnSpacing * (numColumns - 1)) / numColumns;
            float bigFactionIconSize = 100f;
            float factionDescHeight = 0;
            float buttonHeight = 45f;

            // Calculate the max faction description rect height from all the different faction descriptions
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Small;
            foreach (Faction factionOption in factionOptions) 
            {
                factionDescHeight = Math.Max(factionDescHeight, Text.CalcHeight(factionOption.def.Description.Substring(0, factionOption.def.Description.IndexOf("\n")), columnWidth - 17f * 2));
            }

            // Draw the actual 
            for (int i = 0; i < numColumns; i++) 
            {
                AlliableFactionExtension currentFactionExtension = factionOptions[i].def.GetModExtension<AlliableFactionExtension>();
                
                Rect factionChoiceRect = new Rect(i * (columnWidth + columnSpacing), y + columnSpacing, columnWidth, rect.height - y - columnSpacing);
                Widgets.DrawMenuSection(factionChoiceRect);
                Rect innerRect = factionChoiceRect.ContractedBy(17f);

                float innerY = innerRect.yMin;

                // Faction icon, name and subtitle               
                Text.Anchor = TextAnchor.MiddleCenter;

                innerY += 30f;
                Rect bigFactionIconRect = new Rect((innerRect.width - bigFactionIconSize) / 2 + innerRect.xMin, innerY, bigFactionIconSize, bigFactionIconSize);
                GUI.color = factionOptions[i].Color;
                GUI.DrawTexture(bigFactionIconRect, factionOptions[i].def.FactionIcon);
                GUI.color = Color.white;
                innerY += bigFactionIconSize;

                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(innerRect.xMin, innerY, innerRect.width, Text.CalcHeight(factionOptions[i].Name, innerRect.width)), factionOptions[i].Name);
                innerY += Text.CalcHeight(factionOptions[i].Name, innerRect.width);

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(innerRect.xMin, innerY, innerRect.width, Text.CalcHeight(factionOptions[i].def.LabelCap, innerRect.width)), factionOptions[i].def.LabelCap);
                innerY += Text.CalcHeight(factionOptions[i].def.LabelCap, innerRect.width);

                innerY += StandardMargin;

                // Faction short description (first paragraph of full description)
                string factionShortDesc = factionOptions[i].def.Description.Substring(0, factionOptions[i].def.Description.IndexOf("\n"));
                Widgets.Label(new Rect(innerRect.xMin, innerY, innerRect.width, factionDescHeight), factionShortDesc);
                innerY += factionDescHeight;

                innerY += StandardMargin;

                // Requirements
                Text.Anchor = anchor;
                Widgets.Label(new Rect(innerRect.xMin, innerY, innerRect.width, Text.CalcHeight("Requirements:", innerRect.width)), "Requirements");
                innerY += Text.CalcHeight("Requirements:", innerRect.width);

                Widgets.DrawLineHorizontal(innerRect.xMin, innerY, innerRect.width);

                innerY += 2f;

                //DrawJoiningRequirementRow(innerRect, ref innerY, currentFactionExtension.joinRequirements.minDaysPassedSinceSettle, "MousekinRace_AllegianceSys_ReqMinColonyAge".Translate((float)(currentFactionExtension.joinRequirements.minDaysPassedSinceSettle/GenDate.DaysPerYear)), GenDate.DaysPassedSinceSettle);

                /*DrawJoiningRequirementRow(innerRect, ref innerY, currentFactionExtension.joinRequirements.minMousekinPopulationPercentage, "MousekinRace_AllegianceSys_ReqMinMousekinPopPct".Translate(currentFactionExtension.joinRequirements.minMousekinPopulationPercentage.ToStringPercent()), );

                DrawJoiningRequirementRow(innerRect, ref innerY, currentFactionExtension.joinRequirements.minGoodwill, );*/

                // Benefits & Costs buttons

                // todo

                // Join button

                Widgets.ButtonText(new Rect(innerRect.xMin, innerRect.yMax - buttonHeight, innerRect.width, buttonHeight), currentFactionExtension.joinButtonLabel);
            }

        }

        public void DrawJoiningRequirementRow(Rect rect, ref float y, object requirement, string requirementFormatted, object gameVarToCompare)
        { 
            // todo
        }
    }
}
