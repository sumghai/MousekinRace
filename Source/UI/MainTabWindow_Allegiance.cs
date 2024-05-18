using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MousekinRace
{
    public class MainTabWindow_Allegiance : MainTabWindow
    {
        public List<MenuOption> menuOptions = new()
        {
            new MenuOption("AllegianceSys_Overview", "MousekinRace_AllegianceSys_Overview".Translate()),
            new MenuOption("AllegianceSys_Benefits", "MousekinRace_AllegianceSys_Benefits".Translate()),
            new MenuOption("AllegianceSys_Costs", "MousekinRace_AllegianceSys_Costs".Translate()),
            new MenuOption("AllegianceSys_Recruit", "MousekinRace_AllegianceSys_Recruit".Translate()),
            new MenuOption("AllegianceSys_CallTrader", "MousekinRace_AllegianceSys_CallTrader".Translate()),
            new MenuOption("AllegianceSys_CallMilitaryAid", "MousekinRace_AllegianceSys_CallMilitaryAid".Translate()),
            new MenuOption("AllegianceSys_Log", "MousekinRace_AllegianceSys_Log".Translate()) //,
            // new MenuOption("AllegianceSys_Change", "MousekinRace_AllegianceSys_Change".Translate()), // not yet implemented
        };

        public string selectedPageTag = "AllegianceSys_Overview";

        public override Vector2 RequestedTabSize
        {
            get
            { 
                Vector2 originalTabSize = base.RequestedTabSize;
                originalTabSize.y = 800f;
                return originalTabSize;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            float y = 0;
            if (ModsConfig.RoyaltyActive && Faction.OfEmpire != null && GameComponent_Allegiance.Instance.anyColonistsWithShatteredEmpireTitle)
            {
                DrawSystemDisabledByShatteredEmpire(inRect, ref y);
            } 
            else 
            {
                DrawTitleBlock(inRect, ref y);

                if (GameComponent_Allegiance.Instance.alignedFaction != null)
                {
                    DrawFactionHome(inRect, ref y);
                }
                else
                {
                    DrawFactionChooser(inRect, ref y);
                }
            }            
        }

        public void DrawTitleBlock(Rect rect, ref float y)
        {
            Faction playerFaction = Find.FactionManager.OfPlayer;
            string playerFactionName = playerFaction.Name;
            string playerFactionSubtitle = playerFaction.def.LabelCap;
            Texture2D factionIcon = playerFaction.def.FactionIcon;
            Color factionIconColor = playerFaction.Color;

            // Change title block contents if player has aligned with a faction
            if (GameComponent_Allegiance.Instance.alignedFaction is Faction alignedFaction)
            {
                AlliableFactionExtension alignedFactionExtension = alignedFaction.def.GetModExtension<AlliableFactionExtension>();

                playerFactionSubtitle = AllegianceSys_Utils.MembershipToFactionLabel(alignedFaction).CapitalizeFirst();
                factionIcon = alignedFaction.def.FactionIcon;
                factionIconColor = alignedFaction.Color;
            }

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

            y += titleBlockHeight + StandardMargin;
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

            // Draw the actual faction option
            for (int i = 0; i < numColumns; i++) 
            {
                AlliableFactionExtension currentFactionExtension = factionOptions[i].def.GetModExtension<AlliableFactionExtension>();
                
                Rect factionChoiceRect = new Rect(i * (columnWidth + columnSpacing), y, columnWidth, rect.height - y);
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
                Widgets.Label(new Rect(innerRect.xMin, innerY, innerRect.width, Text.CalcHeight("MousekinRace_AllegianceSys_ReqSubheading".Translate(), innerRect.width)), "MousekinRace_AllegianceSys_ReqSubheading".Translate());
                innerY += Text.CalcHeight("MousekinRace_AllegianceSys_ReqSubheading".Translate(), innerRect.width);

                Widgets.DrawLineHorizontal(innerRect.xMin, innerY, innerRect.width);

                innerY += 5f;

                float minReqYears = (float)currentFactionExtension.joinRequirements.minDaysPassedSinceSettle / GenDate.DaysPerYear;
                float yearsPassedSinceSettle = (float)GenDate.DaysPassedSinceSettle / GenDate.DaysPerYear;
                DrawJoiningRequirementRow(innerRect, ref innerY, "MousekinRace_AllegianceSys_ReqMinColonyAge".Translate(Utils.YearHumanReadable(minReqYears)), Utils.YearHumanReadable(yearsPassedSinceSettle), GenDate.DaysPassedSinceSettle >= currentFactionExtension.joinRequirements.minDaysPassedSinceSettle);

                DrawJoiningRequirementRow(innerRect, ref innerY, "MousekinRace_AllegianceSys_ReqMinMousekinPopPct".Translate(currentFactionExtension.joinRequirements.minMousekinPopulationPercentage.ToStringPercent()), Utils.PercentColonistsAreMousekins().ToStringPercent(), Utils.PercentColonistsAreMousekins() >= currentFactionExtension.joinRequirements.minMousekinPopulationPercentage);

                DrawJoiningRequirementRow(innerRect, ref innerY, "MousekinRace_AllegianceSys_ReqMinGoodwill".Translate(currentFactionExtension.joinRequirements.minGoodwill), factionOptions[i].PlayerGoodwill.ToString(), factionOptions[i].PlayerGoodwill >= currentFactionExtension.joinRequirements.minGoodwill);

                innerY += StandardMargin;

                // Benefits & Costs buttons
                float infoButtonsWidth = (innerRect.width - StandardMargin) / 2;
                if (Widgets.ButtonText(new Rect(innerRect.xMin, innerY, infoButtonsWidth, buttonHeight), "MousekinRace_AllegianceSys_ViewExtraInfoButtonLabel".Translate("MousekinRace_AllegianceSys_Benefits".Translate())))
                {
                    Find.WindowStack.Add(new Dialog_AllegianceExtraInfo("MousekinRace_AllegianceSys_ViewExtraInfoDialog_Title".Translate("MousekinRace_AllegianceSys_Benefits".Translate(), AllegianceSys_Utils.MembershipToFactionLabel(factionOptions[i], true)), AllegianceSys_Utils.GenerateBenefitsDesc(factionOptions[i]), currentFactionExtension.factionRestrictedCraftableThingDefs));
                }
                if (Widgets.ButtonText(new Rect(innerRect.xMin + infoButtonsWidth + StandardMargin, innerY, infoButtonsWidth, buttonHeight), "MousekinRace_AllegianceSys_ViewExtraInfoButtonLabel".Translate("MousekinRace_AllegianceSys_Costs".Translate())))
                {
                    Find.WindowStack.Add(new Dialog_AllegianceExtraInfo("MousekinRace_AllegianceSys_ViewExtraInfoDialog_Title".Translate("MousekinRace_AllegianceSys_Costs".Translate(), AllegianceSys_Utils.MembershipToFactionLabel(factionOptions[i], true)), AllegianceSys_Utils.GenerateCostsDesc(factionOptions[i])));
                }

                innerY += buttonHeight;
                innerY += StandardMargin;

                // Conditional info box if join requirements are not met
                bool joinRequirementsMet = GenDate.DaysPassedSinceSettle >= currentFactionExtension.joinRequirements.minDaysPassedSinceSettle
                    && Utils.PercentColonistsAreMousekins() >= currentFactionExtension.joinRequirements.minMousekinPopulationPercentage
                    && factionOptions[i].PlayerGoodwill >= currentFactionExtension.joinRequirements.minGoodwill;

                if (!joinRequirementsMet)
                {
                    innerY += 12f;
                    MainTabWindow_Quests mainTabWindow_Quests = new MainTabWindow_Quests();

                    mainTabWindow_Quests.DrawInfoBox(innerRect, false, ref innerY, "MousekinRace_AllegianceSys_ReqDesc".Translate(), MainTabWindow_Quests.acceptanceRequirementsBoxBgColor, MainTabWindow_Quests.AcceptanceRequirementsBoxColor, MainTabWindow_Quests.AcceptanceRequirementsColor);
                }

                // Development / God mode debug buttons
                if (DebugSettings.godMode)
                {
                    if (Widgets.ButtonText(new Rect(innerRect.xMin, innerRect.yMax - buttonHeight * 2 - StandardMargin, 150f, buttonHeight), "DEV: +10 Goodwill"))
                    {
                        factionOptions[i].TryAffectGoodwillWith(Faction.OfPlayer, 10, canSendMessage: true, canSendHostilityLetter: true, HistoryEventDefOf.DebugGoodwill);
                    }
                    if (Widgets.ButtonText(new Rect(innerRect.xMin + 150f + StandardMargin, innerRect.yMax - buttonHeight * 2 - StandardMargin, 150f, buttonHeight), "DEV: -10 Goodwill"))
                    {
                        factionOptions[i].TryAffectGoodwillWith(Faction.OfPlayer, -10, canSendMessage: true, canSendHostilityLetter: true, HistoryEventDefOf.DebugGoodwill);
                    }
                }

                // Join button
                Color orgColor = GUI.color;
                if (!joinRequirementsMet)
                {
                    GUI.color = Color.gray;
                }
                if (Widgets.ButtonText(new Rect(innerRect.xMin, innerRect.yMax - buttonHeight, innerRect.width, buttonHeight), currentFactionExtension.joinButtonLabel))
                {
                    if (joinRequirementsMet)
                    {
                        Faction targetFaction = factionOptions[i];
                        TaggedString targetFactionNameRendered = targetFaction.Name.IndexOf("DefiniteArticle".Translate(), StringComparison.OrdinalIgnoreCase) >= 0 ? targetFaction.Name.Colorize(targetFaction.Color) : "DefiniteArticle".Translate() + " " + targetFaction.Name.Colorize(targetFaction.Color);

                        List<Tuple<Pawn, string>> quittingColonists = AllegianceSys_Utils.GetQuittingColonistsWithReasons(targetFaction);

                        Find.TickManager.Pause();

                        TaggedString joinConfirmationMsg = 
                            "MousekinRace_AllegianceSys_JoinConfirmation".Translate(targetFactionNameRendered) + "\n\n" + 
                            AllegianceSys_Utils.GenerateShatteredEmpireQuestsDisabledDesc() +
                            AllegianceSys_Utils.GenerateQuittingColonistsWithReasonsDesc("MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartQuittingPawns", quittingColonists);

                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(joinConfirmationMsg, delegate
                        {
                            AllegianceSys_Utils.JoinFaction(targetFaction);
                        }));
                    }
                    else
                    {
                        Messages.Message("MousekinRace_MessageCannotJoinFaction".Translate(factionOptions[i].Name), MessageTypeDefOf.RejectInput, false);
                    }
                }
                GUI.color = orgColor;
            }

        }

        public void DrawSystemDisabledByShatteredEmpire(Rect rect, ref float y)
        {
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Small;

            TaggedString empireNameColored = Faction.OfEmpire.Name.Colorize(Faction.OfEmpire.Color);
            TaggedString empireNameWithArticle = empireNameColored.RawText.IndexOf("DefiniteArticle".Translate(), StringComparison.OrdinalIgnoreCase) >= 0 ? empireNameColored : "DefiniteArticle".Translate().CapitalizeFirst() + " " + empireNameColored;

            Widgets.Label(rect, "MousekinRace_AllegianceSys_DisabledDueToShatteredEmpire".Translate(empireNameWithArticle));

            Text.Anchor = anchor;
        }

        public void DrawJoiningRequirementRow(Rect rect, ref float y, string requirementFormatted, string gameVarFormatted, bool requirementMet)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(new Rect(rect.xMin, y, rect.width, Text.CalcHeight(requirementFormatted, rect.width)), requirementFormatted);
            Text.Anchor = TextAnchor.UpperRight;
            GUI.color = requirementMet ? Color.green : Color.red;
            Widgets.Label(new Rect(rect.xMin, y, rect.width, Text.CalcHeight(gameVarFormatted, rect.width)), gameVarFormatted);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            y += Text.CalcHeight(requirementFormatted, rect.width);
        }

        public void DrawFactionHome(Rect rect, ref float y)
        {
            float optionButtonWidth = 160f;
            float optionButtonHeight = 48f;
            float optionButtonVerticalSpacing = 2f;

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Small;

            for (int i = 0; i < menuOptions.Count; i++)
            {
                Rect buttonRect = new(rect.xMin, (float)i * (optionButtonHeight + optionButtonVerticalSpacing) + y, optionButtonWidth, optionButtonHeight);
                DrawMenuOptionButton(buttonRect.ContractedBy(4f), menuOptions[i]);
            }
            Text.Anchor = anchor;

            DrawSubpage(new Rect(rect.xMin + optionButtonWidth + StandardMargin, y, rect.width - (rect.xMin + optionButtonWidth + StandardMargin), rect.height - y), selectedPageTag);
        }

        public void DrawMenuOptionButton(Rect r, MenuOption option)
        {
            Widgets.DrawOptionBackground(r, option.pageTag == selectedPageTag); // todo - change background dynamically
            if (Widgets.ButtonInvisible(r))
            {
                selectedPageTag = option.pageTag;
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
            Widgets.Label(r, option.buttonLabel);
        }

        public void DrawSubpage(Rect r, string subpageTag)
        {
            Widgets.DrawMenuSection(r);
            Widgets.Label(r, subpageTag);
        }

        public class MenuOption
        {
            public string pageTag;
            public string buttonLabel;

            public MenuOption(string pageTagInput, string buttonLabelInput)
            {
                pageTag = pageTagInput;
                buttonLabel = buttonLabelInput;
            }
        }
    }
}
