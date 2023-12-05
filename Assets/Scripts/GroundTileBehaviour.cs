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
        
        TileManager.Instance.SetHoverOrSelectVisible(false);
        
    }

    private void OnMouseExit()
    {
        _foundHoverTile = null;
    }

    private void OnMouseDown()
    {
        if (_foundHoverTile) TileManager.Instance.SelectOrUnselectTile(gameObject);
    }

    public void DarkenTile(Color color, bool isChangingTiles)
    {
        StartCoroutine(SmoothColorChange(_originalColor, color, isChangingTiles));
    }


    public void LightTile(Color color, bool isChangingTiles)
    {
        StartCoroutine(SmoothColorChange(color, _originalColor, isChangingTiles));
    }

    private IEnumerator SmoothColorChange(Color startColor, Color endColor, bool isChangingTiles = false)
    {
        float time = 0.1f;


        float elapsedTime = 0;

        if (!isChangingTiles)
        {
            while (elapsedTime < time)
            {
                Color transitioningColor = Color.Lerp(startColor, endColor, (elapsedTime / time));
                GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(s => s.color = transitioningColor);
            
                elapsedTime += Time.deltaTime;
                yield return null;
            }    
        }
        
        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(s => s.color = endColor);
    }

    
}
