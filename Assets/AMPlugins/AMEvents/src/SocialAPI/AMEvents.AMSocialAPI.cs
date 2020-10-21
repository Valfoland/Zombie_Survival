#pragma warning disable
using UnityEngine;
using System.Collections;

namespace AMEvents
{
	class AMSocialAPI 
	{
		protected string gender;
		protected string birthday;
		protected string maritalStatus;
		protected string age;

		protected void SetNativeSDK ()
		{
			Hashtable json = new Hashtable ();
			json.Add ("gender", gender);
			json.Add ("birthday", birthday);
			json.Add ("marital_status", maritalStatus);
			json.Add ("age", age);

			string userInfo = AMUtils.AMJSON.JsonEncode (json);

			AMEvents.amLogger.Log ("json social: "+ userInfo);
			NativeBridge.SetDebugMode (true);
		}
	}
}