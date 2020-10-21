using UnityEngine;
using System.Collections;

namespace AMEvents.SocialAPI
{
	class VK : AMSocialAPI
	{
		public VK SetGender (int gender)
		{
			switch (gender) {
			case 1:
				this.gender = "female";
				break;
			case 2:
				this.gender = "male";
				break;
			default:
				break;
			}
			return this;
		}
		public VK SetBirthday (string birthday)
		{
			var parse = birthday.Split (new char[]{'.'}, System.StringSplitOptions.RemoveEmptyEntries);
			var current = System.DateTime.Now;

			int year = 0;
			int mount = 0;
			int day = 0;
			if ((parse.Length > 2) && (int.TryParse (parse [2], out year)) && (int.TryParse (parse [1], out mount)) && (int.TryParse (parse [0], out day))) 
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
				this.birthday = "";
				this.age = "";
			}

			return this;
		}
		public VK SetMaritalStatus (int maritalStatus)
		{
			switch (maritalStatus) 
			{
			case 1:
				this.maritalStatus = "single";
				break;
			case 2:
				this.maritalStatus = "in a relationship";
				break;
			case 3:
				this.maritalStatus = "engaged";
				break;
			case 4:
				this.maritalStatus = "married";
				break;
			case 5:
				this.maritalStatus = "it's complicated";
				break;
			case 6:
				this.maritalStatus = "actively searching";
				break;
			case 7:
				this.maritalStatus = "in love";
				break;
			default:
				this.maritalStatus = "unknown";
				break;
			}
			return this;
		}
		public void ConfigureFinished ()
		{
			SetNativeSDK ();
		}
	}
}