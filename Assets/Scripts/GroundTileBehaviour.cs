using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundTileBehaviour : MonoBehaviour
{
    private Color _originalColor;
    private const float YZOffsetToCenter = 0.2f;
    private GameObject _foundHoverTile;

    private void Start()
    {
        _originalColor = GetComponent<SpriteRenderer>().color;
    }

    public void DarkenTile(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(s => s.color = color);
    }

    public void LightTile()
    {
        GetComponent<SpriteRenderer>().color = _originalColor;
        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(s => s.color = _originalColor);
    }
    private void OnMouseEnter()
    {
        _foundHoverTile = null;
        
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
