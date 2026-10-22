using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 挂在和 HeartShape、HeartGrow 相同的物体上。
// 玩家只需要移动鼠标，不需要按键，沿着虚线描完爱心即可。
public class HeartTrace : MonoBehaviour
{
    [Header("描线设置")]
    public float hitRadiusPixels = 35f;
    [Range(0.5f, 1f)] public float requiredPercent = 0.8f;
    public float traceTime = 5f;

    [Header("颜色")]
    public Color guideColor = new Color(1f, 0.4f, 0.75f, 0.35f);
    public Color filledColor = new Color(1f, 0.05f, 0.55f, 1f);
    public Color ringColor = new Color(1f, 0.25f, 0.75f, 1f);

    [Header("提示环和音效")]
    public float innerRingScale = 1.08f;
    public float outerRingScale = 1.18f;
    public float blinkSpeed = 5f;
    public AudioClip heartbeatClip;

    private LineRenderer guideLine;
    private LineRenderer filledLine;
    private LineRenderer innerRing;
    private LineRenderer outerRing;
    private AudioSource audioSource;
    private Vector3[] points;
    private bool[] touched;
    private int touchedCount;
    private float traceTimer;
    private bool tracing;
    private bool completed;

    IEnumerator Start()
    {
        // 等 HeartShape 先生成 LineRenderer 的点
        yield return null;

        guideLine = GetComponent<LineRenderer>();
        if (guideLine == null || guideLine.positionCount == 0)
        {
            Debug.LogError("HeartTrace 找不到 HeartShape 使用的 LineRenderer。");
            enabled = false;
            yield break;
        }

        points = new Vector3[guideLine.positionCount];
        touched = new bool[points.Length - 1];

        for (int i = 0; i < points.Length; i++)
            points[i] = guideLine.GetPosition(i);

        // 原来的线作为虚线底稿。
        guideLine.colorGradient = CreateGuideGradient();

        // 再叠一条线，只显示玩家已经描过的部分。
        GameObject filledObject = new GameObject("FilledHeartTrace");
        filledObject.transform.SetParent(transform, false);
        filledLine = filledObject.AddComponent<LineRenderer>();
        CopyLineSettings(guideLine, filledLine);
        filledLine.positionCount = points.Length;
        filledLine.SetPositions(points);
        filledLine.sortingOrder = guideLine.sortingOrder + 1;
        filledLine.colorGradient = CreateFilledGradient();

        innerRing = CreateRing("InnerDashedRing", innerRingScale, guideLine.sortingOrder + 2);
        outerRing = CreateRing("OuterDashedRing", outerRingScale, guideLine.sortingOrder + 2);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        innerRing.gameObject.SetActive(false);
        outerRing.gameObject.SetActive(false);
    }

    void Update()
    {
        if (completed || guideLine == null || points == null)
            return;

        // 爱心放大完成后才开始描线和计时。
        if (!tracing)
        {
            if (transform.localScale.x < 0.99f)
                return;

            tracing = true;
            innerRing.gameObject.SetActive(true);
            outerRing.gameObject.SetActive(true);

            if (heartbeatClip != null)
                audioSource.PlayOneShot(heartbeatClip);
        }

        float blink = Mathf.Lerp(0.25f, 1f,
            (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f);
        innerRing.colorGradient = CreateDashedGradient(ringColor, blink);
        outerRing.colorGradient = CreateDashedGradient(ringColor, blink);

        traceTimer += Time.deltaTime;

        if (traceTimer > traceTime)
        {
            Debug.Log("Heart failed: time is up.");
            enabled = false;
            return;
        }

        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Camera camera = Camera.main;

        // 不检查按键：移动鼠标经过哪一段，就填上哪一段。
        for (int i = 0; i < touched.Length; i++)
        {
            if (touched[i])
                continue;

            Vector3 a = GetScreenPoint(points[i], camera);
            Vector3 b = GetScreenPoint(points[i + 1], camera);
            float distance = DistanceToSegment(mousePosition, a, b);

            if (distance <= hitRadiusPixels)
            {
                touched[i] = true;
                touchedCount++;
            }
        }

        filledLine.colorGradient = CreateFilledGradient();

        if (touchedCount >= touched.Length * requiredPercent)
        {
            completed = true;
            filledLine.colorGradient = CreateSolidGradient(filledColor);
            Debug.Log("Heart complete!");
        }
    }

    Vector3 GetScreenPoint(Vector3 point, Camera camera)
    {
        Vector3 worldPoint = guideLine.useWorldSpace
            ? point
            : transform.TransformPoint(point);
        return camera.WorldToScreenPoint(worldPoint);
    }

    float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 direction = b - a;
        if (direction.sqrMagnitude == 0f)
            return Vector2.Distance(point, a);

        float t = Mathf.Clamp01(Vector2.Dot(point - a, direction) / direction.sqrMagnitude);
        return Vector2.Distance(point, a + direction * t);
    }

    Gradient CreateGuideGradient()
    {
        return CreateDashedGradient(guideColor, 1f);
    }

    Gradient CreateFilledGradient()
    {
        return CreateSegmentGradient(true, filledColor);
    }

    Gradient CreateSegmentGradient(bool useTouchedSegments, Color color)
    {
        List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>();
        int segmentCount = points.Length - 1;

        for (int i = 0; i < segmentCount; i++)
        {
            float position = (float)i / segmentCount;
            bool visible = useTouchedSegments
                ? touched[i]
                : (i / 2) % 2 == 0;
            float alpha = visible ? color.a : 0f;
            alphaKeys.Add(new GradientAlphaKey(alpha, position));
        }

        alphaKeys.Add(new GradientAlphaKey(0f, 1f));

        Gradient gradient = new Gradient();
        gradient.mode = GradientMode.Fixed;
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            alphaKeys.ToArray());
        return gradient;
    }

    Gradient CreateDashedGradient(Color color, float alphaMultiplier)
    {
        int segmentCount = points != null ? points.Length - 1 : 1;
        List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>();

        for (int i = 0; i < segmentCount; i++)
        {
            float position = (float)i / segmentCount;
            bool visible = (i / 2) % 2 == 0;
            float alpha = visible ? color.a * alphaMultiplier : 0f;
            alphaKeys.Add(new GradientAlphaKey(alpha, position));
        }

        alphaKeys.Add(new GradientAlphaKey(0f, 1f));

        Gradient gradient = new Gradient();
        gradient.mode = GradientMode.Fixed;
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            alphaKeys.ToArray());
        return gradient;
    }

    LineRenderer CreateRing(string ringName, float scale, int sortingOrder)
    {
        GameObject ringObject = new GameObject(ringName);
        ringObject.transform.SetParent(transform, false);

        LineRenderer ring = ringObject.AddComponent<LineRenderer>();
        CopyLineSettings(guideLine, ring);
        ring.positionCount = points.Length;
        ring.sortingOrder = sortingOrder;

        Vector3[] ringPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            ringPoints[i] = points[i] * scale;

        ring.SetPositions(ringPoints);
        ring.colorGradient = CreateDashedGradient(ringColor, 1f);
        return ring;
    }

    Gradient CreateSolidGradient(Color color)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(color.a, 0f),
                new GradientAlphaKey(color.a, 1f)
            });
        return gradient;
    }

    void CopyLineSettings(LineRenderer source, LineRenderer target)
    {
        target.useWorldSpace = source.useWorldSpace;
        target.widthCurve = source.widthCurve;
        target.widthMultiplier = source.widthMultiplier;
        target.numCapVertices = source.numCapVertices;
        target.numCornerVertices = source.numCornerVertices;
        target.alignment = source.alignment;
        target.textureMode = source.textureMode;
        target.material = source.material;
    }
}
