using DG.Tweening;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private void OnEnable()
    {
        SetDisabled();
    }

    protected void SetDisabled()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }

    protected void SetEnabled()
    {
        canvasGroup.DOFade(1, fadeDuration)
        .OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }
}
