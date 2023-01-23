using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeNode nodePrefab;
    [SerializeField] private Vector2Int mazeSize;
    [SerializeField] private Slider[] sliders;
    [SerializeField] private Text[] text;

    private bool _generatorType = true;
    private List<MazeNode> _nodes = new List<MazeNode>();
    private List<MazeNode> _currentPath = new List<MazeNode>();
    private List<MazeNode> _completedNodes = new List<MazeNode>();
    private List<GameObject> _mazeNodes = new List<GameObject>();

    private MazeNode _chosenNode;
    private int _chosenDirection;
    private int _randomRange;
    private Vector2Int _startMazeSize;

    public void Start()
    {
        _startMazeSize = mazeSize;
    }

    public void Update()
    {
     
        text[0].text = sliders[0].value.ToString();
        text[1].text = sliders[1].value.ToString();

    }

    public void ChangeType()
    {
        _generatorType = !_generatorType;
    }
    
    public void GenerateMaze()
    {
        mazeSize.x = (int)sliders[1].value;
        mazeSize.y = (int)sliders[0].value;
        for (var i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].ResetWalls();
            _nodes[i].SetState(NodeState.Available);
        }
        switch (_generatorType)
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
        // Create Nods;
        if (!_nodes.Any() || _startMazeSize != size)
        {
            _startMazeSize = size;
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                    var newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                    _nodes.Add(newNode);
                }
            }
        }

        _currentPath = new List<MazeNode>();
        _completedNodes = new List<MazeNode>();
        _randomRange = Random.Range(0, _nodes.Count);
        
        // Choose starting node;
        _currentPath.Add(_nodes[_randomRange]);

        while (_completedNodes.Count < _nodes.Count)
        {
            //Check nodes next to the current node;
            var possibleNextNodes = new List<int>();
            var possibleDirections = new List<int>();

            var currentNodeIndex = _nodes.IndexOf(_currentPath[_currentPath.Count - 1]);
            var currentNodeX = currentNodeIndex / size.y;
            var currentNodeY = currentNodeIndex % size.y;

            if (currentNodeX < size.x - 1)
            {
                if (!_completedNodes.Contains(_nodes[currentNodeIndex + size.y]) &&
                    !_currentPath.Contains(_nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            if (currentNodeX > 0)
            {
                if (!_completedNodes.Contains(_nodes[currentNodeIndex - size.y]) &&
                    !_currentPath.Contains(_nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            
            if (currentNodeY < size.y - 1)
            {
                if (!_completedNodes.Contains(_nodes[currentNodeIndex + 1]) &&
                    !_currentPath.Contains(_nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            
            if (currentNodeY > 0)
            {
                if (!_completedNodes.Contains(_nodes[currentNodeIndex - 1]) &&
                    !_currentPath.Contains(_nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            // Choose next node;
            if (possibleDirections.Count > 0)
            {
                _chosenDirection = Random.Range(0, possibleDirections.Count);
                _chosenNode = _nodes[possibleNextNodes[_chosenDirection]];

                switch (possibleDirections[_chosenDirection])
                {
                    case 1:
                        _chosenNode.RemoveWall(1);
                        _currentPath[_currentPath.Count - 1].RemoveWall(0);
                        break;
                    case 2:
                        _chosenNode.RemoveWall(0);
                        _currentPath[_currentPath.Count - 1].RemoveWall(1);
                        break;
                    case 3:
                        _chosenNode.RemoveWall(3);
                        _currentPath[_currentPath.Count - 1].RemoveWall(2);
                        break;
                    case 4:
                        _chosenNode.RemoveWall(2);
                        _currentPath[_currentPath.Count - 1].RemoveWall(3);
                        break;
                }
                _currentPath.Add(_chosenNode);
            }
            else
            {
                _completedNodes.Add(_currentPath[_currentPath.Count - 1]);
                _currentPath.RemoveAt(_currentPath.Count - 1);
            }
        }
    }

    private IEnumerator FollowGenerateMaze(Vector2Int size)
    {

        // Create Nods;

        if (!_nodes.Any() || mazeSize.y != size.y || mazeSize.x != size.x)
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                    var newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                    _nodes.Add(newNode);
                
                    yield return null;
                }
            }
        }

        _currentPath = new List<MazeNode>();
        _completedNodes = new List<MazeNode>();
        _randomRange = Random.Range(1, _nodes.Count);
        
        // Choose starting node;
        _currentPath.Add(_nodes[_randomRange]);
        _currentPath[0].SetState(NodeState.Current);

        while (_completedNodes.Count < _nodes.Count)
        {
            //Check nodes next to the current node;
            var possibleNextNodes = new List<int>();
            var possibleDirections = new List<int>();

            var currentNodeIndex = _nodes.IndexOf(_currentPath[_currentPath.Count - 1]);
            var currentNodeX = currentNodeIndex / size.y;
            var currentNodeY = currentNodeIndex % size.y;

            if (currentNodeX < size.x - 1)
            {
                if (!_completedNodes.Contains(_nodes[currentNodeIndex + size.y]) &&
                    !_currentPath.Contains(_nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            if (currentNodeX > 0)
            {
                if (!_completedNodes.Contains(_nodes[currentNodeIndex - size.y]) &&
                    !_currentPath.Contains(_nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            
            if (currentNodeY < size.y - 1)
            {
                if (!_completedNodes.Contains(_nodes[currentNodeIndex + 1]) &&
                    !_currentPath.Contains(_nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            
            if (currentNodeY > 0)
            {
                if (!_completedNodes.Contains(_nodes[currentNodeIndex - 1]) &&
                    !_currentPath.Contains(_nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            // Choose next node;
            if (possibleDirections.Count > 0)
            {
                _chosenDirection = Random.Range(0, possibleDirections.Count);
                _chosenNode = _nodes[possibleNextNodes[_chosenDirection]];

                switch (possibleDirections[_chosenDirection])
                {
                    case 1:
                        _chosenNode.RemoveWall(1);
                        _currentPath[_currentPath.Count - 1].RemoveWall(0);
                        break;
                    case 2:
                        _chosenNode.RemoveWall(0);
                        _currentPath[_currentPath.Count - 1].RemoveWall(1);
                        break;
                    case 3:
                        _chosenNode.RemoveWall(3);
                        _currentPath[_currentPath.Count - 1].RemoveWall(2);
                        break;
                    case 4:
                        _chosenNode.RemoveWall(2);
                        _currentPath[_currentPath.Count - 1].RemoveWall(3);
                        break;
                }
                _currentPath.Add(_chosenNode);
                _chosenNode.SetState(NodeState.Current);

                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                _completedNodes.Add(_currentPath[_currentPath.Count - 1]);
                _currentPath.RemoveAt(_currentPath.Count - 1);
            }
        }
    }
}

