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

        public const float uiElementSpacing = 10f;

        public const float scrollbarWidth = 16f;

        public const int optionNumColumns = 2;

        public const float optionRowHeight = 100f;

        public Color optionBg = new Color(0.13f, 0.13f, 0.13f);

        public Color optionBgMouseover = new Color(1f, 1f, 1f, 0.3f);

        public Vector2 scrollPosition = Vector2.zero;

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

        public override void PostClose()
        {
            base.PostClose();
            selectedPageTag = "AllegianceSys_Overview";
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

            DrawPageBlock(new Rect(rect.xMin + optionButtonWidth + StandardMargin, y, rect.width - (rect.xMin + optionButtonWidth + StandardMargin), rect.height - y), selectedPageTag);
        }

        public void DrawMenuOptionButton(Rect r, MenuOption option)
        {
            Widgets.DrawOptionBackground(r, option.pageTag == selectedPageTag);
            if (Widgets.ButtonInvisible(r))
            {
                selectedPageTag = option.pageTag;
                SoundDefOf.Click.PlayOneShotOnCamera();
            }
            Widgets.Label(r, option.buttonLabel);
        }

        public void DrawPageBlock(Rect r, string pageTag)
        {
            Widgets.DrawMenuSection(r);
            Rect innerRect = r.ContractedBy(17f);

            switch (pageTag)
            {
                case "AllegianceSys_Benefits":
                    DrawPageBenefits(innerRect); 
                    break;
                case "AllegianceSys_Costs":
                    DrawPageCosts(innerRect);
                    break;
                case "AllegianceSys_Recruit":
                    DrawPageRecruit(innerRect);
                    break;
                case "AllegianceSys_CallTrader":
                    DrawPageTraders(innerRect);
                    break;
                default:
                    Widgets.Label(innerRect, pageTag);
                    break;
            }            
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
            Rect silverAvailableReadoutRect = new Rect(0, 0, r.width / 4, Text.CalcHeight(silverAvailableValue, r.width / 4) + StandardMargin / 2);
            Widgets.DrawBoxSolid(silverAvailableReadoutRect, optionBg);
            GUI.color = optionBgMouseover;
            Widgets.DrawHighlightIfMouseover(silverAvailableReadoutRect);
            GUI.color = Color.white;
            Rect silverIconRect = new Rect(silverAvailableReadoutRect.xMin + StandardMargin / 2, silverAvailableReadoutRect.yMin, silverAvailableReadoutRect.height, silverAvailableReadoutRect.height);
            Rect silverTextRect = new Rect(silverIconRect.xMin + silverIconRect.width + StandardMargin / 2, silverAvailableReadoutRect.yMin, silverAvailableReadoutRect.width - silverIconRect.width - StandardMargin / 2, silverAvailableReadoutRect.height);
            Widgets.ThingIcon(silverIconRect, ThingDefOf.Silver);
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(silverTextRect, silverAvailableValue);
            Text.Anchor = anchor;
            TooltipHandler.TipRegion(silverAvailableReadoutRect, ThingDefOf.Silver.LabelCap + ": " + ThingDefOf.Silver.description.CapitalizeFirst());

            listingStandard.curY = silverAvailableReadoutRect.yMax;
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

                anchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleCenter;
                float inviteButtonWidth = (currentOptionControlsRect.width - uiElementSpacing) / 2;
                float inviteButtonHeight = Text.CalcHeight("MousekinRace_AllegianceSys_Recruit_InviteSingleButtonLabel".Translate(), inviteButtonWidth) + uiElementSpacing;

                PawnKindDef recruitablePawnKind = recruitableOptions[i].pawnKind;
                PawnKindDef recruitableSpousePawnKind = recruitableOptions[i].overrideSpousePawnKind ?? recruitableColonistSettings.defaultSpousePawnKind;
                bool recruitablePawnCanHaveFamily = recruitableOptions[i].canHaveFamily;
                int recruitablePawnCount = recruitableOptions[i].count;
                int inviteCost = recruitableOptions[i].basePrice;

                Color orgColor = GUI.color;
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
                        if (inviteCostFamily <= availableSilver)
                        {
                            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("MousekinRace_AllegianceSys_Recruit_Confirmation".Translate("IndefiniteForm".Translate(optionName), "MousekinRace_AllegianceSys_Recruit_ConfirmationFamily".Translate(), inviteCostFamily), delegate
                            {
                                AllegianceSys_Utils.GenerateAndSpawnNewColonists(inviteCostFamily, recruitablePawnKind, 1, recruitablePawnCanHaveFamily, recruitableSpousePawnKind);
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
                    if (inviteCost <= availableSilver)
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("MousekinRace_AllegianceSys_Recruit_Confirmation".Translate(recruitablePawnCount > 1 ? recruitablePawnCount + "x " + recruitablePawnKind.labelPlural.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst() : "IndefiniteForm".Translate(optionName), "", inviteCost), delegate
                        {
                            AllegianceSys_Utils.GenerateAndSpawnNewColonists(inviteCost, recruitablePawnKind, recruitablePawnCount);
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
        }

        public void DrawPageTraders(Rect r)
        {
            List<TraderKindDef> traderOptions = GameComponent_Allegiance.Instance.alignedFaction.def.caravanTraderKinds;

            var listingStandard = new Listing_Standard();
            listingStandard.Begin(r);

            listingStandard.Label("MousekinRace_AllegianceSys_Trader_NextRandomCaravanArrivesIn".Translate(GameComponent_Allegiance.Instance.ticksUntilNextRandomCaravanTrader.ToStringTicksToPeriod()));

            if (GameComponent_Allegiance.Instance.ticksUntilNextRequestedCaravanTraderAllowed > 0)
            {
                if (!GameComponent_Allegiance.Instance.requestedCaravanTradersQueue.NullOrEmpty())
                {
                    RequestedTrader nextRequestedTrader = GameComponent_Allegiance.Instance.requestedCaravanTradersQueue[0];
                    int ticksRemainingForRequestedTradeCaravan = nextRequestedTrader.spawnTick - Find.TickManager.TicksAbs;
                    listingStandard.Label("MousekinRace_AllegianceSys_Trader_NextRequestedCaravanArrivesIn".Translate(ticksRemainingForRequestedTradeCaravan.ToStringTicksToPeriod(), nextRequestedTrader.traderKind.LabelCap));
                }
                else
                {
                    int cooldownTicksRemainingForRequestingTradeCaravans = GameComponent_Allegiance.Instance.ticksUntilNextRequestedCaravanTraderAllowed;
                    listingStandard.Label("MousekinRace_AllegianceSys_Trader_RequestedCaravanCooldown".Translate(cooldownTicksRemainingForRequestingTradeCaravans.ToStringTicksToPeriod()));
                }
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

                if (GameComponent_Allegiance.Instance.ticksUntilNextRequestedCaravanTraderAllowed > 0)
                {
                    GUI.color = Color.gray;
                }

                if (Widgets.ButtonText(requestTraderButtonRect, requestTraderButtonLabel))
                {
                    if (!(GameComponent_Allegiance.Instance.ticksUntilNextRequestedCaravanTraderAllowed > 0))
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("MousekinRace_AllegianceSys_Trader_Confirmation".Translate(traderOptionName, "PeriodDays".Translate(AllegianceSys_Utils.requestArrivalDelayDays)), delegate
                        {
                            AllegianceSys_Utils.AddRequestedTraderToQueue(traderOption);
                            GameComponent_Allegiance.Instance.StartCooldownForRequestingCaravanTrader();
                            SoundDefOf.Click.PlayOneShotOnCamera();
                        }));
                    }
                    else 
                    {
                        Messages.Message("MousekinRace_MessageCannotRequestTraderCooldownActive".Translate(GameComponent_Allegiance.Instance.ticksUntilNextRequestedCaravanTraderAllowed.ToStringTicksToPeriod()), MessageTypeDefOf.RejectInput, false);
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
                    GameComponent_Allegiance.Instance.ticksUntilNextRandomCaravanTrader = 0;
                }

                if (GameComponent_Allegiance.Instance.ticksUntilNextRequestedCaravanTraderAllowed > 0)
                {
                    if (!GameComponent_Allegiance.Instance.requestedCaravanTradersQueue.NullOrEmpty())
                    {
                        string devRequestedCaravanTraderButtonLabel = "DEV: Spawn requested caravan trader now";
                        Vector2 devRequestedCaravanTraderButtonSize = Text.CalcSize(devRequestedCaravanTraderButtonLabel);
                        devRequestedCaravanTraderButtonSize.x += StandardMargin * 2;
                        Rect devRequestedCaravanTraderButtonRect = new Rect(r.xMax - devRequestedCaravanTraderButtonSize.x, r.yMin + devRandCaravanTraderButtonSize.y, devRequestedCaravanTraderButtonSize.x, devRequestedCaravanTraderButtonSize.y);

                        if (Widgets.ButtonText(devRequestedCaravanTraderButtonRect, devRequestedCaravanTraderButtonLabel))
                        {
                            GameComponent_Allegiance.Instance.SpawnRequestedCaravanTrader();
                        }
                    }
                    else
                    {
                        string devResetCooldownForCaravanTraderButtonLabel = "DEV: Reset cooldown now";
                        Vector2 devResetCooldownForCaravanTraderButtonSize = Text.CalcSize(devResetCooldownForCaravanTraderButtonLabel);
                        devResetCooldownForCaravanTraderButtonSize.x += StandardMargin * 2;
                        Rect devResetCooldownForCaravanTraderButtonRect = new Rect(r.xMax - devResetCooldownForCaravanTraderButtonSize.x, r.yMin + devRandCaravanTraderButtonSize.y, devResetCooldownForCaravanTraderButtonSize.x, devResetCooldownForCaravanTraderButtonSize.y);

                        if (Widgets.ButtonText(devResetCooldownForCaravanTraderButtonRect, devResetCooldownForCaravanTraderButtonLabel))
                        {
                            GameComponent_Allegiance.Instance.ResetCooldownForRequestingCaravanTrader();
                        }
                    }
                }
            }
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
