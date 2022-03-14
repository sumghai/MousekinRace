using HarmonyLib;
using UnityEngine;
using Verse;

namespace MousekinRace.Patches
{
    // Offset the position and rotation of weapons on drafted pawns, if custom offset data is provided for the weapon
	[HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.DrawEquipmentAiming))]
    public static class Harmony_PawnRenderer_DrawEquipmentAiming
    {
		public static void Prefix(PawnRenderer __instance, Thing eq, ref Vector3 drawLoc, ref float aimAngle)
		{
			Pawn pawn = __instance.pawn;
			WeaponRenderingExtension weaponRenderExt = eq.def.GetModExtension<WeaponRenderingExtension>();

			if (__instance.CarryWeaponOpenly() && !pawn.stances.curStance.StanceBusy && weaponRenderExt != null)
			{
				if (pawn.Rotation == Rot4.South)
				{
					drawLoc -= new Vector3(0f, 0f, -0.22f) - weaponRenderExt.draftedDrawOffsets.south.posOffset;
					aimAngle = weaponRenderExt.draftedDrawOffsets.south.angOffset;
				}
				else if (pawn.Rotation == Rot4.North)
				{
					drawLoc -= new Vector3(0f, 0f, -0.11f) - weaponRenderExt.draftedDrawOffsets.north.posOffset;
					aimAngle = weaponRenderExt.draftedDrawOffsets.north.angOffset;
				}
				else if (pawn.Rotation == Rot4.East)
				{
					drawLoc -= new Vector3(0.2f, 0f, -0.22f) - weaponRenderExt.draftedDrawOffsets.east.posOffset;
					aimAngle = weaponRenderExt.draftedDrawOffsets.east.angOffset;
				}
				else if (pawn.Rotation == Rot4.West)
				{
					drawLoc -= new Vector3(-0.2f, 0f, -0.22f) - weaponRenderExt.draftedDrawOffsets.west.posOffset;
					aimAngle = weaponRenderExt.draftedDrawOffsets.west.angOffset;
				}
			}
		}
	}
}
