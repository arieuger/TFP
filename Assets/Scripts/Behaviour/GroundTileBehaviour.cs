using System.Collections;
using System.Linq;
using Managers;
using UnityEngine;

namespace Behaviour
{
    public class GroundTileBehaviour : MonoBehaviour
    {
        private Color _originalColor;
        private GameObject _foundHoverTile;

        private void Start()
        {
            _originalColor = GetComponent<SpriteRenderer>().color;
        }

        private void OnMouseEnter()
        {
            if (TileManager.Instance.IsTransitionSelectingTile) return;
            if (TileManager.Instance.TileWithDefensor == null)
            {

                _foundHoverTile = null;

                Transform childTransform = gameObject.GetComponentsInChildren<Transform>().ToList()
                    .Find(g => g.CompareTag("Buildings") || g.CompareTag("Defensors"));
                if (childTransform != null)
                {
                    _foundHoverTile = childTransform.gameObject;
                    TileManager.Instance.MoveHoverTo(gameObject.transform.position);
                    return;
                }

                TileManager.Instance.SetHoverOrSelectVisible(false);
            }
            else
            {
                if (transform.childCount != 0) return;
                _foundHoverTile = null;
                TileManager.Instance.MoveHoverTo(gameObject.transform.position);
            }
        }

        private void OnMouseExit()
        {
            _foundHoverTile = null;
        }

        private void OnMouseDown()
        {
            if (_foundHoverTile && TileManager.Instance.TileWithDefensor == null) TileManager.Instance.SelectOrUnselectTile(gameObject);
            if (TileManager.Instance.TileWithDefensor != null) TileManager.Instance.SelectOrDeactivateTileWithDefensor();
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
}
