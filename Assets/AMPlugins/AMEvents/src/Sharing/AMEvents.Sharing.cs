using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace AMEvents {

	public class Sharing 
	{
		public static event System.Action Completed;
		public static event System.Action Canceled;
		public static event System.Action<string> Error;

		public static void OnCompletedEvent ()
		{
			if (Completed != null)
			{
				Completed ();
			}
		}
		public static void OnCaneledEvent ()
		{
			if (Canceled != null)
			{
				Canceled ();
			}
		}
		public static void OnErrorEvent (string erroeText)
		{
			if (Error != null)
			{
				Error (erroeText);
			}
		}
		public static List<string> GetSocialNetworks () 
		{
			return NativeBridge.GetSocialNetworks ();
		}

		public static void Share (string networkName, Dictionary<string, string> parameters) 
		{
			NativeBridge.Share(networkName, parameters);
		}
	}
}