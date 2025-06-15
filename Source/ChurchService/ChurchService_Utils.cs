using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MousekinRace
{
    public static class ChurchService_Utils
    {
        public const int MinNumWorshippers = 5;

        public const float BaseTitheDefault = 1f;

        public const float BaseTithePious = 3f;

        public const float BaseTitheInquisitionist = 2f;

        public static float TitheMultiplierPriest(int priestPawnSocialSkill) => priestPawnSocialSkill * 0.2f;

        public static float TitheMultiplierNuns(int numNuns) => (numNuns <= 4) ? 1f + 0.1f * numNuns : 1.4f;

        // Get a list of non-Apostate Mousekin player colonists on a given map as (potential) worshippers
        // - Apostates have trait degree = 1
        // - Mousekins with no religious affinity are also considered worshippers
        public static List<Pawn> GetMousekinPotentialWorshippers(Map map)
        { 
            return map.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => p.IsMousekin() && p.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith) != 1).ToList();
        }

        // Determine if there are enough (i.e. 5 or more) Mousekin worshippers on a given map
        public static bool EnoughPlayerMousekinWorshippers(Map map)
        {
            return GetMousekinPotentialWorshippers(map).Count() > MinNumWorshippers;
        }

        // Determine if there is a properly-configured church room/building on a given map
        // - Start by searching for a Mousekin altar
        // - Check that it is enclosed in a room
        // - Check that the room also has a lectern and at least 3 pews
        // - Return additional data regarding what furnishings are present (for showing met/unmet requirements)
        public static bool ValidChurchFound(Map map, out Building altarOnMap, out bool lecternOnMap, out bool enoughPewsOnMap)
        {
            // Get the first (usually only) church altar on the map
            Building altar = map.listerBuildings.AllBuildingsColonistOfDef(MousekinDefOf.Mousekin_ChurchAltar).FirstOrDefault();

            bool altarInValidRoom = false;
            bool lecternFound = false;
            int pewsFound = 0;
            int MinNumPews = 3;

            // If an altar exists, and is enclosed in a room (and is thus a Church)
            if (altar != null && RoomRoleWorker_Church.Validate(altar.GetRoom()))
            {
                altarInValidRoom = true;

                // Check if church furniture requirements have been met
                List<Thing> containedAndAdjacentThings = altar.GetRoom().ContainedAndAdjacentThings;
                for (int j = 0; j < containedAndAdjacentThings.Count; j++)
                {
                    // Look for any lecterns
                    if (containedAndAdjacentThings[j].def == MousekinDefOf.Mousekin_ChurchLectern)
                    {
                        lecternFound = true;
                    }
                    // Count number of pews
                    if (containedAndAdjacentThings[j].def == MousekinDefOf.Mousekin_ChurchPew)
                    {
                        pewsFound++;
                    }
                    // Stop checking if requirements are already met
                    if (lecternFound && (pewsFound >= MinNumPews))
                    {
                        break;
                    }
                }
            }

            altarOnMap = altar ?? null;
            lecternOnMap = lecternFound;
            enoughPewsOnMap = (pewsFound >= MinNumPews);

            return altarInValidRoom && lecternFound && enoughPewsOnMap;
        }

        // Override of ValidChurchFound() that only also returns the church altar on the map
        public static bool ValidChurchFound(Map map, out Building altarOnMap)
        {
            return ValidChurchFound(map, out altarOnMap, out _, out _);
        }

        // Get a list of cells behind the last row of pews/seats in a church,
        // where pawns without seats can stand
        public static List<IntVec3> GetStandableCellsBehindPews(Map map, Building lectern)
        {
            // Find the church room, given a lectern
            Room churchRoom = lectern.Position.GetRoom(map);

            // Get a list of pews/seats in the church room
            List<Thing> sittables = churchRoom.ContainedAndAdjacentThings.Where(x => x.def.category == ThingCategory.Building && x.def.building.isSittable).ToList();

            List<IntVec3> foundSpots = [];

            // Take the direction the lectern is facing, find the furthest row/column of seats from it,
            // then mark all the free cells beyond that as standable
            // (we +/- 1 so that pawns don't simply stand in the exact same row/column as the furthest seats
            switch (lectern.Rotation.AsInt)
            {
                case Rot4.NorthInt:
                    foundSpots = churchRoom.Cells.Where(c => c.z - 1 > sittables.MaxBy(t => t.Position.z).Position.z).ToList();
                    break;
                case Rot4.EastInt:
                    foundSpots = churchRoom.Cells.Where(c => c.x - 1 > sittables.MaxBy(t => t.Position.x).Position.x).ToList();
                    break;
                case Rot4.SouthInt:
                    foundSpots = churchRoom.Cells.Where(c => c.z + 1 < sittables.MaxBy(t => t.Position.z).Position.z).ToList();
                    break;
                case Rot4.WestInt:
                    foundSpots = churchRoom.Cells.Where(c => c.x + 1 < sittables.MaxBy(t => t.Position.x).Position.x).ToList();
                    break;
            }

            return foundSpots;
        }

        // Find a random Mousekin Priest from the player colonists on a given map
        public static Pawn GetRandomMousekinPriest(Map map)
        { 
            return map.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => p.kindDef == MousekinDefOf.MousekinPriest).RandomElementWithFallback();
        }

        // Generate the church service multipliersSummary letter contents and calculate the tithe amount
        public static TaggedString GetSummaryAndTitheAmount(Pawn organizer, List<Pawn> attendeePawns, List<Pawn> absentPawns, out int silverAmount)
        {
            float amount = 0;

            // Count the number of nuns, but exclude them from the worshippers
            int numNuns = attendeePawns.Where(p => p.kindDef == MousekinDefOf.MousekinNun).Count();

            // Count all worshippers (excluding the officating priest and any nuns)
            List<Pawn> worshippers = attendeePawns.Where(p => p != organizer && p.kindDef != MousekinDefOf.MousekinNun).ToList();

            // Calculate baseline tithes based on each worshipper's religious trait affinity degree
            foreach (Pawn worshipper in worshippers)
            {
                int pawnFaithTraitDegree = worshipper.story.traits.DegreeOfTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith);

                amount += pawnFaithTraitDegree switch
                {
                    3 => BaseTithePious, // Pious
                    4 => BaseTitheInquisitionist, // Inqusitionist
                    _ => BaseTitheDefault, // unknown/none (0) and Devotionist (2)
                };
            }

            // Generate breakdown of religious affinities among worshippers
            List<ReligiousAffinityPawnCount> churchReligiousAffinityPawnCounts = new();

            foreach (TraitDegreeData traitDegreeData in MousekinDefOf.Mousekin_TraitSpectrum_Faith.degreeDatas)
            {
                churchReligiousAffinityPawnCounts.Add(new ReligiousAffinityPawnCount(traitDegreeData.degree, 0, traitDegreeData.LabelCap));
            }

            List<Pawn> colonistsWithReligiousAffinities = new();
            foreach (Pawn worshipper in worshippers)
            {
                if (worshipper.story.traits.GetTrait(MousekinDefOf.Mousekin_TraitSpectrum_Faith) is Trait faithTrait)
                {
                    colonistsWithReligiousAffinities.Add(worshipper);
                    churchReligiousAffinityPawnCounts.Find(x => x.degree == faithTrait.Degree).pawnsWithTrait++;
                }                
            }

            int pawnsWithUnknownReligiousAffinity = worshippers.Count - colonistsWithReligiousAffinities.Count;
            churchReligiousAffinityPawnCounts.Add(new ReligiousAffinityPawnCount(-99999, pawnsWithUnknownReligiousAffinity, "UnknownLower".Translate().CapitalizeFirst()));

            TaggedString worshipperReligiousAffinitySummary = TaggedString.Empty;
            
            foreach (ReligiousAffinityPawnCount religiousAffinityPawnCount in churchReligiousAffinityPawnCounts)
            {
                if (religiousAffinityPawnCount.pawnsWithTrait > 0)
                {
                    worshipperReligiousAffinitySummary += "\n    " + religiousAffinityPawnCount.affinityLabel + ": " + religiousAffinityPawnCount.pawnsWithTrait;
                }
            }

            // Apply various multipliers to get the final tithe amount
            // - Priest social skill level
            // - Number of nuns
            // - Mod setting multiplier
            int priestSocialSkill = organizer.skills.GetSkill(SkillDefOf.Social).Level;
            amount *= TitheMultiplierPriest(priestSocialSkill);
            amount *= TitheMultiplierNuns(numNuns);
            amount *= MousekinRaceMod.Settings.ChurchTitheMultiplier;
            silverAmount = (int) amount;

            // Return the (translated) letter contents
            return "MousekinRace_Letter_ChurchServiceConcludedDesc".Translate(
                GenDate.DateFullStringAt(Find.TickManager.TicksAbs, Find.WorldGrid.LongLatOf(organizer.Map.Tile)),
                organizer.NameFullColored,
                priestSocialSkill + " (x" + TitheMultiplierPriest(priestSocialSkill).ToStringPercent() + ")",
                numNuns + " (x" + TitheMultiplierNuns(numNuns).ToStringPercent() + ")",
                worshippers.Count(),
                worshipperReligiousAffinitySummary,
                absentPawns.Count(),
                silverAmount
            );
        }
    }
}
