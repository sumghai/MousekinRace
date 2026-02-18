using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class CompProperties_RemembersCheeseEaten : CompProperties
    {
        public List<ThingDef> cheeseDefs = null;

        public CompProperties_RemembersCheeseEaten()
        {
            compClass = typeof(CompRemembersCheeseEaten);
        }
    }
}
