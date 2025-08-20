using System.Collections;
using UnityEngine;

public class Kunai : MonoBehaviour
{
    
    private Rigidbody2D rigid2D;
    private CapsuleCollider2D capsuleCollider2D;
    private float kunaiSpeed = 15;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isUse;

    #region ShowKnife
    private float duration = 0.15f;
    private float moveDistance = 1f; // �̵��� �Ÿ� (�������� 2 ����)
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Color endColor;
    private Vector3 startPosition;
    private Vector3 endPosition;
    #endregion

    private void Awake()
    {
        
        rigid2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isMoving = false;
        isUse = false;
    }
    void Start()
    {
        startColor = new Color(1, 1, 1, 0); // �ʱ� ���� (�����)
        endColor = Color.white; // ���� ���� (���)
        startPosition = transform.position; // �ʱ� ��ġ
        endPosition = startPosition + new Vector3(0, moveDistance, 0); // ���� ��ġ (���� 2 ����)   
    }

    private void Update()
    {
        if (gameObject.transform.position.y >= 10)
        {
            Destroy(gameObject);
        }
        if (isMoving)
        {
            Vector2 movement = new Vector2(0, 20);
            rigid2D.AddForce(movement * kunaiSpeed);
        }
    }

    public void ShowKunai()
    {
        StartCoroutine(FadeAndMoveCoroutine());
    }
    IEnumerator FadeAndMoveCoroutine()
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t); // EaseInOut ���

            // ���� ����
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);

            // ��ġ ����
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }
        EndShowKunai();
    }
    public void EndShowKunai()
    {
        spriteRenderer.color = endColor;
        transform.position = endPosition;
    }
    public void HideKunai()
    {
        spriteRenderer.color = startColor;
        Debug.Log("Knife Hide");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lock"))
        {
            Destroy(gameObject);
        }
        else
        {
            isUse = true;
        }
    }
}
