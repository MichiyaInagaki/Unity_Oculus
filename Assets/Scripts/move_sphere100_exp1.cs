using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class move_sphere100_exp1 : MonoBehaviour
{
    public GameObject score_object1 = null; // Textオブジェクト
    public GameObject score_object2 = null; // Textオブジェクト
    public GameObject sphere100;
    public GameObject angle;
    public GameObject MASK_right;
    public GameObject MASK_left;
    private Vector3 newAngle = new Vector3(0, 0, 0);    //回転姿勢格納
    //Σ0座標系＝物理世界における回転角
    private float roll = 0.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;
    //デバッグ用
    private string str1 = "huga";
    private string str2 = "piyo";
    private bool MASK_clear = false;
    //書き出し
    private string filePath = @"/Data.csv";   //PC\VR-Headset\内部共有ストレージ\Android\data\com.sens.BedriddenVR_exp1\files に入る
    //実験用パラメータ
    private float condition_angle_roll = 0.0f;  //条件角度
    private int[] condition_order = new int[5]; //条件順を格納
    private int condition_num = 0;              //条件番号
    private float ajust_roll = 0.0f;            //調整角
    private int delay_count = 0;
    private float initial_roll = 0.0f;          //初期値
    private float rotation_angle_roll = 0.0f;   //最終的な回転角
    private int trial_num = 1;                  //試行回数
    private bool next_flag = true;              //試行切り替え


    // Start is called before the first frame update
    void Start()
    {
        //乱数シード
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        //条件順をランダム生成
        for(int i = 0; i < condition_order.Length; i++)
        {
            condition_order[i] = i + 1;
        }
        for(int i = condition_order.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i);
            int temp = condition_order[i];
            condition_order[i] = condition_order[j];
            condition_order[j] = temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //HMD角度取得
        roll = angle.GetComponent<getTracking>().roll;
        pitch = angle.GetComponent<getTracking>().pitch;
        yaw = angle.GetComponent<getTracking>().yaw;

        //条件角度設定
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && Input.GetKey(KeyCode.Escape))
        {
            if(condition_num < 5)
            {
                condition_angle_roll = condition_order[condition_num] * 10.0f;
                condition_num++;
                ajust_roll = 0;     //調整角度リセット
                trial_num = 1;      //試行回数リセット
            }
            else
            {
                MASK_clear = true;      //全条件終わったらマスク全消し
            }
        }

        //初期値生成（上昇系列・下降系列）
        if (next_flag == true)
        {
            if (trial_num % 2 == 0)
            {
                initial_roll = UnityEngine.Random.Range((int)(condition_angle_roll + 15.0f), (int)(condition_angle_roll + 45.0f));
                next_flag = false;
            }
            else
            {
                initial_roll = UnityEngine.Random.Range(-(int)(condition_angle_roll + 45.0f), -(int)(condition_angle_roll + 15.0f));
                next_flag = false;
            }
        }


        //デバッグ用（マスク全消し）
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryTouchpad) && Input.GetKey(KeyCode.Escape))
        {
            if (MASK_clear == false)
            {
                MASK_clear = true;
            }
            else
            {
                MASK_clear = false;
            }
        }
        //条件角度設定用（マスク表示）
        if (MASK_clear == false)
        {
            if ((condition_angle_roll + 1.0f) >= SetHalf(roll) && SetHalf(roll) >= (condition_angle_roll - 1.0f))
            {
                MASK_right.SetActive(false);
                MASK_left.SetActive(false);
            }
            else if ((condition_angle_roll + 1.0f) < SetHalf(roll))
            {
                MASK_right.SetActive(true);
                MASK_left.SetActive(false);
            }
            else
            {
                MASK_right.SetActive(false);
                MASK_left.SetActive(true);
            }
        }
        else
        {
            MASK_right.SetActive(false);
            MASK_left.SetActive(false);
        }


        //調整
        if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
        {
            //毎フレームだと速すぎるので，nフレームで1degree回転
            delay_count++;
            if (delay_count > 3)
            {
                delay_count = 0;
                ajust_roll++;
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            delay_count++;
            if (delay_count > 5)
            {
                delay_count = 0;
                ajust_roll--;
            }
        }


        //補正角度の決定
        rotation_angle_roll = initial_roll + ajust_roll;

        //Debug.Log
        str1 = "roll: " + roll.ToString("f1") + " pitch: " + pitch.ToString("f1") + " yaw: " + yaw.ToString("f1");
        score_object1.GetComponent<TextMesh>().text = str1;
        str2 = "condition: " + condition_angle_roll.ToString() + " num: " + condition_num.ToString() + "-" + trial_num.ToString() + " initial: " + initial_roll.ToString() + " ajust: " + ajust_roll.ToString() + " rotation: " + rotation_angle_roll.ToString();
        score_object2.GetComponent<TextMesh>().text = str2;
        //Debug.Logの表示・非表示
        if (MASK_clear == false)
        {
            score_object1.SetActive(false);
            score_object2.SetActive(false);
        }
        else
        {
            score_object1.SetActive(true);
            score_object2.SetActive(true);
        }

        //補正部分（HMD座標系のyaw回転と相殺させることで常に天球を正面にする）
        newAngle.x = 0;
        newAngle.y = -yaw;
        newAngle.z = SetOffset(rotation_angle_roll);
        sphere100.gameObject.transform.localEulerAngles = newAngle;

        //調整角決定＋書き出し
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            // ファイル書き出し
            string[] str1 = { condition_angle_roll.ToString(), trial_num.ToString(), rotation_angle_roll.ToString(), roll.ToString(), pitch.ToString(), yaw.ToString() };
            string str2 = string.Join(",", str1);
            File.AppendAllText(Application.persistentDataPath + filePath, str2 + "\n");
            //
            ajust_roll = 0;     //調整角度リセット
            trial_num++;        //試行回数インクリメント
            next_flag = true;   //次の試行へ
        }
    }

    //0度～360度に補正
    float SetOffset(float deg)
    {
        float mod_deg;

        if (deg >= 0 && deg <= 360)
        {
            mod_deg = deg;
        }
        else if(deg > 360)
        {
            mod_deg = deg - 360.0f;
        }
        else
        {
            mod_deg = deg + 360.0f;
        }

        return mod_deg;
    }

    //0度～360度を±180度に変換
    float SetHalf(float deg)
    {
        float mod_deg;

        if (deg > 180)
        {
            mod_deg = deg - 360;
        }
        else
        {
            mod_deg = deg;
        }

        return mod_deg;
    }

}
