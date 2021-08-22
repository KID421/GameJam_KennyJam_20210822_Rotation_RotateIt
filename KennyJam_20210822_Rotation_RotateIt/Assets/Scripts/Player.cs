using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : RotationBase
{
    #region ���G���}
    [Header("�򥻰Ѽ�")]
    [Range(0, 1000)]
    public float speed = 10;
    [Range(0, 5000)]
    public float jump = 100;
    [Header("�ˬd�a�O")]
    public Vector3 groundOffset;
    [Range(0, 10)]
    public float groundRadius;
    [Header("����")]
    public Text textRotationCount;
    public Text textLife;
    [Header("�z���S��")]
    public GameObject goExplosion;
    public AudioClip soundExplosion;
    public AudioClip soundJump;
    #endregion

    #region ���G�p�H
    /// <summary>
    /// �O�_�b�a�O�W
    /// </summary>
    private bool isGrounded;
    private float moveInput;
    private bool jumpInput;
    private Animator ani;
    private AudioSource aud;
    /// <summary>
    /// �����e��
    /// </summary>
    private Animator aniFinal;
    /// <summary>
    /// �����e�����D
    /// </summary>
    private Text textFinalTitle;
    #endregion

    #region ���G�R�A
    public static int rotationObjectCount = 3;
    public static int life = 3;
    #endregion

    #region ��k�G�p�H
    /// <summary>
    /// ��J�G���k����
    /// </summary>
    private void InputMove()
    {
        moveInput = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// ��J�G���D - �ť���
    /// </summary>
    private void InputJump()
    {
        jumpInput = Input.GetKeyDown(KeyCode.Space);

        if (jumpInput && isGrounded && !aud.isPlaying) aud.PlayOneShot(soundJump, Random.Range(0.7f, 1.2f));
    }

    /// <summary>
    /// ���ʡGFixedUpdate
    /// </summary>
    private void Move()
    {
        rig.velocity =
            Vector3.right * moveInput * speed * Time.fixedDeltaTime +
            Vector3.up * rig.velocity.y;
    }

    /// <summary>
    /// ���D�GFixedUpdate
    /// </summary>
    private void Jump()
    {
        if (jumpInput && isGrounded) rig.AddForce(transform.up * jump);
    }

    /// <summary>
    /// �ˬd�a�O
    /// </summary>
    private void CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position +
            transform.right * groundOffset.x +
            transform.up * groundOffset.y,
            groundRadius, 1 << 7);

        isGrounded = hit;
    }

    /// <summary>
    /// ��V�G���k
    /// </summary>
    private void Direction()
    {
        if (Input.GetKeyDown(KeyCode.D)) transform.eulerAngles = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.A)) transform.eulerAngles = Vector3.up * 180;
    }

    /// <summary>
    /// �ʵe����
    /// </summary>
    private void AnimationControl()
    {
        ani.SetBool("�����}��", moveInput != 0);
        ani.SetBool("���D�}��", !isGrounded);
    }

    /// <summary>
    /// ���`�G�ͦ��z���ĪG�B��s�����æb���᭫�s�C��
    /// </summary>
    private void Dead()
    {
        aud.PlayOneShot(soundExplosion);
        GameObject explosion = Instantiate(goExplosion, transform.position, Quaternion.identity);
        Destroy(explosion, 0.8f);
        life--;
        textLife.text = "LIFE : " + life;
        if (life > 0) Invoke("Replay", 2);
        else aniFinal.SetTrigger("�H�JĲ�o");

        GetComponent<CapsuleCollider2D>().enabled = false;
        rig.bodyType = RigidbodyType2D.Kinematic;
        spr.enabled = false;
    }

    /// <summary>
    /// ���s�C��
    /// </summary>
    private void Replay()
    {
        rotationObjectCount = 3;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// ��^�}�l���
    /// </summary>
    private void BackToStartMenu()
    {
        if (life == 0 && Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("�}�l�e��");
    }

    /// <summary>
    /// �L���åB�e���U�����d
    /// </summary>
    private IEnumerator PassAndLoadNextLevel()
    {
        life = 3;
        rotationObjectCount = 3;
        int currentLevel = SceneManager.GetActiveScene().buildIndex;

        enabled = false;
        rig.velocity = Vector3.zero;

        if (currentLevel == 5) textFinalTitle.text = "Congratulations! You Win!";
        else textFinalTitle.text = "PASS LEVEL! Wait To Load Next Level!";

        aniFinal.SetTrigger("�H�JĲ�o");

        yield return new WaitForSeconds(1.5f);

        if (SceneController.level <= currentLevel) SceneController.level++;
        SceneManager.LoadScene("���d " + (currentLevel + 1));
    }
    #endregion

    #region ��k�G���}
    /// <summary>
    /// ��s���ફ�󦸼ƻP����
    /// </summary>
    public void UpdateRotationObjectCount()
    {
        rotationObjectCount--;
        textRotationCount.text = "Rotation Count : " + rotationObjectCount;
    }
    #endregion

    #region �ƥ�
    protected override void Start()
    {
        base.Start();

        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        aniFinal = GameObject.Find("�����e��").GetComponent<Animator>();
        textFinalTitle = GameObject.Find("�����e�����D").GetComponent<Text>();

        textLife.text = "LIFE : " + life;
    }

    private void Update()
    {
        InputMove();
        InputJump();
        CheckGround();
        Jump();
        Direction();
        AnimationControl();
        BackToStartMenu();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(
            transform.position +
            transform.right * groundOffset.x +
            transform.up * groundOffset.y,
            groundRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "�M�I����") Dead();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "�L���ǰe��") StartCoroutine(PassAndLoadNextLevel());
    }
    #endregion
}
