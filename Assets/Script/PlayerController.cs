using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�ړ����x")]
    [SerializeField] private float moveSpeed = 5f;  // �ړ����x
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ���͂��擾 (�����Ɛ���)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // �����̃x�N�g�����Z�b�g
        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // �v���C���[���ړ�
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
