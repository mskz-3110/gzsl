public class Date {
  static public Date Now(){
    return new Date();
  }
  
  private System.DateTime m_LocalTime;
  
  public Date(){
    m_LocalTime = System.DateTime.UtcNow.ToLocalTime();
  }
  
  public override string ToString(){
    return string.Format( "{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}.{6:000}",
      m_LocalTime.Year,
      m_LocalTime.Month,
      m_LocalTime.Day,
      m_LocalTime.Hour,
      m_LocalTime.Minute,
      m_LocalTime.Second,
      m_LocalTime.Millisecond );
  }
}
