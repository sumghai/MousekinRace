using RimWorld;
using System;
using Verse;

namespace MousekinRace
{
    // Custom comp to store fuel filter settings modified by ITab_FuelFilter
    public class CompAllowedFuelTypes : ThingComp
    {
        public ThingFilter allowedFuelTypesFilter;

        public override void Initialize(CompProperties props)
        {
            allowedFuelTypesFilter = new ThingFilter(FilterChanged);
            ThingFilter thingFilter = parent.GetComp<CompRefuelable>()?.Props.fuelFilter;
            if (thingFilter != null) 
            {
                allowedFuelTypesFilter.CopyAllowancesFrom(thingFilter);
            }
        }

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref allowedFuelTypesFilter, "allowedFuelTypesFilter", new Action(FilterChanged));
            if (Scribe.mode == LoadSaveMode.PostLoadInit && allowedFuelTypesFilter == null)
            {
                Initialize(props);
            }
        }

        public void FilterChanged()
        {
            if (!parent.Spawned)
            {
                return;
            }

            // If the fuel settings were changed while any pawns were about to refuel it with a disallowed fuel,
            // cancel the pawns' refueling job(s)
            foreach (Pawn pawn in parent.Map.mapPawns.FreeColonistsSpawned)
            {
                if (parent.Map.reservationManager.ReservedBy(parent, pawn) && pawn.CurJobDef == JobDefOf.Refuel && pawn.CurJob.targetB.Thing != null && !allowedFuelTypesFilter.Allows(pawn.CurJob.targetB.Thing))
                {
                    pawn.jobs.StopAll();
                }
            }
        }
    }
}