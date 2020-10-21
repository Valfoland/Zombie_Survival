#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace AMEditor.UI
{
	class AMProgressBar
	{
		private System.Action CancelHandler;
		
		private string title;
		private string message;
		private float progress;
		
		private bool awaitingShow;
		
		private bool isShow;
		
		private void UpdateGUI ()
		{
			EditorApplication.update -= UpdateGUI;
			isShow = true;
			try 
			{
				if (EditorUtility.DisplayCancelableProgressBar (title, message, progress))
				{
					if (CancelHandler != null)
					{
						CancelHandler ();
					}
				}
			} 
			catch (Exception) 
			{
				Debug.Log ("Error show ProgressBar");	
			}
		}
		
		public AMProgressBar (string title, string message, float progress, System.Action CancelHandler, bool awaitingShow)
		{
			this.CancelHandler = CancelHandler;
			
			this.title = title;
			this.message = message;
			this.progress = progress;
			
			this.awaitingShow = awaitingShow;
			
			isShow = false;
		}
		
		public void Show ()
		{
			EditorApplication.update += UpdateGUI;
			if (!awaitingShow)
			{
				System.Threading.Thread.Sleep (300);
				if (!isShow)
				{
					UpdateGUI ();
				}
			}
		}

		public static void Clean ()
		{
			try 
			{
				EditorUtility.ClearProgressBar ();
			} 
			catch (Exception) 
			{
				Debug.Log ("Error show ProgressBar");	
			}
		}
	}
}
#endif