using UnityEngine;

/// <summary>
/// 배경 패럴랙스 효과. 구름등 배경이 스스로 움직입니다.
/// </summary>
public class Parallax : MonoBehaviour
{
    public float independentSpeed;
    Vector3 pos;

    private void Awake()
    {
        pos = transform.localPosition;
    }

    void LateUpdate()
    {
        pos.x += independentSpeed * Time.deltaTime;
        transform.localPosition = pos;
    }
}
