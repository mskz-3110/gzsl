public class GzslViewer : UnityEngine.MonoBehaviour {
  private UnityEngine.UI.Toggle m_PDFFileTypeToggle;
  private UnityEngine.UI.Toggle m_GZSLFileTypeToggle;
  
  private UnityEngine.UI.InputField m_URLInputField;
  
  private UnityEngine.UI.Button m_LoadButton;
  
  private UnityEngine.UI.Image m_SlideImage;
  
  private Web m_Web;
  
  private Gzsl m_Gzsl;
  private int m_GzslIndex;
  
  void Awake(){
    m_PDFFileTypeToggle = UnityEngine.GameObject.Find( "PDFFileTypeToggle" ).GetComponent< UnityEngine.UI.Toggle >();
    m_GZSLFileTypeToggle = UnityEngine.GameObject.Find( "GZSLFileTypeToggle" ).GetComponent< UnityEngine.UI.Toggle >();
    
    m_URLInputField = UnityEngine.GameObject.Find( "URLInputField" ).GetComponent< UnityEngine.UI.InputField >();
    
    m_LoadButton = UnityEngine.GameObject.Find( "LoadButton" ).GetComponent< UnityEngine.UI.Button >();
    m_LoadButton.onClick.AddListener( OnClickLoadButton );
    
    m_SlideImage = UnityEngine.GameObject.Find( "SlideImage" ).GetComponent< UnityEngine.UI.Image >();
    
    m_Web = new Web( this );
    
    m_Gzsl = new Gzsl( this );
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
  
  private void OnGzslLoadComplete( Gzsl gzsl ){
    m_GzslIndex = 0;
    m_SlideImage.sprite = m_Gzsl.GetImageSprite( ref m_GzslIndex );
    if ( null != m_SlideImage.sprite ) m_SlideImage.GetComponent< UnityEngine.RectTransform >().sizeDelta = new UnityEngine.Vector2( m_SlideImage.sprite.rect.width, m_SlideImage.sprite.rect.height );
  }
  
  public void OnClickLoadButton(){
    string url = m_URLInputField.text;
    if ( m_PDFFileTypeToggle.isOn ){
      url = "https://short-works.herokuapp.com/pdf/view.json?url="+ url;
      UnityEngine.Debug.Log( url );
      m_Web.Get( url, 60, ( UnityEngine.Networking.UnityWebRequest request ) => {
        m_Gzsl.LoadFromJson( request.downloadHandler.text, OnGzslLoadComplete, ( Gzsl gzsl, System.Exception exception ) => {
          UnityEngine.Debug.LogError( exception +"\n"+ request.downloadHandler.text );
        } );
      }, ( UnityEngine.Networking.UnityWebRequest request ) => {
        UnityEngine.Debug.LogError( request.error );
      } );
    }else if ( m_GZSLFileTypeToggle.isOn ){
      UnityEngine.Debug.Log( url );
      m_Web.Get( url, 60, ( UnityEngine.Networking.UnityWebRequest request ) => {
        m_Gzsl.Load( request.downloadHandler.data, OnGzslLoadComplete, ( Gzsl gzsl, System.Exception exception ) => {
          UnityEngine.Debug.LogError( exception );
        } );
      }, ( UnityEngine.Networking.UnityWebRequest request ) => {
        UnityEngine.Debug.LogError( request.error );
      } );
    }
  }
}
