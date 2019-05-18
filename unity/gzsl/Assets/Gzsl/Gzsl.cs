public class Gzsl {
  private int m_Flags;
  public int Flags {
    get { return m_Flags; }
  }
  
  private UnityEngine.Sprite[] m_ImageSprites;
  public UnityEngine.Sprite[] ImageSprites {
    get { return m_ImageSprites; }
  }
  
#pragma warning disable 0649
  private struct GzslJson {
    public int      flags;
    public string[] images;
  }
#pragma warning restore 0649
  
  public delegate void OnLoadComplete();
  public delegate void OnLoadError( System.Exception exception );
  
  public Gzsl(){
    m_Flags = 0;
    m_ImageSprites = new UnityEngine.Sprite[ 0 ];
  }
  
  public System.Collections.IEnumerator LoadFromJsonAsync( string json, OnLoadComplete on_load_complete, OnLoadError on_load_error ){
    GzslJson gzsl_json;
    try{
      gzsl_json = UnityEngine.JsonUtility.FromJson< GzslJson >( json );
      if ( null == gzsl_json.images ) throw new System.Exception( "Invalid json: "+ json );
      m_Flags = gzsl_json.flags;
      m_ImageSprites = new UnityEngine.Sprite[ gzsl_json.images.Length ];
    }catch ( System.Exception exception ){
      on_load_error( exception );
      yield break;
    }
    for ( int i = 0; i < gzsl_json.images.Length; ++i ){
      try{
        byte[] image_bytes = System.Convert.FromBase64String( gzsl_json.images[ i ] );
        UnityEngine.Texture2D texture = new UnityEngine.Texture2D( 1, 1 );
        UnityEngine.ImageConversion.LoadImage( texture, image_bytes, false );
        m_ImageSprites[ i ] = UnityEngine.Sprite.Create( texture, new UnityEngine.Rect( 0, 0, texture.width, texture.height ), UnityEngine.Vector2.zero );
      }catch ( System.Exception exception ){
        on_load_error( exception );
        yield break;
      }
      yield return null;
    }
    on_load_complete();
  }
  
  public UnityEngine.Sprite GetImageSprite( ref int index ){
    int count = m_ImageSprites.Length;
    if ( 0 == count ) return null;
    
    if ( count <= index ){
      index = 0;
    }else if ( index < 0 ){
      index = count - 1;
    }
    return m_ImageSprites[ index ];
  }
}
