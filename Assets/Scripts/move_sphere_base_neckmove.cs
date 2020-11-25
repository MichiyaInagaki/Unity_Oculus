using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class move_sphere_base_neckmove : MonoBehaviour
{
    public GameObject score_object1 = null; // Textオブジェクト
    public GameObject score_object2 = null; // Textオブジェクト
    public GameObject sphere_base;
    public GameObject angle;
    public AudioSource audioSource;
    public AudioClip sound1;
    public AudioClip sound2;
    public AudioClip sound3;
    public AudioClip sound4;
    public AudioClip sound5;
    public AudioClip sound6;
    public AudioClip sound7;
    public AudioClip sound8;
    public AudioClip sound9;
    public AudioClip sound10;
    public AudioClip sound_additional;
    public AudioClip sound_start;
    public AudioClip sound_in;
    public AudioClip sound_out;
    private Vector3 newAngle = new Vector3(0, 0, 0);    //回転姿勢格納
    //書き出し
    private string filePath = @"/Data";   //PC\VR-Headset\内部共有ストレージ\Android\data\com.sens.BedriddenVR_exp1\files に入る
    private bool csv_input_flag = false;
    private int file_num = 0;
    private float elapsedTime = 0.0f;
    //±180度表記
    private float _roll = 0.0f;
    private float _pitch = 0.0f;
    private float _yaw = 0.0f;
    //Σ0座標系＝物理世界における回転角
    private float roll = 0.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;
    //オフセット角
    private float offset_roll = 0.0f;
    private float offset_pitch = 0.0f;
    private float offset_yaw = 0.0f;
    //調整用
    private float roll_gain = 0.0f;
    private float mod_roll = 0.0f;
    //デバッグ用
    private string str1 = "huga";
    private string str2 = "huga";
    private int debug_count;
    private int pre_debug_count;
    private int DEBUG_NUM = 12;
    private bool debug_text_flag = false;

    // Start is called before the first frame update
    void Start()
    {
        debug_count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        pre_debug_count = debug_count;
        //ゲイン調整
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
        {
            debug_count++;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            debug_count--;
        }
        if (debug_count < 0)
        {
            debug_count += DEBUG_NUM;
        }
        debug_count %= DEBUG_NUM;

        //音声出力用
        if(debug_count != pre_debug_count)
        {
            PlaySound(debug_count);
        }

        //HMD角度取得
        roll = angle.GetComponent<getTracking>().roll;
        pitch = angle.GetComponent<getTracking>().pitch;
        yaw = angle.GetComponent<getTracking>().yaw;

        //±180度表記
        _roll = SetHalf(roll);
        _pitch = SetHalf(pitch);
        _yaw = SetHalf(yaw);

        //CSV書き出し
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (csv_input_flag == false)
            {
                audioSource.PlayOneShot(sound_in);
                elapsedTime = 0.0f;
                file_num++;
                csv_input_flag = true;
            }
            else
            {
                audioSource.PlayOneShot(sound_out);
                csv_input_flag = false;
            }
        }
        if (csv_input_flag == true)
        {
            elapsedTime += Time.deltaTime;
            string[] str1 = { elapsedTime.ToString(), roll.ToString(), pitch.ToString(), yaw.ToString(), _roll.ToString(), _pitch.ToString(), _yaw.ToString() };
            string str2 = string.Join(",", str1);
            File.AppendAllText(Application.persistentDataPath + filePath + file_num.ToString() + ".csv", str2 + "\n");
        }

        //オフセットとる
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && Input.GetKey(KeyCode.Escape))
        {
            offset_roll = roll;
            offset_pitch = pitch;
            offset_yaw = yaw;
        }


        //補正部分----------------------------------------------------
        if (debug_count == 0)
        {
            roll_gain = 0.0f;
            mod_roll = 0.0f;
            //roll, pitch補正：roll = 目と水平
            newAngle.x = 0;     //× pitch
            newAngle.y = 0;     //〇 yaw
            newAngle.z = 0;     //× roll
        }
        else if (debug_count == DEBUG_NUM - 1)
        {
            roll_gain = 1.0f;
            mod_roll = -roll;
            //roll補正なし：roll = 物理世界と水平
            newAngle.x = 0;     //×
            newAngle.y = 0;     //〇
            newAngle.z = -roll; //〇
        }
        else
        {
            roll_gain = -debug_count * 0.002f;
            if (roll < 180)
            {
                mod_roll = roll_gain * roll * roll;
            }
            else
            {
                mod_roll = -roll_gain * SetHalf(roll) * SetHalf(roll);
            }

            //rollゲイン補正
            newAngle.x = 0;     //×
            newAngle.y = 0;     //〇
            newAngle.z = mod_roll; //ゲイン補正
        }



        //Debug.Log
        str1 = "roll: " + roll.ToString("f1") + " pitch: " + pitch.ToString("f1") + " yaw: " + yaw.ToString("f1");
        score_object1.GetComponent<TextMesh>().text = str1;
        str2 = "Debug count: " + debug_count.ToString() + " roll_gain: " + roll_gain.ToString() + " mod_roll: " + mod_roll.ToString();
        score_object2.GetComponent<TextMesh>().text = str2;
        //デバッグ用（マスク全消し）
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryTouchpad) && Input.GetKey(KeyCode.Escape))
        {
            if (debug_text_flag == false)
            {
                debug_text_flag = true;
            }
            else
            {
                debug_text_flag = false;
            }
        }
        //Debug.Logの表示・非表示
        if (debug_text_flag == false)
        {
            score_object1.SetActive(false);
            score_object2.SetActive(false);
        }
        else
        {
            score_object1.SetActive(true);
            score_object2.SetActive(true);
        }

        //姿勢の代入
        sphere_base.gameObject.transform.localEulerAngles = newAngle;
        //sphere100.gameObject.transform.rotation = Quaternion.Euler(newAngle);   //意味は上と同じ
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


    void PlaySound(int num)
    {
        switch (num)
        {
            case 0:
                audioSource.PlayOneShot(sound_start);
                break;
            case 1:
                audioSource.PlayOneShot(sound1);
                break;
            case 2:
                audioSource.PlayOneShot(sound2);
                break;
            case 3:
                audioSource.PlayOneShot(sound3);
                break;
            case 4:
                audioSource.PlayOneShot(sound4);
                break;
            case 5:
                audioSource.PlayOneShot(sound5);
                break;
            case 6:
                audioSource.PlayOneShot(sound6);
                break;
            case 7:
                audioSource.PlayOneShot(sound7);
                break;
            case 8:
                audioSource.PlayOneShot(sound8);
                break;
            case 9:
                audioSource.PlayOneShot(sound9);
                break;
            case 10:
                audioSource.PlayOneShot(sound10);
                break;
            default:
                audioSource.PlayOneShot(sound_additional);
                break;
        }
    }

}
