using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class WeaponRenderingExtension : DefModExtension
    {
        public DraftedDrawOffsets draftedDrawOffsets = new();
    }

    public class DraftedDrawOffsets
    {
        public Offset north = new();
        public Offset east = new();
        public Offset south = new();
        public Offset west = new();
    }

    public struct Offset
    {
        public Vector3 posOffset;
        public float angOffset;
    }
}
