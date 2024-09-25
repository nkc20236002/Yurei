using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateController : MonoBehaviour
{
    // Enumで状態を管理
    public enum PlayerState { Human, Ghost }
    public PlayerState currentState;

    // 状態切り替えに使うキー
    public KeyCode switchKey = KeyCode.Space;

    // 壁のコライダー (人状態でのみ物理的に影響を与える)
    public Collider2D wallCollider;

    // スプライトレンダラー (外見を変えるために使用)
    private SpriteRenderer spriteRenderer;

    // カメラの背景色変更用
    private Camera mainCamera;

    // 人状態と幽霊状態の背景色
    public Color humanBackgroundColor = Color.black; // 人状態では黒
    public Color ghostBackgroundColor = Color.white; // 幽霊状態では白

    // 状態に応じた見た目のスプライトを設定
    public Sprite humanSprite;
    public Sprite ghostSprite;

    // 状態切り替えに関する制限
    public int maxSwitchCount = 5; // 最大切り替え回数
    private int currentSwitchCount; // 現在の切り替え回数

    public float switchCooldown = 1f; // クールタイム（秒）
    private bool canSwitch = true; // 切り替え可能かどうかのフラグ

    // 背景色のフェードにかかる時間
    public float fadeDuration = 1f;

    // UI テキスト
    public Text switchCountText; // 残りの使用回数を表示するテキスト

    void Start()
    {
        // 最初は人状態に設定
        currentState = PlayerState.Human;

        // SpriteRendererを取得
        spriteRenderer = GetComponent<SpriteRenderer>();

        // カメラの参照を取得
        mainCamera = Camera.main;

        // 人状態に応じた初期設定
        SetHumanState();

        // 切り替え回数を初期化
        currentSwitchCount = 0;

        // 残りの使用回数をUIに表示
        UpdateSwitchCountText();
    }

    void Update()
    {
        // クールタイム中か、切り替え回数が最大回数に達しているかどうかを確認
        if (canSwitch && currentSwitchCount < maxSwitchCount && Input.GetKeyDown(switchKey))
        {
            if (currentState == PlayerState.Human)
            {
                SetGhostState();
            }
            else
            {
                SetHumanState();
            }

            // 切り替え回数を増加
            currentSwitchCount++;

            // 残りの使用回数をUIに更新
            UpdateSwitchCountText();

            // クールタイムを開始
            StartCoroutine(SwitchCooldown());
        }
    }

    // クールタイム処理（1秒間切り替えを無効化）
    private System.Collections.IEnumerator SwitchCooldown()
    {
        canSwitch = false; // 切り替え不可
        yield return new WaitForSeconds(switchCooldown); // クールタイム待機
        canSwitch = true; // 切り替え可能
    }

    // 人状態に切り替え
    void SetHumanState()
    {
        currentState = PlayerState.Human;
        wallCollider.enabled = true; // 壁にぶつかるようにする
        spriteRenderer.sprite = humanSprite; // 人状態の見た目に変更

        // 背景色をフェードで黒に変更
        StartCoroutine(FadeBackgroundColor(humanBackgroundColor));
    }

    // 幽霊状態に切り替え
    void SetGhostState()
    {
        currentState = PlayerState.Ghost;
        wallCollider.enabled = false; // 壁をすり抜けられるようにする
        spriteRenderer.sprite = ghostSprite; // 幽霊状態の見た目に変更

        // 背景色をフェードで白に変更
        StartCoroutine(FadeBackgroundColor(ghostBackgroundColor));
    }

    // 背景色をフェードさせるコルーチン
    private IEnumerator FadeBackgroundColor(Color targetColor)
    {
        Color currentColor = mainCamera.backgroundColor;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            // 徐々に色を変更
            timer += Time.deltaTime;
            mainCamera.backgroundColor = Color.Lerp(currentColor, targetColor, timer / fadeDuration);
            yield return null; // 次のフレームまで待機
        }

        // 最終的に正確な色に設定
        mainCamera.backgroundColor = targetColor;
    }

    // 残りの使用回数をUIに表示
    void UpdateSwitchCountText()
    {
        // 残りの回数を表示
        int switchesLeft = maxSwitchCount - currentSwitchCount;
        switchCountText.text = "残り切り替え数: " + switchesLeft;
    }
}
