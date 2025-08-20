using System.Collections.Generic;
using UnityEngine;
using static KunaiBehaviour;
public class LockBehaviour : MonoBehaviour
{
    public static LockBehaviour lockBehaviour;
    

    private void Awake()
    {
        lockBehaviour = this;
    }

    [Header("UI Object")]
    [SerializeField] private int failCount = 3;

    [Header("GameObject")]
    [SerializeField] private GameObject lockHolder;
    [SerializeField] private GameObject lockPrefab;

    [Header("List and Array")]
    public List<GameObject> lockObjectList = new List<GameObject>();
    public List<GameObject> orbitingObjects = new List<GameObject>();

    [Header("Other")]
    public Animator animator;
    public int numberOfLocks; // �ڹ��� ����
    public float orbitSpeed; // ���� �ӵ� (���ӵ�, ����: degree/second)
    public float[] angles; // ������ ������ �迭
    private float orbitRadius = 2.0f; // ���� ������


    private void Start()
    {
        animator.SetInteger("HostageID", ScoreManager.instance.currentHostage);
        InitializeLocks();
    }
    private void InitializeLocks()
    {

        for (int i = 0; i < numberOfLocks; i++)
        {
            GameObject lockObject = Instantiate(lockPrefab, Vector2.zero, Quaternion.identity, lockHolder.transform);
            lockObjectList.Add(lockObject);
            orbitingObjects.Add(lockObject);
            lockObject.transform.position = CalculatePosition(angles[i]);

            Debug.Log("Object Added");
        }

    }

    private Vector2 CalculatePosition(float angle)
    {
        // ������ �������� ��ȯ
        float angleRad = angle * Mathf.Deg2Rad;

        // ������ ���� ���� ���� ���
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        // ������ ������ �Ÿ��� ���� ��ġ ��ȯ
        Vector2 spawnPosition = (Vector2)transform.position + direction * orbitRadius;
        Debug.DrawLine(transform.position, spawnPosition, Color.green); // ĳ��Ʈ ����� �Ÿ������� ������ �׸��ϴ�.
        return spawnPosition;
    }

    private void Update()
    {
        OrbitAroundCenter();

        if(numberOfLocks <= 0)
        {
            StageClear();
        }
    }

    private void OrbitAroundCenter()
    {
        lockHolder.transform.Rotate(new Vector3(0, 0, orbitSpeed * Time.deltaTime));
    }
    public void StageClear()
    {
        // Find 'StageRestart' Object and Active
        GameObject parentObject = GameObject.Find("UI_InGameMenu");

        Transform parentTransform = parentObject.transform;
        Transform childTransform = parentTransform.Find("StageClear");

        childTransform.gameObject.SetActive(true);


        Time.timeScale = 0f; // Pause Time when UI Active
    }
    public void StageReStart()
    {
        // Find 'StageRestart' Object and Active
        GameObject parentObject = GameObject.Find("UI_InGameMenu");

        Transform parentTransform = parentObject.transform;
        Transform childTransform = parentTransform.Find("StageRestart");

        childTransform.gameObject.SetActive(true);

        Time.timeScale = 0f; // Pause Time when UI Active
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kunai"))
        {
            if (failCount != 0)
            {
                AudioManager.instance.PlaySound(7);
                animator.SetTrigger("IsDamaged");
                if (kunaiBehaviour.isAllUse)
                    StageReStart();
                failCount--;
            }
            else
            {
                StageReStart();
            }
            Destroy(collision.gameObject);
        }
    }
}
