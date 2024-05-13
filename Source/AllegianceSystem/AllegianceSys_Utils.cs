using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public static class AllegianceSys_Utils
    {
        public static TaggedString MembershipToFactionLabel(Faction faction, bool coloredFactionName = false)
        {
            TaggedString factionNameRendered = coloredFactionName ? faction.Name.Colorize(faction.Color) : faction.Name;
            AlliableFactionExtension factionExtension = faction.def.GetModExtension<AlliableFactionExtension>();

            return "MousekinRace_AllegianceSys_SubtitleFactionRelationship".Translate(factionExtension.membershipTypeLabel, factionNameRendered.RawText.IndexOf("DefiniteArticle".Translate(), StringComparison.OrdinalIgnoreCase) >= 0 ? factionNameRendered : "DefiniteArticle".Translate() + " " + factionNameRendered);
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
            Find.LetterStack.ReceiveLetter("MousekinRace_Letter_AllegianceSysJoinedFaction".Translate(allegianceFaction.Name), GenerateJoinFactionLetterDesc(allegianceFaction, quittingColonistsWithReasons), LetterDefOf.PositiveEvent);
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
        
        public static TaggedString GenerateBenefitsDesc(Faction allegianceFaction)
        {
            TaggedString descBody = new();

            // todo - populate with content

            return descBody;
        }

        public static TaggedString GenerateCostsDesc(Faction allegianceFaction)
        {
            TaggedString descBody = new();

            if (ModsConfig.IdeologyActive)
            {
                descBody += "- " + "MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartIdeoChange".Translate(allegianceFaction.ideos.PrimaryIdeo.ToString().Colorize(allegianceFaction.ideos.PrimaryIdeo.Color)) + "\n\n";
            }

            List<Tuple<Pawn, string>> quittingColonists = GetQuittingColonistsWithReasons(allegianceFaction);
            descBody += "- " + GenerateQuittingColonistsWithReasonsDesc("MousekinRace_AllegianceSys_ViewExtraInfoDialog_PartQuittingPawns", quittingColonists);

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

            return letterBody;
        }
    }
}
