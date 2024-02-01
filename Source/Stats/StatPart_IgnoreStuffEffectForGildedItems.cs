using RimWorld;

namespace MousekinRace
{
    public class StatPart_IgnoreStuffEffectForGildedItems : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            return null;
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!req.HasThing || req.StuffDef == null || req.Thing.def.GetModExtension<ApparelIgnoreStuffStatFactorsExtension>() == null)
            {
                return;
            }

            float factorOriginal = req.StuffDef.stuffProps.statFactors.GetStatFactorFromList(this.parentStat);
            float factorInverted = 1 / factorOriginal;

            val = factorInverted * val;
        }
    }
}
