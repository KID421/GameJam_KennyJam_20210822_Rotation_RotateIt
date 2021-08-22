using UnityEngine;
using System.Collections;

/// <summary>
/// ��������G��������n���઺�ڪ���
/// </summary>
public class SceneObject : RotationBase
{
    /// <summary>
    /// �e���G�L�Ǧ^�P�L�Ѽ�
    /// </summary>
    public delegate void delegateSceneRotation(float sceneRotationTime);

    /// <summary>
    /// �ƥ�G���������
    /// </summary>
    public event delegateSceneRotation onSceneRotation;

    public override void RotationSelf()
    {
        if (Player.rotationObjectCount == 0) return;

        base.RotationSelf();

        onSceneRotation(rotationTime);
    }

    protected override IEnumerator AfterSceneRotation(float sceneRotationTime)
    {
        yield return null;
    }
}
