/*
 VRNeck3DoF Ver.0.1 - zlib License
 (C) NISHIDA Ryota, ship of EYLN http://dev.eyln.com
 Oculus GoなどのポジショントラッキングがないVRデバイスで、
 カメラ位置を１点に固定せず、疑似的な首を軸とした回転でカメラ位置を補正することで
 視界のゆがみ、違和感を軽減します。
 VRNeck3DoFをAddCompornentしたGameObjectの下にMainCameraを配置してください。
 ※OculusRiftで動作テストしているため、実際の各デバイスで正常動作するかは未確認です。
 VRNeck3DoF
   |
   +-- MainCamera
*/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VRNeck3DoF : MonoBehaviour
{
    [TooltipAttribute("位置補正を使うかどうか")]
    public bool enableModify = true;

    [TooltipAttribute("VRNeck3DoF以下にあるCameraのTransform")]
    public Transform camera;

    [TooltipAttribute("基準位置")]
    public Vector3 offset;

    [TooltipAttribute("頭の中心から目までの半径")]
    public float headRadius = 0.08f;

    [TooltipAttribute("首の相対位置")]
    public Vector3 neckPoint = new Vector3(0.0f, -0.2f, -0.05f);

    [Range(0.0f, 1.0f), TooltipAttribute("見上げる/見下ろすときの首の傾き具合")]
    public float neckPitchLevel = 0.5f;

    void Start()
    {
        offset += transform.position;
        if (!camera) camera = Camera.main.transform;
    }

    void Update()
    {
        if (!camera) return;

        // カメラの親位置をカメラ位置の反対方向に同じ量だけ動かすことでポジトラを無効化
        var fixedPos = transform.position - camera.position;

        // 補正計算した目の位置にカメラ位置を設定
        var center = fixedPos + GetEyePoint();
        if (enableModify)
        {
            transform.position = center + camera.forward * headRadius;  // 回転による位置補正あり
        }
        else transform.position = center; // 回転による位置補正なし
    }

    // カメラ向きにあわせて疑似的な首回転を経た目の位置を返す
    Vector3 GetEyePoint()
    {
        if (!enableModify) return offset - neckPoint;

        var neckRotlevel = Mathf.Abs(camera.forward.y);
        var eyeOffset = camera.rotation * -neckPoint;
        var fixedEyeOffset = camera.right * -neckPoint.x + camera.forward * -neckPoint.z;
        fixedEyeOffset.y = -neckPoint.y;
        return offset + Vector3.Lerp(eyeOffset, fixedEyeOffset, 1.0f - neckPitchLevel).normalized * neckPoint.magnitude;
    }

#if UNITY_EDITOR
    // SceneViewで頭、首の軸、目の方向などを表示
    void OnDrawGizmos()
    {
        if (!camera) return;

        var center = GetEyePoint();
        if (!EditorApplication.isPlaying) center += transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(offset, headRadius * 0.1f);
        Gizmos.DrawWireSphere(center, headRadius * 0.1f);
        Gizmos.DrawRay(offset, center - offset);
        if (enableModify)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(center, headRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(center, camera.forward * headRadius);
        }
    }
#endif
}