using UnityEngine;

public enum NodeState
{
    Available,
    Current,
    Completed
}
public class MazeNode : MonoBehaviour
{
    [SerializeField] private GameObject[] wall;
    [SerializeField] private MeshRenderer floor;

    public void RemoveWall(int wallToRemove)
    {
        wall[wallToRemove].gameObject.SetActive(false);
    }

    public void ResetWalls()
    {
        foreach (var walls in wall)
        {
            while (walls.gameObject.activeSelf == false)
            {
                walls.gameObject.SetActive(true);
            }
        }
        
    }

    public void SetState(NodeState state)
    {
        switch (state)
        {
            case NodeState.Available:
                floor.material.color = Color.white;
                break;
            case NodeState.Current:
                floor.material.color = Color.yellow;
                break;
            case NodeState.Completed:
                floor.material.color = Color.blue;
                break;
        }
    }
}
