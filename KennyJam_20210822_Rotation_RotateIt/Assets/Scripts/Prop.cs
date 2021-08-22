using UnityEngine;

/// <summary>
/// �D��G�����W���R�A�D�㪫��
/// </summary>
public class Prop : RotationBase
{
    [Header("�O�_�����")]
    public bool canPush;

    public override void RotationSelf()
    {
        canPush = !canPush;
        rig.bodyType = canPush ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;

        base.RotationSelf();
    }

    protected override void Start()
    {
        base.Start();

        rig.bodyType = canPush ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    }
}
