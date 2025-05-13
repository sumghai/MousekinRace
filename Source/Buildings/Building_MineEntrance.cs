using Verse;

namespace MousekinRace
{
    public class Building_MineEntrance : Building
    {
        public CompUndergroundMineDeposits compUndergroundMineDeposits;

        public MapComponent_UndergroundMineDeposits mapComponent_UndergroundMineDeposits;
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compUndergroundMineDeposits = GetComp<CompUndergroundMineDeposits>();
            mapComponent_UndergroundMineDeposits = Find.CurrentMap.GetComponent<MapComponent_UndergroundMineDeposits>();
            mapComponent_UndergroundMineDeposits.RescanDeposits(compUndergroundMineDeposits.Props.mineables);
        }
    }
}
