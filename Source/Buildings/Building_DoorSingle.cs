using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Building_DoorSingle : Building_Door
    {
        public bool flipped;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref flipped, "flipped", false);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }

            // Extra gizmo to flip door graphics and open/close direction
            yield return new Command_Action
            {
                defaultLabel = "MousekinRace_CommandToggleFlipDoorLabel".Translate(),
                defaultDesc = "MousekinRace_CommandToggleFlipDoorDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Flip", true),
                action = delegate
                {
                    flipped = !flipped;
                }
            };
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            // Single door drawing code modified from Vanilla Furniture Expanded - Architect 

            DoorPreDraw();

            var d = 0f + 0.9f * OpenPct;
            var vector = new Vector3(0f, 0f, -1f);
            var mesh = flipped ? MeshPool.plane10Flip: MeshPool.plane10;
            var rotation = Rotation;
            rotation.Rotate(flipped ? RotationDirection.Counterclockwise : RotationDirection.Clockwise);
            vector = rotation.AsQuat * vector;
            var vector2 = DrawPos;
            vector2.y = AltitudeLayer.DoorMoveable.AltitudeFor();
            vector2 += vector * d;
            Graphics.DrawMesh(mesh, vector2, Rotation.AsQuat, Graphic.MatAt(Rotation), 0);
            Graphic.ShadowGraphic?.DrawWorker(vector2, Rotation, def, this, 0f);

            Comps_PostDraw();
        }
    }
}
