using UnityEngine;
using Verse;

namespace MousekinRace
{
    [StaticConstructorOnStartup]
    public class CompMineEntranceAnim : ThingComp
    {
        public CompProperties_MineEntranceAnim Props => (CompProperties_MineEntranceAnim)props;

        public float treadwheelPosition = 0f;

        public bool IsWorking => (parent as Building_MineEntrance).IsWorking();

        public float treadwheelDirection = 1f;

        public int ticksToChangeTreadwheelDirection = 0;

        public float TreadwheelSpeed => IsWorking ? Props.wheelSpeed * treadwheelDirection : 0f;

        public override void CompTick()
        {
            base.CompTick();

            if (treadwheelPosition >= 360f)
            {
                treadwheelPosition = 0f;
            }
            else if (treadwheelPosition < 0f)
            {
                treadwheelPosition = 359f;
            }
            else
            {
                treadwheelPosition += TreadwheelSpeed;
            }

            if (ticksToChangeTreadwheelDirection <= 0)
            {
                ticksToChangeTreadwheelDirection = Rand.Range(Props.ticksToChangeWheelDirection.min, Props.ticksToChangeWheelDirection.max);
                treadwheelDirection *= -1f;
            }
            else 
            {
                ticksToChangeTreadwheelDirection--;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            GraphicData towerGraphicData = Props.towerGraphicData;
            GraphicData workGlowGraphicData = Props.workGlowGraphicData;
            GraphicData wheelSegmentGraphicData = Props.wheelSegmentGraphicData;
            Vector3 wheelPairOffsetVector = new(Props.wheelPairOffset, 0);
            float wheelDiameter = Props.wheelDiameter;
            int wheelSegmentsPerWheel = Props.wheelSegmentsPerWheel;

            if (workGlowGraphicData != null && IsWorking)
            { 
                Mesh workGlowMesh = Props.workGlowGraphicData.Graphic.MeshAt(parent.Rotation);
                Graphics.DrawMesh(workGlowMesh, parent.DrawPos + workGlowGraphicData.drawOffset, Quaternion.identity, FadedMaterialPool.FadedVersionOf(workGlowGraphicData.Graphic.MatAt(parent.Rotation, null), 1), 0);
            }

            if (towerGraphicData != null) 
            {
                Mesh towerMesh = Props.towerGraphicData.Graphic.MeshAt(parent.Rotation);
                Graphics.DrawMesh(towerMesh, parent.DrawPos + towerGraphicData.drawOffset, Quaternion.identity, towerGraphicData.Graphic.GetColoredVersion(towerGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(parent.Rotation), 0);
            }

            if (wheelSegmentGraphicData != null)
            {
                Mesh wheelSegmentMesh = Props.wheelSegmentGraphicData.Graphic.MeshAt(parent.Rotation);

                for (int segment = 0; segment < wheelSegmentsPerWheel; segment++)
                {
                    float treadwheelSpinPositionRaw = treadwheelPosition - (360f / wheelSegmentsPerWheel) * segment;
                    float treadwheelSpinPositionClean = (treadwheelSpinPositionRaw < 0) ? treadwheelSpinPositionRaw + 360f : treadwheelSpinPositionRaw;

                    float treadwheelSegmentDrawPosZ = (wheelDiameter / 2f) * Mathf.Sin(treadwheelSpinPositionClean * Mathf.PI / 180f);

                    float treadwheelSegmentHeight = Mathf.Cos(treadwheelSpinPositionClean * Mathf.PI / 180f);

                    if (treadwheelSegmentHeight < 0f)
                    {
                        treadwheelSegmentHeight *= -1f;
                    }

                    float treadwheelLayerDrawOffset = (treadwheelSpinPositionClean <= 90f || treadwheelSpinPositionClean >= 270f) ? 1f : -1f;

                    Vector3 foobar = new(0, treadwheelLayerDrawOffset, treadwheelSegmentDrawPosZ);

                    Vector3 treadwheelRenderVector = new(wheelSegmentGraphicData.drawSize.x, 1, treadwheelSegmentHeight);

                    Matrix4x4 treadwheelRenderMatrix1 = default;
                    treadwheelRenderMatrix1.SetTRS(parent.DrawPos + wheelSegmentGraphicData.drawOffset + wheelPairOffsetVector + foobar, Quaternion.identity, treadwheelRenderVector);
                    Matrix4x4 treadwheelRenderMatrix2 = default;
                    treadwheelRenderMatrix2.SetTRS(parent.DrawPos + wheelSegmentGraphicData.drawOffset - wheelPairOffsetVector + foobar, Quaternion.identity, treadwheelRenderVector);

                    Graphics.DrawMesh(wheelSegmentMesh, treadwheelRenderMatrix1, wheelSegmentGraphicData.Graphic.GetColoredVersion(wheelSegmentGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(parent.Rotation), 0);
                    Graphics.DrawMesh(wheelSegmentMesh, treadwheelRenderMatrix2, wheelSegmentGraphicData.Graphic.GetColoredVersion(wheelSegmentGraphicData.Graphic.Shader, parent.DrawColor, parent.DrawColorTwo).MatAt(parent.Rotation), 0);
                }                    
            }
        }
    }
}