using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class RollerCoaster : MonoBehaviour
{
    [SerializeField] private List<GameObject> rollerCoaster;
    [SerializeField] private List<Vector3> gridPositions;
    [SerializeField] private bool[] isDone;
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private LayerMask _layer;

    private bool isControl = false;

    private void Start()
    {
        ResetAllSettings();
    }

    public void ResetAllSettings()
    {
        rollerCoaster = new List<GameObject>();
        for (var i = 0; i < transform.childCount; i++)
        {
            rollerCoaster.Add(transform.GetChild(i).gameObject);
        }
        
        gridPositions = new List<Vector3>();
        foreach (var wagon in rollerCoaster)
        {
            gridPositions.Add(wagon.transform.position);
        }

        isDone = new bool[rollerCoaster.Count];
        if(GameManager.Instance != null) _layer = GameManager.Instance._RollerCoasterLayer;
    }
    
    public void OnSelect()
    {
        foreach (var wagon in rollerCoaster)
        {
            wagon.GetComponent<Wagon>().OnSelect();
        }
    }
    
    public void OnDeSelect()
    {
        foreach (var wagon in rollerCoaster)
        {
            wagon.GetComponent<Wagon>().OnDeSelect();
        }
    }
    
    public void MoveFromFront(Transform selected) => MoveTrain(selected, true);
    
    public void MoveFromBack(Transform selected) => MoveTrain(selected, false);
    
    private void MoveTrain(Transform selected, bool isFront)
    {
        if (isControl) return;

        foreach (var wagon in rollerCoaster)
        {
            if (DOTween.IsTweening(wagon.gameObject.transform)) return;
        }

        isControl = true;
        int leaderIndex = isFront ? 0 : rollerCoaster.Count - 1;
        Vector3 targetPos = selected.position;

        if (gridPositions[leaderIndex] == targetPos)
        {
            isControl = false;
            return;
        }
        
        List<Vector3> path = FindPath(gridPositions[leaderIndex], targetPos);
        if (path == null || path.Count == 0)
        {
            isControl = false;
            return;
        }

        Sequence sequence = DOTween.Sequence();

        foreach (Vector3 nextPos in path)
        {
            sequence.AppendCallback(() =>
            {
                Vector3 direction = nextPos - gridPositions[leaderIndex];
                Vector3 step = direction.normalized;

                if (Physics.Raycast(gridPositions[leaderIndex] + Vector3.up / 2, step, out RaycastHit hit, Vector3.Distance(gridPositions[leaderIndex], nextPos), _layer, QueryTriggerInteraction.Ignore))
                {
                    return;
                }

                if (GameManager.Instance.IsPositionBlocked(nextPos))
                {
                    return;
                }

                Quaternion headRot = new Quaternion();

                if (isFront)
                {
                    for (int j = rollerCoaster.Count - 1; j > 0; j--)
                    {
                        gridPositions[j] = gridPositions[j - 1];
                        rollerCoaster[j].transform.DOMove(gridPositions[j], duration).SetEase(Ease.Linear);
                        rollerCoaster[j].transform.DOLookAt(gridPositions[j - 1], duration).SetEase(Ease.Linear);
                    }
                    headRot = Quaternion.LookRotation(gridPositions[^2] - gridPositions[^1]);
                }
                else
                {
                    for (int j = 0; j < rollerCoaster.Count - 1; j++)
                    {
                        gridPositions[j] = gridPositions[j + 1];
                        rollerCoaster[j].transform.DOMove(gridPositions[j], duration).SetEase(Ease.Linear);
                    }

                    for (int j = rollerCoaster.Count - 2; j > 0; j--)
                    {
                        if (j == 1) headRot = Quaternion.LookRotation(gridPositions[0] - gridPositions[1]);
                        rollerCoaster[j].transform.DOLookAt(gridPositions[j - 1], duration).SetEase(Ease.Linear);
                    }
                }

                gridPositions[leaderIndex] = nextPos;
                rollerCoaster[leaderIndex].transform.DOMove(nextPos, duration).SetEase(Ease.Linear);

                if (isFront)
                {
                    rollerCoaster[leaderIndex].transform.DOLookAt(nextPos + step, duration).SetEase(Ease.Linear);
                    rollerCoaster[^1].transform.DORotateQuaternion(headRot, duration).SetEase(Ease.Linear);
                }
                else
                {
                    rollerCoaster[leaderIndex].transform.DOLookAt(gridPositions[leaderIndex - 1], duration).SetEase(Ease.Linear);
                    rollerCoaster[0].transform.DORotateQuaternion(headRot, duration).SetEase(Ease.Linear);
                }
            });

            sequence.AppendInterval(duration);
        }

        sequence.OnComplete(() => isControl = false);
    }

    private List<Vector3> FindPath(Vector3 start, Vector3 target)
    {
        List<Vector3> openSet = new List<Vector3> { start };
        HashSet<Vector3> closedSet = new HashSet<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, float> gScore = new Dictionary<Vector3, float> { [start] = 0 };
        Dictionary<Vector3, float> fScore = new Dictionary<Vector3, float> { [start] = Vector3.Distance(start, target) };

        Vector3[] directions = new Vector3[]
        {
            new Vector3(1, 0, 0), new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1), new Vector3(0, 0, -1)
        };

        while (openSet.Count > 0)
        {
            Vector3 current = openSet.OrderBy(pos => fScore[pos]).First();

            if (current == target)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector3 dir in directions)
            {
                Vector3 neighbor = current + dir;
                
                if (!IsWithinGridBounds(neighbor))
                {
                    continue;
                }

                if (closedSet.Contains(neighbor) || GameManager.Instance.IsPositionBlocked(neighbor) ||
                    Physics.Raycast(current + Vector3.up / 2, dir, Vector3.Distance(current, neighbor), _layer))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + Vector3.Distance(current, neighbor);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor, target);
            }
        }

        return null;
    }

    private bool IsWithinGridBounds(Vector3 position)
    {
        var gridOrigin = GameManager.Instance.gridOrigin;
        var gridSize = GameManager.Instance.gridSize;
        
        float minX = gridOrigin.x - (int)(gridSize.x/2);
        float maxX = gridOrigin.x + (int)(gridSize.x/2);
        float minZ = gridOrigin.z - (int)(gridSize.y/2);
        float maxZ = gridOrigin.z + (int)(gridSize.y/2);

        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }

    private List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
    {
        List<Vector3> path = new List<Vector3> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        path.RemoveAt(0);
        return path;
    }

    public void FindToSitWagon(StickMan stickMan)
    {
        foreach (var wagon in rollerCoaster)
        {
            var _wagon = wagon.GetComponent<Wagon>();
            if (stickMan.Id != _wagon.Id) continue;
            if (!_wagon.IsHasSitPos()) continue;
            _wagon.SitToPos(stickMan);
            break;
        }
    }

    public bool IsHasIDWagon(StickMan stickMan)
    {
        foreach (var wagon in rollerCoaster)
        {
            var _wagon = wagon.GetComponent<Wagon>();
            if (stickMan.Id != _wagon.Id) continue;
            if (!_wagon.IsHasSitPos()) continue;
            return true;
        }
        return false;
    }

    public void DoneWagon(GameObject wagon)
    {
        for (int i = 0; i < rollerCoaster.Count; i++)
        {
            if(rollerCoaster[i] != wagon) continue;

            isDone[i] = true;
        }

        for (int i = 0; i < isDone.Length; i++)
        {
            if(!isDone[i]) break;
            
            if(i == isDone.Length-1) DestroyTrain();
        }
        
    }

    private void DestroyTrain()
    {
        foreach (var wagon in rollerCoaster)
        {
            wagon.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear);
        }
        
        Destroy(gameObject,1f);
        GameManager.Instance.CheckRollerCoaster();
    }
    
}