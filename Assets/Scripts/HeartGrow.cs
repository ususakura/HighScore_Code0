using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class HeartGrow : MonoBehaviour
{
    public float growTime = 2f;

    RectTransform rectTransform;
    TMP_Text heartText;

    float timer;
    bool canTrace = false;
    bool mouseWasInside = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        heartText = GetComponent<TMP_Text>();

        rectTransform.localScale = Vector3.zero;
        heartText.color = new Color(1f, 0.5f, 0.7f);
    }

    // Update is called once per frame
    void Update()
    {

        if (!canTrace)
        {
            timer += Time.deltaTime;

        float progress = timer / growTime;
        progress = Mathf.Clamp01(progress);

        rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);

        if(progress >= 1f)
            {
                canTrace = true;
                heartText.color = Color.pink;
                print("dokidoki");
            }
        }

        if (canTrace && Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            bool mouseIsInside = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, null);

            if (mouseIsInside && !mouseWasInside){ 
                print("come inside now");
                }
            mouseWasInside = mouseIsInside;
        }
        

    }
}
