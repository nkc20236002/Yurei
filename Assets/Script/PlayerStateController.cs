using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerStateController : MonoBehaviour
{
    public GameObject[] enemies;  // �G�I�u�W�F�N�g�̃��X�g
    public Text remainingSwitchText;  // �c��؂�ւ��񐔂�\������e�L�X�g
    public Sprite humanSprite;  // �l��Ԃ̃X�v���C�g
    public Sprite ghostSprite;  // �H���Ԃ̃X�v���C�g
    public Color ghostColor = Color.white;  // �H���Ԃ̃J�����w�i�F
    public Color humanColor = Color.black;  // �l��Ԃ̃J�����w�i�F

    public Collider2D wallCollider;  // �I���\�ȕǂ̃R���C�_�[

    private bool isGhostState = false; // �v���C���[�̏�ԃt���O
    private int remainingSwitches = 5;  // �c��̐؂�ւ���
    private bool canSwitchState = true; // ��Ԑ؂�ւ��̉ۃt���O
    private float switchCooldown = 1.0f;  // �؂�ւ��̃N�[���_�E������
    private float currentCooldown = 0.0f;  // ���݂̃N�[���_�E������
    private float fadeDuration = 1.0f;  // �t�F�[�h����
    private Camera mainCamera;  // ���C���J����
    private SpriteRenderer spriteRenderer;  // �v���C���[�̃X�v���C�g�����_���[
    private Collider2D playerCollider;  // �v���C���[�̃R���C�_�[

    private void Start()
    {
        mainCamera = Camera.main;  // ���C���J�������擾
        spriteRenderer = GetComponent<SpriteRenderer>();  // �X�v���C�g�����_���[���擾
        playerCollider = GetComponent<Collider2D>();  // �v���C���[�̃R���C�_�[���擾

        // ������Ԃ̐ݒ�
        mainCamera.backgroundColor = humanColor;  // �����w�i�F��ݒ�
        spriteRenderer.sprite = humanSprite;  // �����X�v���C�g��ݒ�
        UpdateSwitchText();
        SetColliderState(true);  // ������Ԃ͐l��ԂŃR���C�_�[��L���ɂ���
    }

    void Update()
    {
        // �N�[���_�E�����ł���΃J�E���g�_�E��
        if (!canSwitchState)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                canSwitchState = true;  // �N�[���_�E���I��
            }
        }

        // �X�y�[�X�L�[�ŏ�Ԃ�؂�ւ���
        if (Input.GetKeyDown(KeyCode.Space) && canSwitchState)
        {
            ToggleState();
        }

        // �c��؂�ւ��񐔂̃e�L�X�g���X�V
        UpdateSwitchText();
    }

    // ��Ԃ�؂�ւ��郁�\�b�h
    void ToggleState()
    {
        if (isGhostState) // �H���Ԃ���l��Ԃɖ߂�ꍇ
        {
            isGhostState = false;  // �H���Ԃ�����
        }
        else // �l��Ԃ���H���Ԃɐ؂�ւ���ꍇ
        {
            if (remainingSwitches > 0)  // �c��؂�ւ��񐔂�����Ό��炷
            {
                remainingSwitches--;
                isGhostState = true;  // �H���Ԃɐ؂�ւ�
            }
        }

        // �G�̕\���E��\����؂�ւ���
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(isGhostState);
        }

        // �J�����̔w�i�F���t�F�[�h�����ŕύX
        StartCoroutine(FadeBackground(isGhostState ? ghostColor : humanColor));

        // �v���C���[�̃X�v���C�g��ύX
        spriteRenderer.sprite = isGhostState ? ghostSprite : humanSprite;

        // �Փ˔���̏�Ԃ�ύX
        SetColliderState(!isGhostState); // �l��Ԃ̂Ƃ��̂ݏՓ˔����L���ɂ���

        // �ǃR���C�_�[�̗L��/������ݒ�
        SetWallColliderState(isGhostState);

        // �؂�ւ���ɃN�[���_�E�����J�n
        canSwitchState = false;
        currentCooldown = switchCooldown;
    }

    // �J�����̔w�i�F�̃t�F�[�h����
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
        mainCamera.backgroundColor = targetColor;  // �ŏI�I�ȐF��ݒ�
    }

    // �c��؂�ւ��񐔂̕\�����X�V
    void UpdateSwitchText()
    {
        if (remainingSwitchText != null)
        {
            remainingSwitchText.text = "�c��؂�ւ���: " + remainingSwitches;
        }
    }

    // �Փ˔���̗L��/������ݒ肷�郁�\�b�h
    private void SetColliderState(bool state)
    {
        playerCollider.enabled = state;  // �v���C���[�̃R���C�_�[��L��/�����ɂ���
    }

    // �ǃR���C�_�[�̗L��/������ݒ肷�郁�\�b�h
    private void SetWallColliderState(bool isGhost)
    {
        if (wallCollider != null)
        {
            wallCollider.enabled = !isGhost;  // �H���Ԃ̂Ƃ��͕ǃR���C�_�[�𖳌��ɂ���
        }
    }

    // �G�ɐG�ꂽ�Ƃ��̏���
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �H���ԂœG�ɐG�ꂽ�ꍇ�ɃQ�[���I�[�o�[
        if (isGhostState && other.CompareTag("Enemy"))
        {
            GameOver();  // �Q�[���I�[�o�[�������Ăяo��
        }

        // �H���Ԃ̂Ƃ�����̃R���C�_�[���ђ�
        if (isGhostState && other.CompareTag("PassThrough"))
        {
            Physics2D.IgnoreCollision(playerCollider, other, true); // �R���C�_�[�𖳎�
        }
    }

    // �Q�[���I�[�o�[����
    public void GameOver()
    {
        // �Q�[���I�[�o�[�V�[���Ɉڍs
        SceneManager.LoadScene("GameOverScene");  // �Q�[���I�[�o�[�V�[���Ɉڍs
    }

    // �H���Ԃ��ǂ�����Ԃ����\�b�h
    public bool IsGhostState()
    {
        return isGhostState;
    }
}
