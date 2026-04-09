using UnityEngine;

// 무기 오브젝트에 부착하여 TrailRenderer의 활성화 및 비활성화를 제어한다
// 애니메이션 이벤트에서 OnTrailStart와 OnTrailEnd를 호출하여 타이밍을 맞춘다
public class WeaponTrailController : MonoBehaviour
{
    // 무기에 부착된 TrailRenderer 컴포넌트 참조
    [SerializeField] TrailRenderer trail;

    public void SetTrailData(TrailRenderer trailRenderer)
    {
        if (trailRenderer == null)
        {
            trail = null;
            return;
        }

        trail = trailRenderer;
        trail.emitting = false;
    }

    // 애니메이션 이벤트에서 호출 공격 스윙 구간 시작 시 트레일을 활성화한다
    public void OnTrailStart()
    {
        if (trail == null)
            return;

        trail.emitting = true;
    }

    // 애니메이션 이벤트에서 호출 공격 스윙 구간 종료 시 트레일을 비활성화한다
    public void OnTrailEnd()
    {
        if (trail == null)
            return;

        trail.emitting = false;
        // 남아있는 트레일 잔상을 즉시 제거한다
        //trail.Clear();
    }
}