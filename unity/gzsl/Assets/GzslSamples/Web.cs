using System.Collections.Generic;

public class Web {
  private UnityEngine.MonoBehaviour m_MonoBehaviour;
  
  public delegate void OnWebComplete( UnityEngine.Networking.UnityWebRequest request );
  public delegate void OnWebError( UnityEngine.Networking.UnityWebRequest request );
  
  public class Params {
    private Dictionary< string, object > m_Params;
    
    public Params(){
      m_Params = new Dictionary< string, object >();
    }
    
    public void Set( string key, object value ){
      if ( m_Params.ContainsKey( key ) ){
        m_Params[ key ] = value;
      }else{
        m_Params.Add( key, value );
      }
    }
    
    public UnityEngine.WWWForm ToForm(){
      UnityEngine.WWWForm form = new UnityEngine.WWWForm();
      foreach ( KeyValuePair< string, object > pair in m_Params ){
        form.AddField( pair.Key, pair.Value );
      }
      return form;
    }
  }
  
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
  
  private System.Collections.IEnumerator PostCoroutine( string url, Params params int timeout, OnWebComplete on_web_complete, OnWebError on_web_error ){
    UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Post( url, params.ToForm() );
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
  
  public void Post( string url, Params params, int timeout, OnWebComplete on_web_complete, OnWebError on_web_error ){
    m_MonoBehaviour.StartCoroutine( PostCoroutine( url, params, timeout, on_web_complete, on_web_error ) );
  }
}
