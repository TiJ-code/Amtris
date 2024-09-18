using UnityEngine;

public class ProgressbarAnimator : MonoBehaviour
{
    public static ProgressbarAnimator instance;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            Infrastructure.IsStartUpComplete = true;
        }
    }

    private RectTransform animRectTransform;

    public void AnimateProgressbar(RectTransform progressbarRectTransform, float maxWidth)
    {
        animRectTransform = progressbarRectTransform;
        animRectTransform.sizeDelta = new Vector2(0, progressbarRectTransform.sizeDelta.y);
        animRectTransform.gameObject.SetActive(true);
        LeanTween.value(0f, maxWidth, 1f)
            .setOnUpdate((float value) =>
            {
                animRectTransform.sizeDelta = new Vector2(value, progressbarRectTransform.sizeDelta.y);
            })
            .setEaseInOutExpo()
            .setOnComplete(() => InvokeDisableProgressbar());
    }

    private void InvokeDisableProgressbar()
    {
        Invoke("DisableProgressbar", 0.5f);
    }

    private void DisableProgressbar()
    {
        animRectTransform.gameObject.SetActive(false);
    }
}
