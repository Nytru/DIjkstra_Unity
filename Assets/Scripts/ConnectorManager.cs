using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _lineRendererPref;

    private List<GameObject> _lines = new List<GameObject>();

    public void Connect(Dictionary<long, GameObject> nodes, Dictionary<(long, long), float> weights, bool show = true)
    {
        foreach (var item in weights)
        {
            if (!nodes.ContainsKey(item.Key.Item1) | !nodes.ContainsKey(item.Key.Item2))
            {
                continue;
            }
            var firstNode = nodes[item.Key.Item1];
            var secondNode = nodes[item.Key.Item2];

            var lineRend = Instantiate(_lineRendererPref, new Vector3(0, 0, 0), Quaternion.identity, transform);
            _lines.Add(lineRend);
            var scriptFromLineRend = lineRend.GetComponent<LineController>();
            scriptFromLineRend.SetUpLine(new Transform[] { firstNode.transform, secondNode.transform }, item.Value, show);
        }
    }

    public void Destroy()
    {
        foreach (var item in _lines)
        {
            Destroy(item);
        }
        _lines.Clear();
    }
}
