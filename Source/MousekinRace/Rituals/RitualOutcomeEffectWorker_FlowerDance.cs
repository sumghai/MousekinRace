using RimWorld;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class RitualOutcomeEffectWorker_FlowerDance : RitualOutcomeEffectWorker_FromQuality
    {
        public override bool SupportsAttachableOutcomeEffect => false;

        public RitualOutcomeEffectWorker_FlowerDance()
        {
        }

        public RitualOutcomeEffectWorker_FlowerDance(RitualOutcomeEffectDef def)
            : base(def)
        {
        }

        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
            // Destroy the ritual focus object
            if (!jobRitual.selectedTarget.HasThing)
            {
                return;
            }
            Thing thing = jobRitual.selectedTarget.Thing;
            def.effecter?.Spawn(thing, jobRitual.selectedTarget.Map).Cleanup();
            if (def.fleckDef != null)
            {
                CellRect cellRect = thing.OccupiedRect();
                for (int i = 0; i < cellRect.Area * def.flecksPerCell; i++)
                {
                    FleckCreationData dataStatic = FleckMaker.GetDataStatic(cellRect.RandomVector3, thing.Map, def.fleckDef, def.fleckScaleRange.RandomInRange);
                    dataStatic.rotation = def.fleckRotationRange.RandomInRange;
                    dataStatic.velocityAngle = def.fleckVelocityAngle.RandomInRange;
                    dataStatic.velocitySpeed = def.fleckVelocitySpeed.RandomInRange;
                    thing.Map.flecks.CreateFleck(dataStatic);
                }
            }
            if (def.filthDefToSpawn != null)
            {
                thing.OccupiedRect();
                foreach (IntVec3 item in thing.OccupiedRect())
                {
                    FilthMaker.TryMakeFilth(item, thing.Map, def.filthDefToSpawn, def.filthCountToSpawn.RandomInRange);
                }
            }
            thing.Destroy();

            // Generate the quality outcome letter, apply ritual outcome memories, and conditionally remove death mood debuffs
            float quality = GetQuality(jobRitual, progress);
            RitualOutcomePossibility outcome = GetOutcome(quality, jobRitual);
            LookTargets letterLookTargets = jobRitual.selectedTarget;
            string extraLetterText = null;
            if (jobRitual.Ritual != null)
            {
                ApplyAttachableOutcome(totalPresence, jobRitual, outcome, out extraLetterText, ref letterLookTargets);
            }
            string text = outcome.description.Formatted(jobRitual.Ritual.Label).CapitalizeFirst();
            string text2 = def.OutcomeMoodBreakdown(outcome);
            if (!text2.NullOrEmpty())
            {
                text = text + "\n\n" + text2;
            }
            if (extraLetterText != null)
            {
                text = text + "\n\n" + extraLetterText;
            }
            if (outcome.Positive)
            {
                text = text + "\n\n" + "MousekinRace_RitualOutcome_FlowerDanceDeathThoughtsRemoved".Translate();
            }
            text = text + "\n\n" + OutcomeQualityBreakdownDesc(quality, progress, jobRitual);
            Find.LetterStack.ReceiveLetter("OutcomeLetterLabel".Translate(outcome.label.Named("OUTCOMELABEL"), jobRitual.Ritual.Label.Named("RITUALLABEL")), text, outcome.Positive ? LetterDefOf.RitualOutcomePositive : LetterDefOf.RitualOutcomeNegative, letterLookTargets);
            foreach (KeyValuePair<Pawn, int> item in totalPresence)
            {
                if (!outcome.roleIdsNotGainingMemory.NullOrEmpty())
                {
                    RitualRole ritualRole = jobRitual.assignments.RoleForPawn(item.Key);
                    if (ritualRole != null && outcome.roleIdsNotGainingMemory.Contains(ritualRole.id))
                    {
                        continue;
                    }
                }
                if (outcome.memory != null)
                {
                    GiveMemoryToPawn(item.Key, outcome.memory, jobRitual);
                }
                if (outcome.Positive && item.Key.needs?.mood?.thoughts != null && def.comps.Find(c => c is RitualOutcomeComp_DeathThoughts) is RitualOutcomeComp_DeathThoughts deathThoughtsComp && deathThoughtsComp.thoughtDefsToRemoveOnPositiveOutcome.Count > 0)
                {
                    foreach (ThoughtDef deathThoughtDef in deathThoughtsComp.thoughtDefsToRemoveOnPositiveOutcome)
                    {
                        item.Key.needs.mood.thoughts.memories.RemoveMemoriesOfDef(deathThoughtDef);
                    }
                }
            }
        }
    }
}
