using UnityEngine;
using System.Collections;

namespace AMEvents.SocialAPI
{
	class FB : AMSocialAPI
	{
		public FB SetGender (string gender)
		{
			if (gender != null)
			{
				this.gender = gender;
			}
			else
			{
				this.gender = string.Empty;
			}
			return this;
		}
		public FB SetBirthday (string birthday)
		{
			var parse = birthday.Split (new char[]{'/'}, System.StringSplitOptions.RemoveEmptyEntries);
			var current = System.DateTime.Now;

			int year = 0;
			int mount = 0;
			int day = 0;
			if ((parse.Length > 2) && (int.TryParse (parse [2], out year)) && (int.TryParse (parse [1], out day)) && (int.TryParse (parse [0], out mount))) 
			{
				try 
				{
					var bday = new System.DateTime (year, mount, day);
					this.birthday = string.Format ("{0}:{1}:{2}", bday.Year.ToString ("0000"), bday.Month.ToString ("00"), bday.Day.ToString ("00"));
					this.age = System.Math.Floor ((current - bday).TotalDays/365.25).ToString ("0");	
				} 
				catch (System.Exception) 
				{
					this.birthday = "";
					this.age = "";	
				}
			}
			else
			{
				if ((parse.Length > 1) && (int.TryParse (parse [1], out day)) && (int.TryParse (parse [0], out mount))) 
				{
					try 
					{
						var bday = new System.DateTime (1, mount, day);
						this.birthday = string.Format ("{0}:{1}:{2}", "0000", bday.Month.ToString ("00"), bday.Day.ToString ("00"));
						this.age = "";
					} 
					catch (System.Exception) 
					{
						this.birthday = "";
						this.age = "";	
					}
				}
				else
				{
					this.birthday = "";
					this.age = "";
				}
			}

			return this;
		}
		public FB SetMaritalStatus (string maritalStatus)
		{
			if (maritalStatus != null)
			{
				this.maritalStatus = maritalStatus;
			}
			else
			{
				this.maritalStatus = string.Empty;
			}
			return this;
		}
		public void ConfigureFinished ()
		{
			SetNativeSDK ();
		}
	}
}
