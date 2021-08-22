using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �������
/// �}�l�e���G�i�J�����B���}�C��
/// ���d�޲z - �������a���d
/// </summary>
public class SceneController : MonoBehaviour
{
    #region ���G���}
    /// <summary>
    /// ���d���s
    /// </summary>
    [Header("���d���s")]
    public Button[] btnsLevel;
    #endregion

    #region ���G�R�A
    public static int level = 1;
    #endregion

    #region �ƥ�
    private void Start()
    {
        for (int i = 0; i < btnsLevel.Length; i++)
        {
            int btnIndex = i + 1;
            btnsLevel[i].onClick.AddListener(() => { LoadScene(btnIndex); });
        }
    }
    #endregion

    #region ��k�G���}
    /// <summary>
    /// ���}�C��
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region ��k�G�p�H
    /// <summary>
    /// �z���s�����J���d�G�Ҧp�G���d 1 ���s���U�ԶǤJ 1 �åB���J�i���d 1�j
    /// </summary>
    /// <param name="level">�n���J�����d�s��</param>
    private void LoadScene(int level)
    {
        Invoke("DelayLoadScene", 0.5f);
    }

    /// <summary>
    /// ����i�J����
    /// 
    /// </summary>
    private void DelayLoadScene()
    {
        SceneManager.LoadScene("���d " + level);
    }
    #endregion
}
