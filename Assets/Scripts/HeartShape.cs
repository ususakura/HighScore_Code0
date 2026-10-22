using UnityEngine;

public class HeartShape : MonoBehaviour
{
    public int pointCount = 100;
    public float size = 0.1f;

    void Start()
    {
        LineRenderer line = GetComponent<LineRenderer>();
        line.positionCount = pointCount + 1;

        for (int i = 0; i <= pointCount; i++)
        {
            float t = Mathf.PI * 2f * i / pointCount;

            float x = 16f * Mathf.Pow(Mathf.Sin(t), 3f);
            float y = 13f * Mathf.Cos(t)
                    - 5f * Mathf.Cos(2f * t)
                    - 2f * Mathf.Cos(3f * t)
                    - Mathf.Cos(4f * t);

            line.SetPosition(i, new Vector3(x * size, y * size, 0f));
        }
    }
}