using RimWorld;
using System;
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
            

            // Send a letter notifying player they have joined the chosen faction, and describing any consequences
            Find.LetterStack.ReceiveLetter("MousekinRace_Letter_AllegianceSysJoinedFaction".Translate(allegianceFaction.Name), "MousekinRace_Letter_AllegianceSysJoinedFactionDesc".Translate(MembershipToFactionLabel(allegianceFaction, true)), LetterDefOf.PositiveEvent);
        }
    }
}
