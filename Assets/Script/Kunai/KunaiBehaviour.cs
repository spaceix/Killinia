using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KunaiBehaviour : MonoBehaviour
{
    // 싱글톤처럼 접근하기 위한 static 변수
    public static KunaiBehaviour kunaiBehaviour;

    [Header("UI Object")]
    [SerializeField] private GameObject KunaiViewHolder; // UI에서 쿠나이 아이콘들을 담는 부모 오브젝트
    [SerializeField] private GameObject KunaiViewPrefab; // UI에서 개별 쿠나이 아이콘 프리팹

    [Header("GameObject")]
    [SerializeField] private GameObject KunaiHolder; // 실제 쿠나이들을 담는 부모 오브젝트
    [SerializeField] private GameObject KunaiPrefab; // 실제 쿠나이 프리팹
    [SerializeField] private GameObject LockRange;  // Lock 범위 오브젝트
    public HostageManager hostageManager;           // 인질 매니저 참조

    [Header("List")]
    [SerializeField] List<GameObject> KunaiViewList; // UI 쿠나이 아이콘 리스트
    [SerializeField] List<Kunai> KunaiList;          // 실제 쿠나이 객체 리스트

    [HideInInspector] public int numberOfKnifes; // 사용할 쿠나이 개수
    [HideInInspector] public bool isAllUse;      // 모든 쿠나이를 사용했는지 여부

    #region PrivateValue
    private LockBehaviour lockBehaviour; // Lock 관련 스크립트
    private int index = 0;               // 현재 사용할 쿠나이의 인덱스
    private bool isFade = true;          // Fade 연출 여부
    private float currentTime;           // 쿨타임 측정용 시간
    private float coolDown = 1f;         // 쿨타임 (1초)
    #endregion

    private void Awake()
    {
        // 인질 정보 업데이트
        hostageManager.UpdateHostageInfo();

        // LockBehaviour 가져오기
        lockBehaviour = LockRange.GetComponent<LockBehaviour>();

        // 싱글톤처럼 자기 자신 할당
        kunaiBehaviour = this;

        // 지정된 수(numberOfKnifes)만큼 쿠나이 생성
        for (int i = 0; i < numberOfKnifes; i++)
        {
            // 쿠나이 생성 (화면 아래쪽 위치)
            Vector2 knifePos = new Vector2(0, -4f);
            GameObject knife = Instantiate(KunaiPrefab, knifePos, Quaternion.identity, KunaiHolder.transform);

            // UI 아이콘 생성
            GameObject knifeView = Instantiate(KunaiViewPrefab, Vector2.zero, Quaternion.identity, KunaiViewHolder.transform);

            // 리스트에 추가
            KunaiViewList.Add(knifeView);
            Debug.Log("Object Add");
        }

        // 생성된 쿠나이들을 리스트에 담음
        foreach (Kunai knife in KunaiHolder.GetComponentsInChildren<Kunai>())
        {
            KunaiList.Add(knife);
            Debug.Log("Knife Add");
        }

        // 첫 번째 쿠나이만 보이고, 나머지는 숨김 처리
        for (int i = 1; i < KunaiList.Count; i++)
        {
            KunaiList[i].HideKunai();
        }
    }

    private void Update()
    {
        // 시간 경과 체크
        currentTime += Time.deltaTime;

        // 아직 사용할 쿠나이가 남아 있는 경우
        if (index < KunaiList.Count)
        {
            if (index == 0)
            {
                // 첫 번째 쿠나이는 바로 보이도록 처리
                KunaiList[index].EndShowKunai();
            }
            else if (isFade)
            {
                // Fade 연출로 다음 쿠나이를 보이게 함
                KunaiList[index].ShowKunai();
                isFade = false;
            }

            // 쿨타임이 지났을 때
            if (currentTime >= coolDown)
            {
                // 마우스 왼쪽 클릭 시 쿠나이 발사
                if (Input.GetMouseButtonDown(0))
                {
                    AudioManager.instance.PlaySound(9); // 사운드 재생
                    KunaiList[index].isMoving = true;   // 이동 시작
                    KunaiView();                        // UI 업데이트
                }
            }
        }
        else
        {
            // 모든 쿠나이를 다 사용한 경우
            isAllUse = true;
        }
    }

    // 쿠나이를 사용한 후 UI와 상태를 갱신하는 함수
    private void KunaiView()
    {
        KunaiList[index].isUse = false; // 현재 쿠나이는 아직 충돌 처리 전이므로 isUse 초기화
        index++;                        // 다음 쿠나이로 인덱스 이동
        isFade = true;                  // 다음 쿠나이를 Fade 연출로 보이게 하기 위해 true 설정

        // 남은 쿠나이 개수에 따라 UI 아이콘 색상 변경
        int viewIndex = KunaiViewList.Count - index;
        KunaiViewList[viewIndex].GetComponent<Image>().color = Color.black;

        Debug.Log("Knife Drop");
    }
}
