using TMPro; //TextMeshProを扱うのに必要
using UnityEngine;
using UnityEngine.SceneManagement; //シーン切替に必要なクラスがある
using UnityEngine.UI;               // UIを使うのに必要

public class GameManager : MonoBehaviour
{
    public GameObject mainImage; //画像を持つImageゲームオブジェクト
    public Sprite gameOverSpr; //GAMEOVER画像
    public Sprite gameClearSpr; //GAMECLER画像
    public GameObject panel; //パネル
    public GameObject restartButton; //RESTARTボタン
    public GameObject nextButton; //NEXTボタン

    Image titleImage; //画像を表示するImageコンポーネント
    GameState gamestate = GameState.InGame; //ゲームの状態

    public string nextSceneName; //次のシーン名

    //時間制限追加
    public GameObject timeBar; //時間表示イメージ
    public GameObject timeText; //時間テキスト
    TimeController timeCnt; //TimeControllerコンポーネント

    //スコア追加
    public GameObject scoreText; //スコアテキスト
    public static int totalScore; //合計スコア
    public int stageScore = 0; //ステージスコア

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //テキストをみて完成させましょう
        Invoke("InactiveImage", 1.0f); //1秒後にInactiveImageメソッドを発動
        panel.SetActive(false); //パネルを即非表示

        //時間制限追加
        timeCnt = GetComponent<TimeController>();
        if (timeCnt != null)
        {
            if (timeCnt.gameTime == 0.0f) //制限時間設定なしなら
            {
                timeBar.SetActive(false); //UIを隠す
            }
        }

        UpdateScore(); //スコアUI表示更新
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.gameState == GameState.GameClear)
        {
            //テキストをみて完成させましょう
            gamestate = GameState.GameClear;
            mainImage.SetActive(true); //画像表示
            panel.SetActive(true); //ボタンの表示
            //RESTARTボタン無効化
            Button bt = restartButton.GetComponent<Button>();
            bt.interactable = false; //ボタン機能を無効化

            //メイン画像の差し替え
            mainImage.GetComponent<Image>().sprite = gameClearSpr;

            //titleImage = mainImage.GetComponent<Image>();
            //titleImage.sprite = gameClearSpr;

            PlayerController.gameState = GameState.GameEnd;

            if (timeCnt != null)
            {
                timeCnt.isTimeOver = true; //カウント停止
                //ボーナススコア追加
                int time = (int)timeCnt.displayTime;
                totalScore += time * 10;
            }
            //ステージスコア更新
            totalScore += stageScore;
            stageScore = 0;
            UpdateScore(); //スコアUI表示更新
        }
        else if (PlayerController.gameState == GameState.GameOver)
        {
            //テキストをみて完成させましょう
            gamestate = GameState.GameOver;
            mainImage.SetActive(true); //画像表示
            panel.SetActive(true); //ボタンの表示
            //RESTARTボタン無効化
            Button bt = nextButton.GetComponent<Button>();
            bt.interactable = false; //ボタン機能を無効化

            //メイン画像の差し替え
            mainImage.GetComponent<Image>().sprite = gameOverSpr;

            PlayerController.gameState = GameState.GameEnd;

            if (timeCnt != null)
            {
                timeCnt.isTimeOver = true; //カウント停止
            }
        }
        else if (PlayerController.gameState == GameState.InGame)
        {
            //ゲーム中
            //「Player」タグがついているオブジェクトを参照
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //PlayerオブジェクトのPlayerControllerコンポーネントを取得
            PlayerController playerCnt = player.GetComponent<PlayerController>();

            //時間制限
            //UIのタイムを更新
            if (timeCnt != null)
            {
                if (timeCnt.gameTime > 0.0f)
                {
                    //整数に代入することで小数を切り捨て
                    int time = (int)timeCnt.displayTime;
                    timeText.GetComponent<TextMeshProUGUI>().text = time.ToString();

                    //タイムオーバー
                    if (time == 0)
                    {
                        playerCnt.GameOver(); //ゲームオーバー処理
                    }
                }
            }

            //スコア追加
            if (playerCnt.score != 0)
            {
                stageScore += playerCnt.score;
                playerCnt.score = 0;
                UpdateScore(); //スコアUI表示更新
            }
        }
    }

    // 画像を非表示にする
    void InactiveImage()
    {
        //テキストではハイライトされていませんがここも編集！テキストをみて完成させましょう
        mainImage.SetActive(false); //オブジェクトを非表示
    }

    //スコアUI表示更新
    void UpdateScore()
    {
        int score = stageScore + totalScore;
        scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    //リスタート
    public void Restart()
    {
        //現シーンの名前を引数に入れる
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //次へ
    public void Next()
    {
        SceneManager.LoadScene(nextSceneName);
    }

}