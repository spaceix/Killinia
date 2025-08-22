using System.Collections;
using UnityEngine;

public class Kunai : MonoBehaviour
{
    // Rigidbody와 Collider 참조
    private Rigidbody2D rigid2D;
    private CapsuleCollider2D capsuleCollider2D;

    // 이동 속도
    private float kunaiSpeed = 15;

    // 외부에서 확인할 수 있지만 인스펙터에는 표시하지 않음
    [HideInInspector] public bool isMoving; // 현재 쿠나이가 움직이고 있는지 여부
    [HideInInspector] public bool isUse;    // 사용되었는지 여부 (충돌 등)

    #region ShowKnife (쿠나이를 보여주는 연출 관련 변수)
    private float duration = 0.15f;     // 페이드 및 이동에 걸리는 시간
    private float moveDistance = 1f;    // 연출 중 이동 거리 (위쪽 방향)

    private SpriteRenderer spriteRenderer; // 색상 변경을 위한 SpriteRenderer
    private Color startColor;              // 투명 상태 색상
    private Color endColor;                // 완전 보이는 상태 색상
    private Vector3 startPosition;         // 시작 위치
    private Vector3 endPosition;           // 종료 위치
    #endregion

    private void Awake()
    {
        // 컴포넌트 가져오기
        rigid2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 초기 상태 세팅
        isMoving = false;
        isUse = false;
    }

    void Start()
    {
        // 색상 초기화
        startColor = new Color(1, 1, 1, 0); // 완전히 투명
        endColor = Color.white;             // 불투명(흰색)

        // 위치 초기화
        startPosition = transform.position;                         // 현재 위치
        endPosition = startPosition + new Vector3(0, moveDistance, 0); // 위쪽으로 moveDistance만큼 이동한 위치
    }

    private void Update()
    {
        // y좌표가 10 이상 올라가면 삭제
        if (gameObject.transform.position.y >= 10)
        {
            Destroy(gameObject);
        }

        // 이동 중일 때 위쪽 방향으로 힘을 가해 날아가게 함
        if (isMoving)
        {
            Vector2 movement = new Vector2(0, 20);
            rigid2D.AddForce(movement * kunaiSpeed);
        }
    }

    // 쿠나이를 화면에 보이도록 하는 연출 시작
    public void ShowKunai()
    {
        StartCoroutine(FadeAndMoveCoroutine());
    }

    // 페이드 인 + 이동 코루틴
    IEnumerator FadeAndMoveCoroutine()
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // SmoothStep(EaseInOut) 보간 적용
            t = t * t * (3f - 2f * t);

            // 색상 보간 (투명 → 불투명)
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);

            // 위치 보간 (startPosition → endPosition)
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 연출 종료 후 최종 상태 고정
        EndShowKunai();
    }

    // 쿠나이 연출이 끝났을 때 최종 위치와 색상 고정
    public void EndShowKunai()
    {
        spriteRenderer.color = endColor;
        transform.position = endPosition;
    }

    // 쿠나이를 숨김 (투명 처리)
    public void HideKunai()
    {
        spriteRenderer.color = startColor;
        Debug.Log("Knife Hide");
    }

    // 충돌 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Lock" 태그를 가진 오브젝트에 부딪히면 파괴
        if (other.CompareTag("Lock"))
        {
            Destroy(gameObject);
        }
        else
        {
            // 다른 오브젝트와 충돌 시 사용 상태로 변경
            isUse = true;
        }
    }
}
