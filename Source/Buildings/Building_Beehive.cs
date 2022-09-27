using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Building_Beehive: Building
    {
        public float progressInt;

        public const int HoneyYield = 25; // Hardcoded yields to prevent exploitation by stack size altering mods

        public const int BeeswaxYield = 75;

        public const int BaseGatheringDuration = 600000; // 10 Days

        public const float MinIdealTemperature = 10f;

        public const float MaxIdealTemperature = 35f;

        public bool ProductsReady
        {
            get
            {
                return Progress >= BaseGatheringDuration;
            }
        }

        public float Progress
        {
            get
            {
                return progressInt;
            }
            set
            {
                if (value != progressInt)
                {
                    progressInt = value;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref progressInt, "progress", 0f);
        }

        public bool AmbientTempIsOk()
        {
            float ambientTemperature = AmbientTemperature;
            return (ambientTemperature >= MinIdealTemperature) && (ambientTemperature <= MaxIdealTemperature);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());

            if (ParentHolder != null && !(ParentHolder is Map))
            {
                // If minified, don't show status messages
            }
            else
            {
                if (ProductsReady)
                {
                    stringBuilder.Append("Awaiting pickup");
                }
                else if (!AmbientTempIsOk())
                {
                    stringBuilder.Append("Paused: Out of temperature range");
                }
                // Todo - string for insufficient plants in search radius
                else
                {
                    int daysRemaining = (int)(BaseGatheringDuration - Progress);

                    stringBuilder.Append("Next honey in: " + GenDate.ToStringTicksToPeriod(daysRemaining, true, false, true, true));
                }
            }

            return stringBuilder.ToString();
        }

        public List<Thing> TakeOutProducts()
        {
            Thing product1 = ThingMaker.MakeThing(MousekinDefOf.Mousekin_RawHoney);
            product1.stackCount = HoneyYield;
            Thing product2 = ThingMaker.MakeThing(MousekinDefOf.Mousekin_Beeswax);
            product2.stackCount = BeeswaxYield;
            Progress = 0f;
            return new List<Thing>{ product1, product2 };
        }

        public override void TickRare()
        {
            base.TickRare();
            
            if (AmbientTempIsOk() && !ProductsReady)
            {
                Progress += GenTicks.TickRareInterval;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Reset",
                    action = delegate
                    {
                        Progress = 0;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Progress +1 Hour",
                    action = delegate
                    {
                        Progress += Mathf.Min(2500, BaseGatheringDuration - Progress);
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Progress +1 Day",
                    action = delegate
                    {
                        Progress += Mathf.Min(60000, BaseGatheringDuration - Progress);
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Progress -1 Day",
                    action = delegate
                    {
                        Progress -= Mathf.Min(60000, Progress);
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Progress -1 Hour",
                    action = delegate
                    {
                        Progress -= Mathf.Min(2500, Progress);
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Finish",
                    action = delegate
                    {
                        Progress = BaseGatheringDuration;
                    }
                };
            }
        }

    }
}
