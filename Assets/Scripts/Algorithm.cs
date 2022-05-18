using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using System.Linq;
using System;
using Enums;

public class Algorithm : MonoBehaviour
{ 
    public static Dictionary<long, Node> _nodesDictionary = new Dictionary<long, Node>();


    public static List<List<string>> _nodesList = new List<List<string>>();

    public static float Solve(long startNodeId, long finishNodeId, Dictionary<long, GameObject> nodesInObjects)
    {
        _nodesDictionary.Clear();
        foreach (var item in nodesInObjects)
        {
            _nodesDictionary.Add(item.Key, item.Value.GetComponent<Node>());
            _nodesDictionary.Last().Value.visited = false;
            _nodesDictionary.Last().Value.pin.lightestId = new List<long>();
            if (startNodeId == item.Key)
            {
                _nodesDictionary[item.Key].pin.weight = 0;
                _nodesDictionary[item.Key].pin.lightestId.Add(startNodeId);
            }
            else
            {
                _nodesDictionary[item.Key].pin.weight = Mathf.Infinity;
            }
        }
        var currentNode = (from node in _nodesDictionary
                          where node.Key == startNodeId
                          select node.Value).ToArray()[0];

        while ((from node in _nodesDictionary
                where node.Value.visited == false
                select node).ToList().Count > 0)
        {
            var order = (from o in currentNode.AwaliblePath
                         orderby o.weight
                         select o).ToList();
            foreach (var item in order)
            {
                if (_nodesDictionary[item.path].pin.weight > item.weight + currentNode.pin.weight) 
                {
                    _nodesDictionary[item.path].pin.weight = item.weight + currentNode.pin.weight;
                    _nodesDictionary[item.path].pin.lightestId.Clear();
                    _nodesDictionary[item.path].pin.lightestId.Add(currentNode.Id);
                }
                else if (_nodesDictionary[item.path].pin.weight == item.weight + currentNode.pin.weight)
                {
                    if (!_nodesDictionary[item.path].pin.lightestId.Contains(currentNode.Id))
                    {
                        _nodesDictionary[item.path].pin.lightestId.Add(currentNode.Id);
                    }
                }
            }
            currentNode.visited = true;
            var next = (from n in _nodesDictionary
                        where n.Value.visited == false
                        orderby n.Value.pin.weight
                        select n).ToList();
            if (next.Count > 0)
            {
                currentNode = (from n in _nodesDictionary
                               where n.Key == next[0].Key
                               select n.Value).ToList()[0];
            }
            else
            {
                continue;
            }
        }
        index = 0;
        var cost = ColorIt(startNodeId, finishNodeId, nodesInObjects);
        
        return cost;
    }
    private static int index = 0;

    private static float ColorIt(long startId, long finishId, Dictionary<long, GameObject> nodes, Node currentNode = null, Colors colors = 0)
    {
        float cost = 0f;
        if (currentNode == null)
        {
            currentNode = nodes[finishId].GetComponent<Node>();
        }else if (currentNode.Id == startId)
        {
            _nodesList.Add(new List<string>());
            _nodesList.Last().Add(currentNode.Id.ToString());
            return currentNode.pin.weight;
        }
        else
        {
            switch (colors)
            {
                case Colors.orange:
                    nodes[currentNode.Id].GetComponent<SpriteRenderer>().color = new Color(1, 0.502f, 0);
                    break;
                case Colors.yellow:
                    nodes[currentNode.Id].GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;
                case Colors.green:
                    nodes[currentNode.Id].GetComponent<SpriteRenderer>().color = Color.green;
                    break;
                case Colors.cyan:
                    nodes[currentNode.Id].GetComponent<SpriteRenderer>().color = Color.cyan;
                    break;
                case Colors.magenta:
                    nodes[currentNode.Id].GetComponent<SpriteRenderer>().color = Color.magenta;
                    break;
                default:
                    nodes[currentNode.Id].GetComponent<SpriteRenderer>().color = Color.black;
                    break;
            }            
        }


        for (int i = 0; i < currentNode.pin.lightestId.Count; i++)
        {
            Color color = new Color(1, 1, 1);
            if (i == 0)
            {
                _ = ColorIt(startId, finishId, nodes, _nodesDictionary[currentNode.pin.lightestId[i]], colors);
                cost += currentNode.pin.weight;
            }
            else
            {
                _ = ColorIt(startId, finishId, nodes, _nodesDictionary[currentNode.pin.lightestId[i]], (Colors)(++index));
            }

        }
        return cost;
    }
}
