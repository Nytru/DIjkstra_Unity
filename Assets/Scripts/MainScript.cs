using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class MainScript : MonoBehaviour
{
    #region SerializeField
    [SerializeField]
    private ConnectorManager _connectorManager;


    [SerializeField]
    private GameObject _TextMeshPro;

    [SerializeField]
    private GameObject _AnsTextGO;


    [SerializeField]
    private GameObject _nodePref;
    #endregion


    #region Fields
    private Dictionary<long, GameObject> _nodesInObjects = new Dictionary<long, GameObject>();


    private Dictionary<(long firstPoint, long secondPoint), float> _weights = new Dictionary<(long, long), float>();


    private GameObject _selectedObject = null;


    private Node[] _selectedPath = new Node[2];


    private string _path;
    #endregion


    private void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.O.ToString())))
        {
            string path = EditorUtility.OpenFilePanel("Open map", @"C:\Users\Lein\Desktop", "key");
            _path = path;
            if (path != "")
            {
                _AnsTextGO.GetComponent<TextMeshPro>().text = "";

                foreach (var item in _nodesInObjects)
                {
                    Destroy(item.Value);
                }
                _nodesInObjects.Clear();
                _connectorManager.Destroy();


                _nodesInObjects = FileOpening.GetNodes(path, _nodePref);
                foreach (var item in _nodesInObjects)
                {
                    var text = Instantiate(_TextMeshPro, item.Value.transform);
                    text.transform.localPosition = new Vector2(Vector2.zero.x + (item.Value.transform.localScale.x  / 4) * 3 , 
                                                               Vector2.zero.y + (item.Value.transform.localScale.y / 4) * 3);

                    text.GetComponent<TextMeshPro>().text = item.Value.GetComponent<Node>().Name.ToString();
                }
                _weights = FileOpening.GetWeights(path);
                _connectorManager.Connect(_nodesInObjects, _weights, false);
                ConnectNodes(_nodesInObjects, _weights);
            }
        }


        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.I.ToString())))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 
                                                         Camera.main.transform.position.y + 0.5f,
                                                         Camera.main.transform.position.z);
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.K.ToString())))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                                                         Camera.main.transform.position.y - 0.5f,
                                                         Camera.main.transform.position.z);
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.J.ToString())))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - 0.5f,
                                                         Camera.main.transform.position.y,
                                                         Camera.main.transform.position.z);
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.L.ToString())))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + 0.5f,
                                                         Camera.main.transform.position.y,
                                                         Camera.main.transform.position.z);
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.C.ToString())))
        {
            if (_selectedPath[0] != null && _selectedPath[1] != null)
            {
                var ans = FindPath(_selectedPath[0], _selectedPath[1]);
                var text = _AnsTextGO.GetComponent<TextMeshPro>();
                if (text != null)
                {
                    text.text = "Длительность: " + ans.ToString();
                }
            }           
        }

        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.S.ToString())))
        {
            FileOpening.Save(_path, _nodesInObjects, _weights);
        }
    }

    /// <summary>
    /// Adding neighbors and cost to them
    /// </summary>
    /// <param name="_nodesInObjects"></param>
    /// <param name="_weights"></param>
    /// <exception cref="Exception"></exception>
    private void ConnectNodes(Dictionary<long, GameObject> _nodesInObjects, Dictionary<(long, long), float> _weights)
    {
        foreach (var item in _weights)
        {
            try
            {
                _nodesInObjects[item.Key.Item1].GetComponent<Node>().AwaliblePath.Add((item.Key.Item2, item.Value));
                _nodesInObjects[item.Key.Item2].GetComponent<Node>().AwaliblePath.Add((item.Key.Item1, item.Value));
            }
            catch (Exception)
            {
                throw new Exception("Cannot connect nodes: " + item.Key.Item1 + " " + item.Key.Item2);
            }
        }
    }


    private float FindPath(Node startPOint, Node finishPoint)
    {
        var ans = Algorithm.Solve(startPOint.Id, finishPoint.Id, new Dictionary<long, GameObject>(_nodesInObjects));
        return ans;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            var cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            if (cam != null)
            {
                cam.orthographicSize -= 0.5f;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            var cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            if (cam != null)
            {
                cam.orthographicSize += 0.5f;
            }
        }
        
        foreach (var item in _nodesInObjects)
        {
            if (Input.GetMouseButtonDown(1) && (Mathf.Pow(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - item.Value.transform.position.x, 2) +
                            Mathf.Pow(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - item.Value.transform.position.y, 2)) <=
                            Mathf.Pow(item.Value.transform.localScale.x / 2, 2))
            {
                foreach (var node in _nodesInObjects)
                {
                    if (node.Value.GetComponent<SpriteRenderer>().color != Color.red & node.Value.GetComponent<SpriteRenderer>().color != Color.blue)
                    {
                        node.Value.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }

                if (_selectedPath[0] == null)
                {
                    _selectedPath[0] = item.Value.GetComponent<Node>();
                    item.Value.GetComponent<SpriteRenderer>().color = Color.blue;
                }
                else if (_selectedPath[0] == item.Value.GetComponent<Node>())
                {
                    _selectedPath[0] = null;
                    item.Value.GetComponent<SpriteRenderer>().color = Color.white;
                }
                else
                {
                    _nodesInObjects[_selectedPath[0].Id].GetComponent<SpriteRenderer>().color = Color.white;
                    _selectedPath[0] = item.Value.GetComponent<Node>();
                    item.Value.GetComponent<SpriteRenderer>().color = Color.blue;
                }
            }

            if (Input.GetMouseButtonDown(2) && (Mathf.Pow(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - item.Value.transform.position.x, 2) +
                            Mathf.Pow(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - item.Value.transform.position.y, 2)) <=
                            Mathf.Pow(item.Value.transform.localScale.x / 2, 2))
            {
                foreach (var node in _nodesInObjects)
                {
                    if (node.Value.GetComponent<SpriteRenderer>().color != Color.red & node.Value.GetComponent<SpriteRenderer>().color != Color.blue)
                    {
                        node.Value.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
                if (_selectedPath[1] == null)
                {
                    _selectedPath[1] = item.Value.GetComponent<Node>();
                    item.Value.GetComponent<SpriteRenderer>().color = Color.red;
                }
                else if (_selectedPath[1] == item.Value.GetComponent<Node>())
                {
                    _selectedPath[1] = null;
                    item.Value.GetComponent<SpriteRenderer>().color = Color.white;
                }
                else
                {
                    _nodesInObjects[_selectedPath[1].Id].GetComponent<SpriteRenderer>().color = Color.white;

                    _selectedPath[1] = item.Value.GetComponent<Node>();
                    item.Value.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }



            if (Input.GetMouseButtonDown(0) && (Mathf.Pow(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - item.Value.transform.position.x, 2) +
                        Mathf.Pow(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - item.Value.transform.position.y, 2)) <=
                        Mathf.Pow(item.Value.transform.localScale.x / 2, 2))
            {
                _selectedObject = item.Value;
                _selectedObject.GetComponent<SpriteRenderer>().color = new Color(_selectedObject.GetComponent<SpriteRenderer>().color.r * 0.8275f,
                                                                                 _selectedObject.GetComponent<SpriteRenderer>().color.g * 0.8275f,
                                                                                 _selectedObject.GetComponent<SpriteRenderer>().color.b * 0.8275f);
            }

            if (Input.GetMouseButton(0))
            {
                if (_selectedObject != null)
                {
                    _selectedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0F));
                    _selectedObject.transform.position = new Vector3(_selectedObject.transform.position.x, _selectedObject.transform.position.y, 0);  
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_selectedObject != null)
                {
                    _selectedObject.GetComponent<SpriteRenderer>().color = new Color(_selectedObject.GetComponent<SpriteRenderer>().color.r / 0.8275f,
                                                                                     _selectedObject.GetComponent<SpriteRenderer>().color.g / 0.8275f,
                                                                                     _selectedObject.GetComponent<SpriteRenderer>().color.b / 0.8275f);
                    _selectedObject = null;
                }
            }
        }       
    }
}
