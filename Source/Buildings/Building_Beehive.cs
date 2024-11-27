using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Building_Beehive : Building
    {
        public List<ThingDef> validFlowerDefs;

        public const float FlowerSearchRadius = 19.9f;

        public const int MinFlowerContainingCells = 20;

        public const int HoneyYield = 25; // Hardcoded yields to prevent exploitation by stack size altering mods

        public const int BeeswaxYield = 75;

        public const int BaseGatheringDuration = 600000; // 10 Days

        public const float MinIdealTemperature = 10f;

        public const float MaxIdealTemperature = 35f;

        public static List<IntVec3> flowerSearchCells = new List<IntVec3>();

        public IEnumerable<IntVec3> FlowerSearchCells => FlowerSearchCellsAround(base.Position, base.Map);

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            validFlowerDefs = base.def.GetModExtension<BeehiveValidFlowersExtension>().validFlowersList.flowerPlants.OrderBy(x => x.label).ToList();
        }

        public int CellsWithValidFlowers(Map map)
        {
            int flowerCells = 0;

            foreach (var cell in FlowerSearchCells)
            {
                Plant plant = cell.GetPlant(map);

                if (plant != null && validFlowerDefs.Contains(plant.def))
                {
                    flowerCells++;
                }
            }

            return flowerCells;
        }

        public float progressInt;

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

        public static List<IntVec3> FlowerSearchCellsAround(IntVec3 pos, Map map)
        {
            flowerSearchCells.Clear();

            if (!pos.InBounds(map))
            {
                return flowerSearchCells;
            }

            Region region = pos.GetRegion(map);

            if (region == null)
            {
                return flowerSearchCells;
            }

            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, delegate (Region r)
            {
                foreach (IntVec3 cell in r.Cells)
                {
                    if (cell.InHorDistOf(pos, FlowerSearchRadius))
                    {
                        flowerSearchCells.Add(cell);
                    }
                }
                return false;
            });

            return flowerSearchCells;
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
                    stringBuilder.Append("MousekinRace_Beehive_HoneyReady".Translate());
                }
                else if (!AmbientTempIsOk())
                {
                    stringBuilder.Append("MousekinRace_Beehive_HoneyPausedBadTemp".Translate());
                }
                else if (CellsWithValidFlowers(base.Map) < MinFlowerContainingCells)
                {
                    stringBuilder.Append("MousekinRace_Beehive_HoneyPausedNotEnoughFlowers".Translate(CellsWithValidFlowers(base.Map), MinFlowerContainingCells));
                }
                else
                {
                    int daysRemaining = (int)(BaseGatheringDuration - Progress);

                    stringBuilder.Append("MousekinRace_Beehive_HoneyProgress".Translate(GenDate.ToStringTicksToPeriod(daysRemaining, true, false, true, true)));
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
            return new List<Thing> { product1, product2 };
        }

        public override void TickRare()
        {
            base.TickRare();

            if (AmbientTempIsOk() && CellsWithValidFlowers(base.Map) >= MinFlowerContainingCells && !ProductsReady)
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
            if (Prefs.DevMode && DebugSettings.godMode)
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
            yield return new Command_Action
            {
                action = new Action(this.ShowManual),
                defaultLabel = "MousekinRace_CommandShowBeehiveManualLabel".Translate(),
                defaultDesc = "MousekinRace_CommandShowBeehiveManualDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Mousekin_BeehiveManual", true)
            };
            yield break;
        }

        public void ShowManual()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("MousekinRace_Beehive_Instructions".Translate(MinIdealTemperature.ToStringTemperature(), MaxIdealTemperature.ToStringTemperature(), MinFlowerContainingCells));

            // Remove any duplicate label entries before displaying
            // (Some modders make both ground-sowable and decorative variants of the same plant)
            List<ThingDef> validFlowerDefsNoDuplicateLabels = validFlowerDefs.Distinct(new FlowerComparer()).ToList();

            foreach (ThingDef flowerDef in validFlowerDefsNoDuplicateLabels)
            {
                stringBuilder.AppendLine("- " + flowerDef.LabelCap);
            }
            Dialog_MessageBox window = new Dialog_MessageBox(stringBuilder.ToString(), null, null, null, null, null, false, null, null);
            Find.WindowStack.Add(window);
        }

        public class FlowerComparer : IEqualityComparer<ThingDef>
        {
            public bool Equals(ThingDef x, ThingDef y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) 
                {
                    return false;
                }

                return x.LabelCap == y.LabelCap;
            }

            public int GetHashCode(ThingDef def) 
            {
                //Check whether the def is null
                if (ReferenceEquals(def, null))
                {
                    return 0;
                }

                //Get hash code for the label
                var hashLabel = def.LabelCap.GetHashCode();

                //Calculate the hash code for the SomeClass.
                return hashLabel;
            }
        }

    }
}
