namespace Dastbytes {
public class Binary {
  static public void PackUInt8( ref byte[] bytes, int offset, byte value ){
    bytes[ offset ] = value;
  }
  
  static public void PackUInt16( ref byte[] bytes, int offset, ushort value ){
    bytes[ offset ] = (byte)(value & 0xFF);
    bytes[ offset + 1 ] = (byte)((value >> 8) & 0xFF);
  }
  
  static public void PackUInt32( ref byte[] bytes, int offset, uint value ){
    bytes[ offset ] = (byte)(value & 0xFF);
    bytes[ offset + 1 ] = (byte)((value >> 8) & 0xFF);
    bytes[ offset + 2 ] = (byte)((value >> 16) & 0xFF);
    bytes[ offset + 3 ] = (byte)((value >> 24) & 0xFF);
  }
  
  static public byte UnpackUInt8( byte[] bytes, int offset ){
    return bytes[ offset ];
  }
  
  static public ushort UnpackUInt16( byte[] bytes, int offset ){
    ushort value = bytes[ offset ];
    value += (ushort)(bytes[ offset + 1 ] << 8);
    return value;
  }
  
  static public uint UnpackUInt32( byte[] bytes, int offset ){
    uint value = bytes[ offset ];
    value += (uint)(bytes[ offset + 1 ] << 8);
    value += (uint)(bytes[ offset + 2 ] << 16);
    value += (uint)(bytes[ offset + 3 ] << 24);
    return value;
  }
  
  static public byte[] Clone( byte[] bytes, int offset, int count ){
    byte[] cloned_bytes = new byte[ count ];
    System.Buffer.BlockCopy( bytes, offset, cloned_bytes, 0, count );
    return cloned_bytes;
  }
}
}
