using RimWorld;
using System;
using System.Collections.Generic;
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

            // Send a custom letter notifying player they have joined the chosen faction, and describing any consequences
            Find.LetterStack.ReceiveLetter("MousekinRace_Letter_AllegianceSysJoinedFaction".Translate(allegianceFaction.Name), "MousekinRace_Letter_AllegianceSysJoinedFactionDesc".Translate(MembershipToFactionLabel(allegianceFaction, true)), LetterDefOf.PositiveEvent);
        }

        public static bool IsEnemyBecauseOfAllegiance(Faction a, Faction b)
        {
            if (GameComponent_Allegiance.Instance.alignedFaction is Faction faction && faction.def.GetModExtension<AlliableFactionExtension>() is AlliableFactionExtension alignedFactionExtension)
            {
                return (a.IsPlayer && alignedFactionExtension.hostileToFactionTypes.Contains(b.def)) || (alignedFactionExtension.hostileToFactionTypes.Contains(a.def) && b.IsPlayer);
            }
            return false;
        }
    }
}
