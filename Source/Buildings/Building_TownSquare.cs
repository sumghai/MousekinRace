using Verse;

namespace MousekinRace
{
    public class Building_TownSquare : Building
    {
        public CompTownSquare compTownSquare;
        
        public IntVec3 centerCellPos;
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            GameComponent_Allegiance.Instance.RecacheTownSquares();
            GameComponent_Allegiance.Instance.ShowAllegianceSysIntroLetterFirstTime();
            compTownSquare = GetComp<CompTownSquare>();
            centerCellPos = base.Position;
            centerCellPos.x += compTownSquare.Props.squareCenterOffset.x;
            centerCellPos.z += compTownSquare.Props.squareCenterOffset.z;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            GameComponent_Allegiance.Instance.RecacheTownSquares();
        }
    }
}
