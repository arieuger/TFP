using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject protestorPrefab;

    public void ClickOnProtestorsButton()
    {
        // Find most centered empty ground
        Vector2 comparerVector = new Vector2(0, 0);
        float distance = 999f;
        GameObject chosenTile = null;
        TileManager.Instance.ground.GetComponentsInChildren<GroundTileBehaviour>().ToList()
            .FindAll(g => g.gameObject.transform.childCount == 0).ForEach(tile =>
            {
                Debug.Log("asdfasdas");
                float actualDistance = PathFinding.Instance.GetManhattenDistance(comparerVector, tile.transform.position);
                if (actualDistance < distance)
                {
                    distance = actualDistance;
                    chosenTile = tile.gameObject;
                }
            });

        // chosenTile.GetComponent<SpriteRenderer>().color = Color.magenta;
        Instantiate(protestorPrefab, chosenTile.transform.position, Quaternion.identity, chosenTile.transform);
        TileManager.Instance.TileWithDefensor = chosenTile;
        TileManager.Instance.UnselectTile(null);
        TileManager.Instance.MoveHoverTo(chosenTile.transform.position);
    }
}
