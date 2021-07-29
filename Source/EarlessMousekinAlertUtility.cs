using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MousekinRace
{
    public class EarlessMousekinAlertUtility
    {
        public const int criticalWarningThresholdDays = 4;

        public const int suicideAttemptThresholdDays = 6;
        
        public static List<Pawn> mousekinsMiserableResult = new List<Pawn>();

        public static List<Pawn> mousekinsSuicidalResult = new List<Pawn>();

        public static List<Pawn> MousekinsMiserable
        {
            get
            {
                mousekinsMiserableResult.Clear();

                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    if ((IsMissingBothEars(pawn) && GetDaysSinceBothEarsLost(pawn) < criticalWarningThresholdDays) || (IsMissingBothEars(pawn) && !MousekinRaceMod.Settings.EarlessMousekinsAreSuicidal))
                    {
                        mousekinsMiserableResult.Add(pawn);
                    }
                }

                return mousekinsMiserableResult;
            }
        }

        public static List<Pawn> MousekinsSuicidal
        {
            get
            {
                mousekinsSuicidalResult.Clear();

                foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
                {
                    if (IsMissingBothEars(pawn) && GetDaysSinceBothEarsLost(pawn) > criticalWarningThresholdDays && MousekinRaceMod.Settings.EarlessMousekinsAreSuicidal)
                    {
                        mousekinsSuicidalResult.Add(pawn);
                    }
                }

                return mousekinsSuicidalResult;
            }
        }

        public static string AlertLabel
        {
            get 
            {
                int num = MousekinsSuicidal.Count();
                string text = "";
                if (num > 0)
                {
                    text = "AlertEarlessMousekinFeelingSuicidalLabel".Translate();
                }
                else
                {
                    num = MousekinsMiserable.Count();
                    if (num > 0)
                    { 
                        text = "AlertEarlessMousekinFeelingMiserableLabel".Translate();
                    }
                }
                if (num > 1)
                {
                    text = text + " x" + num.ToStringCached();
                }
                return text;
            }
        }
    
        public static string AlertExplanation
        {
            get 
            {
                StringBuilder stringBuilder = new StringBuilder();

                if (MousekinsSuicidal.Any())
                {
                    StringBuilder stringBuilder2 = new StringBuilder();
                    foreach (Pawn pawn in MousekinsSuicidal)
                    {
                        stringBuilder2.AppendLine("  - " + pawn.NameShortColored.Resolve());
                    }
                    stringBuilder.Append("AlertEarlessMousekinFeelingSuicidalDesc".Translate(stringBuilder2.ToString()));
                }

                if (MousekinsMiserable.Any())
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.AppendLine();
                    }
                    StringBuilder stringBuilder3 = new StringBuilder();
                    foreach (Pawn pawn2 in MousekinsMiserable)
                    {
                        stringBuilder3.AppendLine("  - " + pawn2.NameShortColored.Resolve());
                    }
                    stringBuilder.Append("AlertEarlessMousekinFeelingMiserableDesc".Translate(stringBuilder3.ToString()));
                }

                stringBuilder.AppendLine();
                stringBuilder.Append("AlertEarlessMousekinDescEnding".Translate());

                return stringBuilder.ToString();
            }
        }

        public static bool IsMissingBothEars(Pawn pawn)
        {
            return pawn.health.hediffSet.cachedMissingPartsCommonAncestors.Count(x => x.Part.def == MousekinDefOf.Mousekin_Ear) > 1;
        }
        
        public static float GetDaysSinceBothEarsLost(Pawn pawn)
        {
            List<Hediff_MissingPart> missingEarHediffs = pawn.health.hediffSet.cachedMissingPartsCommonAncestors.FindAll(x => x.Part.def == MousekinDefOf.Mousekin_Ear);

            if (IsMissingBothEars(pawn))
            {
                int firstEarLostAgeTicks = missingEarHediffs[0].ageTicks;
                int secondEarLostAgeTicks = missingEarHediffs[1].ageTicks;
                int bothEarsLostAgeTicks = Math.Min(firstEarLostAgeTicks, secondEarLostAgeTicks);
                return GenDate.TicksToDays(bothEarsLostAgeTicks);
            }
            else
            {
                return 0;
            }
        }
    }
}
