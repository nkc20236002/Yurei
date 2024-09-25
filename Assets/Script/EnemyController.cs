using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2.0f;  // 敵の移動速度
    public Transform player;  // プレイヤーのTransformを参照
    public float chaseRange = 5.0f;  // プレイヤーを追いかける範囲
    private PlayerStateController playerStateController;  // プレイヤーの状態コントローラー

    private bool playerInRange = false;  // プレイヤーが検知範囲内にいるか
    private float currentCooldown = 0.0f;  // クールダウンの時間
    private float cooldownDuration = 1.0f;  // クールダウンの長さ
    private void Start()
    {
        // プレイヤーの状態コントローラーを取得
        playerStateController = player.GetComponent<PlayerStateController>();
    }
    private void Update()
    {
        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // プレイヤーが幽霊状態かつ指定範囲内にいるかチェック
        if (playerStateController.IsGhostState() && distanceToPlayer <= chaseRange)
        {
            ChasePlayer();  // 幽霊状態かつ範囲内ならプレイヤーを追いかける
        }

        // クールダウン処理
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;  // 時間を減算
        }
    }

    // プレイヤーを追いかけるメソッド
    private void ChasePlayer()
    {
        // プレイヤーとの距離を計算
        float step = speed * Time.deltaTime;  // 一フレームの移動距離
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);  // プレイヤーに向かって移動
    }

    // プレイヤーの状態を確認するためのメソッド
    public void SetPlayerInRange(bool isInRange)
    {
        playerInRange = isInRange;  // プレイヤーが検知範囲内にいるか設定
    }
}
