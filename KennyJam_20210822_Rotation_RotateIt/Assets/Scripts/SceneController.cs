using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 場景控制器
/// 開始畫面：進入場景、離開遊戲
/// 關卡管理 - 紀錄玩家關卡
/// </summary>
public class SceneController : MonoBehaviour
{
    #region 欄位：公開
    /// <summary>
    /// 關卡按鈕
    /// </summary>
    [Header("關卡按鈕")]
    public Button[] btnsLevel;
    #endregion

    #region 欄位：靜態
    public static int level = 1;
    #endregion

    #region 事件
    private void Start()
    {
        for (int i = 0; i < btnsLevel.Length; i++)
        {
            int btnIndex = i + 1;
            btnsLevel[i].onClick.AddListener(() => { LoadScene(btnIndex); });
        }
    }
    #endregion

    #region 方法：公開
    /// <summary>
    /// 離開遊戲
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region 方法：私人
    /// <summary>
    /// 透關編號載入關卡：例如：關卡 1 按鈕按下候傳入 1 並且載入【關卡 1】
    /// </summary>
    /// <param name="level">要載入的關卡編號</param>
    private void LoadScene(int level)
    {
        Invoke("DelayLoadScene", 0.5f);
    }

    /// <summary>
    /// 延遲進入場景
    /// 
    /// </summary>
    private void DelayLoadScene()
    {
        SceneManager.LoadScene("關卡 " + level);
    }
    #endregion
}
