using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompUndergroundMineDeposits : ThingComp
    {
        public CompProperties_UndergroundMineDeposits Props => (CompProperties_UndergroundMineDeposits)props;

        public MapComponent_UndergroundMineDeposits mapComp;

        public List<FloatMenuOption> miningOptions;

        public MiningBillStack miningBillStack = new();

        public MiningBillStack MiningBillStack => miningBillStack;

        public MiningBill MiningBill => miningBillStack.FirstCanDo;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            mapComp = Find.CurrentMap.GetComponent<MapComponent_UndergroundMineDeposits>();
        }

        public List<FloatMenuOption> MiningOptions 
        {
            get 
            { 
                miningOptions = [];

                for (int i = 0; i < Props.mineables.Count; i++) 
                { 
                    MineableCountRange miningOption = Props.mineables[i];

                    string label = "Mine " + miningOption.mineableThing.LabelCap + " x" + miningOption.minedPortionSize;

                    miningOptions.Add(new FloatMenuOption(label, 
                        () => miningBillStack.AddMiningBill(miningOption.mineableThing, parent),
                        miningOption.mineableThing,
                        null,
                        false,
                        MenuOptionPriority.Default,
                        (Rect rect) => miningOption.DoOptionInfoWindow(i, rect),
                        null,
                        29f,
                        (Rect rect) => miningOption.mineableThing != null && Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, miningOption.mineableThing),
                        null,
                        true));
                }
                return miningOptions;
            }
        }


        public override void PostDeSpawn(Map map)
        {
            foreach (MiningBill miningBill in miningBillStack)
            { 
                miningBill.ResetMiningBill();
            }
        }

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref miningBillStack, "miningBillStack");
        }

        public override void CompTick()
        {
            if (parent.IsHashIntervalTick(100))
            {
                Tick(100);
            }
        }

        public override void CompTickRare() => Tick(GenTicks.TickRareInterval);

        public override void CompTickLong() => Tick(GenTicks.TickLongInterval);

        private void Tick(int ticks)
        { 
            // todo - tick current mining bill progress depending on number of pawns working inside building
            //MiningBill?.Tick(ticks);
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
                        foreach (MineableThingCount deposit in mapComp.deposits) 
                        { 
                            output += deposit.mineableThing.defName + " = " + deposit.amountRemaining + " remaining\n";
                        }
                        Log.Warning($"Current deposits:\n" + output);
                        if (!PlayDataLoader.Loaded || Prefs.DevMode)
                        {
                            Log.TryOpenLogWindow();
                        }
                    }
                };
                yield return command_ActionShowCurrentDeposits;

                Command_Action command_ActionRegenDeposits = new()
                {
                    defaultLabel = "DEV: Regen deposits",
                    action = delegate
                    {
                        mapComp.deposits.Clear();
                        mapComp.RescanDeposits(Props.mineables);
                        Log.Warning($"Regenerated {mapComp.deposits.Count} deposits");
                    }
                };
                yield return command_ActionRegenDeposits;
            }
        }
    }
}
