using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MousekinRace
{
    public class Alert_InappropriateApparel : Alert
    {
        public List<Pawn> AffectedPawns {
            get 
            {
                List<Pawn> affectedPawnsResult = [];

                ThoughtDef apparelExpectationsThoughtDef = DefDatabase<ThoughtDef>.GetNamed("Mousekin_Thought_ApparelExpectations");

                foreach (Pawn pawn in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => p.IsColonist))
                {
                    if (apparelExpectationsThoughtDef.Worker.CurrentState(pawn) is { Active: true, StageIndex: 0 }) 
                    { 
                        affectedPawnsResult.Add(pawn);
                    }
                }
                return affectedPawnsResult;
            }
        }

        public override string GetLabel()
        {
            return AffectedPawns.Count > 1 ? "AlertColonistsWearingInappropriateApparelPluralLabel".Translate() : "AlertColonistsWearingInappropriateApparelSingularLabel".Translate();
        }

        public override TaggedString GetExplanation()
        {
            StringBuilder stringBuilder = new();

            foreach (Pawn pawn in AffectedPawns)
            {
                stringBuilder.AppendLine($"  - {pawn.NameShortColored.Resolve()} ({pawn.KindLabel})");
            }

            return "AlertColonistsWearingInappropriateApparelDesc".Translate(stringBuilder, MousekinDefOf.Mousekin_Thought_ApparelExpectations.stages[0].LabelCap);
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(AffectedPawns);
        }
    }
}
