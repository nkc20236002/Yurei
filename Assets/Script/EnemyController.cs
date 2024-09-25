using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2.0f;  // �G�̈ړ����x
    public Transform player;  // �v���C���[��Transform���Q��
    public float chaseRange = 5.0f;  // �v���C���[��ǂ�������͈�
    private PlayerStateController playerStateController;  // �v���C���[�̏�ԃR���g���[���[

    private bool playerInRange = false;  // �v���C���[�����m�͈͓��ɂ��邩
    private float currentCooldown = 0.0f;  // �N�[���_�E���̎���
    private float cooldownDuration = 1.0f;  // �N�[���_�E���̒���
    private void Start()
    {
        // �v���C���[�̏�ԃR���g���[���[���擾
        playerStateController = player.GetComponent<PlayerStateController>();
    }
    private void Update()
    {
        // �v���C���[�Ƃ̋������v�Z
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �v���C���[���H���Ԃ��w��͈͓��ɂ��邩�`�F�b�N
        if (playerStateController.IsGhostState() && distanceToPlayer <= chaseRange)
        {
            ChasePlayer();  // �H���Ԃ��͈͓��Ȃ�v���C���[��ǂ�������
        }

        // �N�[���_�E������
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;  // ���Ԃ����Z
        }
    }

    // �v���C���[��ǂ������郁�\�b�h
    private void ChasePlayer()
    {
        // �v���C���[�Ƃ̋������v�Z
        float step = speed * Time.deltaTime;  // ��t���[���̈ړ�����
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);  // �v���C���[�Ɍ������Ĉړ�
    }

    // �v���C���[�̏�Ԃ��m�F���邽�߂̃��\�b�h
    public void SetPlayerInRange(bool isInRange)
    {
        playerInRange = isInRange;  // �v���C���[�����m�͈͓��ɂ��邩�ݒ�
    }
}
