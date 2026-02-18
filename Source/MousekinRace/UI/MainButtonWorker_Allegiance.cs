using RimWorld;
using Verse;

namespace MousekinRace
{
    // Adds a new menu button to access to Faction Allegiance system
	public class MainButtonWorker_Allegiance : MainButtonWorker
    {
        // Only show the button if the savegame has at least one player-owned town square in their colony map(s)
		public override bool Visible => base.Visible && Current.Game.GetComponent<GameComponent_Allegiance>().HasAnyTownSquares;

        public override void Activate()
        {
            Find.MainTabsRoot.ToggleTab(def);
        }
    }
}
