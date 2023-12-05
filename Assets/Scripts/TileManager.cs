using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    [SerializeField] private GameObject hoverTile;

    private SpriteRenderer _hoverTileSpriteRenderer;
    
    private static TileManager _instance;
    private bool _shouldBeVisible;
    private GameObject _selectedTile;

    public static TileManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
    }
    
    void Start()
    {
        _hoverTileSpriteRenderer = hoverTile.GetComponent<SpriteRenderer>();
        SetHoverVisible(false);
    }

    public void MoveHoverTo(Vector3 position)
    {
        SetHoverVisible(true);
        hoverTile.transform.position = position;
    }

    public void SetHoverVisible(bool shouldBeVisible)
    {
        _shouldBeVisible = shouldBeVisible;
        Color originalColor = _hoverTileSpriteRenderer.color;
        Color changeColor = originalColor;

        if (shouldBeVisible) changeColor.a = 1;
        else changeColor.a = 0;

        _hoverTileSpriteRenderer.color = changeColor;
    }

    public void SelectOrUnselectTile(GameObject groundTile)
    {
        if (_selectedTile == null || _selectedTile != groundTile) SelectTile(groundTile);
        else UnselectTile(groundTile);
    }

    private void SelectTile(GameObject groundTile)
    {
        if (_selectedTile != null) UnselectTile(_selectedTile);
        _selectedTile = groundTile;
        Vector3 pos = groundTile.transform.position;
        pos.y += 0.1f;
        groundTile.transform.position = pos;
        hoverTile.transform.position = pos;
    }
    
    private void UnselectTile(GameObject groundTile)
    {
        _selectedTile = null;
        Vector3 pos = groundTile.transform.position;
        pos.y -= 0.1f;
        groundTile.transform.position = pos;
        hoverTile.transform.position = pos;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
