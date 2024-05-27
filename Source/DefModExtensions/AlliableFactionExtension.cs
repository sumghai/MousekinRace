using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class AlliableFactionExtension : DefModExtension
    {
        public AlliableFactionJoinRequirements joinRequirements = new();
        
        [MustTranslate]
        public string joinButtonLabel;

        [MustTranslate]
        public string membershipTypeLabel;

        public List<FactionDef> hostileToFactionTypes = new();

        public GraphicData flagGraphicData = null;

        public List<PawnKindDef> quittingPawnKinds = new();

        public List<BackstoryTrait> quittingPawnsWithTraits = new();

        public float tradePriceFactor = 1f;

        public List<ThingDef> factionRestrictedCraftableThingDefs = new();

        public RecruitableColonistSettings recruitableColonistSettings = new();

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (var err in base.ConfigErrors())
            {
                yield return err;
            }

            if (joinButtonLabel.NullOrEmpty())
            {
                yield return $"AlliableFactionExtension - No join button text defined!";
            }

            if (membershipTypeLabel.NullOrEmpty())
            {
                yield return $"AlliableFactionExtension - No membership type text defined!";
            }

            if (!recruitableColonistSettings.options.NullOrEmpty()) 
            {
                if (recruitableColonistSettings.defaultSpousePawnKind == null)
                {
                    yield return $"AlliableFactionExtension - No defaultSpousePawnKind defined!";
                }
                
                for (int i = 0; i < recruitableColonistSettings.options.Count; i++) 
                {
                    if (recruitableColonistSettings.options[i].pawnKind == null)
                    {
                        yield return $"AlliableFactionExtension - item " + i + " in recruitableColonistSettings options has no pawnKind defined!";
                    }
                }
            }
        }
    }

    public class AlliableFactionJoinRequirements 
    {
        public int minDaysPassedSinceSettle = 60;
        public float minMousekinPopulationPercentage = 0.75f;
        public int minGoodwill = 50;
    }

    public class RecruitableColonistSettings
    { 
        public PawnKindDef defaultSpousePawnKind;
        public List<RecruitableOptions> options = new();
        public int priceOffsetWithSpouse = 1000;
        public int priceOffsetWithChildren = 1000;
    }

    public class RecruitableOptions
    {
        [NoTranslate]
        public string iconPath = BaseContent.BadTexPath;

        public PawnKindDef pawnKind;

        public PawnKindDef overrideSpousePawnKind = null;

        public int count = 1;

        public bool canHaveFamily = false;

        public int basePrice = 1000;
    }
}
