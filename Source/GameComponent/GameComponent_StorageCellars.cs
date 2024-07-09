using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public class GameComponent_StorageCellars : GameComponent
    {
        public List<Building_StorageCellar> storageCellarsCache = new();

        public static GameComponent_StorageCellars Instance;

        public GameComponent_StorageCellars()
        {
            Instance = this;
        }

        public GameComponent_StorageCellars(Game game)
        {
            Instance = this;
        }

        public void PreInit()
        {
            Instance = this;
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            PreInit();
            RecacheStorageCellars();
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            PreInit();
            RecacheStorageCellars();
        }

        public void RecacheStorageCellars()
        {
            storageCellarsCache.Clear();
            storageCellarsCache = Find.Maps.Where(m => m.IsPlayerHome).SelectMany(m => m.listerBuildings.AllBuildingsColonistOfClass<Building_StorageCellar>()).ToList();
        }
    }
}
