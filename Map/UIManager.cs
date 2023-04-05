using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject m_StartUI;
    private GameObject m_GameUI;

    private Text m_ScoreText;
    private Text m_GemText;

    private Text m_GameScore;
    private Text m_GameGem;

    private Button m_PlayButton;

    private PlayerController m_PLayController;

    //模拟手机按键
    private Button left;
    private Button right;

    void Start()
    {
        m_StartUI = GameObject.Find("Start_UI");
        m_GameUI = GameObject.Find("Game_UI");
        //Start_UI
        m_ScoreText = GameObject.Find("Start_UI/Title/Score").GetComponent<Text>();
        m_GemText = GameObject.Find("Start_UI/Gem/Star").GetComponent<Text>();
        m_PlayButton = GameObject.Find("Start_UI/Btn/Play").GetComponent<Button>();
        m_PlayButton.onClick.AddListener(PLayButtonClick);
        //Game_UI
        m_GameScore = GameObject.Find("Game_UI/Score").GetComponent<Text>();
        m_GameGem = GameObject.Find("Game_UI/Gem/Star").GetComponent<Text>();

        left = GameObject.Find("Game_UI/Left").GetComponent<Button>();
        right = GameObject.Find("Game_UI/Right").GetComponent<Button>();
        left.onClick.AddListener(LeftButton);
        right.onClick.AddListener(RightButton);

        m_PLayController = GameObject.Find("cube_books").GetComponent<PlayerController>();
        
        Init();

        m_GameUI.SetActive(false);
    }

    private void RightButton()
    {
        m_PLayController.PlayerLeft();
    }

    private void LeftButton()
    {
        m_PLayController.PlayerRight();
    }

    /// <summary>
    /// 初始化UI界面
    /// </summary>
    private void Init()
    {
        m_ScoreText.text = PlayerPrefs.GetInt("score", 0) + "";
        m_GemText.text = PlayerPrefs.GetInt("gem", 0) + "/100";

        m_GameScore.text = "0";
        m_GameGem.text = PlayerPrefs.GetInt("gem", 0) + "/100";
    }

    /// <summary>
    /// 更新数值
    /// </summary>
    public void UpdateData(int score, int gem)
    {
        m_GemText.text = gem + "/100";
        m_GameScore.text = score.ToString();
        m_GameGem.text = gem + "/100";
    }
    
    /// <summary>
    /// 点击Play开始按钮
    /// </summary>
    private void PLayButtonClick()
    {
        Debug.Log("开始游戏");
        m_StartUI.SetActive(false);
        m_GameUI.SetActive(true);
        m_PLayController.StartGame();
    }
    
    /// <summary>
    /// 游戏结束，重置UI
    /// </summary>
    public void ResetUI()
    {
        m_StartUI.SetActive(true);
        m_GameUI.SetActive(false);
        m_GameScore.text = "0";
    }
}