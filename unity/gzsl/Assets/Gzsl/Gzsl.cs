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
  
  private struct ThreadLoadFromJsonAsyncData {
    public Gzsl           gzsl;
    public string         json;
    public OnLoadComplete on_load_complete;
    public OnLoadError    on_load_error;
    
    public ThreadLoadFromJsonAsyncData( Gzsl gzsl, string json, OnLoadComplete on_load_complete, OnLoadError on_load_error ){
      this.gzsl = gzsl;
      this.json = json;
      this.on_load_complete = on_load_complete;
      this.on_load_error = on_load_error;
    }
  }
  
  public Gzsl(){
    Clear();
  }
  
  public void Clear(){
    m_Flags = 0;
    m_ImageSprites = new UnityEngine.Sprite[ 0 ];
  }
  
  public void LoadFromJson( string json, OnLoadComplete on_load_complete, OnLoadError on_load_error ){
    try{
      GzslJson gzsl_json = UnityEngine.JsonUtility.FromJson< GzslJson >( json );
      if ( null == gzsl_json.images ) throw new System.Exception( "Invalid json: "+ json );
      m_Flags = gzsl_json.flags;
      m_ImageSprites = new UnityEngine.Sprite[ gzsl_json.images.Length ];
      for ( int i = 0; i < gzsl_json.images.Length; ++i ){
        byte[] image_bytes = System.Convert.FromBase64String( gzsl_json.images[ i ] );
        
        // Main thread only
        UnityEngine.Texture2D texture = new UnityEngine.Texture2D( 0, 0 );
        UnityEngine.ImageConversion.LoadImage( texture, image_bytes, false );
        m_ImageSprites[ i ] = UnityEngine.Sprite.Create( texture, new UnityEngine.Rect( 0, 0, texture.width, texture.height ), UnityEngine.Vector2.zero );
      }
      on_load_complete();
    }catch ( System.Exception exception ){
      Clear();
      on_load_error( exception );
    }
  }
  
#if false
  static private void OnThreadLoadFromJsonAsync( object arg ){
    ThreadLoadFromJsonAsyncData data = (ThreadLoadFromJsonAsyncData)arg;
    data.gzsl.LoadFromJson( data.json, data.on_load_complete, data.on_load_error );
  }
  
  public void LoadFromJsonAsync( string json, OnLoadComplete on_load_complete, OnLoadError on_load_error ){
    try{
      System.Threading.Thread thread = new System.Threading.Thread( OnThreadLoadFromJsonAsync );
      thread.Start( new ThreadLoadFromJsonAsyncData( this, json, on_load_complete, on_load_error ) );
    }catch ( System.Exception exception ){
      Clear();
      on_load_error( exception );
    }
  }
#endif
  
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
