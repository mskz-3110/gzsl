public class GzslViewer : UnityEngine.MonoBehaviour {
  private UnityEngine.UI.Toggle m_PDFFileTypeToggle;
  private UnityEngine.UI.Toggle m_GZSLFileTypeToggle;
  
  private UnityEngine.UI.InputField m_URLInputField;
  
  private UnityEngine.UI.Button m_LoadButton;
  
  private UnityEngine.UI.Image m_SlideImage;
  
  private delegate void OnWebError( string msg );
  private delegate void OnWebComplete( int code, string body );
  
  private Gzsl m_Gzsl;
  private int m_GzslIndex;
  
  void Awake(){
    m_PDFFileTypeToggle = UnityEngine.GameObject.Find( "PDFFileTypeToggle" ).GetComponent< UnityEngine.UI.Toggle >();
    m_GZSLFileTypeToggle = UnityEngine.GameObject.Find( "GZSLFileTypeToggle" ).GetComponent< UnityEngine.UI.Toggle >();
    
    m_URLInputField = UnityEngine.GameObject.Find( "URLInputField" ).GetComponent< UnityEngine.UI.InputField >();
    
    m_LoadButton = UnityEngine.GameObject.Find( "LoadButton" ).GetComponent< UnityEngine.UI.Button >();
    m_LoadButton.onClick.AddListener( OnClickLoadButton );
    
    m_SlideImage = UnityEngine.GameObject.Find( "SlideImage" ).GetComponent< UnityEngine.UI.Image >();
    
    m_Gzsl = new Gzsl();
    m_GzslIndex = 0;
  }
  
  void Update(){
    if ( UnityEngine.Input.GetKeyUp( UnityEngine.KeyCode.LeftArrow ) ){
      --m_GzslIndex;
      m_SlideImage.sprite = m_Gzsl.GetImageSprite( ref m_GzslIndex );
    }else if ( UnityEngine.Input.GetKeyUp( UnityEngine.KeyCode.RightArrow ) ){
      ++m_GzslIndex;
      m_SlideImage.sprite = m_Gzsl.GetImageSprite( ref m_GzslIndex );
    }
  }
  
  public void OnClickLoadButton(){
    string file_type = "pdf";
    if ( m_GZSLFileTypeToggle.isOn ){
      file_type = "gzsl";
    }
    
    UnityEngine.Debug.Log( Date.Now() +" WebGetAsync start" );
    string url = "https://short-works.herokuapp.com/"+ file_type +"/view.json?url="+ m_URLInputField.GetComponentsInChildren< UnityEngine.UI.Text >()[ 1 ].text;
    UnityEngine.Debug.Log( url );
    StartCoroutine( WebGetAsync( url, ( int code, string body ) => {
      UnityEngine.Debug.Log( Date.Now() +" WebGetAsync complete" );
      m_Gzsl.LoadFromJson( body, () => {
        UnityEngine.Debug.Log( Date.Now() +" Gzsl.LoadFromJson complete" );
        m_GzslIndex = 0;
        m_SlideImage.sprite = m_Gzsl.GetImageSprite( ref m_GzslIndex );
        if ( null != m_SlideImage.sprite ) m_SlideImage.GetComponent< UnityEngine.RectTransform >().sizeDelta = new UnityEngine.Vector2( m_SlideImage.sprite.rect.width, m_SlideImage.sprite.rect.height );
        UnityEngine.Debug.Log( Date.Now() +" finish" );
      }, ( System.Exception exception ) => {
        UnityEngine.Debug.LogError( exception +"\n"+ body );
      } );
    }, ( string msg ) => {
      UnityEngine.Debug.LogError( msg );
    } ) );
  }
  
  private System.Collections.IEnumerator WebGetAsync( string url, OnWebComplete on_web_complete, OnWebError on_web_error ){
    UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get( url );
    request.timeout = 60;
    yield return request.SendWebRequest();
    
    if ( request.isNetworkError || request.isHttpError ){
      on_web_error( request.error );
    }else{
      on_web_complete( (int)request.responseCode, request.downloadHandler.text );
    }
  }
}
