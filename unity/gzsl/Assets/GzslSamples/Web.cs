public class Web {
  private UnityEngine.MonoBehaviour m_MonoBehaviour;
  
  public delegate void OnWebComplete( UnityEngine.Networking.UnityWebRequest request );
  public delegate void OnWebError( UnityEngine.Networking.UnityWebRequest request );
  
  public Web( UnityEngine.MonoBehaviour mono_behaviour ){
    m_MonoBehaviour = mono_behaviour;
  }
  
  private System.Collections.IEnumerator GetCoroutine( string url, int timeout, OnWebComplete on_web_complete, OnWebError on_web_error ){
    UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get( url );
    request.timeout = timeout;
    yield return request.SendWebRequest();
    
    if ( request.isNetworkError || request.isHttpError ){
      on_web_error( request );
    }else{
      on_web_complete( request );
    }
  }
  
  public void Get( string url, int timeout, OnWebComplete on_web_complete, OnWebError on_web_error ){
    m_MonoBehaviour.StartCoroutine( GetCoroutine( url, timeout, on_web_complete, on_web_error ) );
  }
}
