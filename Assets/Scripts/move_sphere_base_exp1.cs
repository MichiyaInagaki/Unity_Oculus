﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class move_sphere_base_exp1 : MonoBehaviour
{
    public GameObject score_object = null; // Textオブジェクト
    public GameObject sphere_base;
    public GameObject angle;
    private Vector3 newAngle = new Vector3(0, 0, 0);    //回転姿勢格納
    //Σ0座標系＝物理世界における回転角
    private float roll = 0.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;
    //Σ1座標系＝横臥HMD姿勢における回転角
    private float sigma1_roll = 0.0f;
    private float sigma1_pitch = 0.0f;
    private float sigma1_yaw = 0.0f;
    //オフセット角
    private float offset_roll = 0.0f;
    private float offset_pitch = 0.0f;
    private float offset_yaw = 0.0f;
    //デバッグ用
    private string str = "huga";
    private int debug_count;
    private int DEBUG_NUM = 5;

    // Start is called before the first frame update
    void Start()
    {
        debug_count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //デバッグ用
        //if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
        //{
        //    debug_count++;
        //    debug_count %= DEBUG_NUM;
        //}

        //HMD角度取得
        roll = angle.GetComponent<getTracking>().roll;
        pitch = angle.GetComponent<getTracking>().pitch;
        yaw = angle.GetComponent<getTracking>().yaw;

        //オフセットとる
        //if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && Input.GetKey(KeyCode.Escape))
        //{
        //    offset_roll = roll;
        //    offset_pitch = pitch;
        //    offset_yaw = yaw;
        //}

        ////Debug.Log
        str = "Debug count: " + debug_count.ToString();
        score_object.GetComponent<TextMesh>().text = str;

        //補正部分----------------------------------------------------
        switch (debug_count)
        {
            case 0:
                //roll, pitch, yaw固定＝全トラッキング×    //0がHMD座標に完全追従なのでトラッキング無効化（yawのみyaw代入で無効化）
                newAngle.x = 0;     //×
                newAngle.y = yaw+20;   //×
                newAngle.z = 0;     //×
                break;
            case 1:
                //補正なし（オフセットのみ）＝全トラッキング〇
                newAngle.x = offset_pitch - pitch;    //〇
                newAngle.y = 0;     //〇
                newAngle.z = -roll; //〇
                break;
            case 2:
                //pitch補正のみ（roll：物理世界と水平）
                newAngle.x = 0;     //×
                newAngle.y = 0;     //〇
                newAngle.z = -roll; //〇
                break;
            case 3:
                //roll補正のみ（roll：目線と水平）
                newAngle.x = offset_pitch - pitch;    //〇
                newAngle.y = 0;         //〇
                newAngle.z = 0;         //×
                break;
            case 4:
                //roll, pitch補正（rollとpitchはHMD座標を完全追従＋Yawのみ独自回転）
                newAngle.x = 0;     //× pitch
                newAngle.y = 0;     //〇 yaw
                newAngle.z = 0;     //× roll
                break;
            default:
                break;
        }
        sphere_base.gameObject.transform.localEulerAngles = newAngle;
        //sphere100.gameObject.transform.rotation = Quaternion.Euler(newAngle);   //意味は上と同じ
    }
}
