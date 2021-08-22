using UnityEngine;
using System.Collections;

/// <summary>
/// �����
/// ���ફ��
/// ���U��������ɭn�B�z���Ʊ�
/// </summary>
public class RotationBase : MonoBehaviour
{
    #region ���G���}
    [Header("����ɶ�"), Range(0, 10)]
    public float rotationTime = 0.2f;
    [Header("���স��"), Range(0, 100)]
    public int rotationCount = 30;
    [Header("�ƤJ�C���л\")]
    public Color colorMouseEnter = new Color(207/255f, 87/255f, 93/255f);
    [Header("����e�W�ɻP�����U�����ƭ�")]
    public float valueBeforeRotation;
    public float valueAfterRotation;
    [Header("�O�_�����")]
    public bool canRotation = true;
    #endregion

    #region ���G�p�H
    private SceneObject sceneObject;
    #endregion

    #region ���G�O�@
    protected bool isRotating;
    protected Rigidbody2D rig;
    protected SpriteRenderer spr;
    protected int countClick;
    protected Player player;
    #endregion

    #region ��k�G�O�@
    /// <summary>
    /// ����������
    /// </summary>
    protected virtual void WhenSceneRotation(float sceneRotationTime)
    {
        StartCoroutine(AfterSceneRotation(sceneRotationTime));
    }

    /// <summary>
    /// �ƹ��ƤJ���ĪG
    /// </summary>
    protected virtual void MouseEnterEffect()
    {
        spr.color = colorMouseEnter;
    }

    /// <summary>
    /// �ƹ��ƥX���ĪG
    /// </summary>
    protected virtual void MouseExitEffect()
    {
        spr.color = Color.white;
    }

    /// <summary>
    /// ����������
    /// </summary>
    public virtual void RotationSelf()
    {
        if (!canRotation || Player.rotationObjectCount == 0) return;

        player.UpdateRotationObjectCount();

        StartCoroutine(Rotation());
    }
    #endregion

    #region ��k�G�p�H
    private IEnumerator Rotation()
    {
        if (!isRotating)
        {
            isRotating = true;
            countClick++;

            RigidbodyType2D type = rig.bodyType;
            rig.bodyType = RigidbodyType2D.Static;
            transform.position += Vector3.up * valueBeforeRotation;
            Vector3 originalEuler = transform.eulerAngles;

            for (int i = 0; i < rotationCount; i++)
            {
                float interval = rotationTime / rotationCount;
                float angle = 180 / rotationCount;
                Vector3 euler = transform.eulerAngles;
                euler.z -= angle;                                       // ���ɰw -
                transform.eulerAngles = euler;
                yield return new WaitForSeconds(interval);
            }

            transform.eulerAngles = originalEuler + Vector3.forward * 180;
            isRotating = false;
            rig.bodyType = type;

            transform.position -= Vector3.up * valueAfterRotation;
        }
    }

    /// <summary>
    /// ��������ध��n�����B�z
    /// ��������æb�������ध��Ұ�
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator AfterSceneRotation(float sceneRotationTime)
    {
        transform.position += Vector3.up * 2;
        RigidbodyType2D rigType = rig.bodyType;
        rig.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(sceneRotationTime + 0.5f);
        rig.bodyType = rigType;
    }
    #endregion

    #region �ƥ�
    protected virtual void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();

        sceneObject = GameObject.Find("�����a�O�ڪ���").GetComponent<SceneObject>();
        sceneObject.onSceneRotation += WhenSceneRotation;
    }

    private void OnMouseEnter()
    {
        MouseEnterEffect();
    }

    private void OnMouseExit()
    {
        MouseExitEffect();
    }

    private void OnMouseDown()
    {
        RotationSelf();
    }
    #endregion
}
