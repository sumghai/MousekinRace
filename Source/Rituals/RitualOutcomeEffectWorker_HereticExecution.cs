using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class RitualOutcomeEffectWorker_HereticExecution : RitualOutcomeEffectWorker_FromQuality
    {
        public override bool SupportsAttachableOutcomeEffect => false;

        public RitualOutcomeEffectWorker_HereticExecution()
        {
        }

        public RitualOutcomeEffectWorker_HereticExecution(RitualOutcomeEffectDef def)
            : base(def)
        {
        }

        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
            // Temporary lists for pawns with converted religious affinity traits
            List<Pawn> newPious = [];
            List<Pawn> newInquisitionists = [];
            List<Pawn> newApostates = [];
            
            // Calculate the quality outcome
            float quality = GetQuality(jobRitual, progress);
            RitualOutcomePossibility outcome = GetOutcome(quality, jobRitual);
            
            foreach (Pawn pawn in totalPresence.Keys)
            {
                if (!outcome.roleIdsNotGainingMemory.NullOrEmpty())
                {
                    RitualRole ritualRole = jobRitual.assignments.RoleForPawn(pawn);
                    if (ritualRole != null && outcome.roleIdsNotGainingMemory.Contains(ritualRole.id))
                    {                        
                        continue;
                    }
                }

                // Change the pawn's religious affinity level trait (based on probabilities)
                // and store them into various lists
                ChangePawnReligiousAffinityWithChance(pawn, outcome.positivityIndex, newPious, newInquisitionists, newApostates);

                // Apply ritual outcome memories
                if (outcome.memory != null)
                {
                    GiveMemoryToPawn(pawn, outcome.memory, jobRitual);
                }

                // Suppress slaves 100%
                if (pawn.IsSlave)
                {
                    Need_Suppression need_Suppression = pawn.needs.TryGetNeed<Need_Suppression>();
                    if (need_Suppression != null)
                    {
                        need_Suppression.CurLevel = 1f;
                    }
                }
            }

            // Generate the quality outcome letter
            TaggedString text = outcome.description.Formatted(jobRitual.Ritual.Label).CapitalizeFirst();
            TaggedString text2 = def.OutcomeMoodBreakdown(outcome);
            if (!text2.NullOrEmpty())
            {
                text = text + "\n\n" + text2;
            }
            if (newPious.Any())
            {
                text = text + "\n\n" + "MousekinRace_RitualOutcome_HereticExecutionNewPious".Translate(GetPawnListString(newPious));
            }
            if (newInquisitionists.Any())
            {
                text = text + "\n\n" + "MousekinRace_RitualOutcome_HereticExecutionNewInquisitionists".Translate(GetPawnListString(newInquisitionists));
            }
            if (newApostates.Any())
            {
                text = text + "\n\n" + "MousekinRace_RitualOutcome_HereticExecutionNewApostates".Translate(GetPawnListString(newApostates));
            }
            text = text + "\n\n" + OutcomeQualityBreakdownDesc(quality, progress, jobRitual);

            List<Pawn> pawnsWithChangedTraits =
            [
                .. newPious,
                .. newInquisitionists,
                .. newApostates,
            ];
            LookTargets letterLookTargets = pawnsWithChangedTraits;

            Find.LetterStack.ReceiveLetter("OutcomeLetterLabel".Translate(outcome.label.Named("OUTCOMELABEL"), jobRitual.Ritual.Label.Named("RITUALLABEL")), text, outcome.Positive ? LetterDefOf.RitualOutcomePositive : LetterDefOf.RitualOutcomeNegative, letterLookTargets);
        }

        public void ChangePawnReligiousAffinityWithChance(Pawn pawn, int outcomeIndex, List<Pawn> newPiousList, List<Pawn> newInquisitionistsList, List<Pawn> newApostatesList)
        {            
            float badExecution_traitConvChance_noneToApostate = 0.5f;
            float badExecution_traitConvChance_piousToApostate = 0.3f;
            float badExecution_traitConvChance_inquisitionistToApostate = 0.1f;

            float goodExecution_traitConvChance_noneToPious = 0.4f;
            float goodExecution_traitConvChance_hereticToPious = 0.1f;
            float goodExecution_traitConvChance_piousToInquisitionist = 0.2f;

            ReligiousTraitAffinity pawnReligiousAffinity = (ReligiousTraitAffinity) pawn.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);

            // Terrible execution (ignore clergy)
            if (outcomeIndex < 0 && pawn.kindDef != MousekinDefOf.MousekinPriest && pawn.kindDef != MousekinDefOf.MousekinNun)
            {
                switch (pawnReligiousAffinity)
                { 
                    case ReligiousTraitAffinity.None:
                        TryChangeAffinityTrait(pawn, ReligiousTraitAffinity.Apostate, ref newApostatesList, badExecution_traitConvChance_noneToApostate);
                        break;
                    case ReligiousTraitAffinity.Pious:
                        TryChangeAffinityTrait(pawn, ReligiousTraitAffinity.Apostate, ref newApostatesList, badExecution_traitConvChance_piousToApostate);
                        break;
                    case ReligiousTraitAffinity.Inquisitionist:
                        TryChangeAffinityTrait(pawn, ReligiousTraitAffinity.Apostate, ref newApostatesList, badExecution_traitConvChance_inquisitionistToApostate);
                        break;
                }
            }
            // Satisfying and Spectacular executions
            else
            {
                switch (pawnReligiousAffinity)
                {
                    case ReligiousTraitAffinity.None:
                        TryChangeAffinityTrait(pawn, ReligiousTraitAffinity.Pious, ref newPiousList, goodExecution_traitConvChance_noneToPious * (float) outcomeIndex);
                        break;
                    case ReligiousTraitAffinity.Apostate:
                    case ReligiousTraitAffinity.Devotionist:
                        TryChangeAffinityTrait(pawn, ReligiousTraitAffinity.Pious, ref newPiousList, goodExecution_traitConvChance_hereticToPious * (float)outcomeIndex);
                        break;
                    case ReligiousTraitAffinity.Pious:
                        TryChangeAffinityTrait(pawn, ReligiousTraitAffinity.Inquisitionist, ref newInquisitionistsList, goodExecution_traitConvChance_piousToInquisitionist * (float)outcomeIndex);
                        break;
                }
            }
        }

        public bool TryChangeAffinityTrait(Pawn pawn, ReligiousTraitAffinity newAffinity, ref List<Pawn> pawnListToStoreInOnSuccess, float chance)
        {
            if (Rand.Chance(chance))
            {
                pawn.ChangePawnReligiousAffinity(newAffinity);
                pawnListToStoreInOnSuccess.Add(pawn);
                return true;
            }
            return false;
        }

        public string GetPawnListString(List<Pawn> pawns)
        {
            string pawnList = "";
            foreach (Pawn pawn in pawns)
            {
                pawnList += "  - " + pawn.NameShortColored.Resolve() + "\n";
            }
            return pawnList.TrimEndNewlines();
        }
    }
}
