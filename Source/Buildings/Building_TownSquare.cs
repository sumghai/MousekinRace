using Verse;

namespace MousekinRace
{
    public class Building_TownSquare : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            GameComponent_Allegiance.Instance.RecacheTownSquares();
            GameComponent_Allegiance.Instance.ShowAllegianceSysIntroLetterFirstTime();
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            GameComponent_Allegiance.Instance.RecacheTownSquares();
        }
    }
}
