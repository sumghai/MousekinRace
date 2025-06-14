using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompUndergroundMineDeposits : ThingComp
    {
        public CompProperties_UndergroundMineDeposits Props => (CompProperties_UndergroundMineDeposits)props;

        //public MapComponent_UndergroundMineDeposits mapComp;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            //mapComp = Find.CurrentMap.GetComponent<MapComponent_UndergroundMineDeposits>();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_ActionShowCurrentDeposits = new()
                {
                    defaultLabel = "DEV: Show deposits",
                    action = delegate
                    {
                        string output = "";
                        /*foreach (MineableThingCount deposit in mapComp.deposits) 
                        { 
                            output += deposit.mineableThing.defName + " = " + deposit.amountRemaining + " remaining\n";
                        }
                        Log.Warning($"Current deposits in {mapComp.map}:\n" + output);
                        if (!PlayDataLoader.Loaded || Prefs.DevMode)
                        {
                            Log.TryOpenLogWindow();
                        }*/
                    }
                };
                yield return command_ActionShowCurrentDeposits;

                Command_Action command_ActionRegenDeposits = new()
                {
                    defaultLabel = "DEV: Regen deposits",
                    action = delegate
                    {
                        //mapComp.deposits.Clear();
                        //mapComp.RescanDeposits(Props.mineables);
                        //Log.Warning($"Regenerated {mapComp.deposits.Count} deposits in {mapComp.map}");
                    }
                };
                yield return command_ActionRegenDeposits;
            }
        }
    }
}
