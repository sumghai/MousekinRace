using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompWindmill : ThingComp
    {
        public CompProperties_Windmill Props => (CompProperties_Windmill)props;

        public static List<IntVec3> windmillCells = new();

        public Rot4 capDirection;

        public float spinPosition = Rand.Range(0f, 360f);

        public const float northSouthYscale = 0.75f;

        public override void CompTick()
        {
            base.CompTick();

            if (spinPosition >= 360f)
            {
                spinPosition = 0f;
            }
            else 
            { 
                spinPosition += parent.Map.windManager.WindSpeed;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            GraphicData capGraphicData = Props.capGraphicData;
            GraphicData sailGraphicData = Props.sailGraphicData;

            // Draw the cap of the windmill
            if (capGraphicData != null)
            {
                Mesh capMesh = capGraphicData.Graphic.MeshAt(capDirection);
                Graphics.DrawMesh(capMesh, parent.DrawPos + capGraphicData.drawOffset, Quaternion.identity, capGraphicData.Graphic.GetColoredVersion(capGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(capDirection), 0);
            }

            // Draw the animated sails based on cap orientation
            if (sailGraphicData != null)
            {
                Mesh sailMesh;

                // Select the appropriate draw offset
                Vector3 sailDrawOffset = capDirection.AsInt switch
                {
                    Rot4.NorthInt => sailGraphicData.drawOffsetNorth ?? sailGraphicData.drawOffset,
                    Rot4.EastInt => sailGraphicData.drawOffsetEast ?? sailGraphicData.drawOffset,
                    Rot4.SouthInt => sailGraphicData.drawOffsetSouth ?? sailGraphicData.drawOffset,
                    Rot4.WestInt => sailGraphicData.drawOffsetWest ?? sailGraphicData.drawOffset,
                    _ => sailGraphicData.drawOffset,
                };

                // Set the rotation direction
                int sailDirection = (capDirection == Rot4.South || capDirection == Rot4.East) ? -1 : 1;

                // North/South cap direction
                if (capDirection == Rot4.North || capDirection == Rot4.South)
                {
                    // Iterate through each of the sails
                    for (int sail = 0; sail < Props.sailCount; sail++)
                    {
                        /* WIP - Transformations
                        float oddBladeOffset = (sail % 2 != 0) ? 360f / Props.sailCount : 0;
                        float sailLengthScale = northSouthYscale + ((1 - northSouthYscale) * (Mathf.Cos(2 * (spinPosition + oddBladeOffset) * Mathf.PI / 180) + 1) / 2);
                        float sailWidthScale = northSouthYscale + ((1 - northSouthYscale) * (Mathf.Sin(2 * (spinPosition + oddBladeOffset) * Mathf.PI / 180) + 1) / 2);
                        Vector2 scaledVector = Vector2.Scale(sailGraphicData.Graphic.drawSize, new(sailLengthScale, sailWidthScale));*/

                        sailMesh = (capDirection == Rot4.South) ? MeshPool.GridPlane(sailGraphicData.Graphic.drawSize) : MeshPool.GridPlaneFlip(sailGraphicData.Graphic.drawSize);

                        Graphics.DrawMesh(sailMesh, parent.DrawPos + sailDrawOffset, Quaternion.identity * GenMath.ToQuat(sailDirection * spinPosition + (360f / Props.sailCount) * sail), sailGraphicData.Graphic.GetColoredVersion(capGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(capDirection), 0);
                    }
                }
                // East/West cap direction
                else
                {
                    // Iterate through each of the sails
                    for (int sail = 0; sail < Props.sailCount; sail++)
                    {
                        Vector3 sailAxleDrawPos = parent.DrawPos + sailDrawOffset;

                        float sailSpinPositionRaw = spinPosition - (360f / Props.sailCount) * sail;
                        float sailSpinPositionClean = (sailSpinPositionRaw < 0) ? sailSpinPositionRaw + 360f : sailSpinPositionRaw;

                        float sailRenderLength = Props.sailGraphicData.drawSize.x * Mathf.Sin(sailSpinPositionClean * Mathf.PI / 180);

                        if (sailRenderLength < 0f)
                        {
                            sailRenderLength *= -1f;
                        }

                        float sailLayerDrawOffset = (sailSpinPositionClean <= 90f || sailSpinPositionClean >= 270f) ? 0.1f : -0.1f;

                        Vector3 sailRenderVector = new(sailRenderLength, sailLayerDrawOffset, Props.sailGraphicData.drawSize.y * 0.2f);

                        sailAxleDrawPos.y += sailLayerDrawOffset;

                        Matrix4x4 sailRenderMatrix = default;
                        sailRenderMatrix.SetTRS(sailAxleDrawPos, Quaternion.identity * GenMath.ToQuat((sailDirection == 1) ? 90f : 270f), sailRenderVector);

                        Graphics.DrawMesh(sailSpinPositionClean < 180f ? MeshPool.plane10 : MeshPool.plane10Flip, sailRenderMatrix, sailGraphicData.Graphic.GetColoredVersion(capGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(capDirection), 0);
                    }   
                }
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