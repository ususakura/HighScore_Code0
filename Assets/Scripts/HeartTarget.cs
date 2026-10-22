using UnityEngine;

public class HeartTarget : MonoBehaviour
{
    [SerializeField] float fallSpeed = 2f;
    [SerializeField] float missY = -6f;

    GameController gameController;
    bool alreadyResolved;

    public void Initialize(GameController controller)
    {
        gameController = controller;
    }

    void Update()
    {
        transform.position +=
            Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y <= missY)
        {
            Miss();
        }
    }

    public void Slice()
    {
        if (alreadyResolved)
            return;

        alreadyResolved = true;

        gameController.RegisterScore();
        Destroy(gameObject);
    }

    void Miss()
    {
        if (alreadyResolved)
            return;

        alreadyResolved = true;

        gameController.RegisterMiss();
        Destroy(gameObject);
    }
}