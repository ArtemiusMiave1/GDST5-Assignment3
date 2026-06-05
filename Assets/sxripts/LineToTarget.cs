using UnityEngine;

public class LineToTarget : MonoBehaviour
{
    public Transform target;

    [Header("Line Settings")]
    public Color lineColor = Color.grey;
    public float width = 0.05f;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        if (line == null)
            line = gameObject.AddComponent<LineRenderer>();

        line.positionCount = 2;

        line.material = new Material(Shader.Find("Sprites/Default"));

        line.startWidth = width;
        line.endWidth = width;

        line.startColor = lineColor;
        line.endColor = lineColor;

        line.useWorldSpace = true;
    }

    void LateUpdate()
    {
        if (target == null) return;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
    }

    void OnValidate()
    {
        // updates in editor when you change values
        if (line != null)
        {
            line.startColor = lineColor;
            line.endColor = lineColor;

            line.startWidth = width;
            line.endWidth = width;
        }
    }
}