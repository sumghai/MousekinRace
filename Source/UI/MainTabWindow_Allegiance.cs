using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MousekinRace
{
    public class MainTabWindow_Allegiance : MainTabWindow
    {
        public List<MenuOption> menuOptions = new()
        {
            new MenuOption(PageTag.AllegianceSys_Overview, "MousekinRace_AllegianceSys_Overview".Translate()),
            new MenuOption(PageTag.AllegianceSys_Benefits, "MousekinRace_AllegianceSys_Benefits".Translate()),
            new MenuOption(PageTag.AllegianceSys_Costs, "MousekinRace_AllegianceSys_Costs".Translate()),
            new MenuOption(PageTag.AllegianceSys_Recruit, "MousekinRace_AllegianceSys_Recruit".Translate()),
            new MenuOption(PageTag.AllegianceSys_CallTrader, "MousekinRace_AllegianceSys_CallTrader".Translate()),
            new MenuOption(PageTag.AllegianceSys_CallMilitaryAid, "MousekinRace_AllegianceSys_CallMilitaryAid".Translate()),
            new MenuOption(PageTag.AllegianceSys_Log, "MousekinRace_AllegianceSys_Log".Translate()) //,
            // new MenuOption("AllegianceSys_Change", "MousekinRace_AllegianceSys_Change".Translate()), // not yet implemented
        };

        public PageTag selectedPageTag = PageTag.AllegianceSys_Overview;

        public const float uiElementSpacing = 10f;

        public const float scrollbarWidth = 16f;

        public const int optionNumColumns = 2;

        public const float optionRowHeight = 100f;

        public Color optionBg = new Color(0.13f, 0.13f, 0.13f);

        public Color optionBgMouseover = new Color(1f, 1f, 1f, 0.3f);

        public Vector2 scrollPosition = Vector2.zero;

        public static float overviewListHeight;

        public override Vector2 RequestedTabSize
        {
            get
            { 
                Vector2 originalTabSize = base.RequestedTabSize;
                originalTabSize.y = 900f;
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

        public override void PreOpen()
        {
            base.PreOpen();
            AllegianceSys_Utils.RecacheDemographicsData();
        }

        public override void PostClose()
        {
            base.PostClose();
            selectedPageTag = PageTag.AllegianceSys_Overview;
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

            float titleBlockHeight = playerFactionNameRect.y + playerFactionSubtitleRect.y;
            float titleBlockTextWidth = Math.Max(playerFactionNameRect.x, playerFactionSubtitleRect.x) + uiElementSpacing;
            float titleBlockWidth = titleBlockHeight + uiElementSpacing + titleBlockTextWidth;

            float centeredTitleRectStartingX = (rect.width - titleBlockWidth) / 2;
            float centeredTitleRectStartingY = y;

            Rect centeredTitleRect = new Rect(centeredTitleRectStartingX, centeredTitleRectStartingY, titleBlockWidth, titleBlockHeight);

            GUI.color = factionIconColor;
            GUI.DrawTexture(new Rect(centeredTitleRect.xMin, centeredTitleRect.yMin, titleBlockHeight, titleBlockHeight), factionIcon);
            GUI.color = Color.white;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(centeredTitleRect.xMin + titleBlockHeight + uiElementSpacing, centeredTitleRect.yMin, titleBlockTextWidth, playerFactionNameRect.y), playerFactionName);
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(centeredTitleRect.xMin + titleBlockHeight + uiElementSpacing, centeredTitleRect.yMin + playerFactionNameRect.y, titleBlockTextWidth, playerFactionSubtitleRect.y), playerFactionSubtitle);

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
                Faction factionOption = factionOptions[i];
                AlliableFactionExtension currentFactionExtension = factionOption.def.GetModExtension<AlliableFactionExtension>();
                
                Rect factionChoiceRect = new Rect(i * (columnWidth + columnSpacing), y, columnWidth, rect.height - y);
                Widgets.DrawMenuSection(factionChoiceRect);
                Rect innerRect = factionChoiceRect.ContractedBy(17f);

                float innerY = innerRect.yMin;

                // Faction icon, name and subtitle               
                Text.Anchor = TextAnchor.MiddleCenter;

                innerY += 30f;
                Rect bigFactionIconRect = new Rect((innerRect.width - bigFactionIconSize) / 2 + innerRect.xMin, innerY, bigFactionIconSize, bigFactionIconSize);
                GUI.color = factionOption.Color;
                GUI.DrawTexture(bigFactionIconRect, factionOption.def.FactionIcon);
                GUI.color = Color.white;
                innerY += bigFactionIconSize;

                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(innerRect.xMin, innerY, innerRect.width, Text.CalcHeight(factionOption.Name, innerRect.width)), factionOption.Name);
                innerY += Text.CalcHeight(factionOption.Name, innerRect.width);

                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(innerRect.xMin, innerY, innerRect.width, Text.CalcHeight(factionOption.def.LabelCap, innerRect.width)), factionOption.def.LabelCap);
                innerY += Text.CalcHeight(factionOption.def.LabelCap, innerRect.width);

                innerY += StandardMargin;

                // Faction short description (first paragraph of full description)
                string factionShortDesc = factionOption.def.Description.Substring(0, factionOption.def.Description.IndexOf("\n"));
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

                DrawJoiningRequirementRow(innerRect, ref innerY, "MousekinRace_AllegianceSys_ReqMinGoodwill".Translate(currentFactionExtension.joinRequirements.minGoodwill), factionOption.PlayerGoodwill.ToString(), factionOption.PlayerGoodwill >= currentFactionExtension.joinRequirements.minGoodwill);

                innerY += StandardMargin;

                // Benefits & Costs buttons
                float infoButtonsWidth = (innerRect.width - StandardMargin) / 2;
                if (Widgets.ButtonText(new Rect(innerRect.xMin, innerY, infoButtonsWidth, buttonHeight), "MousekinRace_AllegianceSys_ViewExtraInfoButtonLabel".Translate("MousekinRace_AllegianceSys_Benefits".Translate())))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    Find.WindowStack.Add(new Dialog_AllegianceExtraInfo("MousekinRace_AllegianceSys_ViewExtraInfoDialog_Title".Translate("MousekinRace_AllegianceSys_Benefits".Translate(), AllegianceSys_Utils.MembershipToFactionLabel(factionOption, true)), AllegianceSys_Utils.GenerateBenefitsDesc(factionOption), currentFactionExtension.factionRestrictedCraftableThingDefs));
                }
                if (Widgets.ButtonText(new Rect(innerRect.xMin + infoButtonsWidth + StandardMargin, innerY, infoButtonsWidth, buttonHeight), "MousekinRace_AllegianceSys_ViewExtraInfoButtonLabel".Translate("MousekinRace_AllegianceSys_Costs".Translate())))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    Find.WindowStack.Add(new Dialog_AllegianceExtraInfo("MousekinRace_AllegianceSys_ViewExtraInfoDialog_Title".Translate("MousekinRace_AllegianceSys_Costs".Translate(), AllegianceSys_Utils.MembershipToFactionLabel(factionOption, true)), AllegianceSys_Utils.GenerateCostsDesc(factionOption)));
                }

                innerY += buttonHeight;
                innerY += StandardMargin;

                // Conditional info box if join requirements are not met
                bool joinRequirementsMet = AllegianceSys_Utils.FactionJoinRequirementsMet(factionOption);

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
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        factionOption.TryAffectGoodwillWith(Faction.OfPlayer, 10, canSendMessage: true, canSendHostilityLetter: true, HistoryEventDefOf.DebugGoodwill);
                    }
                    if (Widgets.ButtonText(new Rect(innerRect.xMin + 150f + StandardMargin, innerRect.yMax - buttonHeight * 2 - StandardMargin, 150f, buttonHeight), "DEV: -10 Goodwill"))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        factionOption.TryAffectGoodwillWith(Faction.OfPlayer, -10, canSendMessage: true, canSendHostilityLetter: true, HistoryEventDefOf.DebugGoodwill);
                    }
                    if (Widgets.ButtonText(new Rect(innerRect.xMin + (150f + StandardMargin) * 2, innerRect.yMax - buttonHeight * 2 - StandardMargin, 100f, buttonHeight), "DEV: Join"))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        AllegianceSys_Utils.JoinFaction(factionOption);
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
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    if (joinRequirementsMet)
                    {
                        
                        TaggedString targetFactionNameRendered = factionOption.Name.IndexOf("DefiniteArticle".Translate(), StringComparison.OrdinalIgnoreCase) >= 0 ? factionOption.Name.Colorize(factionOption.Color) : "DefiniteArticle".Translate() + " " + factionOption.Name.Colorize(factionOption.Color);

                        List<Tuple<Pawn, string>> quittingColonists = AllegianceSys_Utils.GetQuittingColonistsWithReasons(factionOption);

                        Find.TickManager.Pause();

                        TaggedString joinConfirmationMsg = 
                            "MousekinRace_AllegianceSys_JoinConfirmation".Translate(targetFactionNameRendered) + "\n\n" + 
                            AllegianceSys_Utils.GenerateShatteredEmpireQuestsDisabledDesc() +
                            AllegianceSys_Utils.GenerateQuittingColonistsWithReasonsDesc("MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartQuittingPawns", quittingColonists);

                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(joinConfirmationMsg, delegate
                        {
                            AllegianceSys_Utils.JoinFaction(factionOption);
                        }));
                    }
                    else
                    {
                        Messages.Message("MousekinRace_MessageCannotJoinFaction".Translate(factionOption.Name), MessageTypeDefOf.RejectInput, false);
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

            DrawPageBlock(new Rect(rect.xMin + optionButtonWidth + StandardMargin, y, rect.width - (rect.xMin + optionButtonWidth + StandardMargin), rect.height - y), selectedPageTag);
        }

        public void DrawMenuOptionButton(Rect r, MenuOption option)
        {
            Widgets.DrawOptionBackground(r, option.pageTag == selectedPageTag);
            if (Widgets.ButtonInvisible(r))
            {
                selectedPageTag = option.pageTag;
                SoundDefOf.Click.PlayOneShotOnCamera();

                switch (selectedPageTag)
                {
                    // Update colony pawn demographics data every time we switch to the Overview page
                    case PageTag.AllegianceSys_Overview:
                        AllegianceSys_Utils.RecacheDemographicsData();
                        break;
                    // Update available silver every time we switch to the Invite Settlers page
                    case PageTag.AllegianceSys_Recruit:
                        GameComponent_Allegiance.Instance.RecacheAvailableSilver();
                        break;
                }
            }
            Widgets.Label(r, option.buttonLabel);
        }

        public void DrawPageBlock(Rect r, PageTag pageTag)
        {
            Widgets.DrawMenuSection(r);
            Rect innerRect = r.ContractedBy(17f);

            switch (pageTag)
            {
                case PageTag.AllegianceSys_Overview:
                    DrawPageOverview(innerRect);
                    break;
                case PageTag.AllegianceSys_Benefits:
                    DrawPageBenefits(innerRect); 
                    break;
                case PageTag.AllegianceSys_Costs:
                    DrawPageCosts(innerRect);
                    break;
                case PageTag.AllegianceSys_Recruit:
                    DrawPageRecruit(innerRect);
                    break;
                case PageTag.AllegianceSys_CallTrader:
                    DrawPageTraders(innerRect);
                    break;
                case PageTag.AllegianceSys_CallMilitaryAid:
                    DrawPageMilitaryAid(innerRect);
                    break;
                case PageTag.AllegianceSys_Log:
                    DrawPageLog(innerRect);
                    break;
                default:
                    Widgets.Label(innerRect, pageTag.ToStringSafe());
                    break;
            }            
        }

        public void DrawPageOverview(Rect r)
        {
            GameComponent_Allegiance gameComponent = GameComponent_Allegiance.Instance;           
            AlliableFactionExtension factionExtension = gameComponent.alignedFaction.def.GetModExtension<AlliableFactionExtension>();

            // Update demographics data every x period, and only when the current (Overview) page is open
            if (GenTicks.TicksGame % (GenTicks.TicksPerRealSecond * 5) == 0)
            {
                AllegianceSys_Utils.RecacheDemographicsData();
            }

            Rect scrollableAreaRect = new Rect(r);

            float scrollableContentsWidth = scrollableAreaRect.width - scrollbarWidth;
            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, overviewListHeight);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
            float curY = 0f;

            // --- General ---
            Widgets.ListSeparator(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_GeneralCatHeading".Translate());

            Vector2 location = ((Find.CurrentMap != null) ? Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile) : default);
            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_GeneralJoinDate".Translate(), GenDate.DateReadoutStringAt(gameComponent.allegiancePledgedTick, location));
            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_GeneralTimeAsMember".Translate(factionExtension.membershipTypeLabel), (Find.TickManager.TicksAbs - gameComponent.allegiancePledgedTick).ToStringTicksToPeriod());

            // --- Demographics ---
            Widgets.ListSeparator(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_DemographicsCatHeading".Translate());

            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_DemographicsPopulation".Translate(), gameComponent.histDemographicsPopulationTotal.ToString());
            List<(TaggedString label, float valuePct)> raceDemographicsData = new List<(TaggedString label, float valuePct)>
            {
                (label: MousekinDefOf.Mousekin.LabelCap, valuePct: (float) gameComponent.histDemographicsPopulationMousekin / gameComponent.histDemographicsPopulationTotal),
                (label: "MousekinRace_AllegianceSys_Overview_DemographicsNonMousekinRodents".Translate(), valuePct: (float) gameComponent.histDemographicsPopulationNonMousekinRodentkinds / gameComponent.histDemographicsPopulationTotal),
                (label: "MousekinRace_AllegianceSys_Overview_DemographicsOther".Translate(), valuePct: (float) gameComponent.histDemographicsPopulationOther / gameComponent.histDemographicsPopulationTotal)
            };
            TaggedString raceDemographicDataOutput = TaggedString.Empty;
            foreach ((TaggedString label, float valuePct) in raceDemographicsData)
            {
                if (valuePct > 0) 
                {
                    raceDemographicDataOutput += valuePct.ToStringPercent("0.0") + " " + label + "\n";
                }
            }
            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_DemographicsRaces".Translate(), raceDemographicDataOutput.Trim());
            if (ModsConfig.BiotechActive)
            {
                List<(TaggedString label, float valuePct)> ageDemographicsData = new List<(TaggedString label, float valuePct)>
                {
                    (label: "Adults".Translate().CapitalizeFirst(), valuePct: (float) gameComponent.histDemographicsPopulationAgeAdults / gameComponent.histDemographicsPopulationTotal),
                    (label: "Children".Translate().CapitalizeFirst(), valuePct: (float) gameComponent.histDemographicsPopulationAgeChildren / gameComponent.histDemographicsPopulationTotal),
                    (label: "Babies".Translate().CapitalizeFirst(), valuePct: (float) gameComponent.histDemographicsPopulationAgeBabies / gameComponent.histDemographicsPopulationTotal)
                };
                TaggedString ageDemographicDataOutput = TaggedString.Empty;
                foreach ((TaggedString label, float valuePct) in ageDemographicsData)
                {
                    if (valuePct > 0)
                    {
                        ageDemographicDataOutput += valuePct.ToStringPercent("0.0") + " " + label + "\n";
                    }
                }
                DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_DemographicsAges".Translate(), ageDemographicDataOutput.Trim());
            }
            
            TaggedString religiousAffinityDemographicDataOutput = TaggedString.Empty;
            foreach (ReligiousAffinityPawnCount religiousAffinityPawnCount in gameComponent.histDemographicsPopulationReligiousAffinity)
            {
                if (religiousAffinityPawnCount.pawnsWithTrait > 0)
                {
                    float currentAffinityPct = (float) religiousAffinityPawnCount.pawnsWithTrait / gameComponent.histDemographicsPopulationTotal;
                    religiousAffinityDemographicDataOutput += currentAffinityPct.ToStringPercent("0.0") + " " + religiousAffinityPawnCount.affinityLabel + "\n";
                }
            }
            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_DemographicsReligiousAffinity".Translate(), religiousAffinityDemographicDataOutput.Trim());

            // --- Statistics ---
            Widgets.ListSeparator(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_StatisticsCatHeading".Translate());

            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_StatisticsNewSettlersInvited".Translate(), gameComponent.histNewColonistsInvited.ToString());
            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_StatisticsRandCaravanVisits".Translate(), gameComponent.histVisitsFromRandTraders.ToString());
            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_StatisticsRequestedCaravanVisits".Translate(), gameComponent.histVisitsFromRequestedTraders.ToString());
            DrawOverviewStatListing(ref curY, scrollableContentsWidth, "MousekinRace_AllegianceSys_Overview_StatisticsMilitaryAid".Translate(), gameComponent.histMilitaryAidRequested.ToString());

            overviewListHeight = curY;

            Widgets.EndScrollView();
        }

        public void DrawPageBenefits(Rect r)
        {
            Rect scrollableAreaRect = new Rect(r);

            TaggedString contents = AllegianceSys_Utils.GenerateBenefitsDesc(GameComponent_Allegiance.Instance.alignedFaction);
            float scrollableContentsWidth = scrollableAreaRect.width - scrollbarWidth;
            float textBlockHeight = Text.CalcHeight(contents, scrollableContentsWidth);

            float craftableItemsHyperlinksY = textBlockHeight;
            float craftableItemsHyperlinkRowHeight = 24f;
            Rect craftableItemsHyperlinksRect = new Rect(0f, craftableItemsHyperlinksY, scrollableContentsWidth, 0f);
            List<ThingDef> unlockedCraftableThingDefs = GameComponent_Allegiance.Instance.alignedFaction.def.GetModExtension<AlliableFactionExtension>().factionRestrictedCraftableThingDefs;
            if (unlockedCraftableThingDefs != null && unlockedCraftableThingDefs.Count > 0)
            {
                craftableItemsHyperlinksRect.height = craftableItemsHyperlinkRowHeight * unlockedCraftableThingDefs.Count;
            }

            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, textBlockHeight + craftableItemsHyperlinksRect.height);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
            Widgets.Label(scrollableContentsRect, contents);
            if (unlockedCraftableThingDefs != null && unlockedCraftableThingDefs.Count > 0)
            {
                foreach (ThingDef def in unlockedCraftableThingDefs)
                {
                    Dialog_InfoCard.Hyperlink hyperlink = new Dialog_InfoCard.Hyperlink(def);
                    Widgets.HyperlinkWithIcon(new Rect(0f, craftableItemsHyperlinksY, scrollableContentsWidth, craftableItemsHyperlinkRowHeight), hyperlink);
                    craftableItemsHyperlinksY += craftableItemsHyperlinkRowHeight;
                }
            }
            Widgets.EndScrollView();
        }

        public void DrawPageCosts(Rect r)
        {
            Rect scrollableAreaRect = new Rect(r);

            TaggedString contents = AllegianceSys_Utils.GenerateCostsDesc(GameComponent_Allegiance.Instance.alignedFaction);
            float scrollableContentsWidth = scrollableAreaRect.width - scrollbarWidth;
            float textBlockHeight = Text.CalcHeight(contents, scrollableContentsWidth);

            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, textBlockHeight);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
            Widgets.Label(scrollableContentsRect, contents);
            Widgets.EndScrollView();
        }

        public void DrawPageRecruit(Rect r)
        {
            float availableSilver = GameComponent_Allegiance.Instance.availableSilver;
            
            RecruitableColonistSettings recruitableColonistSettings = GameComponent_Allegiance.Instance.alignedFaction.def.GetModExtension<AlliableFactionExtension>().recruitableColonistSettings;
            List<RecruitableOptions> recruitableOptions = recruitableColonistSettings.options;

            var listingStandard = new Listing_Standard();
            listingStandard.Begin(r);

            string silverAvailableValue = availableSilver.ToString();
            Vector2 silverTextSize = Text.CalcSize(silverAvailableValue);
            Rect silverAvailableReadoutRect = new Rect(0, 0, silverTextSize.y + StandardMargin / 2 + silverTextSize.x, silverTextSize.y);
            Rect silverIconRect = new Rect(silverAvailableReadoutRect.xMin, silverAvailableReadoutRect.yMin, silverAvailableReadoutRect.height, silverAvailableReadoutRect.height);
            Rect silverTextRect = new Rect(silverIconRect.xMin + silverIconRect.width + StandardMargin / 2, silverAvailableReadoutRect.yMin, silverTextSize.x, silverAvailableReadoutRect.height);
            Widgets.ThingIcon(silverIconRect, ThingDefOf.Silver);
            Widgets.Label(silverTextRect, silverAvailableValue);

            string viewNewColonistQueueButtonLabel = "MousekinRace_AllegianceSys_Recruit_ViewQueueButtonLabel".Translate();
            Vector2 viewNewColonistQueueButtonSize = Text.CalcSize(viewNewColonistQueueButtonLabel);
            viewNewColonistQueueButtonSize.x += StandardMargin * 2;
            Rect viewNewColonistQueueButtonRect = new Rect(0, silverAvailableReadoutRect.yMax + listingStandard.verticalSpacing, viewNewColonistQueueButtonSize.x, viewNewColonistQueueButtonSize.y);
            Color orgColor = GUI.color;
            if (GameComponent_Allegiance.Instance.recruitedColonistsQueue.NullOrEmpty())
            {
                GUI.color = Color.gray;
            }
            if (Widgets.ButtonText(viewNewColonistQueueButtonRect, viewNewColonistQueueButtonLabel))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                if (GameComponent_Allegiance.Instance.recruitedColonistsQueue.Count > 0)
                {
                    Find.WindowStack.Add(new Dialog_AllegianceQueuedNewColonists(GameComponent_Allegiance.Instance.recruitedColonistsQueue));
                }
                else
                {
                    Messages.Message("MousekinRace_MessageNewColonistsQueueEmpty".Translate(), MessageTypeDefOf.RejectInput, false);
                }
            }
            GUI.color = orgColor;

            listingStandard.curY = viewNewColonistQueueButtonRect.yMax + listingStandard.verticalSpacing;
            listingStandard.GapLine();
            string familyOptionNote = "MousekinRace_AllegianceSys_Recruit_FamilyNoteDesc".Translate(ModsConfig.BiotechActive ? "MousekinRace_AllegianceSys_Recruit_FamilyNoteBiotechSuffix".Translate() : null);
            listingStandard.Label(familyOptionNote);
            listingStandard.Gap();
            
            Rect scrollableAreaRect = new Rect(0, listingStandard.curY, r.width, r.height - listingStandard.curY);
            int rows = recruitableOptions.Count / optionNumColumns + ((recruitableOptions.Count % optionNumColumns > 0) ? 1 : 0);
            float optionSpacing = StandardMargin;
            float scrollableContentsWidth = scrollableAreaRect.width - scrollbarWidth;
            float scrollableContentsHeight = rows * (optionRowHeight + optionSpacing) - optionSpacing;
            float optionColumnWidth = (scrollableContentsWidth - optionSpacing * (optionNumColumns - 1)) / optionNumColumns;
            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, scrollableContentsHeight);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect); 
            for (int i = 0; i < recruitableOptions.Count; i++) 
            {
                float currentOptionRectOriginX = (i % optionNumColumns == 0) ? 0f : optionColumnWidth + optionSpacing;
                float currentOptionRectOriginY = (i / optionNumColumns) * (optionRowHeight + optionSpacing);
                Rect currentOptionRect = new Rect(currentOptionRectOriginX, currentOptionRectOriginY, optionColumnWidth, optionRowHeight); 
                Widgets.DrawBoxSolid(currentOptionRect, optionBg);
                GUI.color = optionBgMouseover;
                Widgets.DrawHighlightIfMouseover(currentOptionRect);
                GUI.color = Color.white;

                Rect currentOptionInnerRect = currentOptionRect.ContractedBy(uiElementSpacing);
                Rect currentOptionIconRect = new Rect(currentOptionInnerRect.xMin, currentOptionInnerRect.yMin, currentOptionInnerRect.height, currentOptionInnerRect.height);
                Rect currentOptionControlsRect = new Rect(currentOptionIconRect.xMax + uiElementSpacing, currentOptionInnerRect.yMin, currentOptionInnerRect.width - currentOptionIconRect.width - uiElementSpacing, currentOptionInnerRect.height);

                Texture2D iconTex = ContentFinder<Texture2D>.Get(recruitableOptions[i].iconPath, true);
                GUI.color = GameComponent_Allegiance.Instance.alignedFaction.Color;
                GUI.DrawTexture(currentOptionIconRect, iconTex);
                GUI.color = Color.white;

                TaggedString optionName = recruitableOptions[i].pawnKind.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst();
                TaggedString optionPawnCount = ((recruitableOptions[i].count > 1) ? "(x" + recruitableOptions[i].count + ")" : "");
                Widgets.Label(currentOptionControlsRect, optionName + " " + optionPawnCount);

                TextAnchor anchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleCenter;
                float inviteButtonWidth = (currentOptionControlsRect.width - uiElementSpacing) / 2;
                float inviteButtonHeight = Text.CalcHeight("MousekinRace_AllegianceSys_Recruit_InviteSingleButtonLabel".Translate(), inviteButtonWidth) + uiElementSpacing;

                PawnKindDef recruitablePawnKind = recruitableOptions[i].pawnKind;
                PawnKindDef recruitableSpousePawnKind = recruitableOptions[i].overrideSpousePawnKind ?? recruitableColonistSettings.defaultSpousePawnKind;
                bool recruitablePawnCanHaveFamily = recruitableOptions[i].canHaveFamily;
                int recruitablePawnCount = recruitableOptions[i].count;
                int inviteCost = recruitableOptions[i].basePrice;

                if (recruitablePawnCanHaveFamily)
                {
                    int inviteCostFamily = inviteCost + recruitableColonistSettings.priceOffsetWithSpouse + (ModsConfig.BiotechActive ? recruitableColonistSettings.priceOffsetWithChildren : 0);

                    Rect inviteFamilyButtonRect = new Rect(currentOptionControlsRect.xMin, currentOptionControlsRect.yMax - inviteButtonHeight, inviteButtonWidth, inviteButtonHeight);

                    if (inviteCostFamily > availableSilver)
                    {
                        GUI.color = Color.gray;
                    }

                    if (Widgets.ButtonText(inviteFamilyButtonRect, "MousekinRace_AllegianceSys_Recruit_InviteFamilyButtonLabel".Translate(inviteCostFamily)))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        if (inviteCostFamily <= availableSilver)
                        {
                            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("MousekinRace_AllegianceSys_Recruit_Confirmation".Translate("IndefiniteForm".Translate(optionName), "MousekinRace_AllegianceSys_Recruit_ConfirmationFamily".Translate(), inviteCostFamily), delegate
                            {
                                AllegianceSys_Utils.GenerateNewColonistsToQueue(inviteCostFamily, recruitablePawnKind, 1, recruitablePawnCanHaveFamily, recruitableSpousePawnKind);
                                SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                            }));
                        }
                        else
                        {
                            Messages.Message("NotEnoughSilver".Translate(), MessageTypeDefOf.RejectInput, false);
                        }
                    }

                    GUI.color = orgColor;
                }

                Rect inviteSingleButtonRect = new Rect(currentOptionControlsRect.xMin + uiElementSpacing + inviteButtonWidth, currentOptionControlsRect.yMax - inviteButtonHeight, inviteButtonWidth, inviteButtonHeight);

                if (inviteCost > availableSilver)
                {
                    GUI.color = Color.gray;
                }

                if (Widgets.ButtonText(inviteSingleButtonRect, "MousekinRace_AllegianceSys_Recruit_InviteSingleButtonLabel".Translate(inviteCost)))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    if (inviteCost <= availableSilver)
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("MousekinRace_AllegianceSys_Recruit_Confirmation".Translate(recruitablePawnCount > 1 ? recruitablePawnCount + "x " + recruitablePawnKind.labelPlural.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst() : "IndefiniteForm".Translate(optionName), "", inviteCost), delegate
                        {
                            AllegianceSys_Utils.GenerateNewColonistsToQueue(inviteCost, recruitablePawnKind, recruitablePawnCount);
                            SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                        }));
                    }
                    else
                    {
                        Messages.Message("NotEnoughSilver".Translate(), MessageTypeDefOf.RejectInput, false);
                    }                    
                }

                GUI.color = orgColor;

                Text.Anchor = anchor;

            }
            Widgets.EndScrollView();

            listingStandard.End();

            // Development / God mode debug buttons
            if (DebugSettings.godMode)
            {
                if (!GameComponent_Allegiance.Instance.recruitedColonistsQueue.NullOrEmpty())
                {
                    string devSpawnNextNewColonistsButtonLabel = "DEV: Spawn next new colonists now";
                    Vector2 devSpawnNextNewColonistsButtonSize = Text.CalcSize(devSpawnNextNewColonistsButtonLabel);
                    devSpawnNextNewColonistsButtonSize.x += StandardMargin * 2;
                    Rect devSpawnNextNewColonistsButtonRect = new Rect(r.xMax - devSpawnNextNewColonistsButtonSize.x, r.yMin + devSpawnNextNewColonistsButtonSize.y + listingStandard.verticalSpacing, devSpawnNextNewColonistsButtonSize.x, devSpawnNextNewColonistsButtonSize.y);

                    if (Widgets.ButtonText(devSpawnNextNewColonistsButtonRect, devSpawnNextNewColonistsButtonLabel))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        GameComponent_Allegiance.Instance.SpawnNextNewColonists();
                    }
                }
            }
        }

        public void DrawPageTraders(Rect r)
        {
            List<TraderKindDef> traderOptions = GameComponent_Allegiance.Instance.alignedFaction.def.caravanTraderKinds;

            var listingStandard = new Listing_Standard();
            listingStandard.Begin(r);

            int ticksUntilNextRandTrader = GameComponent_Allegiance.Instance.nextRandTraderTick - Find.TickManager.TicksGame;
            listingStandard.Label("MousekinRace_AllegianceSys_Trader_NextRandomCaravanArrivesIn".Translate(ticksUntilNextRandTrader.ToStringTicksToPeriod()));

            if (GameComponent_Allegiance.Instance.nextRequestedTraderKind is TraderKindDef nextRequestedTraderKind)
            {
                int ticksRemainingForRequestedTradeCaravan = GameComponent_Allegiance.Instance.nextRequestedTraderTick - Find.TickManager.TicksGame;
                listingStandard.Label("MousekinRace_AllegianceSys_Trader_NextRequestedCaravanArrivesIn".Translate(ticksRemainingForRequestedTradeCaravan.ToStringTicksToPeriod(), nextRequestedTraderKind.LabelCap));
            }
            else if (GameComponent_Allegiance.Instance.nextRequestedTraderCooldownTick > Find.TickManager.TicksGame)
            {
                int cooldownTicksRemainingForRequestingTradeCaravans = GameComponent_Allegiance.Instance.nextRequestedTraderCooldownTick - Find.TickManager.TicksGame;
                listingStandard.Label("MousekinRace_AllegianceSys_Trader_RequestedCaravanCooldown".Translate(cooldownTicksRemainingForRequestingTradeCaravans.ToStringTicksToPeriod()));
            }
            else
            {
                listingStandard.Label("");
            }

            listingStandard.GapLine();
            listingStandard.Gap();

            Rect scrollableAreaRect = new Rect(0, listingStandard.curY, r.width, r.height - listingStandard.curY);
            int rows = traderOptions.Count / optionNumColumns + ((traderOptions.Count % optionNumColumns > 0) ? 1 : 0);
            float optionSpacing = StandardMargin;
            float scrollableContentsWidth = scrollableAreaRect.width - scrollbarWidth;
            float scrollableContentsHeight = rows * (optionRowHeight + optionSpacing) - optionSpacing;
            float optionColumnWidth = (scrollableContentsWidth - optionSpacing * (optionNumColumns - 1)) / optionNumColumns;
            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, scrollableContentsHeight);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
            for (int i = 0; i < traderOptions.Count; i++)
            {
                float currentOptionRectOriginX = (i % optionNumColumns == 0) ? 0f : optionColumnWidth + optionSpacing;
                float currentOptionRectOriginY = (i / optionNumColumns) * (optionRowHeight + optionSpacing);
                Rect currentOptionRect = new Rect(currentOptionRectOriginX, currentOptionRectOriginY, optionColumnWidth, optionRowHeight);
                Widgets.DrawBoxSolid(currentOptionRect, optionBg);
                GUI.color = optionBgMouseover;
                Widgets.DrawHighlightIfMouseover(currentOptionRect);
                GUI.color = Color.white;

                Rect currentOptionInnerRect = currentOptionRect.ContractedBy(uiElementSpacing);
                Rect currentOptionIconRect = new Rect(currentOptionInnerRect.xMin, currentOptionInnerRect.yMin, currentOptionInnerRect.height, currentOptionInnerRect.height);
                Rect currentOptionControlsRect = new Rect(currentOptionIconRect.xMax + uiElementSpacing, currentOptionInnerRect.yMin, currentOptionInnerRect.width - currentOptionIconRect.width - uiElementSpacing, currentOptionInnerRect.height);

                Texture2D iconTex = ContentFinder<Texture2D>.Get(traderOptions[i].GetModExtension<TraderExtension>().iconPath, true);
                GUI.color = GameComponent_Allegiance.Instance.alignedFaction.Color;
                GUI.DrawTexture(currentOptionIconRect, iconTex);
                GUI.color = Color.white;

                TraderKindDef traderOption = traderOptions[i];
                TaggedString traderOptionName = traderOption.LabelCap;
                Widgets.Label(currentOptionControlsRect, traderOptionName);

                TextAnchor anchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleCenter;
                string requestTraderButtonLabel = "MousekinRace_AllegianceSys_CallTrader".Translate();
                float requestTraderButtonWidth = (currentOptionControlsRect.width - uiElementSpacing) / 2;
                float requestTraderButtonHeight = Text.CalcHeight(requestTraderButtonLabel, requestTraderButtonWidth) + uiElementSpacing;
                
                Rect requestTraderButtonRect = new Rect(currentOptionControlsRect.xMin + uiElementSpacing + requestTraderButtonWidth, currentOptionControlsRect.yMax - requestTraderButtonHeight, requestTraderButtonWidth, requestTraderButtonHeight);

                Color orgColor = GUI.color;

                if (GameComponent_Allegiance.Instance.nextRequestedTraderKind != null || GameComponent_Allegiance.Instance.nextRequestedTraderCooldownTick > Find.TickManager.TicksGame)
                {
                    GUI.color = Color.gray;
                }

                if (Widgets.ButtonText(requestTraderButtonRect, requestTraderButtonLabel))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    if (GameComponent_Allegiance.Instance.nextRequestedTraderKind == null && Find.TickManager.TicksGame > GameComponent_Allegiance.Instance.nextRequestedTraderCooldownTick)
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("MousekinRace_AllegianceSys_Trader_Confirmation".Translate(traderOptionName, "PeriodDays".Translate(GameComponent_Allegiance.requestArrivalDelayDays)), delegate
                        {
                            GameComponent_Allegiance.Instance.SetNextRequestedTraderKind(traderOption);
                            GameComponent_Allegiance.Instance.SetNextRequestedTraderTick();
                            GameComponent_Allegiance.Instance.SetNextRequestedTraderCooldownTick();
                        }));
                    }
                    else 
                    {
                        int cooldownTicksRemainingForRequestingTradeCaravans = GameComponent_Allegiance.Instance.nextRequestedTraderCooldownTick - Find.TickManager.TicksGame;
                        Messages.Message("MousekinRace_MessageCannotRequestTraderCooldownActive".Translate(cooldownTicksRemainingForRequestingTradeCaravans.ToStringTicksToPeriod()), MessageTypeDefOf.RejectInput, false);
                    }  
                }

                GUI.color = orgColor;

                Text.Anchor = anchor;

            }
            Widgets.EndScrollView();

            listingStandard.End();

            // Development / God mode debug buttons
            if (DebugSettings.godMode)
            {              
                string devRandCaravanTraderButtonLabel = "DEV: Spawn random caravan trader now";
                Vector2 devRandCaravanTraderButtonSize = Text.CalcSize(devRandCaravanTraderButtonLabel);
                devRandCaravanTraderButtonSize.x += StandardMargin * 2;
                Rect devRandCaravanTraderButtonRect = new Rect(r.xMax - devRandCaravanTraderButtonSize.x, r.yMin, devRandCaravanTraderButtonSize.x, devRandCaravanTraderButtonSize.y);

                if (Widgets.ButtonText(devRandCaravanTraderButtonRect, devRandCaravanTraderButtonLabel))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    GameComponent_Allegiance.Instance.SpawnRandTrader();
                }

                if (GameComponent_Allegiance.Instance.nextRequestedTraderTick > Find.TickManager.TicksGame)
                {
                    string devRequestedCaravanTraderButtonLabel = "DEV: Spawn requested caravan trader now";
                    Vector2 devRequestedCaravanTraderButtonSize = Text.CalcSize(devRequestedCaravanTraderButtonLabel);
                    devRequestedCaravanTraderButtonSize.x += StandardMargin * 2;
                    Rect devRequestedCaravanTraderButtonRect = new Rect(r.xMax - devRequestedCaravanTraderButtonSize.x, r.yMin + devRandCaravanTraderButtonSize.y, devRequestedCaravanTraderButtonSize.x, devRequestedCaravanTraderButtonSize.y);

                    if (Widgets.ButtonText(devRequestedCaravanTraderButtonRect, devRequestedCaravanTraderButtonLabel))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        GameComponent_Allegiance.Instance.SpawnRequestedTrader();
                    }
                }
                else if (GameComponent_Allegiance.Instance.nextRequestedTraderCooldownTick > Find.TickManager.TicksGame)
                {
                    string devResetCooldownForCaravanTraderButtonLabel = "DEV: Reset cooldown now";
                    Vector2 devResetCooldownForCaravanTraderButtonSize = Text.CalcSize(devResetCooldownForCaravanTraderButtonLabel);
                    devResetCooldownForCaravanTraderButtonSize.x += StandardMargin * 2;
                    Rect devResetCooldownForCaravanTraderButtonRect = new Rect(r.xMax - devResetCooldownForCaravanTraderButtonSize.x, r.yMin + devRandCaravanTraderButtonSize.y, devResetCooldownForCaravanTraderButtonSize.x, devResetCooldownForCaravanTraderButtonSize.y);

                    if (Widgets.ButtonText(devResetCooldownForCaravanTraderButtonRect, devResetCooldownForCaravanTraderButtonLabel))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        GameComponent_Allegiance.Instance.ClearRequestedTraderCooldownTick();
                    }
                }
            }
        }

        public void DrawPageMilitaryAid(Rect r)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(r);
            listingStandard.Label("MousekinRace_AllegianceSys_MilitaryAid_Desc".Translate(AllegianceSys_Utils.MembershipToFactionLabel(GameComponent_Allegiance.Instance.alignedFaction)));

            int ticksUntilMilitaryAid = GameComponent_Allegiance.Instance.militaryAidArrivalTick - Find.TickManager.TicksGame;

            if (GameComponent_Allegiance.Instance.militaryAidArrivalTick > 0)
            {
                listingStandard.Gap();

                // Development / God mode debug button
                if (DebugSettings.godMode)
                {
                    string devMilitaryAidNowButtonLabel = "DEV: Spawn military aid now";
                    Vector2 devMilitaryAidNowButtonSize = Text.CalcSize(devMilitaryAidNowButtonLabel);
                    devMilitaryAidNowButtonSize.x += StandardMargin * 2;
                    Rect devMilitaryAidNowButtonRect = new Rect(r.width - devMilitaryAidNowButtonSize.x, listingStandard.curY, devMilitaryAidNowButtonSize.x, devMilitaryAidNowButtonSize.y);

                    if (Widgets.ButtonText(devMilitaryAidNowButtonRect, devMilitaryAidNowButtonLabel))
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        GameComponent_Allegiance.Instance.SpawnMilitaryAid();
                    }
                }

                listingStandard.Label("MousekinRace_AllegianceSys_MilitaryAid_ArrivesIn".Translate(ticksUntilMilitaryAid.ToStringTicksToPeriod()));
            }

            listingStandard.GapLine();
            listingStandard.Gap();

            bool hostilesOnMap = GameComponent_Allegiance.Instance.townSquares.Where(b => AllegianceSys_Utils.MapHasHostiles(b.Map)).FirstOrDefault() != null;
            bool canCallMilitaryAidNow = !(GameComponent_Allegiance.Instance.militaryAidArrivalTick > 0) && hostilesOnMap;

            Color orgColor = GUI.color;
            if (!canCallMilitaryAidNow)
            {
                GUI.color = Color.gray;
            }

            string callMilitaryAidButtonLabel = "MousekinRace_AllegianceSys_CallMilitaryAid".Translate();
            float buttonHeight = 45f;
            float buttonWidth = Text.CalcSize(callMilitaryAidButtonLabel).x + StandardMargin;
            Rect callMilitaryAidButton = new Rect(0, listingStandard.curY, buttonWidth, buttonHeight);
            if (Widgets.ButtonText(callMilitaryAidButton, callMilitaryAidButtonLabel))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                if (canCallMilitaryAidNow)
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("MousekinRace_AllegianceSys_MilitaryAid_Confirmation".Translate(GameComponent_Allegiance.militaryAidDelayTicks.ToStringTicksToPeriod()), delegate
                    {
                        SoundDefOf.Click.PlayOneShotOnCamera();
                        GameComponent_Allegiance.Instance.SetMilitaryAidArrivalTick();
                        Messages.Message("MousekinRace_MessageMilitaryAidWillArrive".Translate(AllegianceSys_Utils.FactionNameWithDefiniteArticle(GameComponent_Allegiance.Instance.alignedFaction.Name), GameComponent_Allegiance.militaryAidDelayTicks.ToStringTicksToPeriod()), MessageTypeDefOf.PositiveEvent, false);
                    }));
                }
                else
                {
                    if (GameComponent_Allegiance.Instance.militaryAidArrivalTick > 0)
                    {
                        Messages.Message("MousekinRace_MessageCannotCallMilitaryAidAlreadyRequested".Translate("MousekinRace_MessageMilitaryAidWillArrive".Translate(AllegianceSys_Utils.FactionNameWithDefiniteArticle(GameComponent_Allegiance.Instance.alignedFaction.Name), GameComponent_Allegiance.militaryAidDelayTicks.ToStringTicksToPeriod())), MessageTypeDefOf.RejectInput, false);
                    }
                    else if (!hostilesOnMap)
                    {
                        Messages.Message("MousekinRace_MessageCannotCallMilitaryAidNoHostilesOnMap".Translate(), MessageTypeDefOf.RejectInput, false);
                    }
                }
            }

            GUI.color = orgColor;

            listingStandard.End();
        }
        
        public void DrawPageLog(Rect r)
        {
            Rect scrollableAreaRect = new Rect(r);

            float scrollableContentsWidth = scrollableAreaRect.width - scrollbarWidth;
            Rect scrollableContentsRect = new Rect(0f, 0f, scrollableContentsWidth, overviewListHeight);

            Widgets.BeginScrollView(scrollableAreaRect, ref scrollPosition, scrollableContentsRect);
            float curY = 0f;

            int rowNum = 0;
            foreach (AllegianceEventLogEntry logEntry in GameComponent_Allegiance.Instance.allegianceEventLog.AsEnumerable().Reverse())
            {
                DrawLogEntryRow(ref curY, scrollableContentsWidth, logEntry, rowNum % 2 == 0);
                rowNum++;
            }

            overviewListHeight = curY;

            Widgets.EndScrollView();
        }

        public void DrawOverviewStatListing(ref float curY, float width, TaggedString label, TaggedString content) 
        {
            float contentColWidth = width * 0.66f;
            float rowIndent = 8f;
            Rect entryLabelRect = new Rect(rowIndent, curY, width - rowIndent - contentColWidth, Text.CalcHeight(content, contentColWidth));
            Widgets.Label(entryLabelRect, label + ":");
            Rect entryContentRect = new Rect(entryLabelRect.xMax, curY, contentColWidth, entryLabelRect.height);
            Widgets.Label(entryContentRect, content);
            curY += entryLabelRect.height;
        }

        public void DrawLogEntryRow(ref float curY, float width, AllegianceEventLogEntry entry, bool altStyleRow = false)
        {
            Vector2 location = ((Find.CurrentMap != null) ? Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile) : default);
            string entryTypeTransKey = "MousekinRace_AllegianceSys_Log_" + entry.logType.ToString();
            TaggedString content = entryTypeTransKey.Translate(!entry.data.NullOrEmpty() ? entry.data.ToString() : null);

            float rowInnerRectWidth = width - uiElementSpacing * 2;
            float contentColWidth = rowInnerRectWidth * 0.66f;
            float contentColHeight = Text.CalcHeight(content, contentColWidth);

            Rect rowOuterRect = new Rect(0, curY, width, contentColHeight + uiElementSpacing * 2);
            if (altStyleRow)
            {
                Widgets.DrawBoxSolid(rowOuterRect, optionBg);
            }

            Rect rowInnerRect = rowOuterRect.ContractedBy(uiElementSpacing);

            Rect entryDateRect = new Rect(rowInnerRect.xMin, rowInnerRect.yMin, rowInnerRectWidth - contentColWidth, contentColHeight);
            Widgets.Label(entryDateRect, GenDate.DateFullStringWithHourAt(entry.arrivalTick, location));
            Rect entryContentRect = new Rect(entryDateRect.xMax, entryDateRect.yMin, contentColWidth, entryDateRect.height);
            Widgets.Label(entryContentRect, content);
            curY += rowOuterRect.height;
        }

        public enum PageTag 
        {
            AllegianceSys_Overview,
            AllegianceSys_Benefits,
            AllegianceSys_Costs,
            AllegianceSys_Recruit,
            AllegianceSys_CallTrader,
            AllegianceSys_CallMilitaryAid,
            AllegianceSys_Log,
            AllegianceSys_Change // not yet implemented
        }

        public class MenuOption
        {
            public PageTag pageTag;
            public string buttonLabel;

            public MenuOption(PageTag pageTagInput, string buttonLabelInput)
            {
                pageTag = pageTagInput;
                buttonLabel = buttonLabelInput;
            }
        }
    }
}
