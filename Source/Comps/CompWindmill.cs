using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompWindmill : ThingComp
    {
        public static List<IntVec3> windmillCells = new();

        public Rot4 capDirection;

        public CompProperties_Windmill Props => (CompProperties_Windmill)props;

        public override void PostDraw()
        {
            base.PostDraw();

            GraphicData capGraphicData = Props.capGraphicData;

            if (capGraphicData != null)
            {
                Mesh capMesh = capGraphicData.Graphic.MeshAt(capDirection);
                Graphics.DrawMesh(capMesh, parent.DrawPos + capGraphicData.drawOffset, Quaternion.identity, capGraphicData.Graphic.GetColoredVersion(capGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(capDirection), 0);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref capDirection, "capDirection", defaultValue: Rot4.South, forceSave: true);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetWornGizmosExtra())
            {
                yield return item;
            }

            Command_Action command_Action = new()
            {
                defaultLabel = "MousekinRace_CommandRotateWindmillCapLabel".Translate(),
                defaultDesc = "MousekinRace_CommandRotateWindmillCapDesc".Translate(),
                action = delegate
                {
                    capDirection.Rotate(RotationDirection.Clockwise);
                }
            };
            yield return command_Action;
        }

        public static bool HasObstructedCellsWithinRadius(IntVec3 pos, Map map, int radius)
        {
            return GenRadial.RadialCellsAround(pos, radius, true).Where(c => c.InBounds(map) && c.Impassable(map)).Count() > 0;
        }

        public static bool HasOtherWindmillOrBlueprintWithinRadius(IntVec3 pos, Map map, int radius)
        {
            return GenRadial.RadialCellsAround(pos, radius, true).Where(c => c.InBounds(map) && (c.GetFirstThing(map, MousekinDefOf.Mousekin_Windmill) != null || c.GetFirstThing(map, MousekinDefOf.Blueprint_Mousekin_Windmill) != null)).Count() > 0;
        }
    }
}