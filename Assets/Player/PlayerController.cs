using UnityEngine;

public enum GameState           // ゲームの状態
{
    InGame,                     // ゲーム中
    GameClear,                  // ゲームクリア
    GameOver,                   // ゲームオーバー
    GameEnd,                    // ゲーム終了
}

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;              // Rigidbody2D型の変数
    float axisH = 0.0f;             // 入力
    public float speed = 3.0f;      // 移動速度   

    public float jump = 9.0f;
    public LayerMask groundLayer;
    bool goJump = false;
    bool onGround = false;

    // ゲームの状態（テキストは誤植なのでここは次の記述が正解）
    public static GameState gameState = GameState.InGame;

    //ア二メーション対応
    Animator animator; //アニメーター
    public string stopAnime = "PlayerStop";
    public string moveAnime = "PlayerMove";
    public string jumpAnime = "PlayerJump";
    public string goalAnime = "PlayerGoal";
    public string deadAnime = "PlayerOver";
    string nowAnime = "";
    string oldAnime = "";

    //カメラ制御
    public float camLeft = 0.0f; //カメラ左スクロールリミット
    public float camRight = 0.0f; //カメラ右スクロールリミット
    public float camTop = 0.0f; //カメラ上スクロールリミット
    public float camBottom = 0.0f; //カメラ下スクロールリミット

    //多重スクロール
    public GameObject subScreen; //サブスクリーン

    //強制スクロール
    public bool isForceScrollX = false; //X方向の強制スクロールフラグ
    public float forceScrollSpeedX = 0.5f; //1秒間で動かすX距離
    public bool isForceScrollY = false; //Y方向の強制スクロールフラグ
    public float forceScrollSpeedY = 0.5f; //1秒間で動かすY距離

    public int score = 0; //スコア

    void Start()
    {
        rbody = this.GetComponent<Rigidbody2D>();   // Rigidbody2Dを取ってくる
        animator = GetComponent<Animator>(); //Animatorを取ってくる
        nowAnime = stopAnime; //停止から開始する
        oldAnime = stopAnime; //停止から開始する

        gameState = GameState.InGame; //ゲーム中にする
    }

    void Update()
    {
        if (gameState != GameState.InGame)
        {
            return; //このフレームをキャンセル
        }

        onGround = Physics2D.CircleCast(
            transform.position,　//どこから？
            0.2f, //円の半径は？
            Vector2.down, //向き？
            0.0f, //距離
            groundLayer);

        if (Input.GetButtonDown("Jump"))
        {
            goJump = true;
        }

        axisH = Input.GetAxisRaw("Horizontal");     //水平方向の入力をチェックする


        if (axisH > 0.0f)                           // 向きの調整
        {
            transform.localScale = new Vector2(1, 1);   // 右移動
        }
        else if (axisH < 0.0f)
        {
            transform.localScale = new Vector2(-1, 1); // 左右反転させる
        }

        //アニメーション更新
        if (onGround) //地面の上
        {
            if (axisH == 0)
            {
                nowAnime = stopAnime; //停止中
            }
            else
            {
                nowAnime = moveAnime; //移動
            }
        }
        else
        {
            nowAnime = jumpAnime; //空中
        }
        if (nowAnime != oldAnime) //1フレーム前のクリップと異なっていれば発動
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime); //アニメーションの再生
        }

        //カメラ制御
        float x;
        float y;

        if (isForceScrollX)
        {
            x = Camera.main.transform.position.x + (forceScrollSpeedX * Time.deltaTime);
        }
        else
        {
            //左の限界はcamLeft、右の限界はcamRight、その間であればPlayerの座標を追いかける
            x = Mathf.Clamp(transform.position.x, camLeft, camRight);

        }

        if (isForceScrollY)
        {
            y = Camera.main.transform.position.y + (forceScrollSpeedY * Time.deltaTime);
        }
        else
        {
            y = Mathf.Clamp(transform.position.y, camBottom, camTop);
        }

        //カメラに与えるべき理想の値を変数に代入
        Vector3 camPos = new Vector3(x, y, -10);
        Camera.main.transform.position = camPos; //カメラのPositionに代入

        //Camera.main.transform.position = new Vector3(Mathf.Clamp(transform.position.x, camLeft, camRight), Mathf.Clamp(transform.position.y, camBottom, camTop), -10);

        //サブスクリーンスクロール
        if (subScreen != null) //null（何もない） でなければ
        {
            y = subScreen.transform.position.y;
            Vector3 subpos = new Vector3(x / 2.0f, y, subScreen.transform.position.z);
            subScreen.transform.position = subpos;
        }


    }

    void FixedUpdate()
    {
        if (gameState != GameState.InGame)
        {
            return; //このフレームをキャンセル
        }

        if (onGround || axisH != 0)
        {
            //速度を更新する
            rbody.linearVelocity = new Vector2(axisH * speed, rbody.linearVelocity.y);
        }
        if (onGround && goJump)
        {
            Vector2 jumpPw = new Vector2(0, jump);
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);
            goJump = false;
        }
    }

    // 接触開始
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal();         // ゴール！！
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver();     // ゲームオーバー
        }
        else if (collision.gameObject.tag == "ScoreItem")
        {
            // スコアアイテム
            ScoreItem item = collision.gameObject.GetComponent<ScoreItem>();  // ScoreItemを得る			
            score = item.itemdata.value;                // スコアを得る
            Destroy(collision.gameObject);              // アイテム削除する
        }
    }
    // ゴール
    public void Goal()
    {
        animator.Play(goalAnime);
        gameState = GameState.GameClear; //ステータス変更
        GameStop(); //ゲーム停止
    }
    // ゲームオーバー
    public void GameOver()
    {
        animator.Play(deadAnime);
        gameState = GameState.GameOver; //ステータス変更
        GameStop(); //ゲーム停止
        //ゲームオーバー演出
        GetComponent<CapsuleCollider2D>().enabled = false; //当たり判定を無効にする
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);//上に少し跳ね上げる
    }

    // ゲーム停止 Playerの左右の動作を封じる
    void GameStop()
    {
        rbody.linearVelocity = new Vector2(0, 0); //速度0にしてプレイヤーの動きを強制停止
    }
}