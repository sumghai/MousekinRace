using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class BookNameDescByGenreExtension : DefModExtension
    {
        public List<BookNameDescPair> nameDescPairs;
    }

    public class BookNameDescPair
    {
        public RulePackDef nameMaker;

        public RulePackDef descriptionMaker;

        public float weight = 1f;
    }
}
