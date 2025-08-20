using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LockAngle : MonoBehaviour
{
    private float distance = 10.0f; // ĳ��Ʈ �Ÿ�
    public Vector2 Angle(float angle)
    {
        // ������ �������� ��ȯ
        float angleRad = angle * Mathf.Deg2Rad;

        // ������ ���� ���� ���� ���
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));


        // �浹�� ������ ������ ������ �Ÿ��� ������ ����
        Vector2 spawnPosition = (Vector2)transform.position + direction * distance;
        Debug.DrawLine(transform.position, spawnPosition, Color.green); // ĳ��Ʈ ����� �Ÿ������� ������ �׸��ϴ�.
        return spawnPosition;
    }
}
