using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompCellarOutdoor : ThingComp
    {
        public CompProperties_CellarOutdoor Props => (CompProperties_CellarOutdoor)props;

        public override void PostDraw()
        {
            base.PostDraw();

            GraphicData exposedWallGraphicData = Props.exposedWallGraphicData;

            if (exposedWallGraphicData != null)
            {
                Mesh exposedWallMesh = Props.exposedWallGraphicData.Graphic.MeshAt(parent.Rotation);
                Vector3 exposedWallDrawPos = parent.DrawPos + exposedWallGraphicData.drawOffset;
                exposedWallDrawPos.y = AltitudeLayer.BuildingBelowTop.AltitudeFor() + exposedWallGraphicData.drawOffset.y;
                Graphics.DrawMesh(exposedWallMesh, exposedWallDrawPos, Quaternion.identity, exposedWallGraphicData.Graphic.GetColoredVersion(exposedWallGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(parent.Rotation), 0);
            }
        }
    }
}