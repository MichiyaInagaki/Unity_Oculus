using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class ShaderController : MonoBehaviour
{
	public GameObject score_object1 = null; // Textオブジェクト
	private string str_debug = "huga";
	public AudioSource audioSource;
	public AudioClip sound_upper;
	public AudioClip sound_lower;
	//
	private Renderer _renderer;
	private float ScaleY_upper;
	private float ScaleY_lower;
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
		ScaleY_upper = 1.0f;
		ScaleY_lower = 1.0f;
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

		//上下編集点切り替え
		if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
		{
			if (ul_switch)
			{
				audioSource.PlayOneShot(sound_upper);
				// ファイル書き出し
				string[] str1 = { "upper", ScaleY_upper.ToString("f2") };
				string str2 = string.Join(",", str1);
				File.AppendAllText(Application.persistentDataPath + filePath, str2 + "\n");
				//
				ul_switch = false;
			}
			else
			{
				audioSource.PlayOneShot(sound_lower);
				// ファイル書き出し
				string[] str1 = { "lower", ScaleY_lower.ToString("f2") };
				string str2 = string.Join(",", str1);
				File.AppendAllText(Application.persistentDataPath + filePath, str2 + "\n");
				//
				ul_switch = true;
			}
		}

		////調整部
		//if (ul_switch)
		//{
		//	if (Input.GetKey(KeyCode.UpArrow))
		//	{
		//		ScaleY_upper = RangeIn(ScaleY_upper + delta);
		//	}
		//	if (Input.GetKey(KeyCode.DownArrow))
		//	{
		//		ScaleY_upper = RangeIn(ScaleY_upper - delta);
		//	}
		//}
		//else
		//{
		//	if (Input.GetKey(KeyCode.UpArrow))
		//	{
		//		ScaleY_lower = RangeIn(ScaleY_lower - delta);
		//	}
		//	if (Input.GetKey(KeyCode.DownArrow))
		//	{
		//		ScaleY_lower = RangeIn(ScaleY_lower + delta);
		//	}
		//}


		//調整部
		if (ul_switch)
		{
			if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
			{
				//毎フレームだと速すぎるので，nフレームで1増分
				delay_count++;
				if (delay_count > 3)
				{
					delay_count = 0;
					ScaleY_upper = RangeIn(ScaleY_upper + delta);
				}
			}
			if (Input.GetKey(KeyCode.Escape))
			{
				delay_count++;
				if (delay_count > 3)
				{
					delay_count = 0;
					ScaleY_upper = RangeIn(ScaleY_upper - delta);
				}
			}
		}
		else
		{
			if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
			{
				delay_count++;
				if (delay_count > 3)
				{
					delay_count = 0;
					ScaleY_lower = RangeIn(ScaleY_lower - delta);
				}
			}
			if (Input.GetKey(KeyCode.Escape))
			{
				delay_count++;
				if (delay_count > 3)
				{
					delay_count = 0;
					ScaleY_lower = RangeIn(ScaleY_lower + delta);
				}
			}
		}

		//Debug.Log
		str_debug = "upper: " + ScaleY_upper.ToString("f2") + " lower: " + ScaleY_lower.ToString("f2");
		score_object1.GetComponent<TextMesh>().text = str_debug;

		//
		_renderer.material.SetFloat("_ScaleY_upper", ScaleY_upper);
		_renderer.material.SetFloat("_ScaleY_lower", ScaleY_lower);
	}

	//データを0～1に収める
	float RangeIn(float data)
	{
		float num = data;
		if (data > 1.0f)
		{
			num = 1.0f;
		}
		else if (data < 0.0f)
		{
			num = 0.0f;
		}
		return num;
	}
}