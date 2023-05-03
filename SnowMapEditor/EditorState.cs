using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowMapEditor
{
    class EditorState
    {
        public RealPoint CameraPosition = RealPoint.Zero;
        public ColliderWorks.SearchResult drag = null;
        public Editable SelectedItem = null;
        public IntPoint CursorMarker;

        public void Update(MapData mapData, double deltaTime)
        {
            Meta.panelData.Comment = null;

            CameraPosition += Meta.panelData.Speed * (RealPoint)Meta.InputController.MapAxis.Read();

            Meta.panelData.MapData = mapData.Save();

            var cursor = (RealPoint)Meta.InputController.CursorPosition;
            var fixResult = ColliderWorks.Fixate(mapData.Data, cursor);
            CursorMarker = fixResult.position.Round();
            Meta.panelData.CursorPosition = "X: " + CursorMarker.X + "Y: " + CursorMarker.Y;

            if (drag == null)
            {
                if (Meta.InputController.LeftClick.WasPressed())
                {
                    if(SelectedItem != fixResult.owner)
                    {
                        if (fixResult.owner != null)
                        {
                            Meta.panelData.Comment = fixResult.owner.Comment;
                        }
                    }
                    SelectedItem = fixResult.owner;
                    if (SelectedItem != null)
                    {
                        if (!fixResult.exists)
                        {
                            SelectedItem.Polyline.Insert(fixResult.index, fixResult.position);
                            drag = fixResult;
                            drag.exists = true;
                            return;
                        }
                        if (fixResult.exists)
                        {
                            if (!(SelectedItem is Singular))
                            {
                                if (SelectedItem.Polyline.Count == 1 || (Meta.InputController.InventoryButton.IsDown() && (fixResult.index == 0 || fixResult.index == SelectedItem.Polyline.Count - 1)))
                                {
                                    SelectedItem.Polyline.Insert(fixResult.index, fixResult.position);
                                    drag = fixResult;
                                    if (fixResult.index != 0)
                                    {
                                        drag.index++;
                                    }
                                    drag.exists = true;
                                    return;
                                }
                            }
                            drag = fixResult;
                            return;
                        }
                        return;
                    }
                    if(SelectedItem == null)
                    {
                        if (Meta.panelData.CreatingType == 0)
                        {
                            SelectedItem = new Fence(new List<RealPoint>() { fixResult.position });
                        }
                        if (Meta.panelData.CreatingType == 1)
                        {
                            SelectedItem = new Trail(new List<RealPoint>() { fixResult.position });
                        }
                        if (Meta.panelData.CreatingType == 2)
                        {
                            SelectedItem = new Road(new List<RealPoint>() { fixResult.position });
                        }
                        if (Meta.panelData.CreatingType == 3)
                        {
                            SelectedItem = new Singular(new List<RealPoint>() { fixResult.position });
                        }
                        if (Meta.panelData.CreatingType == 4)
                        {
                            SelectedItem = new Wall(new List<RealPoint>() { fixResult.position });
                        }
                        Meta.panelData.Comment = SelectedItem.Comment;
                        mapData.Data.Add(SelectedItem);
                        return;
                    }
                    return;
                }

                if(Meta.InputController.RightClick.WasPressed())
                {
                    var SelectedItem = fixResult.owner;
                    if (SelectedItem != null)
                    {
                        if (fixResult.exists)
                        {
                            SelectedItem.Polyline.RemoveAt(fixResult.index);
                            if(SelectedItem.Polyline.Count == 0)
                            {
                                mapData.Data.Remove(SelectedItem);
                            }
                            return;
                        }
                    }
                }
            }
            else
            {
                drag.owner.Polyline.RemoveAt(drag.index);
                drag.owner.Polyline.Insert(drag.index, cursor);
                drag.position = cursor;
                CursorMarker = cursor.Round();

                if(Meta.InputController.LeftClick.WasReleased())
                {
                    drag = null;
                }
            }
        }
    }
}
