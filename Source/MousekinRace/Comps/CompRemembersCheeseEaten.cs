using System;
using System.Collections.Generic;
using Verse;

namespace MousekinRace
{
    public class CompRemembersCheeseEaten : ThingComp
    {
        public int servingsEaten = 0;
        public int ticksSinceLastServing;
        public int minAllowedTicksBetweenServings = 90000; // 1.5 days or 36 hours

        public CompProperties_RemembersCheeseEaten Props => (CompProperties_RemembersCheeseEaten)props;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref servingsEaten, "servingsEaten");
            Scribe_Values.Look(ref ticksSinceLastServing, "ticksSinceLastServing");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo c in base.CompGetGizmosExtra())
            {
                yield return c;
            }
            if (Prefs.DevMode && DebugSettings.godMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: -1 hour cheese cooldown",
                    action = delegate
                    {
                        ticksSinceLastServing += 2500;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: -12 hour cheese cooldown",
                    action = delegate
                    {
                        ticksSinceLastServing += 2500 * 12;
                    }
                };
            }
        }

        public float EatCheese()
        {
            servingsEaten += servingsEaten < MousekinDefOf.Mousekin_Thought_AteCheese.stages.Count ? 1 : 0;
            ticksSinceLastServing = 0;
            return MousekinDefOf.Mousekin_Thought_AteCheese.stages[servingsEaten - 1].baseMoodEffect;
        }

        public void Reset()
        {
            servingsEaten = 0;
            ticksSinceLastServing = 0;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (ticksSinceLastServing >= minAllowedTicksBetweenServings)
            {
                servingsEaten = 0;
            }

            if (parent.IsHashIntervalTick(60) && ticksSinceLastServing < minAllowedTicksBetweenServings)
            {
                ticksSinceLastServing += Math.Min(minAllowedTicksBetweenServings - ticksSinceLastServing, 60);
            }
        }
    }
}