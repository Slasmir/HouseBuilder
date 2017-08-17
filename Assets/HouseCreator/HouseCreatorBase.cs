using UnityEngine;
using System.Collections.Generic;

public class HouseCreatorBase : MonoBehaviour
{
    public enum PointType
    {
        Wall, HalfWall, Roof, HalfRoof, RoofEnd, HalfRoofEnd, Unsorted, None, Filler
    }
    private class GridPoint
    {
        public PointType Type;
        public Vector3 Location;
        public Vector3 Dir;
        public bool IsCorner = false;

        public bool IsHalf()
        {
            if (Type == PointType.HalfWall) return true;
            if (Type == PointType.HalfRoof) return true;
            if (Type == PointType.HalfRoofEnd) return true;

            return false;
        }

        public bool IsRoofType()
        {
            if (Type == PointType.HalfRoof) return true;
            if (Type == PointType.HalfRoofEnd) return true;
            if (Type == PointType.Roof) return true;
            if (Type == PointType.RoofEnd) return true;

            return false;
        }

        public GridPoint(PointType PT, Vector3 L)
        {
            Type = PT;
            Location = L;
            Dir = Vector3.zero;
        }

        public GridPoint GetNeighbor(Vector3 Dir, List<GridPoint> grid) {
            foreach (GridPoint p in grid)
            {
                if (p.Location == Location + Dir)
                {
                    return p;
                }
            }

            return null;
        }
        
        public PointType GetNeighborType(Vector3 Dir, List<GridPoint> grid)
        {
            foreach (GridPoint p in grid)
            {
                if (p.Location == Location + Dir)
                {
                    return p.Type;
                }
            }

            return PointType.None;

        }

        public bool IsPointValid()
        {
            if (Type == PointType.None)
                return false;
            if (Type == PointType.Filler)
                return false;

            return true;
        }

        public bool IsPointPlaceable()
        {
            if (Type != PointType.None)
                return false;
            if (Type != PointType.Filler)
                return false;
            if (Type != PointType.Unsorted)
                return false;

            return true;
        }

        public GameObject CreateGridObject(Transform parent, HouseCreatorCollection selectedCollection)
        {
            if (Type == PointType.Filler || Type == PointType.None)
                return null;

            GameObject GridObject = new GameObject("GridObject");

            MeshFilter mf = GridObject.AddComponent<MeshFilter>();
            MeshRenderer mr = GridObject.AddComponent<MeshRenderer>();

            GridObject.transform.position = Location + parent.position;
            GridObject.transform.SetParent(parent);
            GridObject.transform.localRotation = Quaternion.LookRotation(Dir);
            GridObject.transform.Rotate(new Vector3(0, -90f, 0));

            Mesh m = selectedCollection.GetMeshBasedOnPointType(Type);
            mf.mesh = m;

            mr.sharedMaterial = selectedCollection.DefaultMat;

            if (IsCorner && !IsRoofType())
            {
                GameObject cornerObject = CreateCornerObject(parent, selectedCollection, GridObject.transform);
                cornerObject.transform.SetParent(GridObject.transform);
            }

            return GridObject;
        }

        //TODO: Do this
        public GameObject CreateCornerObject(Transform parent, HouseCreatorCollection selectedCollection, Transform GridObject)
        {
            GameObject CornerObject = new GameObject("Corner Pillar");

            CornerObject.transform.position = Location + parent.position;
            CornerObject.transform.SetParent(GridObject, true);

            MeshFilter mf = CornerObject.AddComponent<MeshFilter>();
            MeshRenderer mr = CornerObject.AddComponent<MeshRenderer>();

            Mesh m = selectedCollection.GetRandomCorner();
            mf.mesh = m;

            mr.sharedMaterial = selectedCollection.DefaultMat;

            return CornerObject;
        }

        #region DrawDebug
        public void DrawDebug(Vector3 ParentPos)
        {

            DrawDebugType(ParentPos);
            if (IsCorner)
                DrawDirection(ParentPos);
        }

        void DrawDirection(Vector3 ParentPos)
        {
            if (Dir == Vector3.zero)
            {
                if (IsPointPlaceable())
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(ParentPos + Location, Vector3.one * 0.2f);
                }
            } else
            {
                Vector3 Size = Vector3.Scale(Vector3.one * 0.25f, Dir) + Vector3.one * 0.02f;
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(ParentPos + Location + Dir * 0.125f, Size);
            }
        }
        
        void DrawDebugType(Vector3 ParentPos)
        {
            if (Type == PointType.Wall)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(ParentPos + Location, .075f);
            }
            else if (Type == PointType.HalfWall)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(ParentPos + Location, .075f);
            }
            else if (Type == PointType.Roof)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(ParentPos + Location, .075f);
            }
            else if (Type == PointType.HalfRoof)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(ParentPos + Location, .075f);
            }
            else if (Type == PointType.Unsorted)
            {
                Gizmos.color = new Color(0, 0, 0, 0.5f);
                Gizmos.DrawSphere(ParentPos + Location, .075f);
            }
            else if (Type == PointType.None)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(ParentPos + Location, .075f);
            }
            else if(Type == PointType.Filler)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(ParentPos + Location, .075f);
            }
            else
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(ParentPos + Location, .075f);
            }

            if(IsCorner == true)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(ParentPos + Location, new Vector3(.05f, .05f, .05f));
            }
        }
        #endregion
    }

    [SerializeField]
    private List<GridPoint> Grid;
    private HouseCreatorCollection HouseCollection;

    private float FloorHeight = 1f;
    

    public void GenerateHouse(HouseCreatorCollection collection)
    {
        HouseCollection = collection;
        CreateGrid();
        SpawnArt();
    }

    private void CreateGrid()
    {
        InstantiateGridPoints();
        RemoveUnsusedPoints();
        SortGridPoints();
        FindCorners();
    }

    private void InstantiateGridPoints() {
        Mesh m = this.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] Verts = m.vertices;

        //Creates the grid by rounding off values.
        for (int v = 0; v < Verts.Length; v++)
        {
            Vector3 fracPos = new Vector3(transform.position.x % 1, transform.position.y % 1, transform.position.z % 1);
            Vector3 tempVert = Verts[v] - fracPos;
            Verts[v] = (new Vector3(Mathf.Round(tempVert.x * 2f) / 2f, Mathf.Round(tempVert.y), Mathf.Round(tempVert.z * 2f) / 2f) - transform.position + fracPos);
        }

        m.vertices = Verts;
        

        List<Vector3> usedPoints = new List<Vector3>();
        Grid = new List<GridPoint>();
        foreach(Vector3 vert in Verts)
        {
            if (usedPoints.Contains(vert)) continue;

            Grid.Add(new GridPoint(PointType.Unsorted, vert));
            usedPoints.Add(vert);
        }

    }

    void RemoveUnsusedPoints()
    {
        List<GridPoint> PointsToRemove = new List<GridPoint>();
        foreach (GridPoint gp in Grid)
        {
            //Directs
            if (gp.GetNeighborType(new Vector3(.5f, 0, 0), Grid) == PointType.None)
                continue;
            if (gp.GetNeighborType(new Vector3(-.5f, 0, 0), Grid) == PointType.None)
                continue;
            if (gp.GetNeighborType(new Vector3(0, 0, .5f), Grid) == PointType.None)
                continue;
            if (gp.GetNeighborType(new Vector3(0, 0, -.5f), Grid) == PointType.None)
                continue;

            //corners
            if (gp.GetNeighborType(new Vector3(.5f, 0, .5f), Grid) == PointType.None)
                continue;
            if (gp.GetNeighborType(new Vector3(-.5f, 0, -.5f), Grid) == PointType.None)
                continue;
            if (gp.GetNeighborType(new Vector3(.5f, 0, -.5f), Grid) == PointType.None)
                continue;
            if (gp.GetNeighborType(new Vector3(-.5f, 0, .5f), Grid) == PointType.None)
                continue;

            PointsToRemove.Add(gp);
        }

        foreach (GridPoint RemovePoint in PointsToRemove)
        {
            RemovePoint.Type = PointType.Filler;
            //Grid.Remove(RemovePoint);
        }
    }

    void SortGridPoints() {
        for (int i = 0; i < 4; i++)
        {
            SortGridPointsPrElevation();
        }
    }

    void SortGridPointsPrElevation()
    {
        //Finds starting grid point
        GridPoint CurrentGridPoint = FindCornerPoint(Grid,true);

        Vector3[] DirectionArray = new Vector3[4] {
            new Vector3(1f, 0, 0),
             new Vector3(0, 0, 1f),
            new Vector3(-1f, 0, 0),     
            new Vector3(0, 0, -1f)
        };

        GridPoint NextGridPoint = CurrentGridPoint;

        List<GridPoint> SelectedElevation = FindAllPointsOfSameElevation(CurrentGridPoint, Grid);
        List<GridPoint> RunThrughList = CloneList(SelectedElevation);

        bool IsBreaking = false;
        while (RunThrughList.Count > 0)
        {
            IsBreaking = false;
            if (CurrentGridPoint.Type == PointType.Filler) { Debug.Log("FIlle is chosen"); break; }
            if (!CurrentGridPoint.IsPointPlaceable())
            {
                for (int i = 0; i < 4; i++)
                {
                    GridPoint FullNeighbor = CurrentGridPoint.GetNeighbor(DirectionArray[i], SelectedElevation);
                    GridPoint HalfNeighbor = CurrentGridPoint.GetNeighbor(DirectionArray[i] * 0.5f, SelectedElevation);
                    if (FullNeighbor != null && FullNeighbor.IsPointValid() && FullNeighbor.Type == PointType.Unsorted)
                    {
                        if (HalfNeighbor != null && HalfNeighbor.Type != PointType.Filler)
                        {
                            HalfNeighbor.Type = PointType.None;
                            RunThrughList.Remove(HalfNeighbor);

                            if (CurrentGridPoint.GetNeighbor(new Vector3(0, 1f, 0), Grid) != null)
                                CurrentGridPoint.Type = PointType.Wall;
                            else
                                CurrentGridPoint.Type = PointType.Roof;
                            

                            CurrentGridPoint.Dir = DirectionArray[i];
                            NextGridPoint = FullNeighbor;
                            IsBreaking = true;
                            break;
                        }
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    GridPoint FullNeighbor = CurrentGridPoint.GetNeighbor(DirectionArray[i], SelectedElevation);
                    GridPoint HalfNeighbor = CurrentGridPoint.GetNeighbor(DirectionArray[i] * 0.5f, SelectedElevation);
                    if (FullNeighbor != null && FullNeighbor.IsPointValid())
                    {
                        float FNdotGP = Vector3.Dot(DirectionArray[i], FullNeighbor.Dir);
                        if (HalfNeighbor != null && HalfNeighbor.Type != PointType.Filler && FNdotGP != -1)
                        {
                            HalfNeighbor.Type = PointType.None;
                            RunThrughList.Remove(HalfNeighbor);

                            if (CurrentGridPoint.GetNeighbor(new Vector3(0, 1f, 0), Grid) != null)
                                CurrentGridPoint.Type = PointType.Wall;
                            else
                                CurrentGridPoint.Type = PointType.Roof;
                            

                            CurrentGridPoint.Dir = DirectionArray[i];
                            NextGridPoint = FullNeighbor;
                            IsBreaking = true;
                            break;
                        }
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (IsBreaking) break;

                    GridPoint HalfNeighbor = CurrentGridPoint.GetNeighbor(DirectionArray[i] * 0.5f, SelectedElevation);
                    if (HalfNeighbor != null && HalfNeighbor.IsPointValid())
                    {
                        if (CurrentGridPoint.GetNeighbor(new Vector3(0, 1f, 0), Grid) != null)
                            CurrentGridPoint.Type = PointType.HalfWall;
                        else
                            CurrentGridPoint.Type = PointType.HalfRoof;
                        
                        CurrentGridPoint.Dir = DirectionArray[i];
                        NextGridPoint = HalfNeighbor;
                        break;
                    }
                }

            }

            RunThrughList.Remove(CurrentGridPoint);
            if (NextGridPoint != null)
            {
                if (NextGridPoint.Type == PointType.Unsorted)
                {
                    CurrentGridPoint = NextGridPoint;
                    continue;
                }
            }
            if (RunThrughList.Count != 0)
                CurrentGridPoint = RunThrughList[0];

        }
    }

    void FindCorners()
    {
        foreach(GridPoint gp in Grid)
        {
            Vector3 dir = gp.Dir;
            if (dir == Vector3.zero) continue;

            if (gp.IsHalf())
            {
                dir *= .5f;
            }

            GridPoint g = gp.GetNeighbor(dir, Grid);
            if(g != null)
            {
                if (g.Dir != gp.Dir && g.Dir != gp.Dir * -1f && g.IsPointValid())
                    g.IsCorner = true;
            }
        }
    }

    GridPoint FindCornerPoint(List<GridPoint> grid, bool OnlyLookInUnsorted)
    {
        GridPoint CurrentGridPoint = grid[0];
        float CurrentLowDistance = Mathf.Infinity;
        foreach (GridPoint gp in grid)
        {
            if (gp.Type == PointType.Filler) continue;
            if (OnlyLookInUnsorted && gp.Type != PointType.Unsorted) continue;

            float Distance = Vector3.Distance(gp.Location + transform.position, transform.position);
            if (Distance < CurrentLowDistance)
            {
                CurrentGridPoint = gp;
                CurrentLowDistance = Distance;
            }

            if (gp.Location.y < CurrentGridPoint.Location.y)
            {
                CurrentGridPoint = gp;
                CurrentLowDistance = Distance;
            }
        }

        return CurrentGridPoint;
    }

    List<GridPoint> FindAllPointsOfSameElevation(GridPoint ParentPoint, List<GridPoint> grid)
    {
        List<GridPoint> SelectedElevation = new List<GridPoint>();
        SelectedElevation.Add(ParentPoint);
        foreach (GridPoint gp in grid)
        {
            if (gp == ParentPoint) continue;
            if (gp.Type == PointType.Filler) continue;

            if (gp.Location.y == ParentPoint.Location.y)
            {
                SelectedElevation.Add(gp);
                //gp.Type = PointType.Wall;
            }
        }

        return SelectedElevation;
    }

    private void SpawnArt()
    {
        foreach (GridPoint gp in Grid)
        {
            gp.CreateGridObject(transform, HouseCollection);
        }
    }

    List<GridPoint> CloneList(List<GridPoint> list)
    {
        List<GridPoint> ClonedList = new List<GridPoint>();
        foreach (GridPoint gp in list)
        {
            ClonedList.Add(gp);
        }

        return ClonedList;
    }

    //Debug;
    void OnDrawGizmosSelected()
    {
        foreach (GridPoint p in Grid)
        {
            p.DrawDebug(transform.position);
        }
    }
}
