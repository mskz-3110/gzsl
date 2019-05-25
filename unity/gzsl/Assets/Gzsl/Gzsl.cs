public class Gzsl {
  private UnityEngine.MonoBehaviour m_MonoBehaviour;
  
  private UnityEngine.Sprite[] m_ImageSprites;
  public UnityEngine.Sprite[] ImageSprites {
    get { return m_ImageSprites; }
  }
  
#pragma warning disable 0649
  private struct JsonData {
    public int      flags;
    public string[] images;
  }
#pragma warning restore 0649
  
  public struct ImageData {
    public byte[] bytes;
  }
  
  public struct Data {
    public ImageData[] images;
  }
  
  public delegate void OnParseComplete( Data data );
  public delegate void OnParseError( System.Exception exception );
  
  public delegate void OnLoadComplete( Gzsl gzsl );
  public delegate void OnLoadError( Gzsl gzsl, System.Exception exception );
  
  private System.Collections.IEnumerator ParseCoroutine( byte[] bytes, OnParseComplete on_parse_complete, OnParseError on_parse_error, bool is_async ){
    int offset = 0;
    byte flags = 0;
    try{
      flags = Dastbytes.Binary.UnpackUInt8( bytes, offset );
      if ( 0 != flags ) throw new System.Exception( "Invalid flags: "+ flags );
    }catch ( System.Exception exception ){
      on_parse_error( exception );
      yield break;
    }
    offset += sizeof( byte );
    
    ushort count = 0;
    Data data;
    try{
      count = Dastbytes.Binary.UnpackUInt16( bytes, offset );
      data = new Data();
      data.images = new ImageData[ count ];
    }catch ( System.Exception exception ){
      on_parse_error( exception );
      yield break;
    }
    offset += sizeof( ushort );
    if ( is_async ) yield return null;
    
    for ( int i = 0; i < count; ++i ){
      try{
        uint image_size = Dastbytes.Binary.UnpackUInt32( bytes, offset );
        offset += sizeof( uint );
        data.images[ i ].bytes = Dastbytes.Binary.Clone( bytes, offset, (int)image_size );
        offset += (int)image_size;
      }catch ( System.Exception exception ){
        on_parse_error( exception );
        yield break;
      }
      if ( is_async ) yield return null;
    }
    on_parse_complete( data );
  }
  
  public void Parse( byte[] bytes, OnParseComplete on_parse_complete, OnParseError on_parse_error, bool is_async ){
    m_MonoBehaviour.StartCoroutine( ParseCoroutine( bytes, on_parse_complete, on_parse_error, is_async ) );
  }
  
  private System.Collections.IEnumerator ParseFromJsonCoroutine( string json, OnParseComplete on_parse_complete, OnParseError on_parse_error, bool is_async ){
    JsonData json_data;
    try{
      json_data = UnityEngine.JsonUtility.FromJson< JsonData >( json );
      if ( null == json_data.images ) throw new System.Exception( "Invalid json: "+ json );
    }catch ( System.Exception exception ){
      on_parse_error( exception );
      yield break;
    }
    if ( is_async ) yield return null;
    
    Data data;
    try{
      data = new Data();
      data.images = new ImageData[ json_data.images.Length ];
    }catch ( System.Exception exception ){
      on_parse_error( exception );
      yield break;
    }
    if ( is_async ) yield return null;
    
    for ( int i = 0; i < json_data.images.Length; ++i ){
      try{
        data.images[ i ].bytes = System.Convert.FromBase64String( json_data.images[ i ] );
      }catch ( System.Exception exception ){
        on_parse_error( exception );
        yield break;
      }
      if ( is_async ) yield return null;
    }
    on_parse_complete( data );
  }
  
  public void ParseFromJson( string json, OnParseComplete on_parse_complete, OnParseError on_parse_error, bool is_async ){
    m_MonoBehaviour.StartCoroutine( ParseFromJsonCoroutine( json, on_parse_complete, on_parse_error, is_async ) );
  }
  
  public Gzsl( UnityEngine.MonoBehaviour mono_behaviour ){
    m_MonoBehaviour = mono_behaviour;
    Clear();
  }
  
  public void Clear(){
    m_ImageSprites = new UnityEngine.Sprite[ 0 ];
  }
  
  private System.Collections.IEnumerator LoadImageSpritesCoroutine( Data data, OnLoadComplete on_load_complete, OnLoadError on_load_error, bool is_async ){
    try{
      m_ImageSprites = new UnityEngine.Sprite[ data.images.Length ];
    }catch ( System.Exception exception ){
      on_load_error( this, exception );
      yield break;
    }
    if ( is_async ) yield return null;
    
    for ( int i = 0; i < data.images.Length; ++i ){
      UnityEngine.Texture2D texture;
      try{
        texture = new UnityEngine.Texture2D( 0, 0 );
      }catch ( System.Exception exception ){
        on_load_error( this, exception );
        yield break;
      }
      if ( is_async ) yield return null;
      
      try{
        UnityEngine.ImageConversion.LoadImage( texture, data.images[ i ].bytes, false );
      }catch ( System.Exception exception ){
        on_load_error( this, exception );
        yield break;
      }
      if ( is_async ) yield return null;
      
      try{
        m_ImageSprites[ i ] = UnityEngine.Sprite.Create( texture, new UnityEngine.Rect( 0, 0, texture.width, texture.height ), UnityEngine.Vector2.zero );
      }catch ( System.Exception exception ){
        on_load_error( this, exception );
        yield break;
      }
      if ( is_async ) yield return null;
    }
    on_load_complete( this );
  }
  
  public void Load( byte[] bytes, OnLoadComplete on_load_complete, OnLoadError on_load_error, bool is_async ){
    Parse( bytes, ( Data data ) => {
      m_MonoBehaviour.StartCoroutine( LoadImageSpritesCoroutine( data, on_load_complete, on_load_error, is_async ) );
    }, ( System.Exception exception ) => {
      on_load_error( this, exception );
    }, is_async );
  }
  
  public void LoadFromJson( string json, OnLoadComplete on_load_complete, OnLoadError on_load_error, bool is_async ){
    ParseFromJson( json, ( Data data ) => {
      m_MonoBehaviour.StartCoroutine( LoadImageSpritesCoroutine( data, on_load_complete, on_load_error, is_async ) );
    }, ( System.Exception exception ) => {
      on_load_error( this, exception );
    }, is_async );
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
