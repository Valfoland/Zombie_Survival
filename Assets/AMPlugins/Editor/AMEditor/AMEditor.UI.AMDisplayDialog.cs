#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace AMEditor.UI
{
	class AMDisplayDialog
	{
		private System.Action OkHandler;
		private System.Action CancelHandler;
		
		private string title;
		private string message;
		private string ok;
		private string cancel;
		
		private bool awaitingShow;
		
		private bool isShow;
		
		private void UpdateGUI ()
		{
			EditorApplication.update -= UpdateGUI;
			isShow = true;
			if (cancel == string.Empty)
			{
				try 
				{
					if (EditorUtility.DisplayDialog (title, message, ok))
					{
						if (OkHandler != null)
						{
							OkHandler ();
						}
					}
				} 
				catch (System.Exception) 
				{
					Debug.Log ("Error show DisplayDialog");	
				}
			}
			else
			{
				try 
				{
					if (EditorUtility.DisplayDialog (title, message, ok, cancel))
					{
						if (OkHandler != null)
						{
							OkHandler ();
						}
					}
					else
					{
						if (CancelHandler != null)
						{
							CancelHandler ();
						}
					}
				} 
				catch (System.Exception ex) 
				{
					Debug.Log ("Error show DisplayDialog\n" + ex.ToString ());
				}
			}
		}
		
		public AMDisplayDialog (string title, string message, string ok, string cancel, System.Action OkHandler, System.Action CancelHandler, bool awaitingShow)
		{
			this.OkHandler = OkHandler;
			this.CancelHandler = CancelHandler;
			
			this.title = title;
			this.message = message;
			this.ok = ok;
			this.cancel = cancel;
			
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
	}
}
#endif

