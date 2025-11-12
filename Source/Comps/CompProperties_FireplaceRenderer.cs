using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class CompProperties_FireplaceRenderer : CompProperties_FireOverlay
    {
        public List<FiresWithOffsets> fires = [];
        
        public GraphicData fireboxGraphicData = null;

        public GraphicData glowGraphicData = null;

        public CompProperties_FireplaceRenderer()
        {
            compClass = typeof(CompFireplaceRenderer);
        }
    }

    public class FiresWithOffsets
    {
        public float fireSize = 1f;

        public Vector3 offset;
    }
}
