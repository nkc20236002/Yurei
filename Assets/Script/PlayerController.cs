using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 5f;  // 移動速度
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 入力を取得 (水平と垂直)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // 動きのベクトルをセット
        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // プレイヤーを移動
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
