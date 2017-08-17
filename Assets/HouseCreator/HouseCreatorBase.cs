using UnityEngine;
using System.Collections.Generic;

public class HouseCreatorBase : MonoBehaviour
{
    public enum PointType
    {
        Wall, HalfWall, Roof, HalfRoof, RoofEnd, HalfRoofEnd, Unsorted, None, Filler
    }

    Vector3[] DirectionArray = new Vector3[4] {
        new Vector3(1f, 0, 0),
        new Vector3(0, 0, 1f),
        new Vector3(-1f, 0, 0),
        new Vector3(0, 0, -1f)
    };

    [SerializeField]
    private List<GridPoint> Grid;

    [SerializeField]
    private List<RoofPlanes> roofPlanes;
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
        RecheckRoofCorners();
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

        GridPoint NextGridPoint = CurrentGridPoint;

        List<GridPoint> SelectedElevation = FindAllPointsOfSameElevation(CurrentGridPoint, Grid);
        List<GridPoint> RunThrughList = CloneList(SelectedElevation);

        bool IsBreaking = false;
        while (RunThrughList.Count > 0)
        {
            IsBreaking = false;
            if (CurrentGridPoint.Type == PointType.Filler) { Debug.Log("FIlle is chosen"); break; }

            //Looks thrugh Unsorted to find neighbor
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

                //checks thrugh fulls to find neighthbor
                for (int i = 0; i < 4; i++)
                {
                    if (IsBreaking) break;

                    GridPoint FullNeighbor = CurrentGridPoint.GetNeighbor(DirectionArray[i], SelectedElevation);
                    GridPoint HalfNeighbor = CurrentGridPoint.GetNeighbor(DirectionArray[i] * 0.5f, SelectedElevation);
                    if (FullNeighbor != null && FullNeighbor.IsPointValid())
                    {
                        if (HalfNeighbor != null && HalfNeighbor.Type != PointType.Filler && FullNeighbor.Dir * -1 != DirectionArray[i])
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

                //looks thrugh halfs to find neighbor
                for (int i = 0; i < 4; i++)
                {
                    if (IsBreaking) break;

                    GridPoint HalfNeighbor = CurrentGridPoint.GetNeighbor(DirectionArray[i] * 0.5f, SelectedElevation);
                    if (HalfNeighbor != null && HalfNeighbor.IsPointValid() && HalfNeighbor.Dir * -1 != DirectionArray[i])
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

    void RecheckRoofCorners()
    {
        List<GridPoint> newPoints = new List<GridPoint>();
        foreach (GridPoint gp in Grid)
        {
            if (!gp.IsPointValid())
                continue;
            if (gp.IsRoofType())
                continue;

            GridPoint NeighboorPoint = gp.GetNeighbor(gp.Dir, Grid, true);
            if(NeighboorPoint != null)
            {
                if (NeighboorPoint.IsRoofType())
                {
                    GridPoint newGP = new GridPoint(PointType.Wall, gp.Location); ;
                    newPoints.Add(newGP);

                    if (gp.IsHalf())
                    {
                        gp.Type = PointType.HalfRoof;
                    } else {
                        gp.Type = PointType.Roof;
                    }

                    for (int i = 0; i < DirectionArray.Length; i++)
                    {
                        GridPoint g = newGP.GetNeighbor(DirectionArray[i], Grid, false);
                        if(g != null)
                        {
                            if(g.IsPointValid() && !g.IsRoofType())
                            {
                                if(DirectionArray[i] * -1f != g.Dir)
                                {
                                    newGP.Dir = DirectionArray[i];
                                    newGP.IsCorner = true;
                                    g.IsCorner = true;
                                }

                            }
                        }
                    }
                   
                }
            }
        }

        foreach(GridPoint gp in newPoints)
        {
            Grid.Add(gp);
        }
    }

    void GenerateRoofPlanes() {
        
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
