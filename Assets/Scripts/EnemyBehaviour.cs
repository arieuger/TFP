using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    [SerializeField] private GameObject target;
    private GameObject _actualPositionTile;
    


    [ContextMenu("Paint Path")]
    void Start()
    {
        _actualPositionTile = gameObject.GetComponentInParent<GroundTileBehaviour>().gameObject;

        float closestDistance = 9999f;
        Vector2 targetPos = new Vector2();
        
        // Find closest building or enemy
        TileManager.Instance.ground.GetComponentsInChildren<GroundTileBehaviour>().ToList()
            .FindAll(g => g.CompareTag("Buildings")).ForEach(t =>
            {
                float distance = PathFinding.Instance.GetManhattenDistance(_actualPositionTile.transform.position, t.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetPos = t.transform.position;
                }
            });

        GameObject targetGo = PathFinding.Instance.FindGameObjectByPosition(targetPos); 
        PathFinding.Instance.FindGameObjectByPositions(PathFinding.Instance.FindPath(_actualPositionTile, targetGo).ConvertAll(p => p.Position)).ForEach(g =>
        {
            g.GetComponent<SpriteRenderer>().color = Color.green;
        });
    }
}
