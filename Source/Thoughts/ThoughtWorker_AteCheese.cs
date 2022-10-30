using RimWorld;
using System.Text;
using Verse;

namespace MousekinRace
{
    public class ThoughtWorker_AteCheese : ThoughtWorker
    {
        public override string PostProcessDescription(Pawn p, string description)
        {
            CompRemembersCheeseEaten comp = p.TryGetComp<CompRemembersCheeseEaten>();

            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendTagged("ThoughtExpiresIn".Translate((comp.minAllowedTicksBetweenServings - comp.ticksSinceLastServing).ToStringTicksToPeriod()));

            return base.PostProcessDescription(p, description) + stringBuilder.ToString();
        }

        public override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            // Only check for Mousekins
            if (!Utils.IsMousekin(pawn))
            {
                return ThoughtState.Inactive;
            }

            int servingsEaten = pawn.TryGetComp<CompRemembersCheeseEaten>().servingsEaten;

            return (servingsEaten > 0) ? ThoughtState.ActiveAtStage(servingsEaten - 1) : ThoughtState.Inactive;
        }
    }
}
