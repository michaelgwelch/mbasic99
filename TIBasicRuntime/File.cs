
// User: mgwelch at 7:40 PM 8/28/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NativeFile = System.IO.File;

namespace TIBasicRuntime
{
	
	public class File : IDisposable
	{

        readonly StreamReader reader;
        readonly StreamWriter writer;
        readonly bool isCasette;
        

        const int sizeOfNumber = 8;
        static readonly Encoding encoding = Encoding.ASCII;
        
		private File()
		{			
		}
        
        private File(IDeviceMapping deviceMap, String fileName, FileOrganization org, FileType fileType,
                     FileOpenMode mode, FileRecordType recordType)
        {
            //if (fileName != CS1 && fileName != CS2) throw new InvalidOperationException("Only fileNames of CS1 and CS2 are currently supported");
            
            String nativeFileName = deviceMap.GetNativeFileName(fileName);
            //if (fileName == CS2 && mode == FileOpenMode.Input) throw new InvalidOperationException("Can't use CS2 for Input");
            if (org != FileOrganization.Sequential) throw new InvalidOperationException("CS1 and CS2 only support sequential");
            if (recordType != FileRecordType.Fixed) throw new InvalidOperationException("CS1 and CS2 only support Fixed");
            if (fileType != FileType.Internal) throw new NotImplementedException("Internal is preferred for casettes, Display is not yet implemented.");
            if (mode == FileOpenMode.Input)
            {
                FileStream stream = NativeFile.Open(nativeFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                reader = new StreamReader(stream, encoding);
            }
            else
            {
                FileStream stream = NativeFile.Open(nativeFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                 
                writer = new StreamWriter(stream, encoding);
                writer.AutoFlush = true;
                
            }

            isCasette = true;
            
        }
        
        public static File Open(IDeviceMapping deviceMap, String fileName, FileOrganization org, FileType fileType, 
                                FileOpenMode mode, FileRecordType recordType)
        {
            return new File(deviceMap, fileName, org, fileType, mode, recordType);
        }
        
        public void Print(params object[] items)
        {
            PrintInternal(items);
        }
        
        private void PrintInternal(params object[] items)
        {
            foreach(object item in items)
            {
                if (item is String)
                {
                    string s = (string) item;
                    if (s.Length == 1)
                    {
                        char ch = s[0];
                        switch (ch)
                        {
                        case '\0': // used for semicolons
                        case '\t': // used for commas
                        case '\n': // used for colons
                            continue; // go to next item
                        }
                    }
                    
                    // if item is not one of the three chars above then print it
                    PrintInternalString(s);
                                
                }
                else
                {
                    PrintInternalNumber((double)item);
                }
            }
        }
        
        private void PrintInternalString(string item)
        {
            if (item.Length > 255) throw new ArgumentException("print items can't be 255 chars long");
            writer.Write(item.Length);
            writer.Write(item);
            
        }
        
        private void PrintInternalNumber(double item)
        {
            writer.Write(sizeOfNumber);
            writer.Write(item);
        }
        
        public void Input(object[] inputSpecifiers, out object[] items)
        {
            InputInternal(inputSpecifiers, out items);
        }
        
        public void InputInternal(object[] inputSpecifiers, out object[] items)
        {

            items = null;
        }

        public int Eof()
        {
            if (isCasette) throw new InvalidOperationException("Eof does not apply to casettes");
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            if (reader != null) reader.Dispose();
            if (writer != null) writer.Dispose();

        }
        

        
        
	}
        
    public interface TIStream : IDisposable
    {
         void Print(object[] items);
         void Input(params InputSlot[] inputSpecs);
         int Eof();
         void Restore();
    }
    
    public interface IDeviceMapping
    {
        String GetNativeFileName(String tiFileName);
            
    }
        
	public class StubbedDeviceMapping : IDeviceMapping
	{
	
        const string CS1 = "CS1";
	    const string CS2 = "CS2";
		public String GetNativeFileName(String tiFileName)
		{
            if (tiFileName == CS1) return @"/Documents And Settings/Michael Welch/CS1";
            if (tiFileName == CS2) return "/home/mgwelch/CS2";
            throw new ArgumentOutOfRangeException();
		}
	}
    
    public class CasetteStream : TIStream, IDisposable
    {
        
        // assuming only internal for now.
        const byte sizeOfNumber = 8;     // size of a double in bytes.
        const int defaultRecordSize = 64;
        static readonly int[] allowableRecordSizes = new int[] { 64, 128, 192};
        static readonly Encoding tiEncoding = Encoding.ASCII;
        
        readonly int recordSize;
        readonly FileType fileType;
        readonly FileOpenMode fileMode;
            
        // buffer
        BinaryWriter writer;
        BinaryReader reader;
        byte[] buffer;
        
        // file stream
        FileStream stream;
        
        public CasetteStream(IDeviceMapping deviceMap, String tiFileName, FileType fileType, 
                             FileOpenMode fileMode, int recSize)
        {
            String nativeFileName = deviceMap.GetNativeFileName(tiFileName);
            
            if (recordSize > allowableRecordSizes[allowableRecordSizes.Length-1]) throw new ArgumentOutOfRangeException("recordSize");
            if (fileType == FileType.Display) throw new NotImplementedException("Display mode not yet supported");
            
            foreach(int i in allowableRecordSizes)
            {
                if (i > recSize)
                {
                    this.recordSize = i;
                    break;
                }
            }
            this.fileType = fileType;
            this.fileMode = fileMode;
            
            if (this.fileMode == FileOpenMode.Output) 
            {
                CreateBuffer();
                stream = NativeFile.Open(nativeFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            }
            else
            {
                buffer = new byte[this.recordSize];
                stream = NativeFile.Open(nativeFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }
        
        private void CreateBuffer()
        {
           buffer = new byte[recordSize];
           MemoryStream stream = new MemoryStream(buffer);
           writer = new BinaryWriter(stream);
        }
        
        private void ReadBuffer()
        {
           
           stream.Read(buffer, 0, recordSize);
           MemoryStream memStream = new MemoryStream(buffer);
           reader = new BinaryReader(memStream);            
        }
        
        private void FlushBuffer()
        {
            Console.WriteLine("Flushing");

            stream.Write(buffer, 0, recordSize);
        }
        
        private void Write(String str)
        {
            if (fileType == FileType.Internal)
            {
                byte[] bytes = tiEncoding.GetBytes(str);
                writer.Write((byte)bytes.Length);
                writer.Write(bytes, 0, bytes.Length);
            }
            else 
            {
                // must be FileType.Display
                
            }
        }
        
        private void Write(double d)
        {
            if (fileType == FileType.Internal)
            {
                writer.Write(sizeOfNumber);
                writer.Write(d);
            }
            else
            {
                // must be FileType.Display
            }
        }
        
        public void Print(object[] items)
        {

            if (fileMode == FileOpenMode.Input) throw new InvalidOperationException("Can't write");
            bool pendingPrint = false;
            
            foreach(object item in items)
            {
                pendingPrint = false;
                if (IsPrintSeperator(item))
                {
                    pendingPrint = true;
                }
                else if (item is String)
                {
                    Write((string) item);
                }
                else
                {
                    Console.WriteLine(item.GetType());
                    Write((double) item);
                }
            }
            
            if (!pendingPrint) 
            {
                FlushBuffer();
                CreateBuffer();
            }
        }
        
        private String ReadString()
        {
            byte length = reader.ReadByte();
            byte[] chars = new byte[length];
            reader.Read(chars, 0, length);
            return tiEncoding.GetString(chars);
        }
        
        private double ReadDouble()
        {
            reader.ReadByte(); // will always be 8
            return reader.ReadDouble();
        }
       
        private static bool IsPrintSeperator(object item)
        {
            if (item == null) return false;
            if (!(item is string)) return false;
            string s = (string) item;
            if (s.Length != 1) return false;
            char ch = s[0];
            return (ch == '\0' || ch == '\t' || ch == '\n');
        }
        
        public int Eof() { throw new NotImplementedException(); }
        
        public void Dispose()
        {
            // If there is a print statement pending, then flush.
            if (writer != null)
            {
                if (writer.BaseStream.Position != 0) FlushBuffer();
            }
            stream.Close();
            
        }

        public void Input(params InputSlot[] inputSlots)
        {
            ReadBuffer();
            foreach(InputSlot slot in inputSlots)
            {
                if (slot.Type.Equals(typeof(string)))
                {
                    slot.Value = ReadString();
                }
                else
                {
                    slot.Value = ReadDouble();
                }
            }
        }
        public void Restore() {}
        
    }
        
      
    public class InputSlot
    {
        public readonly Type Type;
        public Object Value;
        private InputSlot(Type type) 
        {
            this.Type = type;
        }

        public static InputSlot String { get { return new InputSlot(typeof(string)); } }
        public static InputSlot Number { get { return new InputSlot(typeof(double)); } }
    }
        
}
