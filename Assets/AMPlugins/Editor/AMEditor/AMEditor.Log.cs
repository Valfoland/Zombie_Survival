#if UNITY_EDITOR
using UnityEngine;

namespace AMEditor
{
	public class AMEditorLog
	{
		/// <summary>
		/// Write the specified log.
		/// </summary>
		/// <param name="log">Log.</param>
		public static void Write (string log)
		{
			Debug.Log (log);
		}
	}
}
#endif