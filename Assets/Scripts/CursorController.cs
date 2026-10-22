using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController: MonoBehaviour
{   void Start()
    {
        UnityEngine.Cursor.visible = false;    }

    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
    }



    }
