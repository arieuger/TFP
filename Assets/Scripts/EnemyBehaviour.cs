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
        PathFinding.Instance.FindPath(_actualPositionTile, target);
    }
}
