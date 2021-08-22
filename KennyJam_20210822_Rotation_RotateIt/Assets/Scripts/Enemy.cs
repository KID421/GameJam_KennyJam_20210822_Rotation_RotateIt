using UnityEngine;
using System.Linq;

/// <summary>
/// 敵人
/// 等待、隨機走動與攻擊
/// </summary>
public class Enemy : RotationBase
{
    #region 欄位：公開
    [Header("基本數值")]
    [Range(0, 100)]
    public float speed = 1;
    /// <summary>
    /// 等待時間限制，最小 0.5 最大為此欄位
    /// </summary>
    [Range(0, 5)]
    public float timeIdleLimit = 2;
    /// <summary>
    /// 走路時間限制，最小 0.5 最大為此欄位
    /// </summary>
    [Range(0, 5)]
    public float timeWalkLimit = 2;
    [Header("檢查前方地板或障礙物")]
    public Vector3 forwardCheckOffset;
    public float forwardCheckRadius = 0.2f;
    [Header("爆炸特效")]
    public GameObject goExplosion;
    public AudioClip soundExplosion;
    #endregion

    #region 欄位：私人
    private Animator ani;
    private AudioSource aud;
    private float timerIdle;
    private float timeIdle;
    private float timerWalk;
    private float timeWalk;
    private StateEnemy state;
    #endregion

    #region 事件
    protected override void Start()
    {
        base.Start();

        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        timeIdle = Random.Range(0.5f, timeIdleLimit);
    }

    private void Update()
    {
        CheckRotetedState();
        CheckForward();
        CheckState();
    }

    private void FixedUpdate()
    {
        WalkFixedUpdate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.2f, 0.2f, 0.3f);
        Gizmos.DrawSphere(
            transform.position +
            transform.right * forwardCheckOffset.x +
            transform.up * forwardCheckOffset.y,
            forwardCheckRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "危險物件") Dead();
    }
    #endregion

    #region 方法：公開

    #endregion

    #region 方法：私人
    /// <summary>
    /// 檢查狀態
    /// </summary>
    private void CheckState()
    {
        switch (state)
        {
            case StateEnemy.idle:
                Idle();
                break;
            case StateEnemy.walk:
                Walk();
                break;
            case StateEnemy.attack:
                Attack();
                break;
            case StateEnemy.rotated:
                Roteted();
                break;
        }
    }

    /// <summary>
    /// 等待 - 可以切換到走路
    /// </summary>
    private void Idle()
    {
        if (timerIdle < timeIdle)
        {
            timerIdle += Time.deltaTime;
            ani.SetBool("走路開關", false);
            rig.velocity = Vector3.zero;
        }
        else
        {
            timerIdle = 0;
            state = StateEnemy.walk;
            timeWalk = Random.Range(0.5f, timeWalkLimit);
            RandomChangeDirection();
        }
    }
    
    /// <summary>
    /// 走路：可以切換到等待
    /// </summary>
    private void Walk()
    {
        if (timerWalk < timeWalk)
        {
            timerWalk += Time.deltaTime;
            ani.SetBool("走路開關", true);
        }
        else
        {
            timerWalk = 0;
            state = StateEnemy.idle;
            timeIdle = Random.Range(0.5f, timeIdleLimit);
        }
    }

    /// <summary>
    /// 固定更新內執行的走路 - 剛體運動
    /// </summary>
    private void WalkFixedUpdate()
    {
        if (state == StateEnemy.walk) rig.velocity = transform.right * speed * Time.fixedDeltaTime + transform.up * rig.velocity.y;
    }

    /// <summary>
    /// 隨機變更方向：0 - 右邊，1 - 左邊
    /// </summary>
    private void RandomChangeDirection()
    {
        int r = Random.Range(0, 2);

        transform.eulerAngles = new Vector3(0, r == 0 ? 0 : 180, transform.eulerAngles.z);
    }

    /// <summary>
    /// 變更方向
    /// </summary>
    private void ChangeDirection()
    {
        int y = (int)transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, y == 0 ? 180 : 0, transform.eulerAngles.z);
    }

    /// <summary>
    /// 檢查前方：是否沒有地板或者有障礙物
    /// </summary>
    private void CheckForward()
    {
        if (state == StateEnemy.rotated) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position +
            transform.right * forwardCheckOffset.x +
            transform.up * forwardCheckOffset.y,
            forwardCheckRadius);

        Collider2D[] result = hits.Where(x => !x.name.Contains("地板")).ToArray();

        if (hits.Length == 0 || result.Length > 0) ChangeDirection();
    }

    private void Attack()
    {

    }

    /// <summary>
    /// 檢查是否為旋轉後的狀態：Z 軸角度 180
    /// </summary>
    private void CheckRotetedState()
    {
        if (isRotating)
        {
            state = StateEnemy.rotated;
        }
        else if (countClick % 2 == 0 && state != StateEnemy.walk)
        {
            state = state = StateEnemy.idle;
        }
    }

    private void Roteted()
    {

    }

    /// <summary>
    /// 死亡：生成爆炸效果、更新介面並在兩秒後重新遊戲
    /// </summary>
    private void Dead()
    {
        aud.PlayOneShot(soundExplosion);
        GameObject explosion = Instantiate(goExplosion, transform.position, Quaternion.identity);
        Destroy(explosion, 0.8f);
        GetComponent<CapsuleCollider2D>().enabled = false;
        spr.enabled = false;
        Destroy(gameObject, 0.5f);
        enabled = false;
    }
    #endregion
}

/// <summary>
/// 敵人狀態：等待、走路與攻擊
/// </summary>
public enum StateEnemy
{
    idle, walk, attack, rotated
}