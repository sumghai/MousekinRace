using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Designator_FertilizeSoil : Designator_Cells
    {
        public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Plants;

        public ThingDef fertilizerDef = MousekinDefOf.Mousekin_Saltpeter;

        public float SelFertilizerBonusOffset => fertilizerDef.GetModExtension<FertilizerExtension>().fertilityBonus;

        public override string Label => "MousekinRace_DesignatorFertilizeSoil".Translate();

        public override string Desc => defaultDesc = "MousekinRace_DesignatorFertilizeSoilDesc".Translate(fertilizerDef.label, SelFertilizerBonusOffset.ToStringPercent());

        public override DesignationDef Designation => MousekinDefOf.Mousekin_FertilizeSoil;

        public Designator_FertilizeSoil() 
        {
            icon = fertilizerDef.uiIcon;
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_HarvestPlants;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map) || loc.Fogged(Map))
            {
                return false;
            }
            if (loc.GetZone(Map) is not Zone_Growing)
            {
                return "MousekinRace_DesignatorFertilizeSoil_ErrorMsg_MustDesignateGrowingZones".Translate();
            }
            if (Map.GetComponent<MapComponent_Fertilizer>().cellFertilityBonus.ContainsKey(loc))
            {
                return "MousekinRace_DesignatorFertilizeSoil_ErrorMsg_CannotFertilizeAlreadyFertilizedCell".Translate();
            }
            return AcceptanceReport.WasAccepted;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            Map.designationManager.TryRemoveDesignation(loc, Designation);
            Map.designationManager.AddDesignation(new(loc, Designation));
        }

        public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
        {
            Widgets.DefIcon(rect, fertilizerDef, null, 0.85f);
        }

        public override void DrawMouseAttachments()
        {
            base.DrawMouseAttachments();
            if (Find.DesignatorManager.Dragger.Dragging)
            {
                Vector2 vector = Event.current.mousePosition + Designator_Place.PlaceMouseAttachmentDrawOffset;
                if (useMouseIcon)
                {
                    vector.y += Text.LineHeight * 2f;
                }
                Widgets.ThingIcon(new Rect(vector.x, vector.y, 27f, 27f), fertilizerDef);
                int num = NumHighlightedCells();
                string text = num.ToStringCached();
                if (Map.resourceCounter.GetCount(fertilizerDef) < num)
                {
                    GUI.color = Color.red;
                    text += " (" + "NotEnoughStoredLower".Translate() + ")";
                }
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(new Rect(vector.x + 29f, vector.y, 999f, 29f), text);
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
            }
        }

        public int NumHighlightedCells()
        {
            return Find.DesignatorManager.Dragger.DragCells.Count;
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }

        public override void RenderHighlight(List<IntVec3> dragCells)
        {
            DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells);
        }
    }
}
