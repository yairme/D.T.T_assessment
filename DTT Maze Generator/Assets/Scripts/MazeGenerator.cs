using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeNode nodePrefab;
    [SerializeField] private Vector2Int mazeSize;
    [SerializeField] private Slider[] sliders;
    [SerializeField] private Text[] text;

    public void Update()
    {
        
        text[0].text = sliders[0].value.ToString();
        text[1].text = sliders[1].value.ToString();
        
    }

    public void ChangeYValue(int yValue)
    {
        mazeSize.y = yValue;
    }

    public void ChangeXValue(int xValue)
    {
        mazeSize.x = xValue;
    }
    
    public void ChangeType(bool generatorType)
    {

        switch (generatorType)
        {
            
            case true:
                GenerateMaze(mazeSize);
                break;
            case false:
                StartCoroutine(FollowGenerateMaze(mazeSize));
                break;

        }
    }

    private void GenerateMaze(Vector2Int size)
    {
        var nodes = new List<MazeNode>();
        
        // Create Nods;
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                var newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
            }
        }

        var currentPath = new List<MazeNode>();
        var completedNodes = new List<MazeNode>();
        
        // Choose starting node;
        currentPath.Add(nodes[Random.Range(0, nodes.Count)]);

        while (completedNodes.Count < nodes.Count)
        {
            //Check nodes next to the current node;
            var possibleNextNodes = new List<int>();
            var possibleDirections = new List<int>();

            var currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            var currentNodeX = currentNodeIndex / size.y;
            var currentNodeY = currentNodeIndex % size.y;

            if (currentNodeX < size.x - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            if (currentNodeX > 0)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            
            if (currentNodeY < size.y - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            
            if (currentNodeY > 0)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }
            
            // Choose next node;
            if (possibleDirections.Count > 0)
            {
                var chosenDirection = Random.Range(0, possibleDirections.Count);
                var chosenNode = nodes[possibleNextNodes[chosenDirection]];

                switch (possibleDirections[chosenDirection])
                {
                    case 1:
                        chosenNode.RemoveWall(1);
                        currentPath[currentPath.Count - 1].RemoveWall(0);
                        break;
                    case 2:
                        chosenNode.RemoveWall(0);
                        currentPath[currentPath.Count - 1].RemoveWall(1);
                        break;
                    case 3:
                        chosenNode.RemoveWall(3);
                        currentPath[currentPath.Count - 1].RemoveWall(2);
                        break;
                    case 4:
                        chosenNode.RemoveWall(2);
                        currentPath[currentPath.Count - 1].RemoveWall(3);
                        break;
                }
                currentPath.Add(chosenNode);
            }
            else
            {
                completedNodes.Add(currentPath[currentPath.Count - 1]);
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
    }
    
    private IEnumerator FollowGenerateMaze(Vector2Int size)
    {
        var nodes = new List<MazeNode>();
        
        // Create Nods;
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                var newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
                
                yield return null;
            }
        }

        var currentPath = new List<MazeNode>();
        var completedNodes = new List<MazeNode>();
        
        // Choose starting node;
        currentPath.Add(nodes[Random.Range(0, nodes.Count)]);
        currentPath[0].SetState(NodeState.Current);

        while (completedNodes.Count < nodes.Count)
        {
            //Check nodes next to the current node;
            var possibleNextNodes = new List<int>();
            var possibleDirections = new List<int>();

            var currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            var currentNodeX = currentNodeIndex / size.y;
            var currentNodeY = currentNodeIndex % size.y;

            if (currentNodeX < size.x - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            if (currentNodeX > 0)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            
            if (currentNodeY < size.y - 1)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            
            if (currentNodeY > 0)
            {
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }
            
            // Choose next node;
            if (possibleDirections.Count > 0)
            {
                var chosenDirection = Random.Range(0, possibleDirections.Count);
                var chosenNode = nodes[possibleNextNodes[chosenDirection]];

                switch (possibleDirections[chosenDirection])
                {
                    case 1:
                        chosenNode.RemoveWall(1);
                        currentPath[currentPath.Count - 1].RemoveWall(0);
                        break;
                    case 2:
                        chosenNode.RemoveWall(0);
                        currentPath[currentPath.Count - 1].RemoveWall(1);
                        break;
                    case 3:
                        chosenNode.RemoveWall(3);
                        currentPath[currentPath.Count - 1].RemoveWall(2);
                        break;
                    case 4:
                        chosenNode.RemoveWall(2);
                        currentPath[currentPath.Count - 1].RemoveWall(3);
                        break;
                }
                currentPath.Add(chosenNode);
                chosenNode.SetState(NodeState.Current);

                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                completedNodes.Add(currentPath[currentPath.Count - 1]);
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
    }
}

