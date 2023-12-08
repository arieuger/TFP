using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject protestorPrefab;
    [SerializeField] private GameObject journalistPrefab;

    public void ClickOnJournalistsButton()
    {
        InstanceDefensor(journalistPrefab);
    }
    
    public void ClickOnProtestorsButton()
    {
       InstanceDefensor(protestorPrefab);
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

        // chosenTile.GetComponent<SpriteRenderer>().color = Color.magenta;
        Instantiate(prefab, chosenTile.transform.position, Quaternion.identity, chosenTile.transform);
        TileManager.Instance.TileWithDefensor = chosenTile;
        TileManager.Instance.UnselectTile(null);
        TileManager.Instance.IsStartingSetDefensor = TileManager.Instance.SelectedTile == null;
        TileManager.Instance.MoveHoverTo(chosenTile.transform.position);
    }
}
