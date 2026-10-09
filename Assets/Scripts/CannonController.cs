using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject objPrefab;            //発生させるPrefabデータ
    public float delayTime = 3.0f;          //遅延時間
    public float fireSpeed = 4.0f;          //発射速度
    public float length = 8.0f;             //範囲

    GameObject player;                      //プレイヤー
    Transform gateTransform;                //発射口のTransform
    float passedTimes = 0;                  //経過時間
                                            //距離チェック
                                            // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //発射口オブジェクトのTransformを取得
        gateTransform = transform.Find("gate");
        //プレイヤーを取得
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //待機時間加算
        passedTimes += Time.deltaTime;
        //Playerとの距離チェック
        if (CheckLength(player.transform.position))
        {
            //待機時間経過
            if (passedTimes > delayTime)
            {

                //テキストをみて完成させましょう

                passedTimes = 0; //時間を0にリセット
                //砲弾をプレハブから作る                
                Vector2 pos = new Vector2(gateTransform.position.x, gateTransform.position.y);
                GameObject obj = Instantiate(objPrefab, pos, Quaternion.identity);
                //砲身が向いている方向に発射する
                Rigidbody2D rbody = obj.GetComponent<Rigidbody2D>(); void Update()
                {
                    //待機時間加算
                    passedTimes += Time.deltaTime;
                    //Playerとの距離チェック
                    if (CheckLength(player.transform.position))
                    {
                        //待機時間経過
                        if (passedTimes > delayTime)
                        {

                            //テキストをみて完成させましょう

                            passedTimes = 0; //時間を0にリセット
                                             //砲弾をプレハブから作る                
                            Vector2 pos = new Vector2(gateTransform.position.x, gateTransform.position.y);
                            //① 砲弾をプレハブから作って変数「obj」に入れる
                            //砲身が向いている方向に発射する
                            Rigidbody2D rbody = obj.GetComponent<Rigidbody2D>();//②【この行で】生み出したばかりの弾（obj）から Rigidbody2D を取得して「rbody」に入れている
                        }
                    }
                    float anglez = transform.localEulerAngles.z;
                    float x = Mathf.Cos(anglez * Mathf.Deg2Rad);
                    float y = Mathf.Sin(anglez * Mathf.Deg2Rad);
                    Vector2 v = new Vector2(x, y) * fireSpeed;
                    rbody.AddForce(v, ForceMode2D.Impulse);
                }
            }
        }

        bool CheckLength(Vector2 targetPos)
        {
            bool ret = false;
            float d = Vector2.Distance(transform.position, targetPos);
            if (length >= d)
            {
                ret = true;
            }
            return ret;
        }

        //範囲表示
        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, length);
        }
    }
}
