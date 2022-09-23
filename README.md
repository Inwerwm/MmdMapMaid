# MmdMapMaid

MmdMapMaid は MikuMikuDance のプロジェクトファイルにまつわる諸々をいい感じにしてくれる機能を詰め合わせたアプリケーションです。

## 機能

### EMM 材質並び替え
EMM ファイルではモデルの各材質に対するエフェクト設定を順番によって管理しているため、 PMX を編集して材質順が変化すると正しい材質の割当ができなくなってしまいます。  
この機能は EMM ファイルと材質順変更前後の PMX モデルファイルを読み込んで、EMMファイルの材質設定を正しく並び替えます。

### EMD 抽出
この機能は EMM ファイルからモデル単位の設定を抽出し、 EMD ファイルとして保存します。  
抽出元 EMM ファイル、抽出対象オブジェクト、エフェクト設定(MME 材質割当のタブ) を指定してください。

### PMM パス置き換え
この機能はPMMファイルで読み込むモデル・アクセサリーのパス設定を書き換えます。

## アイコン画像について

![アイコン画像](images/icon.png)

この画像は Stable-Diffusion によって生成されました。  
生成パラメータは以下の通りです。

```
Prompt: 1girl ahoge aqua_eyes aqua_hair aqua_necktie bangs collared_shirt dot_nose light_blush long_hair looking_at_viewer maid maid_apron maid_headdress necktie parted_lips simple_background solo twintails very_long_hair, hyperrealistic, octane_renderer, portrait, pale skin
Negative prompt: bad anatomy disfigured mutated
Steps: 50, Sampler: Euler a, CFG scale: 7.5, Seed: 1918698426, Size: 512x512
Checkpoint: Waifu-Diffusion 1.2 full EMA
```
