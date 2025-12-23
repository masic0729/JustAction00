using DG.Tweening;
using UnityEngine;

public class RoulletTest : MonoBehaviour
{
    private DOTweenAnimation tweenAnim;
    private float targetRotate = 100f;

    void Awake()
    {
        tweenAnim = GetComponent<DOTweenAnimation>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            tweenAnim.DOKill();

            tweenAnim.endValueV3= new Vector3(0, 0, targetRotate);
            tweenAnim.CreateTween();
            tweenAnim.DORestart();

            targetRotate += 100f;
        }
    }
}
