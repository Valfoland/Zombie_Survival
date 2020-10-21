#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMEditor
{
	public class AMEditorFileStorage
	{
		//folder
		/// <summary>
		/// Exists the folder.
		/// </summary>
		/// <returns><c>true</c>, if folder was existed, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		public static bool ExistFolder (string path)
		{
			bool result = false;
			try 
			{
				result = Directory.Exists (path);
			} 
			catch (System.Exception ex) 
			{
				result = false;
				AMEditorLog.Write ("Error directory exist. Message: "+ ex.Message);
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns><c>true</c>, if folder was removed, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		public static bool RemoveFolder (string path)
		{
			bool result = false;
			try 
			{
				Directory.Delete (path, true);
				result = true;
			}
			catch (System.Exception ex) 
			{
				result = false;
				AMEditorLog.Write ("Error directory remove. Message: "+ ex.Message);
			}
			return result;
		}

		/// <summary>
		/// Creates the folder.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="rewrite">If set to <c>true</c> rewrite.</param>
		public static bool CreateFolder (string path, bool rewrite)
		{
			if (path == string.Empty)
				return false;
			if (rewrite)
			{
				RemoveFolder (path);
			}
//			Debug.Log ("!!path : " + path + " exist : " + ExistFolder (path));
			
			if (!ExistFolder (path)) 
			{
				try 
				{
					Directory.CreateDirectory (path);
					return true;
				}
				catch (System.Exception ex) 
				{
					AMEditorLog.Write ("Error create directory. Message: " + ex.Message);	
					return false;
				}
			}
			return true;
		}
		public static void CopyFolder (string pathSource, string newPath, bool overwrite)
		{
			var listFiles = GetAllFiles (pathSource);

			foreach (var item in listFiles) 
			{
				var newp = newPath + item.Substring (pathSource.Length);	
				CopyFile (item, newp, overwrite);
			}
		}

		public static List<string> GetAllFiles (string folder)
		{
			List<string> result = new List<string> ();
			var dirlist = Directory.GetDirectories (folder);
			var filelist = Directory.GetFiles (folder);

			foreach (var item in filelist) 
			{
				result.Add (item);
			}

			foreach (var item in dirlist) 
			{
				var subfiles = GetAllFiles (item);
				foreach (var f in subfiles)
				{
					result.Add (f);
				}
			}
			return result;
		}
		////file
		/// <summary>
		/// существование файла
		/// </summary>
		/// <returns><c>true</c>, если файл существует, <c>false</c> иначе.</returns>
		/// <param name="path">Путь.</param>
		public static bool FileExist (string path)
		{
			bool result = false;
			try 
			{
				result = File.Exists (path);
			} 
			catch (System.Exception ex) 
			{
				result = false;
				AMEditorLog.Write ("Error file exist. Message: "+ ex.Message);
			}
			return result;
		}
		/// <summary>
		/// Removes the file.
		/// </summary>
		/// <param name="path">Path.</param>
		public static void RemoveFile (string path)
		{
			try 
			{
				File.Delete (path);
			} 
			catch (Exception ex) 
			{
				AMEditorLog.Write ("Error remove file. Message: " + ex.Message);	
			}
		}
		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <returns><c>true</c>, if file was saved, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		/// <param name="text">Text.</param>
		static bool SaveFile (string path, string text)
		{
			try 
			{
				using (StreamWriter sw = new StreamWriter (path))
				{
					//CCLog.Write ("file: " + path + " text: " + text);
					sw.Write (text);
				}
			} 
			catch (System.Exception ex) 
			{
				AMEditorLog.Write ("Error write file. Message: " + ex.Message);
			}
			return true;
		}
		/// <summary>
		/// Reads the file.
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="path">Path.</param>
		public static ArrayList ReadLinesFile (string path)
		{
			var result = new ArrayList (); 
			try 
			{
				var lines = File.ReadAllLines (path);
				foreach (var item in lines) {
					result.Add (item);
				}
			} 
			catch (System.Exception ex) 
			{
				AMEditorLog.Write ("Error read file. Message: " + ex.Message);
			}
			return result;
		}
		/// <summary>
		/// Reads the text file.
		/// </summary>
		/// <returns>The text file.</returns>
		/// <param name="path">Path.</param>
		public static string ReadTextFile (string path)
		{
			var result = string.Empty; 
			try 
			{
				result = File.ReadAllText (path);
			} 
			catch (System.Exception ex) 
			{
				AMEditorLog.Write ("Error read file. Message: " + ex.Message +"\n"+ ex.StackTrace);
			}
			return result;
		}
		/// <summary>
		/// поиск файла рекурсивно во всех вложенных папках
		/// </summary>
		/// <returns>список файлов с таким именем</returns>
		/// <param name="path">путь диррекиории где начать поиск</param>
		/// <param name="fileName">Имя файла</param>
		public static List<string> SearchFile (string path, string fileName)
		{
			var result = new List<string> ();
			try 
			{
				var dirlist = Directory.GetDirectories (path);
				var filelist = Directory.GetFiles (path);
				foreach (var directory in dirlist) 
				{
					var list = SearchFile (directory, fileName);
					foreach (var f in list) 
					{
						result.Add (f);
					}
				}
				foreach (var file in filelist) 
				{
					var tempFile = file.Replace ("\\", "/");
					if (fileName.ToLower ().Equals (tempFile.Substring (tempFile.LastIndexOf ("/") + 1, tempFile.Length - tempFile.LastIndexOf ("/") - 1).ToLower ()))
					{
						result.Add (tempFile);
					}
				}
			} 
			catch (System.Exception ex) 
			{
				AMEditorLog.Write ("Error Search file. Message: " + ex.Message);
			}
			return result;
		}

		public static Dictionary<string, List<string>> SearchFile (string path, List<string> fileNames)
		{
			var result = new Dictionary<string, List<string>> ();
			foreach (var item in fileNames) 
			{
				if (!result.ContainsKey (item))
					result.Add (item, new List<string> ());
			}
			try 
			{
				var dirlist = Directory.GetDirectories (path);
				var filelist = Directory.GetFiles (path);
				foreach (var directory in dirlist) 
				{
					var dictionary = SearchFile (directory, fileNames);
					foreach (var d in dictionary) 
					{
						foreach (var f in d.Value) 
						{
							result[d.Key].Add (f);
						}
					}
				}
				foreach (var file in filelist) 
				{
					var tempFile = file.Replace ("\\", "/");
					var tempNameFile = tempFile.Substring (tempFile.LastIndexOf ("/")+1, tempFile.Length - tempFile.LastIndexOf ("/")-1);
					if (fileNames.Contains (tempNameFile))
					{
						result[tempNameFile].Add (tempFile);
					}
				}
			} 
			catch (System.Exception ex) 
			{
				AMEditorLog.Write ("Error Search file. Message: " + ex.Message);
			}
			
			return result;
		}

		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="oldPath">Old path.</param>
		/// <param name="newPath">New path.</param>
		/// <param name="overwrite">If set to <c>true</c> overwrite.</param>
		public static void CopyFile (string oldPath, string newPath, bool overwrite)
		{
			try 
			{
				var listFolders = newPath.Split ('/');
				string temp = string.Empty;
				if (newPath.IndexOf ('/') == 0)
					temp = "/";
				for (int i = 0; i < listFolders.Length - 1; i++) 
				{
					if ((temp != string.Empty)&& (temp != "/"))
						temp += "/";
					temp += listFolders[i];
					//Debug.Log ("" + temp);
					CreateFolder (temp, false);
				}
				File.Copy (oldPath, newPath, overwrite);
			} 
			catch (Exception ex) 
			{
				AMEditorLog.Write ("Error Copy File. Message : " + ex.Message);
			}
		}
		/// <summary>
		/// Removes the list folder.
		/// </summary>
		/// <param name="paths">Paths.</param>
		public static void RemoveListFolder (List<string> paths)
		{
			foreach (var item in paths) 
			{
				RemoveFolder (item);
			}
		}
		/// <summary>
		/// Removes the list files.
		/// </summary>
		/// <param name="paths">Paths.</param>
		public static void RemoveListFiles (List<string> paths)
		{
			foreach (var item in paths) 
			{
				RemoveFile (item);
			}
		}
		public static string HashFile (string path)
		{
			try 
			{
				var b = File.ReadAllBytes (path);
				return MD5Sum (b);
			} 
			catch (Exception) 
			{
				return string.Empty;
			}
		}

		public static string MD5Sum (string source)
		{		
#if !UNITY_METRO || UNITY_EDITOR
			System.Security.Cryptography.MD5 h = System.Security.Cryptography.MD5.Create ();
			byte[] data = h.ComputeHash (System.Text.Encoding.Default.GetBytes (source));
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			
			for (int i = 0; i < data.Length; ++i)
			{
				sb.Append (data[i].ToString ("x2"));
			}
			return sb.ToString ();
#else
			byte[] data = Crypto.ComputeMD5Hash (Encoding.UTF8.GetBytes (s));
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			
			for (int i = 0; i < data.Length; ++i)
			{
				sb.Append (data[i].ToString ("x2"));
			}
			return sb.ToString ();
#endif
		}
		public static string MD5Sum (byte[] bytes)
		{
#if !UNITY_METRO || UNITY_EDITOR
			System.Security.Cryptography.MD5 h = System.Security.Cryptography.MD5.Create ();
			byte[] data = h.ComputeHash (bytes);
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			
			for (int i = 0; i < data.Length; ++i)
			{
				sb.Append (data[i].ToString ("x2"));
			}
			return sb.ToString ();
#else
			byte[] data = Crypto.ComputeMD5Hash (Encoding.UTF8.GetBytes (s));
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			
			for (int i = 0; i < data.Length; ++i)
			{
				sb.Append (data[i].ToString ("x2"));
			}
			return sb.ToString ();
#endif
		}
	}
}
#endif