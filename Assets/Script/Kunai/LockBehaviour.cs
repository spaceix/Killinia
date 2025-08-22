using System.Collections.Generic;
using UnityEngine;
using static KunaiBehaviour;

public class LockBehaviour : MonoBehaviour
{
    // 싱글톤처럼 접근하기 위한 static 변수
    public static LockBehaviour lockBehaviour;

    private void Awake()
    {
        lockBehaviour = this;
    }

    [Header("UI Object")]
    [SerializeField] private int failCount = 3; // 쿠나이 충돌 시 버틸 수 있는 횟수

    [Header("GameObject")]
    [SerializeField] private GameObject lockHolder; // Lock 오브젝트들을 담는 부모
    [SerializeField] private GameObject lockPrefab; // Lock 프리팹

    [Header("List and Array")]
    public List<GameObject> lockObjectList = new List<GameObject>(); // 생성된 Lock 오브젝트 리스트
    public List<GameObject> orbitingObjects = new List<GameObject>(); // 회전 궤도를 도는 오브젝트 리스트

    [Header("Other")]
    public Animator animator;      // Lock 애니메이션
    public int numberOfLocks;      // 스테이지에 배치될 Lock 개수
    public float orbitSpeed;       // 회전 속도 (degree/second)
    public float[] angles;         // Lock 초기 배치 각도 배열
    private float orbitRadius = 2.0f; // Lock 회전 반지름

    private void Start()
    {
        // 현재 인질 ID를 애니메이터에 전달 (애니메이션 변환용)
        animator.SetInteger("HostageID", ScoreManager.instance.currentHostage);

        // Lock 오브젝트 초기화
        InitializeLocks();
    }

    // Lock 오브젝트 생성 및 초기 위치 설정
    private void InitializeLocks()
    {
        for (int i = 0; i < numberOfLocks; i++)
        {
            // Lock 생성 (lockHolder의 자식으로)
            GameObject lockObject = Instantiate(lockPrefab, Vector2.zero, Quaternion.identity, lockHolder.transform);

            // 리스트에 등록
            lockObjectList.Add(lockObject);
            orbitingObjects.Add(lockObject);

            // 각도 기반 위치 설정
            lockObject.transform.position = CalculatePosition(angles[i]);

            Debug.Log("Object Added");
        }
    }

    // 특정 각도(angle)에 해당하는 좌표 계산
    private Vector2 CalculatePosition(float angle)
    {
        // 각도를 라디안으로 변환
        float angleRad = angle * Mathf.Deg2Rad;

        // 방향 벡터 구하기
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        // 중심점(transform.position)에서 orbitRadius 만큼 떨어진 좌표 반환
        Vector2 spawnPosition = (Vector2)transform.position + direction * orbitRadius;

        // Scene 뷰에서 디버그 선 표시
        Debug.DrawLine(transform.position, spawnPosition, Color.green);

        return spawnPosition;
    }

    private void Update()
    {
        // Lock 오브젝트 회전
        OrbitAroundCenter();

        // 모든 Lock이 제거된 경우 스테이지 클리어 처리
        if (numberOfLocks <= 0)
        {
            StageClear();
        }
    }

    // 중심을 기준으로 lockHolder 전체를 회전시킴
    private void OrbitAroundCenter()
    {
        lockHolder.transform.Rotate(new Vector3(0, 0, orbitSpeed * Time.deltaTime));
    }

    // 스테이지 클리어 처리
    public void StageClear()
    {
        // UI_InGameMenu → StageClear 오브젝트 활성화
        GameObject parentObject = GameObject.Find("UI_InGameMenu");
        Transform parentTransform = parentObject.transform;
        Transform childTransform = parentTransform.Find("StageClear");
        childTransform.gameObject.SetActive(true);

        // 게임 정지
        Time.timeScale = 0f;
    }

    // 스테이지 재시작 처리
    public void StageReStart()
    {
        // UI_InGameMenu → StageRestart 오브젝트 활성화
        GameObject parentObject = GameObject.Find("UI_InGameMenu");
        Transform parentTransform = parentObject.transform;
        Transform childTransform = parentTransform.Find("StageRestart");
        childTransform.gameObject.SetActive(true);

        // 게임 정지
        Time.timeScale = 0f;
    }

    // 쿠나이와 충돌했을 때 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kunai"))
        {
            if (failCount != 0)
            {
                // 피격 사운드 재생 및 애니메이션 트리거
                AudioManager.instance.PlaySound(7);
                animator.SetTrigger("IsDamaged");

                // 모든 쿠나이를 사용했을 경우 스테이지 재시작
                if (kunaiBehaviour.isAllUse)
                    StageReStart();

                // 체력 감소
                failCount--;
            }
            else
            {
                // 남은 체력이 없으면 바로 재시작
                StageReStart();
            }

            // 쿠나이 제거
            Destroy(collision.gameObject);
        }
    }
}
