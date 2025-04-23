using DG.Tweening;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<ParticleSystem>().Play();

            transform.DOScale(Vector3.zero, 1f).SetDelay(0.2f);

            transform.DOMoveY(1, 0.5f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
