using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundSelector : MonoBehaviour
{
    private const float YZOffsetToCenter = 0.2f;
    private GameObject _foundHoverTile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        _foundHoverTile = null;
        
        Vector3 checkPosition = gameObject.transform.position;
        checkPosition.y += YZOffsetToCenter;

        Debug.Log(gameObject.name + " - " + gameObject.tag);
        
        Transform childTransform = gameObject.GetComponentsInChildren<Transform>().ToList().Find(g => g.CompareTag("Buildings"));
        if (childTransform != null)
        {
            _foundHoverTile = childTransform.gameObject;
            TileManager.Instance.MoveHoverTo(gameObject.transform.position);
            return;
        }
        
        TileManager.Instance.SetHoverVisible(false);
        
    }

    private void OnMouseExit()
    {
        _foundHoverTile = null;
    }

    private void OnMouseDown()
    {
        if (_foundHoverTile) TileManager.Instance.SelectOrUnselectTile(gameObject);
    }
}
