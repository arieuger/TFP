using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    
    private readonly Vector2 _upVector = new Vector2(.5f, .25f);
    private readonly Vector2 _doVector = new Vector2(-.5f, -.25f);
    private readonly Vector2 _leVector = new Vector2(-.5f, .25f);
    private readonly Vector2 _riVector = new Vector2(.5f, -.25f);

    private float _g; // distancia desde punto inicial
    private float _h; // distancia desde punto final
    private float F => _g + _h; // suma
    
     
    
    private static PathFinding _instance;
    public static PathFinding Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
    }
    
    public List<PathFindPosition> FindPath(GameObject source, GameObject target)
    {
        List<PathFindPosition> openList = new List<PathFindPosition>();
        List<PathFindPosition> closedList = new List<PathFindPosition>();

        Vector2 sourcePos = source.transform.position;
        Vector2 targetPos = target.transform.position;
        PathFindPosition startPosition = new PathFindPosition(sourcePos, 0, GetManhattenDistance(targetPos, sourcePos));
        PathFindPosition endPosition = new PathFindPosition(targetPos, GetManhattenDistance(sourcePos, targetPos), 0);
       
        openList.Add(startPosition);

        startPosition.Position = new Vector2(Mathf.Round(startPosition.Position.x * 100f) / 100f, Mathf.Round(startPosition.Position.y * 100f) / 100f);
        endPosition.Position = new Vector2(Mathf.Round(endPosition.Position.x * 100f) / 100f, Mathf.Round(endPosition.Position.y * 100f) / 100f);
        
        while (openList.Count > 0)
        {
            PathFindPosition current = openList.OrderBy(x => x.F).First();
            
            openList.Remove(current);
            closedList.Add(current);

            if (current.Position.Equals(endPosition.Position))
            {
                return GetFinishedList(startPosition, current);
            }
            
            GetNeighbours(current.Position).ForEach(tilePos =>
            {
                PathFindPosition tile = new PathFindPosition(tilePos,
                    GetManhattenDistance(startPosition.Position, tilePos),
                    GetManhattenDistance(endPosition.Position, tilePos));

                if (!closedList.ConvertAll(p => p.Position).Contains(tilePos))
                {
                    tile.Previous = current;

                    if (!openList.ConvertAll(p => p.Position).Contains(tilePos))
                    {
                        openList.Add(tile);
                    }
                }
            });
        }

        return new List<PathFindPosition>();
    }

    private List<PathFindPosition> GetFinishedList(PathFindPosition start, PathFindPosition end)
    {
        List<PathFindPosition> finishedList = new List<PathFindPosition>();
        PathFindPosition current = end;
        
        while (current.Position != start.Position)
        {
            finishedList.Add(current);
            current = current.Previous;
        }

        finishedList.Reverse();

        return finishedList;
    }

    public List<GameObject> FindGameObjectByPositions(List<Vector2> positions)
    {
        return TileManager.Instance.ground.GetComponentsInChildren<Transform>().ToList()
            .FindAll(g => g.GetComponent<GroundTileBehaviour>() != null)
            .FindAll(g => positions.Contains(g.position))
            .ConvertAll(t => t.gameObject);
        
    }

    private List<Vector2> GetNeighbours(Vector2 center)
    {
        var neighbours = new List<Vector2>();
        neighbours.Add(center + _upVector);
        neighbours.Add(center + _doVector);
        neighbours.Add(center + _leVector);
        neighbours.Add(center + _riVector);
        return neighbours;
    }
    
    
    private float GetManhattenDistance(Vector2 start, Vector2 tile)
    {
        // Convert 2D isometric coordinates to 3D
        Vector3 a = new Vector3(start.x, start.y * 2 / Mathf.Sqrt(3), 0);
        Vector3 b = new Vector3(tile.x, tile.y * 2 / Mathf.Sqrt(3), 0);

        // Calculate the Euclidean distance in 3D space
        float distance3D = Vector3.Distance(a, b);

        // Convert the 3D distance back to 2D isometric space
        float distance2D = distance3D * Mathf.Sqrt(3) / 2;

        return distance2D;
    }
    
}
