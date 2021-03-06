﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class move_sphere100 : MonoBehaviour
{
    //public GameObject score_object = null; // Textオブジェクト
    public GameObject sphere100;
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
        //HMD角度取得
        roll = angle.GetComponent<getTracking>().roll;
        pitch = angle.GetComponent<getTracking>().pitch;
        yaw = angle.GetComponent<getTracking>().yaw;

        ////オフセットとる
        //if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && Input.GetKey(KeyCode.Escape))
        //{
        //    offset_roll = roll;
        //    offset_pitch = pitch;
        //    offset_yaw = yaw;
        //}
        //sigma1_roll = SetOffset(roll - offset_roll);       //オフセットを差し引いて360度で丸める
        //sigma1_pitch = SetOffset(pitch - offset_pitch);
        //sigma1_yaw = SetOffset(yaw - offset_yaw);

        ////Debug.Log
        //str = "sigma1_roll: " + sigma1_roll.ToString("f1") + " pitch: " + sigma1_pitch.ToString("f1") + " yaw: " + sigma1_yaw.ToString("f1") + " num: " + debug_count.ToString();
        //score_object.GetComponent<TextMesh>().text = str;

        //補正部分（HMD座標系のyaw回転と相殺させることで常に天球を正面にする）
        newAngle.x = 0;
        newAngle.y = -yaw;
        newAngle.z = 0;
        sphere100.gameObject.transform.localEulerAngles = newAngle;
        //sphere100.gameObject.transform.rotation = Quaternion.Euler(newAngle);   //意味は上と同じ
    }


    //0度～360度に補正
    float SetOffset(float deg)
    {
        float mod_deg;

        if (deg > 0)
        {
            mod_deg = deg;
        }
        else
        {
            mod_deg = 360.0f + deg;
        }

        return mod_deg;
    }
}
