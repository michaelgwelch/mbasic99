// FileOgranization.cs created with MonoDevelop
// User: mgwelch at 7:32 PM 8/28/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.IO;
namespace TIBasicRuntime
{
	public enum FileOrganization
	{
		Sequential,
		Relative
	}
	
	public enum FileType
	{
		Display,
		Internal
	}
	
	public enum FileOpenMode
	{
		Input,
		Output,
		Update,
		Append
	}
	
	public enum FileRecordType
	{
		Fixed,
		Variable
	}
    
}
