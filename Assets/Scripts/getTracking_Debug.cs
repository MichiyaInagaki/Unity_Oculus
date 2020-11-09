using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getTracking_Debug : MonoBehaviour
{
    public GameObject score_object = null; // Textオブジェクト
    private string str = "hoge";
    private float roll = 0.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        roll = this.transform.rotation.eulerAngles.z;
        pitch = this.transform.rotation.eulerAngles.x;
        yaw = this.transform.rotation.eulerAngles.y;
        str = "roll: " + roll.ToString("f1") + " pitch: " + pitch.ToString("f1") + " yaw: " + yaw.ToString("f1");
        // オブジェクトからTextコンポーネントを取得
        score_object.GetComponent<TextMesh>().text = str;

    }
}
