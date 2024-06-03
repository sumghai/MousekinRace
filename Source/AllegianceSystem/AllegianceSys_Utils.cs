﻿using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MousekinRace
{
    public static class AllegianceSys_Utils
    {
        public static TaggedString FactionNameWithDefiniteArticle(TaggedString name)
        {
            return name.RawText.IndexOf("DefiniteArticle".Translate(), StringComparison.OrdinalIgnoreCase) >= 0 ? name : "DefiniteArticle".Translate() + " " + name;
        }
        
        public static TaggedString MembershipToFactionLabel(Faction faction, bool coloredFactionName = false)
        {
            TaggedString factionNameRendered = coloredFactionName ? faction.Name.Colorize(faction.Color) : faction.Name;
            AlliableFactionExtension factionExtension = faction.def.GetModExtension<AlliableFactionExtension>();

            return "MousekinRace_AllegianceSys_SubtitleFactionRelationship".Translate(factionExtension.membershipTypeLabel, FactionNameWithDefiniteArticle(factionNameRendered));
        }

        public static void JoinFaction(Faction allegianceFaction)
        {
            GameComponent_Allegiance.Instance.alignedFaction = allegianceFaction;
            AlliableFactionExtension allegianceFactionExtension = allegianceFaction.def.GetModExtension<AlliableFactionExtension>();
            List<Pawn> colonists = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists;

            // Set chosen faction as ally
            Faction.OfPlayer.SetRelationDirect(allegianceFaction, FactionRelationKind.Ally, false);

            // Set enemies of chosen faction to be hostile to player
            List<FactionDef> hostileFactionDefs = allegianceFactionExtension.hostileToFactionTypes;
            foreach (Faction faction in Find.FactionManager.AllFactionsVisible)
            {
                if (hostileFactionDefs.Contains(faction.def))
                {
                    Faction.OfPlayer.SetRelationDirect(faction, FactionRelationKind.Hostile, false);
                    Faction.OfPlayer.RelationWith(faction).baseGoodwill = -100;
                    faction.RelationWith(Faction.OfPlayer).baseGoodwill = -100;
                }
            }

            // Pawns with blacklist pawnkind types and traits should leave the player faction
            List<Tuple<Pawn, string>> quittingColonistsWithReasons = GetQuittingColonistsWithReasons(allegianceFaction);
            List<Pawn> quittingColonists = quittingColonistsWithReasons.Select(x => x.Item1).ToList();
            foreach (Pawn quittingColonist in quittingColonists)
            {
                // Wake the dissidents pawn up (if they're sleeping)
                RestUtility.WakeUp(quittingColonist);
                // Use a custom mental break to make the dissident pawns leave the map
                quittingColonist.mindState.mentalBreaker.TryDoMentalBreak(null, MousekinDefOf.Mousekin_MentalBreak_ExitAfterAllegianceChange);
            }

            // Ideology DLC: Convert all (remaining) player colonists to the chosen faction's ideo
            if (ModsConfig.IdeologyActive) 
            {
                foreach (Pawn colonist in colonists.Where(p => !quittingColonists.Contains(p)).ToList())
                {
                    colonist.ideo.SetIdeo(allegianceFaction.ideos.primaryIdeo);
                }
            }

            // Send a custom letter notifying player they have joined the chosen faction, and describing any consequences
            Find.LetterStack.ReceiveLetter("MousekinRace_Letter_AllegianceSysJoinedFaction".Translate(allegianceFaction.Name), GenerateJoinFactionLetterDesc(allegianceFaction, quittingColonistsWithReasons), LetterDefOf.PositiveEvent, quittingColonists.Count > 0 ? new LookTargets(quittingColonists) : null);

            // Remove any workbench bills for items disallowed by faction allegiance/restriction
            AllegianceSys_Utils.ResetFactionRestrictedCraftingBills();

            // Schedule the first random trade caravan from the chosen faction
            GameComponent_Allegiance.Instance.SetNextRandTraderTick();
        }

        public static bool IsEnemyBecauseOfAllegiance(Faction a, Faction b)
        {
            if (GameComponent_Allegiance.Instance.alignedFaction is Faction faction && faction.def.GetModExtension<AlliableFactionExtension>() is AlliableFactionExtension alignedFactionExtension)
            {
                return (a.IsPlayer && alignedFactionExtension.hostileToFactionTypes.Contains(b.def)) || (alignedFactionExtension.hostileToFactionTypes.Contains(a.def) && b.IsPlayer);
            }
            return false;
        }

        public static void SetFactionToOpposingAllegiance(Pawn pawn)
        { 
            pawn.SetFaction(GameComponent_Allegiance.Instance.alliableFactions.FirstOrDefault(f => GameComponent_Allegiance.Instance.alignedFaction.def.GetModExtension<AlliableFactionExtension>().hostileToFactionTypes.Contains(f.def)));
        }

        public static List<Tuple<Pawn, string>> GetQuittingColonistsWithReasons(Faction allegianceFaction)
        {
            AlliableFactionExtension alliableFactionExtension = allegianceFaction.def.GetModExtension<AlliableFactionExtension>();
            List<PawnKindDef> quittingPawnKinds = alliableFactionExtension.quittingPawnKinds;
            List<BackstoryTrait> quittingPawnWithTraits = alliableFactionExtension.quittingPawnsWithTraits;
            List<Pawn> tempAllColonists = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists;
            List<Tuple<Pawn, string>> quittingColonists = new();
            List<Tuple<Pawn, string>> quittingColonistsByPawnKind = new();
            List<Tuple<Pawn, string>> quittingColonistsByTraits = new();

            foreach (Pawn curColonist in tempAllColonists)
            {
                // By pawnkinds
                if (!quittingPawnKinds.NullOrEmpty() && quittingPawnKinds.Contains(curColonist.kindDef))
                {
                    quittingColonistsByPawnKind.Add(new Tuple<Pawn, string>(curColonist, curColonist.kindDef.LabelCap));
                }
                // By traits (with degrees)
                else if (!quittingPawnWithTraits.NullOrEmpty())
                {
                    foreach (BackstoryTrait quittingTrait in quittingPawnWithTraits)
                    {
                        if (curColonist.story.traits.HasTrait(quittingTrait.def, quittingTrait.degree))
                        {
                            quittingColonistsByTraits.Add(new Tuple<Pawn, string>(curColonist, "MousekinRace_AllegianceSys_ViewExtraInfoDialog_Trait".Translate(quittingTrait.def.degreeDatas.Find(tdd => tdd.degree == quittingTrait.degree).LabelCap)));
                            break;
                        }
                    }
                }
            }
            quittingColonistsByPawnKind.OrderByDescending(t => t.Item2).ToList();
            quittingColonistsByTraits.OrderByDescending(t => t.Item2).ToList();

            quittingColonists.AddRange(quittingColonistsByPawnKind);
            quittingColonists.AddRange(quittingColonistsByTraits);

            return quittingColonists;
        }

        public static TaggedString GenerateQuittingColonistsWithReasonsDesc(string preambleTranslationKey, List<Tuple<Pawn, string>> quittingColonistsWithReasons)
        {
            TaggedString output = new();
            if (quittingColonistsWithReasons.Count > 0)
            {
                TaggedString quittingColonistsListString = new();
                Tuple<Pawn, string> lastQuittingColonist = quittingColonistsWithReasons.Last();
                foreach (Tuple<Pawn, string> quittingColonist in quittingColonistsWithReasons)
                {
                    quittingColonistsListString += "  - " + quittingColonist.Item1.Name.ToStringShort + (", " + quittingColonist.Item1.story.TitleShortCap).Colorize(ColoredText.SubtleGrayColor) + " (" + quittingColonist.Item2 + ")";
                    
                    if (!quittingColonist.Equals(lastQuittingColonist))
                    {
                        quittingColonistsListString += "\n";
                    }
                }
                output = preambleTranslationKey.Translate(quittingColonistsListString);
            }
            return output;
        }

        public static TaggedString GenerateShatteredEmpireQuestsDisabledDesc()
        {
            TaggedString output = new();
            if (ModsConfig.RoyaltyActive && Faction.OfEmpire != null)
            {
                TaggedString empireNameColored = Faction.OfEmpire.Name.Colorize(Faction.OfEmpire.Color);
                TaggedString empireNameWithArticle = empireNameColored.RawText.IndexOf("DefiniteArticle".Translate(), StringComparison.OrdinalIgnoreCase) >= 0 ? empireNameColored : "DefiniteArticle".Translate().CapitalizeFirst() + " " + empireNameColored;
                output = "MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartEmpireQuestsDisabled".Translate(empireNameWithArticle) + "\n\n";
            }
            return output;
        }

        public static TaggedString GenerateBenefitsDesc(Faction allegianceFaction)
        {
            TaggedString descBody = new();

            AlliableFactionExtension alliableFactionExtension = allegianceFaction.def.GetModExtension<AlliableFactionExtension>();

            if (alliableFactionExtension.tradePriceFactor != 1f)
            {
                descBody += "- " + "MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartTradePriceFactor".Translate(alliableFactionExtension.tradePriceFactor.ToStringPercent()) + "\n\n";
            }

            if (alliableFactionExtension.recruitableColonistSettings.options.Count > 0)
            {
                string recruitableColonists = "";
                foreach (RecruitableOptions option in alliableFactionExtension.recruitableColonistSettings.options)
                {
                    recruitableColonists += "  - " + option.pawnKind.label.Replace(MousekinDefOf.Mousekin.label, "").Trim().CapitalizeFirst() + "\n";
                }
                descBody += "- " + "MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartRecruitableColonists".Translate(recruitableColonists) + "\n";
            }

            if (alliableFactionExtension.factionRestrictedCraftableThingDefs != null && alliableFactionExtension.factionRestrictedCraftableThingDefs.Count > 0) 
            {
                descBody += "- " + "MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartFactionItemsUnlocked".Translate();
            }

            return descBody;
        }

        public static TaggedString GenerateCostsDesc(Faction allegianceFaction)
        {
            TaggedString descBody = new();
            
            if (ModsConfig.IdeologyActive)
            {
                descBody += "- " + "MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartIdeoChange".Translate(allegianceFaction.ideos.PrimaryIdeo.ToString().Colorize(allegianceFaction.ideos.PrimaryIdeo.Color)) + "\n\n";
            }

            if (ModsConfig.RoyaltyActive && Faction.OfEmpire != null)
            {
                descBody += "- " + GenerateShatteredEmpireQuestsDisabledDesc();
            }

            List<Tuple<Pawn, string>> quittingColonists = GetQuittingColonistsWithReasons(allegianceFaction);
            if (quittingColonists.Count > 0)
            {
                descBody += "- " + GenerateQuittingColonistsWithReasonsDesc("MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartQuittingPawns", quittingColonists);
            }    

            return descBody;
        }

        public static TaggedString GenerateJoinFactionLetterDesc(Faction allegianceFaction, List<Tuple<Pawn, string>> quittingColonistsWithReasons)
        {
            TaggedString letterBody = new();
            letterBody += "MousekinRace_Letter_AllegianceSysJoinedFactionDesc".Translate(MembershipToFactionLabel(allegianceFaction, true)) + "\n\n";
            
            if (ModsConfig.IdeologyActive) 
            {
                letterBody += "MousekinRace_Letter_AllegianceSysJoinedFactionDesc_PartIdeoChanged".Translate(allegianceFaction.ideos.PrimaryIdeo.ToString().Colorize(allegianceFaction.ideos.PrimaryIdeo.Color)) + "\n\n";
            }

            letterBody += GenerateQuittingColonistsWithReasonsDesc("MousekinRace_Letter_AllegianceSysJoinedFactionDesc_PartQuittingColonists", quittingColonistsWithReasons);

            return letterBody.Trim();
        }

        public static bool AnyColonistsWithShatteredEmpireTitle()
        {          
            List<Pawn> tempAllColonists = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists;
            foreach (Pawn pawn in tempAllColonists)
            {
                if (pawn.royalty.HasAnyTitleIn(Faction.OfEmpire))
                { 
                    return true;
                }
            }
            return false;
        }

        public static void ResetFactionRestrictedCraftingBills()
        {
            List <Building_WorkTable> colonistWorktablesWithFactionRestrictedItemBills = new();
            
            foreach (Map map in Find.Maps)
            {
                colonistWorktablesWithFactionRestrictedItemBills.AddRange(map.listerBuildings.AllColonistBuildingsOfType<Building_WorkTable>().Where(worktable => worktable.BillStack.Bills.Where(bill => MousekinDefOf.FactionRestrictedThingDefs.Contains(bill.recipe.ProducedThingDef)).Count() > 0));
            }
            
            if (GameComponent_Allegiance.Instance.alignedFaction is Faction allegianceFaction)
            {
                List<ThingDef> thingDefsToRemove = MousekinDefOf.FactionRestrictedThingDefs.Except(allegianceFaction.def.GetModExtension<AlliableFactionExtension>().factionRestrictedCraftableThingDefs).ToList();

                foreach (Building_WorkTable worktable in colonistWorktablesWithFactionRestrictedItemBills)
                {
                    worktable.BillStack.bills.RemoveAll(bill => thingDefsToRemove.Contains(bill.recipe.ProducedThingDef));
                }
            }
            else
            {
                foreach (Building_WorkTable worktable in colonistWorktablesWithFactionRestrictedItemBills) 
                {
                    worktable.BillStack.bills.RemoveAll(bill => MousekinDefOf.FactionRestrictedThingDefs.Contains(bill.recipe.ProducedThingDef));
                }
            }
        }
        
        public static void GenerateNewColonistsToQueue(float silverToPay, PawnKindDef pawnKind, int reqCount = 1, bool makeFamily = false, PawnKindDef spousePawnKind = null)
        {
            Faction alignedFaction = GameComponent_Allegiance.Instance.alignedFaction;
            List<Pawn> pawnsToRecruit = new();  // Placeholder list for new colonists

            if (makeFamily)
            {
                // Calculate the wife's potential age based on potential children
                int minChildren = 2;
                int maxChildren = 3;
                int childrenToGenerate = Rand.Range(minChildren, maxChildren + 1); // Rand.Range for int is usually max-exclusive
                float minAgeGapBetweenChildren = 1.5f;
                float maxAgeGapBetweenChildren = 3.5f;
                float youngestChildAge = MousekinDefOf.Mousekin.race.lifeStageAges.FirstOrDefault(stage => stage.def.Equals(LifeStageDefOf.HumanlikeChild)).minAge; // Minimum age capable of locomotion
                List<float> childrenAges = new();
                for (int i = 0; i < childrenToGenerate; i++)
                {
                    childrenAges.Add(i == 0 ? youngestChildAge + Rand.Range(0f, 2f) : Rand.Range(minAgeGapBetweenChildren, maxAgeGapBetweenChildren) + childrenAges[i - 1]);
                }
                float oldestChildAge = childrenAges.Last();
                float minAgeOfFirstTimeMother = 20f;
                float maxAgeOfFirstTimeMother = 25f;
                float ageOfMother = Rand.Range(minAgeOfFirstTimeMother, maxAgeOfFirstTimeMother) + oldestChildAge;

                // Generate the required primary pawnkind and their spouse, ensuring that:
                // - the wife is younger than the husband
                // - both spouses are of reasonable parental age
                //
                // (if the Biotech DLC is active, we generate a temporary age that we modify later,
                // so that we don't get grey-haired parents of young children)
                float minAgeDiffBetweenSpouses = 0f;
                float maxAgeDiffBetweenSpouses = 4f;
                float ageDiffBetweenSpouses = Rand.Range(minAgeDiffBetweenSpouses, maxAgeDiffBetweenSpouses);
                float ageOfFather = ageOfMother + ageDiffBetweenSpouses;
                float tmpStartingAge = 25f; // Minimum age for most starting colonist bios, ensures we get good selection of bio choices
                PawnGenerationRequest primaryPawnGenRequest = new PawnGenerationRequest(
                    pawnKind,
                    faction: Faction.OfPlayer,
                    canGeneratePawnRelations: false,
                    colonistRelationChanceFactor: 0f,
                    allowGay: false,
                    allowPregnant: false,
                    allowAddictions: false,
                    relationWithExtraPawnChanceFactor: 0f,
                    fixedBiologicalAge: ModsConfig.BiotechActive ? tmpStartingAge : null,
                    fixedChronologicalAge: ModsConfig.BiotechActive ? tmpStartingAge : null
                );
                Pawn primaryPawn = PawnGenerator.GeneratePawn(primaryPawnGenRequest);
                PawnGenerationRequest spouseGenRequest = new PawnGenerationRequest(
                    spousePawnKind,
                    faction: Faction.OfPlayer,
                    canGeneratePawnRelations: false,
                    colonistRelationChanceFactor: 0f,
                    allowGay: false,
                    allowPregnant: false,
                    allowAddictions: false,
                    relationWithExtraPawnChanceFactor: 0f,
                    fixedBiologicalAge: ModsConfig.BiotechActive ? tmpStartingAge : primaryPawn.ageTracker.AgeBiologicalYearsFloat + (primaryPawn.gender == Gender.Male ? -1 : 1) * ageDiffBetweenSpouses,
                    fixedChronologicalAge: ModsConfig.BiotechActive ? tmpStartingAge : primaryPawn.ageTracker.AgeChronologicalYearsFloat + (primaryPawn.gender == Gender.Male ? -1 : 1) * ageDiffBetweenSpouses,
                    fixedGender: primaryPawn.gender.Opposite()
                );
                Pawn spousePawn = PawnGenerator.GeneratePawn(spouseGenRequest);
                spousePawn.relations.AddDirectRelation(PawnRelationDefOf.Spouse, primaryPawn);
                SpouseRelationUtility.DetermineManAndWomanSpouses(primaryPawn, spousePawn, out Pawn maleSpousePawn, out Pawn femaleSpousePawn);
                if (ModsConfig.BiotechActive)
                {
                    maleSpousePawn.ageTracker.AgeBiologicalTicks = (long) (ageOfFather * 3600000);
                    maleSpousePawn.ageTracker.AgeChronologicalTicks = (long) (ageOfFather * 3600000);
                    femaleSpousePawn.ageTracker.AgeBiologicalTicks = (long)(ageOfMother * 3600000);
                    femaleSpousePawn.ageTracker.AgeChronologicalTicks = (long)(ageOfMother * 3600000);
                }
                SpouseRelationUtility.ChangeNameAfterMarriage(maleSpousePawn, femaleSpousePawn, MarriageNameChange.MansName);

                // Biotech DLC: Generate 2~3 children
                List<Pawn> childPawns = new(); // Placeholder list for any children generated

                if (ModsConfig.BiotechActive)
                {                  
                    string childLastname = (maleSpousePawn.Name as NameTriple).Last;

                    for (int i = 0; i < childrenToGenerate; i++) 
                    {
                        PawnGenerationRequest currentChildGenRequest = new PawnGenerationRequest(
                            MousekinDefOf.MousekinChild,
                            faction: Faction.OfPlayer,
                            canGeneratePawnRelations: false,
                            colonistRelationChanceFactor: 0f,
                            allowGay: false,
                            allowPregnant: false,
                            allowAddictions: false,
                            relationWithExtraPawnChanceFactor: 0f,
                            fixedBiologicalAge: childrenAges[i], 
                            fixedChronologicalAge: childrenAges[i],
                            fixedBirthName: childLastname,
                            developmentalStages: DevelopmentalStage.Child
                        );
                        Pawn currentChild = PawnGenerator.GeneratePawn(currentChildGenRequest);

                        currentChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, maleSpousePawn);
                        currentChild.relations.AddDirectRelation(PawnRelationDefOf.Parent, femaleSpousePawn);
                        ((NameTriple)currentChild.Name).lastInt = childLastname;
                        childPawns.Add(currentChild);
                    }
                }

                pawnsToRecruit.Add(maleSpousePawn);
                pawnsToRecruit.Add(femaleSpousePawn);
                pawnsToRecruit.AddRange(childPawns);
            }
            else 
            {
                // Get a list of living world pawns from the player-chosen allegiance faction (excluding the faction leader),
                // with (any) relatives of existing colonists taking priority
                List<Pawn> alignedFactionWorldPawns = Find.WorldPawns.AllPawnsAlive.Where(p => p.Faction == alignedFaction && p != alignedFaction.leader).OrderByDescending(p => PawnRelationUtility.GetMostImportantColonyRelative(p) != null).ToList();

                // Downselect to world pawns that match the desired pawnKind
                List<Pawn> candidateWorldPawns = alignedFactionWorldPawns.Where(p => p.kindDef == pawnKind).ToList();

                // Try to grab as many candidate pawns as requested
                pawnsToRecruit.AddRange(candidateWorldPawns.Take(reqCount));

                // Generate additional pawns of pawnKind if required
                if (pawnsToRecruit.Count < reqCount)
                {
                    int additionalPawnsToGenerate = reqCount - pawnsToRecruit.Count;

                    for (int i = 0; i < additionalPawnsToGenerate; i++)
                    {
                        Pawn additionalPawn = PawnGenerator.GeneratePawn(pawnKind, Faction.OfPlayer);
                        pawnsToRecruit.Add(additionalPawn);
                    }
                }
            }

            pawnsToRecruit.SortByDescending(p => p.ageTracker.AgeBiologicalTicks);

            foreach (Pawn pawn in pawnsToRecruit)
            {
                // Save the new colonists as world pawns (if they aren't already)
                if (!Find.WorldPawns.Contains(pawn))
                {
                    // Note: PassToWorld() tends to reassign pawns to a random faction, so we need to
                    // explicitly change their faction back to the player in a subsequent operation
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
                }

                // Set the new colonists' faction and ideology to that of the player
                pawn.SetFaction(Faction.OfPlayer);
                if (ModsConfig.IdeologyActive)
                {
                    pawn.ideo.SetIdeo(alignedFaction.ideos.primaryIdeo);
                }
            }

            // Add the new colonist(s) to the queue
            GameComponent_Allegiance.Instance.AddNewColonistsToQueue(pawnsToRecruit, Find.TickManager.TicksGame + GameComponent_Allegiance.requestArrivalDelayTicks);

            // Pay for the new colonist(s)
            int remainingCost = Mathf.RoundToInt(silverToPay);
            List<Thing> silverList = Find.Maps.Where(m => m.IsPlayerHome).SelectMany(m => m.listerThings.ThingsOfDef(ThingDefOf.Silver)
                                    .Where(x => !x.Position.Fogged(x.Map) && (m.areaManager.Home[x.Position] || x.IsInAnyStorage()))).ToList();
            while (remainingCost > 0)
            {
                Thing silver = silverList.First(t => t.stackCount > 0);
                int num = Mathf.Min(remainingCost, silver.stackCount);
                silver.SplitOff(num).Destroy();
                remainingCost -= num;
            }

            GameComponent_Allegiance.Instance.RecacheAvailableSilver();

            // Notify the player
            Messages.Message("MousekinRace_MessageNewColonistsWillArrive".Translate(pawnsToRecruit.Count(), "PeriodDays".Translate(GameComponent_Allegiance.requestArrivalDelayDays)), MessageTypeDefOf.PositiveEvent, false);
        }

        public static void SpawnNewColonists(List<Pawn> newColonistPawns)
        {
            // Find the map belonging to the first town square and set a random spawn location near the edge of the map,
            // while ensuring the player settlement is still reachable
            // todo - handle multiple player maps (currently defaults to first player colony map with town square)
            Building_TownSquare building_TownSquare = GameComponent_Allegiance.Instance.townSquares.First();
            Map map = building_TownSquare.Map;
            CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out IntVec3 recruiteeEntryCell);

            TaggedString newRecruits = new TaggedString();
            // Spawn the recruitees
            foreach (Pawn pawn in newColonistPawns) 
            {
                newRecruits += "\n- "+ pawn.NameFullColored + ", " + pawn.story.TitleShort;
                GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(recruiteeEntryCell, building_TownSquare.Map, 3), building_TownSquare.Map);
            }

            // Notify the player of the new colonists
            Find.LetterStack.ReceiveLetter("MousekinRace_Letter_AllegianceSysNewColonists".Translate(), "MousekinRace_Letter_AllegianceSysNewColonistsDesc".Translate(newRecruits), LetterDefOf.PositiveEvent, new LookTargets(newColonistPawns));
        }

        public static void SpawnTradeCaravanFromAllegianceFaction(TraderKindDef specificTraderKind = null)
        {
            IncidentDef incidentDef = IncidentDefOf.TraderCaravanArrival;
            // todo - handle multiple player maps (currently defaults to first player colony map with town square)
            Map targetMap = GameComponent_Allegiance.Instance.townSquares.FirstOrDefault().Map;
            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, targetMap);
            incidentParms.faction = GameComponent_Allegiance.Instance.alignedFaction;
            incidentParms.traderKind = specificTraderKind ?? GameComponent_Allegiance.Instance.alignedFaction.def.caravanTraderKinds.RandomElement();
            if (incidentDef.Worker.CanFireNowSub(incidentParms))
            {
                incidentParms.forced = true;
                incidentDef.Worker.TryExecuteWorker(incidentParms);
            }
        }

        public static bool MapHasHostiles(Map map)
        {
            foreach (IAttackTarget item in map.attackTargetsCache.TargetsHostileToColony)
            {
                if (GenHostility.IsActiveThreatToPlayer(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static void SpawnMilitaryAidFromAllegianceFaction()
        { 
            IncidentDef incidentDef = IncidentDefOf.RaidFriendly;
            // todo - handle multiple player maps (currently defaults to first player colony map with hostile pawns)
            Map targetMap = Find.Maps.Where(m => m.IsPlayerHome && MapHasHostiles(m)).FirstOrDefault();
            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, targetMap);
            incidentParms.faction = GameComponent_Allegiance.Instance.alignedFaction;
            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            incidentParms.forced = true;
            incidentDef.Worker.TryExecuteWorker(incidentParms);
        }
    }
}
