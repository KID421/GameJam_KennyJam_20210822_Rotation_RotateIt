using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : RotationBase
{
    #region 欄位：公開
    [Header("基本參數")]
    [Range(0, 1000)]
    public float speed = 10;
    [Range(0, 5000)]
    public float jump = 100;
    [Header("檢查地板")]
    public Vector3 groundOffset;
    [Range(0, 10)]
    public float groundRadius;
    [Header("介面")]
    public Text textRotationCount;
    public Text textLife;
    [Header("爆炸特效")]
    public GameObject goExplosion;
    public AudioClip soundExplosion;
    public AudioClip soundJump;
    #endregion

    #region 欄位：私人
    /// <summary>
    /// 是否在地板上
    /// </summary>
    private bool isGrounded;
    private float moveInput;
    private bool jumpInput;
    private Animator ani;
    private AudioSource aud;
    /// <summary>
    /// 結束畫面
    /// </summary>
    private Animator aniFinal;
    /// <summary>
    /// 結束畫面標題
    /// </summary>
    private Text textFinalTitle;
    #endregion

    #region 欄位：靜態
    public static int rotationObjectCount = 3;
    public static int life = 3;
    #endregion

    #region 方法：私人
    /// <summary>
    /// 輸入：左右移動
    /// </summary>
    private void InputMove()
    {
        moveInput = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// 輸入：跳躍 - 空白鍵
    /// </summary>
    private void InputJump()
    {
        jumpInput = Input.GetKeyDown(KeyCode.Space);

        if (jumpInput && isGrounded && !aud.isPlaying) aud.PlayOneShot(soundJump, Random.Range(0.7f, 1.2f));
    }

    /// <summary>
    /// 移動：FixedUpdate
    /// </summary>
    private void Move()
    {
        rig.velocity =
            Vector3.right * moveInput * speed * Time.fixedDeltaTime +
            Vector3.up * rig.velocity.y;
    }

    /// <summary>
    /// 跳躍：FixedUpdate
    /// </summary>
    private void Jump()
    {
        if (jumpInput && isGrounded) rig.AddForce(transform.up * jump);
    }

    /// <summary>
    /// 檢查地板
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
    /// 方向：左右
    /// </summary>
    private void Direction()
    {
        if (Input.GetKeyDown(KeyCode.D)) transform.eulerAngles = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.A)) transform.eulerAngles = Vector3.up * 180;
    }

    /// <summary>
    /// 動畫控制
    /// </summary>
    private void AnimationControl()
    {
        ani.SetBool("走路開關", moveInput != 0);
        ani.SetBool("跳躍開關", !isGrounded);
    }

    /// <summary>
    /// 死亡：生成爆炸效果、更新介面並在兩秒後重新遊戲
    /// </summary>
    private void Dead()
    {
        aud.PlayOneShot(soundExplosion);
        GameObject explosion = Instantiate(goExplosion, transform.position, Quaternion.identity);
        Destroy(explosion, 0.8f);
        life--;
        textLife.text = "LIFE : " + life;
        if (life > 0) Invoke("Replay", 2);
        else aniFinal.SetTrigger("淡入觸發");

        GetComponent<CapsuleCollider2D>().enabled = false;
        rig.bodyType = RigidbodyType2D.Kinematic;
        spr.enabled = false;
    }

    /// <summary>
    /// 重新遊戲
    /// </summary>
    private void Replay()
    {
        rotationObjectCount = 3;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 返回開始選單
    /// </summary>
    private void BackToStartMenu()
    {
        if (life == 0 && Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("開始畫面");
    }

    /// <summary>
    /// 過關並且前往下個關卡
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

        aniFinal.SetTrigger("淡入觸發");

        yield return new WaitForSeconds(1.5f);

        if (SceneController.level <= currentLevel) SceneController.level++;
        SceneManager.LoadScene("關卡 " + (currentLevel + 1));
    }
    #endregion

    #region 方法：公開
    /// <summary>
    /// 更新旋轉物件次數與介面
    /// </summary>
    public void UpdateRotationObjectCount()
    {
        rotationObjectCount--;
        textRotationCount.text = "Rotation Count : " + rotationObjectCount;
    }
    #endregion

    #region 事件
    protected override void Start()
    {
        base.Start();

        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        aniFinal = GameObject.Find("結束畫面").GetComponent<Animator>();
        textFinalTitle = GameObject.Find("結束畫面標題").GetComponent<Text>();

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
        if (collision.gameObject.tag == "危險物件") Dead();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "過關傳送門") StartCoroutine(PassAndLoadNextLevel());
    }
    #endregion
}
