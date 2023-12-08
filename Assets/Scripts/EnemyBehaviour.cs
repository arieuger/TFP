using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    private GameObject _actualPositionTile;
    private List<PathFindPosition> _pathFindPositions;
    private Animator _anim;
    
    private static readonly int UpOrLeft = Animator.StringToHash("upOrLeft");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    [ContextMenu("Paint Path")]
    void PaintPath()
    {
        _actualPositionTile = gameObject.GetComponentInParent<GroundTileBehaviour>().gameObject;

        float closestDistance = 999f;
        Vector2 targetPos = new Vector2();
        
        // Find closest building or enemy
        TileManager.Instance.ground.GetComponentsInChildren<GroundTileBehaviour>().ToList()
            .FindAll(g => g.transform.position != _actualPositionTile.transform.position).ForEach(t =>
            {
                
                if (!t.GetComponentsInChildren<Transform>().ToList()
                        .Exists(c => c.CompareTag("Buildings") || c.CompareTag("Defensors"))) return;
                
                float distance = PathFinding.Instance.GetManhattenDistance(_actualPositionTile.transform.position, t.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetPos = t.transform.position;
                }
            });

        GameObject targetGo = TileManager.Instance.FindGameObjectByPosition(targetPos);
        _pathFindPositions = PathFinding.Instance.FindPath(_actualPositionTile, targetGo);
        TileManager.Instance.FindGameObjectByPositions(_pathFindPositions.ConvertAll(p => p.Position)).ForEach(g =>
        {
            g.GetComponent<SpriteRenderer>().color = Color.green;
        });
    }

    [ContextMenu("Advance")]
    void Advance()
    {
        if (_pathFindPositions == null || _pathFindPositions.Count == 0) return;

        PathFindPosition nextPos = _pathFindPositions.OrderBy(p => p.G).First();
        
        UpdateAnimation(nextPos);
        StartCoroutine(MoveEnemy(nextPos));
        
        _pathFindPositions.Remove(nextPos);
    }

    private IEnumerator MoveEnemy(PathFindPosition nextPos)
    {
        GameObject nextParent = TileManager.Instance.FindGameObjectByPosition(nextPos.Position);
        _anim.SetBool(IsMoving, true);
        
        float time = 1f;

        Vector2 startingPosition = transform.position;
        Vector2 finalPosition = nextPos.Position;
        
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector2.Lerp(startingPosition, finalPosition, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.parent = nextParent.transform;
        transform.position = nextParent.transform.position;
        _anim.SetBool(IsMoving, false);
    }

    private void UpdateAnimation(PathFindPosition nextPos)
    {
        Vector2 direction = PathFinding.Instance.GetDirectionOfMovement(nextPos.Position, transform.position);
        Vector3 localScale = transform.localScale;
        
        if (direction.Equals(Constants.UpVector) || direction.Equals(Constants.DoVector))
        {
            localScale.x = Mathf.Abs(localScale.x) * -1F;
        }
        else
        {
            localScale.x = Mathf.Abs(localScale.x);
        }
        transform.localScale = localScale;

        if (direction.Equals(Constants.UpVector) || direction.Equals(Constants.LeVector))
        {
            _anim.SetBool(UpOrLeft, false);
        }
        else
        {
            _anim.SetBool(UpOrLeft, true);
        }
    }
}
