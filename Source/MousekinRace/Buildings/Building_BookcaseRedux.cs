using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace MousekinRace
{
    public class Building_BookcaseRedux : Building_Bookcase
    {
        public Graphic bookshelfGraphicSouthInt;

        public Graphic BookshelfGraphicSouth => bookshelfGraphicSouthInt ??= def.GetModExtension<BookcaseRenderExtension>().bookshelfGraphicSouth.GraphicColoredFor(this);
        
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            // Call the grandparent ThingWithComps.DrawAt() method instead of base.DrawAt()
            // (which otherwise references Building_Bookcase.DrawAt())
            IntPtr ptr = AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.DrawAt)).MethodHandle.GetFunctionPointer();
            var baseDrawAt = (Action<Vector3, bool>)Activator.CreateInstance(typeof(Action<Vector3, bool>), this, ptr);
            baseDrawAt(drawLoc, flip);

            // Fetch custom offsets from def extension
            BookcaseRenderExtension ext = def.GetModExtension<BookcaseRenderExtension>();
            Vector3[] bookshelfRotOffsets =
            [
                ext.bookshelfRotOffsets.offsetNorth,
                ext.bookshelfRotOffsets.offsetEast,
                ext.bookshelfRotOffsets.offsetSouth,
                ext.bookshelfRotOffsets.offsetWest,
            ];

            // Draw books on the shelves
            int MaxBooksPerShelf = MaximumBooks / 2;

            Rot4 rot = Rotation.Rotated(RotationDirection.Counterclockwise);
            float num = ((Rotation == Rot4.North || Rotation == Rot4.South) ? BookWidthNorthSouth : BookWidthEastWest);
            Vector3 vector = rot.FacingCell.ToVector3() * num;
            //vector.y *= 0.5f;
            Vector3 vector2 = rot.FacingCell.ToVector3() * ((float)(-MaxBooksPerShelf) * num * 0.5f);
            Vector3 vector3 = bookshelfRotOffsets[Rotation.AsInt];
            for (int i = 0; i < HeldBooks.Count; i++)
            {
                // Skip drawing books on lower shelf if bookcase is not facing south
                if (Rotation != Rot4.South && i > (MaxBooksPerShelf - 1)) 
                {
                    continue;
                }
                
                Book book = HeldBooks[i];
                Rot4 opposite = Rotation.Opposite;
                if (opposite == Rot4.East || opposite == Rot4.West)
                {
                    opposite = opposite.Opposite;
                }

                // Special handling when bookcase faces west (3?)
                Vector3 westRotLayerOffset = (Rotation == Rot4.West) ? ext.bookWestRotDecrementOffset * (i - 1) + (((MaxBooksPerShelf - i < 3) ? ext.bookWestRotFalloffOffset : Vector3.zero)) : Vector3.zero;

                int increment = i < MaxBooksPerShelf ? i : i - MaxBooksPerShelf;
                Vector3 southRotBottomRowOffset = (i < MaxBooksPerShelf) ? Vector3.zero : ext.bookSouthRotSecondRowOffset;
                Vector3 loc = drawLoc + Graphic.DrawOffset(Rotation) + vector2 + vector3 + (vector * increment) + westRotLayerOffset + southRotBottomRowOffset;
                book.VerticalGraphic.Draw(loc, opposite, this);
            }

            // Draw additional bookshelf if bookcase is facing south
            if (Rotation == Rot4.South)
            { 
                BookshelfGraphicSouth.Draw(drawLoc + ext.bookshelfDrawOffsetSouth, Rot4.North, this);
            }

            // Draw bookcase ends to cover exposed books
            if (Rotation != Rot4.South)
            {
                if (Rotation != Rot4.North && def.building.bookendGraphicEast != null)
                {
                    var bookendMesh = Rotation == Rot4.West ? MeshPool.GridPlaneFlip(def.graphicData.drawSize.Rotated()) : MeshPool.GridPlane(def.graphicData.drawSize.Rotated());
                    Vector3 bookendDrawLoc = drawLoc + Graphic.DrawOffset(Rotation) + ext.bookendDrawOffsetEast;
                    BookendGraphicEast.DrawMeshInt(bookendMesh, bookendDrawLoc, Quaternion.identity, BookendGraphicEast.MatAt(Rot4.North));

                }
                else if (Rotation == Rot4.North && def.building.bookendGraphicNorth != null)
                {
                    BookendGraphicNorth.Draw(drawLoc + Graphic.DrawOffset(Rotation) + ext.bookendDrawOffsetNorth, Rot4.North, this);
                }
            }
        }
    }
}
