using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] public GameObject ground;
    [SerializeField] private GameObject hoverTile;
    [SerializeField] private GameObject selectionTile;
    [SerializeField] private float onSelectElevation = 0.15f;
    [SerializeField] private Color darkenColor;

    [HideInInspector] public bool IsTransitionSelectingTile { get; set; }
    [HideInInspector] public bool IsStartingSetDefensor { get; set; }
    [HideInInspector] public GameObject TileWithDefensor { get; set; }

    private SpriteRenderer _hoverTileSpriteRenderer;
    private SpriteRenderer _selectionTileSpriteRenderer;

    private GameObject _selectedTile;
    public GameObject SelectedTile => _selectedTile;

    private static TileManager _instance;
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
        _selectionTileSpriteRenderer = selectionTile.GetComponent<SpriteRenderer>();
        SetHoverOrSelectVisible(false, true);
        SetHoverOrSelectVisible(false, false);
    }

    public void MoveHoverTo(Vector3 position)
    {
        SetHoverOrSelectVisible(true, true);
        hoverTile.transform.position = position;
        
        if (TileWithDefensor != null)
        {
            GameObject defensor = TileWithDefensor.GetComponentInChildren<DefensorsBehaviour>().gameObject;
            GameObject tile = FindGameObjectByPosition(position).gameObject;
            defensor.transform.parent = tile.transform;
            defensor.transform.position = tile.transform.position;
            TileWithDefensor = tile;
            ground.GetComponentsInChildren<GroundTileBehaviour>().ToList().FindAll(g => g.gameObject != TileWithDefensor)
                .ForEach(g => g.DarkenTile(darkenColor, !IsStartingSetDefensor));
            tile.GetComponent<GroundTileBehaviour>().LightTile(darkenColor, !IsStartingSetDefensor);
            IsStartingSetDefensor = false;
        }
    }

    public void SetHoverOrSelectVisible(bool shouldBeVisible, bool isHover = true)
    {
        SpriteRenderer rend = isHover ? _hoverTileSpriteRenderer : _selectionTileSpriteRenderer;
        Color originalColor = rend.color;
        Color changeColor = originalColor;

        changeColor.a = shouldBeVisible ? 1 : 0;

        rend.color = changeColor;
    }

    public void SelectOrUnselectTile(GameObject groundTile)
    {
        if (_selectedTile == null || _selectedTile != groundTile) SelectTile(groundTile);
        else UnselectTile(groundTile);
    }
    
    public void UnselectTile(GameObject groundTile, bool isChangingTiles = false)
    {
        if (groundTile == null && _selectedTile == null) return;
        if (groundTile == null) groundTile = _selectedTile; // Default value
        LightAllTiles(isChangingTiles);
        _selectedTile = null;
        StartCoroutine(SmoothTileMovement(groundTile, -onSelectElevation));
        SetHoverOrSelectVisible(false, false);
        SetHoverOrSelectVisible(true);
    }

    public void LightAllTiles(bool isChangingTiles)
    {
        ground.GetComponentsInChildren<GroundTileBehaviour>().ToList().ForEach(g => g.LightTile(darkenColor, isChangingTiles));
    }
    
    private void SelectTile(GameObject groundTile)
    {
        bool isChangingTiles = false;
        if (_selectedTile != null)
        {
            isChangingTiles = true;
            UnselectTile(_selectedTile, true);
        }

        SetHoverOrSelectVisible(false);
        selectionTile.transform.position = groundTile.transform.position;
        SetHoverOrSelectVisible(true, false);
        
        _selectedTile = groundTile;
        ground.GetComponentsInChildren<GroundTileBehaviour>().ToList().FindAll(g => g.gameObject != groundTile)
            .ForEach(g => g.DarkenTile(darkenColor, isChangingTiles));
        StartCoroutine(SmoothTileMovement(groundTile, onSelectElevation));
    }

    public void SelectOrDeactivateTileWithDefensor()
    {
        TileWithDefensor = null;
        SetHoverOrSelectVisible(false);
        LightAllTiles(false);
        UIManager.Instance.EnableOrDisableActionButtons(true);
    }
    
    public GameObject FindGameObjectByPosition(Vector2 position)
    {
        return TileManager.Instance.ground.GetComponentsInChildren<Transform>().ToList()
            .FindAll(g => g.GetComponent<GroundTileBehaviour>() != null)
            .Find(g => g.position.Equals(position)).gameObject;
    }
    
    public List<GameObject> FindGameObjectByPositions(List<Vector2> positions)
    {
        return TileManager.Instance.ground.GetComponentsInChildren<Transform>().ToList()
            .FindAll(g => g.GetComponent<GroundTileBehaviour>() != null)
            .FindAll(g => positions.Contains(g.position))
            .ConvertAll(t => t.gameObject);
    }
    
    private IEnumerator SmoothTileMovement(GameObject groundTile, float displacement)
    {
        IsTransitionSelectingTile = true;
        
        float time = 0.1f;
        
        Vector3 startingPos  = groundTile.transform.position;
        Vector3 finalPos = startingPos;
        finalPos.y += displacement;

        float elapsedTime = 0;
        
        while (elapsedTime < time)
        {
            groundTile.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            selectionTile.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            if (displacement < 0) hoverTile.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        groundTile.transform.position = finalPos;
        selectionTile.transform.position = finalPos;
        hoverTile.transform.position = finalPos;

        IsTransitionSelectingTile = false;
    }
    
}
