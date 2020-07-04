require "gzsl/version"
require "dastbytes"

module Gzsl
  def self.generate( output_file_path, image_file_paths )
    return false if image_file_paths.empty?
    
    File.open( output_file_path, "wb" ){|output_file|
      output_file.write Dastbytes::Binary.pack( :uint8, 0 )
      output_file.write Dastbytes::Binary.pack( :uint16, image_file_paths.size )
      image_file_paths.each{|image_file_path|
        File.open( image_file_path, "rb" ){|input_file|
          bytes = input_file.read
          output_file.write Dastbytes::Binary.pack( :uint32, bytes.size )
          output_file.write bytes
        }
      }
    }
    true
  end
  
  def self.parse( path )
    return nil if ! File.exists?( path )
    
    gzsl = {
      :flags  => 0,
      :images => []
    }
    File.open( path, "rb" ){|f|
      gzsl[ :flags ] = Dastbytes::Binary.unpack( :uint8, f.read( 1 ) ).first
      Dastbytes::Binary.unpack( :uint16, f.read( 2 ) ).first.times{
        image_size = Dastbytes::Binary.unpack( :uint32, f.read( 4 ) ).first
        gzsl[ :images ].push f.read( image_size )
      }
    }
    gzsl
  end
end
