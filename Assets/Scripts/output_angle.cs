using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class output_angle : MonoBehaviour
{
    public GameObject angle;
    //Σ0座標系＝物理世界における回転角
    private float roll = 0.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;
    //±180度表記
    private float _roll = 0.0f;
    private float _pitch = 0.0f;
    private float _yaw = 0.0f;
    //書き出し
    private string filePath = @"/Data.csv";   //PC\VR-Headset\内部共有ストレージ\Android\data\com.sens.BedriddenVR_exp1\files に入る
    private bool output_flag = false;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //HMD角度取得
        roll = angle.GetComponent<getTracking>().roll;
        pitch = angle.GetComponent<getTracking>().pitch;
        yaw = angle.GetComponent<getTracking>().yaw;

        _roll = SetHalf(roll);
        _pitch = SetHalf(pitch);
        _yaw = SetHalf(yaw);

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && Input.GetKey(KeyCode.Escape))
        {
            output_flag = true;
        }

        if (output_flag == true)
        {
            elapsedTime += Time.deltaTime;
            // ファイル書き出し
            string[] str1 = { elapsedTime.ToString(), roll.ToString(), pitch.ToString(), yaw.ToString(), _roll.ToString(), _pitch.ToString(), _yaw.ToString() };
            string str2 = string.Join(",", str1);
            File.AppendAllText(Application.persistentDataPath + filePath, str2 + "\n");
        }
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
