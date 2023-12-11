using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject protestorPrefab;
    [SerializeField] private GameObject journalistPrefab;

    [SerializeField] private List<Button> actionButtons;  
    [SerializeField] private Button cancelButton;

    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;
    }

    public void ClickOnJournalistsButton()
    {
        InstanceDefensor(journalistPrefab);
    }
    
    public void ClickOnProtestorsButton()
    {
       InstanceDefensor(protestorPrefab);
    }

    public void CancelActionButton()
    {
        Destroy(TileManager.Instance.TileWithDefensor.GetComponentInChildren<DefensorsBehaviour>().gameObject);
        TileManager.Instance.SelectOrDeactivateTileWithDefensor();
    }

    public void EnableOrDisableActionButtons(bool enable)
    {
        actionButtons.ForEach(b => b.interactable = enable);
        cancelButton.interactable = !enable;
    }

    private void InstanceDefensor(GameObject prefab)
    {
        // Find most centered empty ground
        Vector2 comparerVector = new Vector2(0, 0);
        float distance = 999f;
        GameObject chosenTile = null;
        TileManager.Instance.ground.GetComponentsInChildren<GroundTileBehaviour>().ToList()
            .FindAll(g => g.gameObject.transform.childCount == 0).ForEach(tile =>
            {
                float actualDistance = PathFinding.Instance.GetManhattenDistance(comparerVector, tile.transform.position);
                if (actualDistance < distance)
                {
                    distance = actualDistance;
                    chosenTile = tile.gameObject;
                }
            });

        Instantiate(prefab, chosenTile.transform.position, Quaternion.identity, chosenTile.transform);
        TileManager.Instance.TileWithDefensor = chosenTile;
        TileManager.Instance.UnselectTile(null);
        TileManager.Instance.IsStartingSetDefensor = TileManager.Instance.SelectedTile == null;
        TileManager.Instance.MoveHoverTo(chosenTile.transform.position);
        
        EnableOrDisableActionButtons(false);
    }
    
}
