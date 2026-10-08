using TMPro; //TextMeshProを使うの必要
using UnityEngine;
using UnityEngine.SceneManagement; //シーンの切り替えに必要

public class ResultManager : MonoBehaviour
{
    public GameObject scoreTextObject;
    public TextMeshProUGUI scoreText;
    public string sceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = GameManager.totalScore.ToString();

    }

    // Update is called once per frame
    void Update()
    {

    }
    //シーンを読み込み
    public void Load()
    {
    SceneManager.LoadScene(sceneName);
}
}
