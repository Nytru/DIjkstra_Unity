using TMPro;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private Transform[] _points;

    [SerializeField]
    private GameObject _textPref;


    private GameObject _textMesh;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetUpLine(Transform[] _points, float weight, bool show = true)
    {
        _lineRenderer.positionCount = _points.Length;
        this._points = _points;
        if (show)
        {
            _textMesh = Instantiate(_textPref, Vector3.zero, Quaternion.identity, this.transform);
            _textMesh.GetComponent<TextMeshPro>().text = weight.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_points != null)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                _lineRenderer.SetPosition(i, _points[i].position);
            }
            Vector2 a = _lineRenderer.GetPosition(0); // new Vector2(_points[0].position.x, _points[0].position.y);
            Vector2 b = _lineRenderer.GetPosition(1); // new Vector2(_points[1].position.x, _points[1].position.y);
            if (_textMesh != null)
            {
                _textMesh.transform.position= new Vector2(Mathf.Min(a.x, b.x) + Mathf.Abs(a.x - b.x) / 2, Mathf.Min(a.y, b.y) + Mathf.Abs(a.y - b.y) / 2);
            }          
        }
    }

    public void Destroy()
    {
        Destroy(_textPref);
    }
}
