using UnityEngine;
public class LockAngle : MonoBehaviour
{
    private float distance = 10.0f; // 캐릭터로부터의 거리
    public Vector2 Angle(float angle)
    {
        // 입력받은 각도를 라디안 단위로 변환
        float angleRad = angle * Mathf.Deg2Rad;

        // 각도를 기준으로 방향 벡터를 구함 (단위 벡터)
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        // 방향 벡터 * 거리만큼 현재 오브젝트의 위치에서 떨어진 좌표를 계산
        Vector2 spawnPosition = (Vector2)transform.position + direction * distance;

        // Scene 뷰에서 현재 위치와 spawnPosition 사이를 초록색 선으로 그려 확인
        Debug.DrawLine(transform.position, spawnPosition, Color.green);

        // 최종 좌표 반환
        return spawnPosition;
    }
}
