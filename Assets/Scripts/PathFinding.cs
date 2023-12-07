using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
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
    
    public List<PathFindPosition> FindPath2(GameObject source, GameObject target)
    {
        List<PathFindPosition> openList = new List<PathFindPosition>();
        List<PathFindPosition> closedList = new List<PathFindPosition>();
       
        PathFindPosition startPosition = new PathFindPosition(source.transform.position, 0, 100f);
        startPosition.G = GetManhattenDistance(source.transform.position, startPosition.Position);
        startPosition.H = GetManhattenDistance(target.transform.position, startPosition.Position);
        PathFindPosition endPosition = new PathFindPosition(target.transform.position, 100f, 0);
        endPosition.G = GetManhattenDistance(source.transform.position, endPosition.Position);
        endPosition.H = GetManhattenDistance(target.transform.position, endPosition.Position);
        Debug.Log(endPosition.G + " - " + endPosition.H + " - " + endPosition.F);
        
        openList.Add(startPosition);

        startPosition.Position = new Vector2(Mathf.Round(startPosition.Position.x * 100f) / 100f,
            Mathf.Round(startPosition.Position.y * 100f) / 100f);
        endPosition.Position = new Vector2(Mathf.Round(endPosition.Position.x * 100f) / 100f,
            Mathf.Round(endPosition.Position.y * 100f) / 100f);
        
        while (openList.Count > 0)
        {
            PathFindPosition current = openList.OrderBy(x => x.F).First();
            
            openList.Remove(current);
            closedList.Add(current);

            if (current.Position.Equals(endPosition.Position))
            {
                Debug.Log("ashjgajshdgha");
                return GetFinishedList(startPosition, current);
            }
            
            GetNeighbours(current.Position).ForEach(tilePos =>
            {
                PathFindPosition tile = new PathFindPosition(tilePos,
                    GetManhattenDistance(startPosition.Position, tilePos),
                    GetManhattenDistance(target.transform.position, tilePos));

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

    public void FindPath(GameObject source, GameObject target)
    {
        FindGameObjectByPositions(FindPath2(source, target).ConvertAll(p => p.Position)).ForEach(g =>
        {
            g.GetComponent<SpriteRenderer>().color = Color.green;
        });
    }

    private List<GameObject> FindGameObjectByPositions(List<Vector2> positions)
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
        // return Mathf.Sqrt(Mathf.Pow((start.x - tile.x) / 2, 2) + Mathf.Pow(start.y - tile.y, 2));
        // Mathf.Sqrt(((start.x - tile.x) / 2) ^ 2 + (start.y - tile.y) ^ 2);
        // return Mathf.Abs(start.x >= tile.x ? start.x - tile.x : tile.x - start.x) + 
        //        (Mathf.Abs(start.y >= tile.y ? start.y - tile.y : tile.y - start.y) / 2);
        // return Mathf.Abs(start.x - tile.x) + (Mathf.Abs(start.y - tile.y) / 2);
        
        // distancia tiles en x
        // return (Math.Abs(start.x) - Math.Abs(tile.x)) / 0.5f + (Math.Abs(start.y) - Math.Abs(tile.y)) / 0.25f;
        
        // Convert 2D isometric coordinates to 3D
        Vector3 a = new Vector3(start.x, start.y * 2 / Mathf.Sqrt(3), 0);
        Vector3 b = new Vector3(tile.x, tile.y * 2 / Mathf.Sqrt(3), 0);

        // Calculate the Euclidean distance in 3D space
        float distance3D = Vector3.Distance(a, b);

        // Convert the 3D distance back to 2D isometric space
        float distance2D = distance3D * Mathf.Sqrt(3) / 2;

        return distance2D;
    }

    private bool IsNeighbour(Vector2 target, Vector2 possibleNeighbour)
    {
        return GetNeighbours(target).Contains(possibleNeighbour);
    }
    
    public void PaintNeighbours()
    {
        FindGameObjectByPositions(GetNeighbours(new Vector2(0.75f, -1.25f))).ForEach(g => g.GetComponent<SpriteRenderer>().color = Color.blue);
    }
    
}
