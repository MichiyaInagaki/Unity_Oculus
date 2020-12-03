【PictureMaskシェーダー】
写真などのテクスチャをマスクテクスチャの形に切り抜くシェーダーです

〇主な内容物
・PictureMask_standard.shader : テクスチャをマスクテクスチャの形に切り抜くシェーダーです。Standardです
・PictureMask_unlit.shader : 上記シェーダーのUnlit版です
・SamplePicture.prefab : 設定例のPrefabです
・SampleMat.mat : 設定例のマテリアルです
・Textures : いくつかのマスクテクスチャが入っています。
・README : 利用規約や使い方を書いたもの。これです

〇設定可能項目
Auto TextureRatio : チェックをいれるとオブジェクトの縦横比をTextureの縦横比に合わせます
Texture : 切り抜かれるテクスチャです
BackColor : オブジェクトの背面の色です
[Mask]
Inverse : チェックをいれると切り抜く部分を反転します。
MaskTexture : 切り抜く形のテクスチャです。白い部分の形に切り抜かれます。
OffsetX : マスクの位置を横方向に移動します
OffsetY : マスクの位置を縦方向に移動します
Scale : 縦横比を維持したままマスクのサイズを変更します
ScaleX : マスクの横のサイズを変更します。Scaleと合わせて使ってください
ScaleY : マスクの縦のサイズを変更します。Scaleと合わせて使ってください

〇使い方
Quadにこのシェーダーを適用したマテリアルを設定します。
Textureに切り抜きたいテクスチャ、MaskTextureに切り抜く形の白黒のテクスチャを設定します
その他の項目を調整して切り抜くを変更します

〇利用規約
プログラムの著作権はgatosyocoraに帰属します。
商用利用、改変、再配布はしていただいて大丈夫です。
しかし、使用や改変、再配布時に自作発言を禁止します。
改変して配布する場合には以下のクレジットを配布物のどこかに含めるようにしてください。
使用時にはクレジットを記載する必要はありません。

〇免責事項
本製品を使って発生した問題に対してはgatosyocoraは一切の責任を負いません。

〇クレジット
Created by gatosyocora
Twitter: @gatosyocora
Booth: https://gatosyocora.booth.pm/

お問い合わせはBoothかTwitterへ
