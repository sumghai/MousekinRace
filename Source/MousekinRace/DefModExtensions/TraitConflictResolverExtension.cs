using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class TraitConflictResolverExtension : DefModExtension
    {
        public Dictionary<int, List<TraitDef>> conflictingTraitsByFaithDegree = new Dictionary<int, List<TraitDef>>();
    }
}
