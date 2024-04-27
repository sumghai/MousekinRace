using RimWorld;
using System;
using Verse;

namespace MousekinRace
{
    public static class AllegianceSys_Utils
    {
        public static string RelationshipToJoinedFaction(string factionName, string relationshipType)
        {
            return "MousekinRace_AllegianceSys_SubtitleFactionRelationship".Translate(relationshipType, factionName.IndexOf("DefiniteArticle".Translate(), StringComparison.OrdinalIgnoreCase) >= 0 ? factionName : "DefiniteArticle".Translate() + " " + factionName);
        }

        public static void JoinFaction(Faction allegianceFaction)
        {
            GameComponent_Allegiance.Instance.alignedFaction = allegianceFaction;
        }
    }
}
