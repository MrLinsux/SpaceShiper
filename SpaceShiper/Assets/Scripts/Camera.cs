using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject target;
    public float speed;

    void FixedUpdate()
    {
        // ��� �� �������� ���� ���������� � ����� ������
        // � �������� ��� �� �������� �� FixedUpdate �� �������� �� fixedDeltaTim�
        transform.Translate(
            ((target.transform.position - this.transform.position) - Vector3.forward * 11) * speed * Time.fixedDeltaTime);
    }
}
