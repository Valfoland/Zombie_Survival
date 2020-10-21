using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace AMEvents {
	public class Internet {
		public static event System.Action Succeeded;
		public static event System.Action NotEstablished;

		public static void OnSucceededEvent () 
		{
			if (Succeeded != null) 
			{
				Succeeded ();
			}
		}
		public static void OnNotEstablishedEvent () 
		{
			if (NotEstablished != null) 
			{
				NotEstablished ();
			}
		}
		public static void CheckInternetAccess (string url = "") 
		{
			NativeBridge.CheckInternetAccess(url);
		}
		public static void StartInternetChecking (string url = "", int timeInterval = 5) 
		{
			NativeBridge.StartInternetChecking(url, timeInterval);
		}
		public static void StopInternetChecking () 
		{
			NativeBridge.StopInternetChecking();
		}
	}
}
