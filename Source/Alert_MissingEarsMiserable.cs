using RimWorld;
using Verse;

namespace MousekinRace
{
    public class Alert_MissingEarsMiserable : Alert
    {
        public Alert_MissingEarsMiserable()
        { 
            defaultPriority = AlertPriority.High;
        }
        
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
            if (EarlessMousekinAlertUtility.MousekinsSuicidal.Any())
            {
                return false;
            }
            return AlertReport.CulpritsAre(EarlessMousekinAlertUtility.MousekinsMiserable);
        }
    }
}