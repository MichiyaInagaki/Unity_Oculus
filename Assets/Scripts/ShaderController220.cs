using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class ShaderController220 : MonoBehaviour
{
	public GameObject score_object1 = null; // Textオブジェクト
	private string str_debug = "huga";
	public AudioSource audioSource;
	public AudioClip sound_upper;
	public AudioClip sound_lower;
	//
	private Renderer _renderer;
	private float Offset_upper;
	private float Offset_lower;
	private float ScaleY_upper;
	private float ScaleY_lower;
	//
	private float OFFSET_UPPER_MIN = -0.5f;
	private float OFFSET_UPPER_MAX = 0.0f;
	private float OFFSET_LOWER_MIN = 0.0f;
	private float OFFSET_LOWER_MAX = 0.5f;
	private float SCALEY_MIN = 0.01f;
	private float SCALEY_MAX = 2.0f;
	//
	private float delta = 0.01f;    //刻み幅
	private bool ul_switch = true;  //編集点切り替え
	private int delay_count = 0;
	//書き出し
	private string filePath = @"/Data.csv";   //PC\VR-Headset\内部共有ストレージ\Android\data\com.sens.BedriddenVR_exp1\files に入る


	void Start()
	{
		//マテリアル取得用
		_renderer = GetComponent<Renderer>();
		//パラメータ初期値
		Offset_upper = -0.25f;
		Offset_lower = 0.0f;
		ScaleY_upper = 0.01f;
		ScaleY_lower = 0.01f;
	}

	void Update()
	{
		////上下編集点切り替え
		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//	if (ul_switch)
		//	{
		//		ul_switch = false;
		//	}
		//	else
		//	{
		//		ul_switch = true;
		//	}
		//}

		//////調整部
		//if (ul_switch)
		//{
		//	if (Input.GetKey(KeyCode.UpArrow))
		//	{
		//		Offset_upper = RangeIn(Offset_upper + delta, OFFSET_UPPER_MIN, OFFSET_UPPER_MAX);
		//	}
		//	if (Input.GetKey(KeyCode.DownArrow))
		//	{
		//		Offset_upper = RangeIn(Offset_upper - delta, OFFSET_UPPER_MIN, OFFSET_UPPER_MAX);
		//	}
		//	if (Input.GetKey(KeyCode.W))
		//	{
		//		ScaleY_upper = RangeIn(ScaleY_upper - delta, SCALEY_MIN, SCALEY_MAX);
		//	}
		//	if (Input.GetKey(KeyCode.S))
		//	{
		//		ScaleY_upper = RangeIn(ScaleY_upper + delta, SCALEY_MIN, SCALEY_MAX);
		//	}
		//}
		//else
		//{
		//	if (Input.GetKey(KeyCode.UpArrow))
		//	{
		//		Offset_lower = RangeIn(Offset_lower + delta, OFFSET_LOWER_MIN, OFFSET_LOWER_MAX);
		//	}
		//	if (Input.GetKey(KeyCode.DownArrow))
		//	{
		//		Offset_lower = RangeIn(Offset_lower - delta, OFFSET_LOWER_MIN, OFFSET_LOWER_MAX);
		//	}
		//	if (Input.GetKey(KeyCode.W))
		//	{
		//		ScaleY_lower = RangeIn(ScaleY_lower - delta, SCALEY_MIN, SCALEY_MAX);
		//	}
		//	if (Input.GetKey(KeyCode.S))
		//	{
		//		ScaleY_lower = RangeIn(ScaleY_lower + delta, SCALEY_MIN, SCALEY_MAX);
		//	}
		//}


		//以下Oculus用------------------------------------------------------------

		//上下編集点切り替え
		if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
		{
			if (ul_switch)
			{
				audioSource.PlayOneShot(sound_upper);
				// ファイル書き出し
				string[] str1 = { "offset_upper", Offset_upper.ToString("f2"), "scaleY_upper", ScaleY_upper.ToString("f2") };
				string str2 = string.Join(",", str1);
				File.AppendAllText(Application.persistentDataPath + filePath, str2 + "\n");
				//
				ul_switch = false;
			}
			else
			{
				audioSource.PlayOneShot(sound_lower);
				// ファイル書き出し
				string[] str1 = { "offset_lower", Offset_lower.ToString("f2"), "scaleY_lower", ScaleY_lower.ToString("f2") };
				string str2 = string.Join(",", str1);
				File.AppendAllText(Application.persistentDataPath + filePath, str2 + "\n");
				//
				ul_switch = true;
			}
		}

		////調整部
		if (ul_switch)
		{
			if (OVRInput.Get(OVRInput.Button.Up))
			{
				Offset_upper = RangeIn(Offset_upper + delta, OFFSET_UPPER_MIN, OFFSET_UPPER_MAX);
			}
			if (OVRInput.Get(OVRInput.Button.Down))
			{
				Offset_upper = RangeIn(Offset_upper - delta, OFFSET_UPPER_MIN, OFFSET_UPPER_MAX);
			}
			if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
			{
				delay_count++;
				if (delay_count > 3)
				{
					delay_count = 0;
					ScaleY_upper = RangeIn(ScaleY_upper - delta, SCALEY_MIN, SCALEY_MAX);
				}
			}
			if (Input.GetKey(KeyCode.Escape))
			{
				delay_count++;
				if (delay_count > 3)
				{
					delay_count = 0;
					ScaleY_upper = RangeIn(ScaleY_upper + delta, SCALEY_MIN, SCALEY_MAX);
				}
			}
		}
		else
		{
			if (OVRInput.Get(OVRInput.Button.Up))
			{
				Offset_lower = RangeIn(Offset_lower + delta, OFFSET_LOWER_MIN, OFFSET_LOWER_MAX);
			}
			if (OVRInput.Get(OVRInput.Button.Down))
			{
				Offset_lower = RangeIn(Offset_lower - delta, OFFSET_LOWER_MIN, OFFSET_LOWER_MAX);
			}
			if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
			{
				delay_count++;
				if (delay_count > 3)
				{
					delay_count = 0;
					ScaleY_lower = RangeIn(ScaleY_lower - delta, SCALEY_MIN, SCALEY_MAX);
				}
			}
			if (Input.GetKey(KeyCode.Escape))
			{
				delay_count++;
				if (delay_count > 3)
				{
					delay_count = 0;
					ScaleY_lower = RangeIn(ScaleY_lower + delta, SCALEY_MIN, SCALEY_MAX);
				}
			}
		}


		//上下編集点切り替え
		//if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
		//{
		//	if (ul_switch)
		//	{
		//		audioSource.PlayOneShot(sound_upper);
		//		// ファイル書き出し
		//		string[] str1 = { "upper", ScaleY_upper.ToString("f2") };
		//		string str2 = string.Join(",", str1);
		//		File.AppendAllText(Application.persistentDataPath + filePath, str2 + "\n");
		//		//
		//		ul_switch = false;
		//	}
		//	else
		//	{
		//		audioSource.PlayOneShot(sound_lower);
		//		// ファイル書き出し
		//		string[] str1 = { "lower", ScaleY_lower.ToString("f2") };
		//		string str2 = string.Join(",", str1);
		//		File.AppendAllText(Application.persistentDataPath + filePath, str2 + "\n");
		//		//
		//		ul_switch = true;
		//	}
		//}

		////調整部
		//if (ul_switch)
		//{
		//	if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
		//	{
		//		//毎フレームだと速すぎるので，nフレームで1増分
		//		delay_count++;
		//		if (delay_count > 3)
		//		{
		//			delay_count = 0;
		//			ScaleY_upper = RangeIn(ScaleY_upper + delta);
		//		}
		//	}
		//	if (Input.GetKey(KeyCode.Escape))
		//	{
		//		delay_count++;
		//		if (delay_count > 3)
		//		{
		//			delay_count = 0;
		//			ScaleY_upper = RangeIn(ScaleY_upper - delta);
		//		}
		//	}
		//}
		//else
		//{
		//	if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
		//	{
		//		delay_count++;
		//		if (delay_count > 3)
		//		{
		//			delay_count = 0;
		//			ScaleY_lower = RangeIn(ScaleY_lower - delta);
		//		}
		//	}
		//	if (Input.GetKey(KeyCode.Escape))
		//	{
		//		delay_count++;
		//		if (delay_count > 3)
		//		{
		//			delay_count = 0;
		//			ScaleY_lower = RangeIn(ScaleY_lower + delta);
		//		}
		//	}
		//}
		//----------------------------------------------------------------------------

		//Debug.Log
		str_debug = "Ou: " + Offset_upper.ToString("f2") + " Ol: " + Offset_lower.ToString("f2") + " Su: " + ScaleY_upper.ToString("f2") + " Sl: " + ScaleY_lower.ToString("f2");
		score_object1.GetComponent<TextMesh>().text = str_debug;

		//
		_renderer.material.SetFloat("_Offset_upper", Offset_upper);
		_renderer.material.SetFloat("_Offset_lower", Offset_lower);
		_renderer.material.SetFloat("_ScaleY_upper", ScaleY_upper);
		_renderer.material.SetFloat("_ScaleY_lower", ScaleY_lower);
	}

	//データをmin～maxに収める
	float RangeIn(float data, float min, float max)
	{
		float num = data;
		if (data > max)
		{
			num = max;
		}
		else if (data < min)
		{
			num = min;
		}
		return num;
	}
}