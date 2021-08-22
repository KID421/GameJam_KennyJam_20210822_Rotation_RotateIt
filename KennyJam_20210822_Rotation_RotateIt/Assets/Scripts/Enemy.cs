using UnityEngine;
using System.Linq;

/// <summary>
/// �ĤH
/// ���ݡB�H�����ʻP����
/// </summary>
public class Enemy : RotationBase
{
    #region ���G���}
    [Header("�򥻼ƭ�")]
    [Range(0, 100)]
    public float speed = 1;
    /// <summary>
    /// ���ݮɶ�����A�̤p 0.5 �̤j�������
    /// </summary>
    [Range(0, 5)]
    public float timeIdleLimit = 2;
    /// <summary>
    /// �����ɶ�����A�̤p 0.5 �̤j�������
    /// </summary>
    [Range(0, 5)]
    public float timeWalkLimit = 2;
    [Header("�ˬd�e��a�O�λ�ê��")]
    public Vector3 forwardCheckOffset;
    public float forwardCheckRadius = 0.2f;
    [Header("�z���S��")]
    public GameObject goExplosion;
    public AudioClip soundExplosion;
    #endregion

    #region ���G�p�H
    private Animator ani;
    private AudioSource aud;
    private float timerIdle;
    private float timeIdle;
    private float timerWalk;
    private float timeWalk;
    private StateEnemy state;
    #endregion

    #region �ƥ�
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
        if (collision.gameObject.tag == "�M�I����") Dead();
    }
    #endregion

    #region ��k�G���}

    #endregion

    #region ��k�G�p�H
    /// <summary>
    /// �ˬd���A
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
    /// ���� - �i�H�����쨫��
    /// </summary>
    private void Idle()
    {
        if (timerIdle < timeIdle)
        {
            timerIdle += Time.deltaTime;
            ani.SetBool("�����}��", false);
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
    /// �����G�i�H�����쵥��
    /// </summary>
    private void Walk()
    {
        if (timerWalk < timeWalk)
        {
            timerWalk += Time.deltaTime;
            ani.SetBool("�����}��", true);
        }
        else
        {
            timerWalk = 0;
            state = StateEnemy.idle;
            timeIdle = Random.Range(0.5f, timeIdleLimit);
        }
    }

    /// <summary>
    /// �T�w��s�����檺���� - ����B��
    /// </summary>
    private void WalkFixedUpdate()
    {
        if (state == StateEnemy.walk) rig.velocity = transform.right * speed * Time.fixedDeltaTime + transform.up * rig.velocity.y;
    }

    /// <summary>
    /// �H���ܧ��V�G0 - �k��A1 - ����
    /// </summary>
    private void RandomChangeDirection()
    {
        int r = Random.Range(0, 2);

        transform.eulerAngles = new Vector3(0, r == 0 ? 0 : 180, transform.eulerAngles.z);
    }

    /// <summary>
    /// �ܧ��V
    /// </summary>
    private void ChangeDirection()
    {
        int y = (int)transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, y == 0 ? 180 : 0, transform.eulerAngles.z);
    }

    /// <summary>
    /// �ˬd�e��G�O�_�S���a�O�Ϊ̦���ê��
    /// </summary>
    private void CheckForward()
    {
        if (state == StateEnemy.rotated) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position +
            transform.right * forwardCheckOffset.x +
            transform.up * forwardCheckOffset.y,
            forwardCheckRadius);

        Collider2D[] result = hits.Where(x => !x.name.Contains("�a�O")).ToArray();

        if (hits.Length == 0 || result.Length > 0) ChangeDirection();
    }

    private void Attack()
    {

    }

    /// <summary>
    /// �ˬd�O�_������᪺���A�GZ �b���� 180
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
    /// ���`�G�ͦ��z���ĪG�B��s�����æb���᭫�s�C��
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
/// �ĤH���A�G���ݡB�����P����
/// </summary>
public enum StateEnemy
{
    idle, walk, attack, rotated
}