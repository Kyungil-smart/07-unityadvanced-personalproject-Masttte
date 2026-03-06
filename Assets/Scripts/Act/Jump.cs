using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "Jump", menuName = "Action/Jump")]
public class Jump : ActBase
{
    public float height = 1f;
    public float time = 0.15f;

    public override async Awaitable Act()
    {
        // 닷트윈 사용 리펙토링
        Vector3 origin = owner.transform.localPosition;
        owner.transform.DOLocalMoveY(origin.y + height, time).SetEase(Ease.OutCubic);

        await Awaitable.WaitForSecondsAsync(time);

        owner.transform.DOLocalMoveY(origin.y, time).SetEase(Ease.OutCubic);
        owner.Lucidity = 0; // 프로퍼티 즉시 호출(문제 없음)

        await Awaitable.WaitForSecondsAsync(time);
    }
}
