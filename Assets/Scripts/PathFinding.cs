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

    public void FindPath(GameObject source, GameObject target)
    {

        Vector2 sourcePos = source.transform.position;
        
        // 1 buscar veciños
        var neighbours = FindGameObjectByPositions(GetNeighbours(sourcePos));

        List<Vector2> path = new List<Vector2>();
        path.Add(sourcePos);
        int counter = 0;
        while (counter < 50)
        {
            float lowestF = 99999F;
            Vector2 possibleNextPoint = new Vector2();

            neighbours.ForEach(g =>
            {
                Vector2 neighbour = g.transform.position;
                g.GetComponent<SpriteRenderer>().color = Color.red;

                // Calcular distancia a target
                _g = GetManhattenDistance(sourcePos, neighbour);
                _h = GetManhattenDistance(target.transform.position, neighbour);
                
                if (F < lowestF)
                {
                    lowestF = F;
                    possibleNextPoint = neighbour;
                    
                } else if (IsNeighbour(target.transform.position, neighbour) && !path.Contains(neighbour))
                {
                    // IsNeighbour(possibleNextPoint, neighbour) &&
                    
                    Debug.Log("lkhlkjhlkjhkj");
                    possibleNextPoint = neighbour;
                }

            });
            
            path.Add(possibleNextPoint);
            neighbours = FindGameObjectByPositions(GetNeighbours(possibleNextPoint));
            counter = counter + 1;
        }
        
        path.Add(target.transform.position);
        
        FindGameObjectByPositions(path).ForEach(g =>
        {
            g.GetComponent<SpriteRenderer>().color = Color.green;
        });
        
        // FindGameObjectByPositions(GetNeighbours(target.transform.position)).ForEach(g => g.GetComponent<SpriteRenderer>().color = Color.blue);
    }

    private List<GameObject> FindGameObjectByPositions(List<Vector2> positions)
    {
        return TileManager.Instance.ground.GetComponentsInChildren<Transform>().ToList()
            .FindAll(g => g.GetComponent<GroundTileBehaviour>() != null)
            .FindAll(g => positions.Contains(g.position))
            .ConvertAll(t => t.gameObject);
        
    }

    private void GetPathPoint(Vector2 startPoint, Vector2 target)
    {
        
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
        return Mathf.Abs(start.x - tile.x) + (Mathf.Abs(start.y - tile.y) / 2);
    }

    private List<Vector2> FindClosestHToTarget(Vector2 target, float h)
    {
        // return GetNeighbours(target).Find(v => GetManhattenDistance(target, v).Equals(h));
        List<Vector2> returnVectors = new List<Vector2>();
        GetNeighbours(target).ForEach(v =>
        {
            if (GetManhattenDistance(target, v).Equals(h)) returnVectors.Add(v);
        });
        
        return returnVectors;
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
