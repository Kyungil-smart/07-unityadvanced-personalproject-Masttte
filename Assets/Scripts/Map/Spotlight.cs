using DG.Tweening;
using UnityEngine;

/// <summary>
/// 유닛 집중 조명 모듈. 카메라를 대상에 집중시키고 해제합니다
/// Main Camera에 부착합니다
/// </summary>
public class Spotlight : MonoBehaviour
{
    public static Spotlight Instance;

    Camera cam;
    public Transform camPos;
    public bool followTarget;

    [Header("줌")]
    public float defaultSize = 5f;
    public float focusSize = 3.75f;

    [Header("연출")]
    public float moveTime = 0.47f;
    public bool smooth = true;
    public float smoothSpeed = 5f;

#if UNITY_EDITOR
    private void Reset()
    {
        camPos = GameObject.FindWithTag("Player").transform;
    }
#endif
    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    /// <summary>
    /// 대상에 카메라를 집중시킵니다 (확대 + 이동)
    /// </summary>
    public async Awaitable FocusOn(Transform target, bool isRange = false)
    {
        followTarget = true;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y + 1.1f, 0);
        camPos.transform.DOMove(targetPos, moveTime).SetEase(Ease.OutCubic);

        if (!isRange)
            DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, focusSize, moveTime).SetEase(Ease.OutCubic);

        await Awaitable.WaitForSecondsAsync(moveTime);
    }

    /// <summary>
    /// 집중 조명을 해제하고 카메라를 플레이어 에게 줍니다
    /// </summary>
    public async Awaitable Release()
    {
        DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, defaultSize, moveTime).SetEase(Ease.OutCubic);
        Vector3 playerPos = new Vector3(Map.Instance.player.transform.position.x, Map.Instance.player.transform.position.y + 4.2f, 0);
        camPos.DOMove(playerPos, moveTime).SetEase(Ease.OutCubic);

        followTarget = true;
        await Awaitable.WaitForSecondsAsync(moveTime);
        followTarget = false;
    }

    void LateUpdate()
    {
        if (!followTarget) return;

        Vector3 desired = new Vector3(
            camPos.position.x,
            camPos.position.y,
            -10f);

        transform.position = smooth
            ? Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime)
            : desired;
    }
}
