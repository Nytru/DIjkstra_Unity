using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;

public static class FileOpening
{
    public static Dictionary<long, GameObject> GetNodes(string path, GameObject pref)
    {
        Dictionary<long, GameObject> pairs = new Dictionary<long, GameObject>();

        StreamReader streamReader = new StreamReader(path);
        while (!streamReader.EndOfStream)
        {
            float X;
            float Y;
            long ID;
            string name;
            var line = streamReader.ReadLine();
            if (line.Contains("id"))
            {
                ID = Convert.ToInt64(line.Substring(line.IndexOf("(") + 1, line.IndexOf(")") - line.IndexOf("(") - 1));
                line = line.Remove(0, line.IndexOf(")") + 1);
                line = line.Remove(0, line.IndexOf("("));

                X = (float)Convert.ToDouble(line.Substring(line.IndexOf("(") + 1, line.IndexOf(';') - 1));
                Y = (float)Convert.ToDouble(line.Substring(line.IndexOf(';') + 1, line.IndexOf(')') - line.IndexOf(';') - 1));

                line = line.Remove(0, line.IndexOf("name"));

                name = line.Substring(line.IndexOf("(") + 1, line.IndexOf(')') - line.IndexOf("(") - 1);

                var obj = GameObject.Instantiate(pref, new Vector2(X, Y), Quaternion.identity);
                obj.GetComponent<Node>().Id = ID;
                obj.GetComponent<Node>().Name = name;


                pairs.Add(ID, obj);
            }
        }
        streamReader.Close();
        return pairs;
    }

    public static Dictionary<(long, long), float> GetWeights(string path)
    {
        var weights = new Dictionary<(long, long), float>();
        StreamReader streamReader = new StreamReader(path);

        while (!streamReader.EndOfStream)
        {
            long firstNode;
            long secondNode;
            float weight;

            var line = streamReader.ReadLine();
            if (line.Contains("between"))
            {
                firstNode = Convert.ToInt64(line.Substring(line.IndexOf('(') + 1, line.IndexOf(';') - line.IndexOf('(') - 1));

                secondNode = Convert.ToInt64(line.Substring(line.IndexOf(';') + 1, line.IndexOf(')') - line.IndexOf(';') - 1));

                line = line.Remove(0, line.IndexOf(")") + 1);
                line = line.Remove(0, line.IndexOf("("));

                weight = (float)Convert.ToDouble(line.Substring(line.IndexOf("(") + 1, line.IndexOf(")") - 1));
                weights.Add((firstNode, secondNode), weight);
            }
        }
        streamReader.Close();
        return weights;
    }

    public static void Save(string path, Dictionary<long, GameObject> nodes, Dictionary<(long, long), float> weights)
    {
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine("node:");
        foreach (var item in nodes)
        {
            writer.WriteLine($"id:({item.Key}) pos:({item.Value.transform.position.x};{item.Value.transform.position.y}) name:({item.Value.GetComponent<Node>().Name})");
        }
        writer.WriteLine("weights:");
        foreach (var item in weights)
        {
            writer.WriteLine($"between:({item.Key.Item1};{item.Key.Item2}) weight:({item.Value})");
        }
        writer.Close();
    }
}
