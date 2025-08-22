using System;
using UnityEngine;
using static LockBehaviour;

public class Lock : MonoBehaviour
{
    public Animator animator; // Lock 애니메이션 제어용

    private GameObject rotateObject; // 회전 기준이 되는 오브젝트 (Hostage)
    private BoxCollider2D boxCollider; // 충돌 감지용 Collider
    private bool isHit; // 쿠나이에 맞았는지 여부

    private void Start()
    {
        // 회전 기준이 되는 오브젝트를 찾음
        rotateObject = GameObject.Find("Hostage");

        // 자신의 Collider 가져오기
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Lock 자체가 Hostage와 반대 방향으로 회전하도록 설정
        // (LockBehaviour에서 lockHolder가 회전하므로, 개별 Lock이 반대로 회전해서 "제자리 도는 듯한 연출"을 만드는 로직)
        transform.Rotate(new Vector3(0, 0, -rotateObject.GetComponent<LockBehaviour>().orbitSpeed * Time.deltaTime));

        // 쿠나이에 맞았을 때 실행
        if (isHit)
        {
            // LockBehaviour의 리스트에서 해당 Lock 제거
            lockBehaviour.lockObjectList.Remove(gameObject);
            lockBehaviour.orbitingObjects.Remove(gameObject);

            // Lock 오브젝트 삭제
            Destroy(gameObject);

            // 중복 실행 방지를 위해 false 처리
            isHit = false;
        }
    }

    // 쿠나이와 충돌했을 때 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Kunai"))
        {
            // 사운드 재생
            AudioManager.instance.PlaySound(8);

            // LockBehaviour에서 전체 Lock 개수 감소
            lockBehaviour.numberOfLocks--;

            // 충돌을 막기 위해 Collider 제거
            Destroy(this.boxCollider);

            // 애니메이션 실행 (죽는 연출)
            animator.SetTrigger("IsDead");

            // 일정 시간(0.5초) 뒤 KunaiHitAction 실행 → 실제 삭제 처리
            Invoke("KunaiHitAction", 0.5f);
        }
    }

    // Lock 제거 작업 실행
    public void KunaiHitAction()
    {
        isHit = true;
    }
}
