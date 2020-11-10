using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class move_sphere100_exp1 : MonoBehaviour
{
    public GameObject score_object = null; // Textオブジェクト
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
    private string str = "huga";
    //書き出し
    private string filePath = @"/test_0.csv";   //PC\VR-Headset\内部共有ストレージ\Android\data\com.sens.BedriddenVR_exp1\files に入る
    //実験用パラメータ
    private float condition_angle_roll = 30.0f;  //条件角度
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
    }

    // Update is called once per frame
    void Update()
    {
        //HMD角度取得
        roll = angle.GetComponent<getTracking>().roll;
        pitch = angle.GetComponent<getTracking>().pitch;
        yaw = angle.GetComponent<getTracking>().yaw;

        //初期値生成（上昇系列・下降系列）
        if(next_flag == true)
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

        //条件角度設定用（マスク）
        if ((condition_angle_roll + 1.0f) >= SetHalf(roll) && SetHalf(roll) >= (condition_angle_roll - 1.0f))
        {
            MASK_right.SetActive(false);
            MASK_left.SetActive(false);
        }
        else if((condition_angle_roll + 1.0f) < SetHalf(roll))
        {
            MASK_right.SetActive(true);
            MASK_left.SetActive(false);
        }
        else
        {
            MASK_right.SetActive(false);
            MASK_left.SetActive(true);
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
        ////Debug.Log
        str = "num: " + trial_num.ToString() + " initial: " + initial_roll.ToString() + " ajust: " + ajust_roll.ToString() + " rotation: " + rotation_angle_roll.ToString();
        score_object.GetComponent<TextMesh>().text = str;

        //補正部分（HMD座標系のyaw回転と相殺させることで常に天球を正面にする）
        newAngle.x = 0;
        newAngle.y = -yaw;
        newAngle.z = SetOffset(rotation_angle_roll);
        sphere100.gameObject.transform.localEulerAngles = newAngle;
        //sphere100.gameObject.transform.rotation = Quaternion.Euler(newAngle);   //意味は上と同じ


        //調整角決定＋書き出し
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            // ファイル書き出し
            string[] str1 = { trial_num.ToString(), rotation_angle_roll.ToString(), roll.ToString(), pitch.ToString(), yaw.ToString() };
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
