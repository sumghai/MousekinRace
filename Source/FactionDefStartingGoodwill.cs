using RimWorld;
using System.Xml;
using Verse;

namespace MousekinRace
{
    public class FactionDefStartingGoodwill : IExposable
    {
        public FactionDef factionDef;

        public IntRange startingGoodwill;

        public int Min => startingGoodwill.min;

        public int Max => startingGoodwill.max;

        public FactionDefStartingGoodwill()
        {
        }

        public FactionDefStartingGoodwill(FactionDef factionDef, int min, int max) 
            : this(factionDef, new IntRange(min, max))
        {
        }

        public FactionDefStartingGoodwill(FactionDef factionDef, IntRange startingGoodwill)
        {
            this.factionDef = factionDef;
            this.startingGoodwill = startingGoodwill;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref factionDef, "factionDef");
            Scribe_Values.Look(ref startingGoodwill, "startingGoodwill");
        }

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count != 1)
            {
                Log.Error("Misconfigured FactionDefStartingGoodwill: " + xmlRoot.OuterXml);
                return;
            }
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "factionDef", xmlRoot.Name);
            startingGoodwill = ParseHelper.FromString<IntRange>(xmlRoot.FirstChild.Value);
        }

        public override string ToString()
        {
            return "(" + ((factionDef != null) ? factionDef.defName : "null") + " with starting goodwill of " + startingGoodwill + ")";
        }
    }
}
