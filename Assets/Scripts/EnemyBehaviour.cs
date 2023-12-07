using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    [SerializeField] private GameObject target;
    private GameObject _actualPositionTile;
    


    void Start()
    {
        _actualPositionTile = gameObject.GetComponentInParent<GroundTileBehaviour>().gameObject;

        // List<GroundTileBehaviour> buildingTiles = TileManager.Instance.ground.GetComponentsInChildren<GroundTileBehaviour>()
        //     .ToList().FindAll(g => g.CompareTag("Buildings"));
        //
        // buildingTiles.ForEach(b => Debug.Log(b.name));
        //
        // GameObject target = null;
        // buildingTiles.ForEach(b =>
        // {
        //     // a cada building pathfind ao propio gameobject
        // });
        //
        // if (target != null) target.GetComponent<SpriteRenderer>().color = Color.red;;
        
        PathFinding.Instance.FindPath(gameObject, target);
    }

    [ContextMenu("Paint Neighbours")]
    public void PaintNeighbours()
    {
        PathFinding.Instance.PaintNeighbours();
    }

    void Update()
    {
        
    }
}
