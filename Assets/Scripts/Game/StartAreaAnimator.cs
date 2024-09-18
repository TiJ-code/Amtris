using UnityEngine;

public class StartAreaAnimator : MonoBehaviour
{
    private float startY;

    public void Start()
    {
        startY = transform.position.y;
        transform.LeanMoveY(startY * 5, 0.75f)
            .setEaseInOutExpo()
            .setDelay(2f);
    }
}
