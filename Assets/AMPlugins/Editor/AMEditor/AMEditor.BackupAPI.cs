#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

namespace AMEditor 
{
	public class AMEditorBackupAPI 
	{
		public static string pathBackup = string.Empty;
		public static string nameBackup = "AMBackup";
		
		public static string separator = Path.DirectorySeparatorChar.ToString ();
		
		/// <summary>
		/// Gets the backups.
		/// </summary>
		/// <returns>The backups.</returns>
		public static ArrayList GetBackups ()
		{
			var result = new ArrayList ();
			try 
			{
				string[] backupList = Directory.GetDirectories (nameBackup);
				
				foreach (var item in backupList) 
				{
					result.Add (item);
				}
			}
			catch (Exception ex) 
			{
				AMEditorLog.Write ("Message Error: " + ex.Message);	
			}
			return result;
		}
		
		/// <summary>
		/// Gets the files backup.
		/// </summary>
		/// <returns>The files backup.</returns>
		public static List<Backup> GetFilesBackup ()
		{
			var result = new List<Backup> ();
			
			AMEditorFileStorage.CreateFolder (nameBackup, false);
			
			string[] backupList = Directory.GetDirectories (nameBackup);
			
			foreach (var item in backupList) 
			{
				var backup = new Backup ();
				backup.name = item.Substring (item.LastIndexOf (separator) +1);
				backup.dateTime = ToDateTime (backup.name);
				backup.extended = false;
				
				backup.files = new List<fileToggle> ();
				var source = GetFilesDirectory (item);
				
				foreach (var f in source) 
				{
					backup.files.Add (new fileToggle (f, false));
				}
				//result.Add (item.Substring (item.LastIndexOf ("/") +1), GetFilesDirectory (item));
				result.Add (backup);
			}
			
			return result;
		}
		
		/// <summary>
		/// Gets the files directory.
		/// </summary>
		/// <returns>The files directory.</returns>
		/// <param name="path">Path.</param>
		public static List<string> GetFilesDirectory (string path)
		{
			var result = new List<string> ();
			var InFolders = Directory.GetDirectories (path);
			var InFiles = Directory.GetFiles (path);
			
			foreach (var item in InFiles) 
			{
				result.Add (item);
			}
			foreach (var item in InFolders) 
			{
				var temp = GetFilesDirectory (item);
				foreach (var f in temp) 
				{
					result.Add (f);
				}
			}
			return result;
		}
		
		/// <summary>
		/// Tos the date time.
		/// </summary>
		/// <returns>The date time.</returns>
		/// <param name="time">Time.</param>
		public static string ToDateTime (string time)
		{
			string result = string.Empty;
			try 
			{
				result = time[0].ToString () + time[1].ToString () + time[2].ToString () + time[3].ToString () + "." + 
						time[4].ToString () + time[5].ToString () + "." + 
						time[6].ToString () + time[7].ToString () + " " +
						time[9].ToString () + time[10].ToString () + ":" +
						time[11].ToString () + time[12].ToString () + ":" +
						time[13].ToString () + time[14].ToString ();
			} 
			catch (Exception ex) 
			{
				AMEditorLog.Write ("Message : " + ex.Message);
			}
			return result;
		}
		
		/// <summary>
		/// Backupings the files.
		/// </summary>
		/// <param name="paths">Paths.</param>
		public static void BackupingFiles (List<string> paths)
		{
			foreach (var item in paths) 
			{
				BackupingFile (item);
			}
			pathBackup = string.Empty;
		}
		
		/// <summary>
		/// Backupings the file.
		/// </summary>
		/// <param name="path">Path.</param>
		public static void BackupingFile (string path)
		{
			try 
			{
				CheckBackupFolder (path);
				File.Copy (path, nameBackup +"/" + GetFilenameBackup () + "/" + path, true);

				Debug.Log ("!!!path: " + path);
				if (AMEditorFileStorage.FileExist (path))
					AMEditorFileStorage.RemoveFile (path);
				if (AMEditorFileStorage.FileExist (path + ".meta"))
					AMEditorFileStorage.RemoveFile (path + ".meta");
			} 
			catch (Exception ex) 
			{
				Debug.Log ("Error delete. Message: " + ex.Message);
			}
		}
		
		/// <summary>
		/// Views the name of the mode backup.
		/// </summary>
		/// <returns>The mode backup name.</returns>
		/// <param name="oldName">Old name.</param>
		public static string ViewModeBackupName (string oldName)
		{
			string newName = oldName;
			
			newName = newName.Substring (newName.IndexOf (separator) + 1);
			newName = newName.Substring (newName.IndexOf (separator) + 1);
			
			return newName;
		}
		
		/// <summary>
		/// Checks the backup folder.
		/// </summary>
		/// <returns>The backup folder.</returns>
		/// <param name="pathfile">Pathfile.</param>
		static string CheckBackupFolder (string pathfile)
		{
			try 
			{	
				string path = nameBackup + "/" + GetFilenameBackup () + "/" + pathfile.Substring (0, pathfile.LastIndexOf ("/"));
				if (!Directory.Exists (path)) 
				{
					Directory.CreateDirectory (path);
				}
			} 
			catch (Exception ex) 
			{
				Debug.Log ("Error create directory. Message: " + ex.Message);
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Gets the filename backup.
		/// </summary>
		/// <returns>The filename backup.</returns>
		static string GetFilenameBackup ()
		{
			var result = string.Empty;
			if (pathBackup == string.Empty)
			{
				var t = DateTime.Now;
				result = t.ToString ("u").Replace ("-", "").Replace (":", "").Replace (" ", "_").Replace ("Z", "");
			}
			else
				result = pathBackup;
			return result;
		}
		
		/// <summary>
		/// Restores the file.
		/// </summary>
		/// <param name="path">Path.</param>
		public static void RestoreFile (string path)
		{
			AMEditorFileStorage.CopyFile (path, ViewModeBackupName (path), true);
			//File.Copy (path, ViewModeBackupName (path), true);
			AMEditorLog.Write ("restore file : " + ViewModeBackupName (path));	
		}
		
		/// <summary>
		/// Deletes all backups.
		/// </summary>
		public static void DeleteAllBackups ()
		{
			AMEditorFileStorage.RemoveFolder (nameBackup);
			AMEditorLog.Write ("Delete ALL backups");
		}
		
		/// <summary>
		/// 	
		/// </summary>
		/// <param name="name">Name.</param>
		public static void DeleteBackup (string name)
		{
			AMEditorFileStorage.RemoveFolder (nameBackup + separator + name);
			AMEditorLog.Write ("Delete backup : " + name);
		}
	}
	public class ElementRestore
	{
		public string name = "";
		public string dateTime = "";
		public ArrayList fileList = new ArrayList ();
		//public bool change = false; 
	}
	public class fileToggle
	{
		public string name = "";
		public string cause = "";
		public bool delete = false;
		public fileToggle (string name, bool delete, string cause = "")
		{
			this.name = name;
			this.delete = delete;
			this.cause = cause;
		}
	}
}
#endif