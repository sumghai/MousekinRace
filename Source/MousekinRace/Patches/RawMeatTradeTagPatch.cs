using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class RawMeatTradeTagPatch
    {
        static RawMeatTradeTagPatch()
        {
            List<ThingDef> tradeableMeats = new List<ThingDef>()
            {
                MousekinDefOf.Meat_Chicken,
                MousekinDefOf.Meat_Cow,
                MousekinDefOf.Meat_Deer,
                MousekinDefOf.Meat_Duck,
                MousekinDefOf.Meat_Goat,
                MousekinDefOf.Meat_Pig,
                MousekinDefOf.Meat_Sheep
            };

            foreach (ThingDef currentMeat in tradeableMeats)
            {
                // We first check if the meats already have trade tags patched by other mods,
                // and if not, initialize an empty list
                if (currentMeat.tradeTags == null)
                {
                    currentMeat.tradeTags = new List<string>();
                }

                // Then we add our custom trade tag
                currentMeat.tradeTags.Add("Mousekin_TradeGoods_Meat");
            }
        }
    }
}
