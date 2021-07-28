using RimWorld;
using Verse;

namespace MousekinRace
{
    public class Alert_MissingEarsSuicidal : Alert_Critical
    {
        public override string GetLabel()
        {
            return EarlessMousekinAlertUtility.AlertLabel;
        }

        public override TaggedString GetExplanation()
        {
            return EarlessMousekinAlertUtility.AlertExplanation;
        }

        public override AlertReport GetReport()
        {
            return AlertReport.CulpritsAre(EarlessMousekinAlertUtility.MousekinsSuicidal);
        }
    }
}