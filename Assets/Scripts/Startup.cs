using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{

    private void Awake()
    {
        // 画面解像度 scale 1.0 == 1024x1024 ～ 1.25 == 1280x1280(物理的な解像度の限界) 0.5fなどもフレームレートを稼ぐのに有効。
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.25f;

        // 外枠側のレンダリングを荒くすることで速度を稼ぐ。 Off/LMSLow/LMSMedium/LMSHigh　外側の解像度が減っていく
        OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSHigh;

        // 72Hzモード（綺麗になるがフレームレート安定が犠牲になる）
        OVRManager.display.displayFrequency = 72.0f;

        // CPU,GPUのパフォーマンスレベル。0～3の大きいほど放熱が高く高スペックな動作をする。既定値は2。
        //OVRManager.cpuLevel = 2;
        //OVRManager.gpuLevel = 2;

        // テクスチャ解像度 デフォルトは1.0 上げれば上げるほどVR内のジャギーが消えてなめらかに描画できる。(効果は絶大だが劇的に重くなる)
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 2.0f;

        // 画面端の方が赤っぽくなることへの補正を入れる (adb shell setprop debug.oculus.forceChroma 1 相当)
        //OVRPlugin.chromatic = true;

        // アンチエイリアシング。あまり実感できない
        // QualitySettings.antiAliasing = 4;

        // シングルパスステレオレンダリング (普通は右目用と左目用の2回レンダリングを行うところを、1シーンにそれを無理やり押し込める手法)
        // Edit > ProjectSettings > Player
        //   XRSettings > StereoRenderingMethod: MultiPass→SinglePass

        // ・OVRCameraRigのOVR ManagerにあるEnable Adaptive Resolutionをオフにする
        // GPUの使用率が高くなるとフレーム落ちしないように自動的に解像度を下げる機能
        // かなり効果があったとのこと
    }

}