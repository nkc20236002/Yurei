using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerStateController : MonoBehaviour
{
    public GameObject[] enemies;  // 敵オブジェクトのリスト
    public Text remainingSwitchText;  // 残り切り替え回数を表示するテキスト
    public Sprite humanSprite;  // 人状態のスプライト
    public Sprite ghostSprite;  // 幽霊状態のスプライト
    public Color ghostColor = Color.white;  // 幽霊状態のカメラ背景色
    public Color humanColor = Color.black;  // 人状態のカメラ背景色

    public Collider2D wallCollider;  // 選択可能な壁のコライダー

    private bool isGhostState = false; // プレイヤーの状態フラグ
    private int remainingSwitches = 5;  // 残りの切り替え回数
    private bool canSwitchState = true; // 状態切り替えの可否フラグ
    private float switchCooldown = 1.0f;  // 切り替えのクールダウン時間
    private float currentCooldown = 0.0f;  // 現在のクールダウン時間
    private float fadeDuration = 1.0f;  // フェード時間
    private Camera mainCamera;  // メインカメラ
    private SpriteRenderer spriteRenderer;  // プレイヤーのスプライトレンダラー
    private Collider2D playerCollider;  // プレイヤーのコライダー

    private void Start()
    {
        mainCamera = Camera.main;  // メインカメラを取得
        spriteRenderer = GetComponent<SpriteRenderer>();  // スプライトレンダラーを取得
        playerCollider = GetComponent<Collider2D>();  // プレイヤーのコライダーを取得

        // 初期状態の設定
        mainCamera.backgroundColor = humanColor;  // 初期背景色を設定
        spriteRenderer.sprite = humanSprite;  // 初期スプライトを設定
        UpdateSwitchText();
        SetColliderState(true);  // 初期状態は人状態でコライダーを有効にする
    }

    void Update()
    {
        // クールダウン中であればカウントダウン
        if (!canSwitchState)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                canSwitchState = true;  // クールダウン終了
            }
        }

        // スペースキーで状態を切り替える
        if (Input.GetKeyDown(KeyCode.Space) && canSwitchState)
        {
            ToggleState();
        }

        // 残り切り替え回数のテキストを更新
        UpdateSwitchText();
    }

    // 状態を切り替えるメソッド
    void ToggleState()
    {
        if (isGhostState) // 幽霊状態から人状態に戻る場合
        {
            isGhostState = false;  // 幽霊状態を解除
        }
        else // 人状態から幽霊状態に切り替える場合
        {
            if (remainingSwitches > 0)  // 残り切り替え回数があれば減らす
            {
                remainingSwitches--;
                isGhostState = true;  // 幽霊状態に切り替え
            }
        }

        // 敵の表示・非表示を切り替える
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(isGhostState);
        }

        // カメラの背景色をフェード処理で変更
        StartCoroutine(FadeBackground(isGhostState ? ghostColor : humanColor));

        // プレイヤーのスプライトを変更
        spriteRenderer.sprite = isGhostState ? ghostSprite : humanSprite;

        // 衝突判定の状態を変更
        SetColliderState(!isGhostState); // 人状態のときのみ衝突判定を有効にする

        // 壁コライダーの有効/無効を設定
        SetWallColliderState(isGhostState);

        // 切り替え後にクールダウンを開始
        canSwitchState = false;
        currentCooldown = switchCooldown;
    }

    // カメラの背景色のフェード処理
    IEnumerator FadeBackground(Color targetColor)
    {
        Color currentColor = mainCamera.backgroundColor;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            mainCamera.backgroundColor = Color.Lerp(currentColor, targetColor, timer / fadeDuration);
            yield return null;
        }
        mainCamera.backgroundColor = targetColor;  // 最終的な色を設定
    }

    // 残り切り替え回数の表示を更新
    void UpdateSwitchText()
    {
        if (remainingSwitchText != null)
        {
            remainingSwitchText.text = "残り切り替え数: " + remainingSwitches;
        }
    }

    // 衝突判定の有効/無効を設定するメソッド
    private void SetColliderState(bool state)
    {
        playerCollider.enabled = state;  // プレイヤーのコライダーを有効/無効にする
    }

    // 壁コライダーの有効/無効を設定するメソッド
    private void SetWallColliderState(bool isGhost)
    {
        if (wallCollider != null)
        {
            wallCollider.enabled = !isGhost;  // 幽霊状態のときは壁コライダーを無効にする
        }
    }

    // 敵に触れたときの処理
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 幽霊状態で敵に触れた場合にゲームオーバー
        if (isGhostState && other.CompareTag("Enemy"))
        {
            GameOver();  // ゲームオーバー処理を呼び出し
        }

        // 幽霊状態のとき特定のコライダーを貫通
        if (isGhostState && other.CompareTag("PassThrough"))
        {
            Physics2D.IgnoreCollision(playerCollider, other, true); // コライダーを無視
        }
    }

    // ゲームオーバー処理
    public void GameOver()
    {
        // ゲームオーバーシーンに移行
        SceneManager.LoadScene("GameOverScene");  // ゲームオーバーシーンに移行
    }

    // 幽霊状態かどうかを返すメソッド
    public bool IsGhostState()
    {
        return isGhostState;
    }
}
