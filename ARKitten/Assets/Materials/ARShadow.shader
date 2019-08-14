// based on a shader from https://alastaira.wordpress.com/2014/12/30/adding-shadows-to-a-unity-vertexfragment-shader-in-7-easy-steps/

Shader "Custom/ARShadow"
{
    SubShader {
        Pass {
         
            // 1.) これはフォワードレンダリングのベースパスで、環境光、頂点、メインの平行光源（Directional Light）が適用される。
            // 追加のライトはLightModeに"ForwardAdd"を使用して加算パスにすることがを必要。
            // https://docs.unity3d.com/ja/current/Manual/SL-PassTags.html を参照のこと。
            Tags { "LightMode" = "ForwardBase" "RenderType"="Opaque" "Queue"="Geometry+1" "ForceNoShadowCasting"="True"  }
            LOD 150
            Blend Zero SrcColor
            ZWrite On
        
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            // 2.) LightModeタグの"ForwardBase"と一致させてシェーダーが確実にforward baseパスにコンパイルされるようにする。
            // LightModeタグと同様に、追加のライトがある場合は_fwdbaseを_fwdaddに変更する。
            #pragma multi_compile_fwdbase
 
            // 3.) ライティングシャドウのマクロを含むUnityのライブラリを参照する
            #include "AutoLight.cginc"
 
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                 
                // 4.) LIGHTING_COORDSマクロ（AutoLight.cgincで定義されている）はシャドウマップをサンプリングするために必要なパラメーターを定義する。
                // (0,1)は、サンプリングされた値を保持するためにどのTEXCOORDを使用していないかを示す。
                // つまり、このシェーダーではどのtexcoords（テクスチャ座標）も使用していないので
                // TEXCOORD0とTEXCOORD1をシャドウのサンプリングに使うことができることを示す。
                // もし既にTEXCOORDをUV座標に使用しているという場合は、LIGHTING_COORDS(1,2)と指定してTEXCOORD1とTEXCOORD2を使うようにすることができる
                LIGHTING_COORDS(0,1)
            };
 
 
            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                 
                // 5.) TRANSFER_VERTEX_TO_FRAGMENTマクロは、v2f構造体で選択されたLIGHTING_COORDSに
                // シャドウ/ライティングマップからサンプリングするための適切な値を追加する
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                 
                return o;
            }
 
            fixed4 frag(v2f i) : COLOR {
             
                // 6.) LIGHT_ATTENUATIONは、TRANSFER_VERTEX_TO_FRAGMENTで計算されてLIGHTING_COORDSで定義された構造体に格納された座標を使って
                // シャドウマップをサンプリングする。そしてその値をfloatで返す。
                float attenuation = LIGHT_ATTENUATION(i);
                return fixed4(1.0,1.0,1.0,1.0) * attenuation;
            }
 
            ENDCG
        }
    }
     
    // 7.) 影を受ける、もしくは落とすには、シェーダーは適切な"Shadow Collector"もしくは"Shadow Caster"のパスを実装しなければならない。
    // このシェーダーではそのように明示してはいないけれども、もしこれらのパスが存在しない場合はフォールバックシェーダーが代わりに読み込まれる。
    // そのフォールバックで使用されるcollectorおよびcasterのパスをインポートするシェーダーをここで指定する。
    Fallback "VertexLit"

}