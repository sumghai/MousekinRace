using RimWorld;
using Verse;

namespace MousekinRace
{
    public class Thought_AttendedRitual_StageByReligiousAffinity : Thought_AttendedRitual
    {
        public ReligiousTraitAffinity Affinity => (ReligiousTraitAffinity) pawn.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);
        
        public override int CurStageIndex
        {
            get
            {
                int stageIndex = 0;

                if (pawn.IsMousekin() && pawn.ideo.Ideo == Faction.OfPlayer.ideos.primaryIdeo)
                { 
                    switch (Affinity)
                    {
                        case ReligiousTraitAffinity.Inquisitionist:
                            stageIndex = 3;
                            break;
                        case ReligiousTraitAffinity.None:
                        case ReligiousTraitAffinity.Pious:
                            stageIndex = 2;
                            break;
                        case ReligiousTraitAffinity.Apostate:
                        case ReligiousTraitAffinity.Devotionist:
                            stageIndex = 1;
                            break;
                    }
                }

                return stageIndex;
            }
        }

        public override string Description
        {
            get 
            {
                string append = "";
                if (pawn.IsNonMousekinRodentkind())
                {
                    append = "\n\n" + "CausedBy".Translate() + ": " + "MousekinRace_NonMousekinRodentkind".Translate().CapitalizeFirst() + $" ({pawn.def.LabelCap})";
                }
                else if (Affinity != ReligiousTraitAffinity.None && Affinity != ReligiousTraitAffinity.Pious)
                {
                    append = "\n\n" + "CausedBy".Translate() + ": " + "MousekinRace_AllegianceSys_ViewExtraInfoDialog_Trait".Translate(pawn.story.traits.GetTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith, (int)Affinity).LabelCap);
                }

                return base.CurStage.description.Formatted(sourcePrecept.Named("RITUAL")) + append;
            }
        }
    }
}
