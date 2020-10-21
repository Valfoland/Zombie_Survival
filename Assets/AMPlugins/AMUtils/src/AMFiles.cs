using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using AMLogging;

namespace AMUtils
{
	public class AMFiles : MonoBehaviour
	{
		public static AMFiles Instance = null;
		
		static AMLogger amLogger = AMLogger.GetInstance("AMFiles: ");
		
		//static char[] symbols = @"./\:".ToCharArray (0, 4);
		//static string drive = "";
		
		void Awake()
		{
			if (Instance == null)
			{				
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
				Destroy(gameObject);
		}
		
		public static void Create(string filePath, string fileName, string fileContent)
		{
#if !UNITY_WINRT || UNITY_EDITOR
			const int size = 262144;
			string fullPath = filePath + "/" + fileName;
			
			if (!Directory.Exists (filePath))
				Directory.CreateDirectory (filePath);
			
			using (FileStream fileStream = File.Create(fullPath, size)) {
				byte[] content = new UTF8Encoding(true).GetBytes(fileContent);
				fileStream.Write(content, 0, content.Length);
			}

			amLogger.Log ("Created file: " + fullPath);
#endif
		}
		
		public static void Delete(string path)
		{
#if !UNITY_WINRT || UNITY_EDITOR
			if (File.Exists (path)) {
				File.Delete (path);
				amLogger.Log (path + " deleted successfully!");
			}
			else
				amLogger.LogWarning (path + " not found!");
#endif
		}
/*		
		public static void Zip(string[] itemsToZip, string zipName, string zipPath)
		{
			//если имя архива не задано - использовать имя первого элемента
			if (zipName == "") {
				//имя архива = имени первого элемента
				zipName = GetFirstItemName(itemsToZip[0]);
			}
			if (!Directory.Exists (zipPath))
				Directory.CreateDirectory (zipPath);
			//создание архива
			try {
				using (ZipFile archive = new ZipFile ()) {
					foreach (string item in itemsToZip) {
						if (IsFile(item))
							archive.AddItem (item);
						else
							archive.AddItem (item, item);
					}
					archive.Save (zipPath+"/"+zipName+".zip");
				}
				amLogger.Log (zipName+".zip successfully created at "+zipPath);
			}
			catch (System.Exception e) {
				amLogger.LogException (e);
			}
		}
		
		public static void UnZip(string archivePath, string extractPath)
		{
			var extractFolderName = GetExtractFolderName (archivePath);
			if (archivePath == "")
				amLogger.LogError ("Path to archive is empty!");
			//распаковка
			try {
				using (ZipFile archive = ZipFile.Read (archivePath)) {
					//если не задан путь распаковки - распаковать рядом с архивом
					if (extractPath == "")
						extractPath = GetExtractDefaultPath(archivePath);
					if (Directory.Exists(extractPath + "/" + extractFolderName))
						Directory.Delete (extractPath + "/" + extractFolderName, true);
					archive.ExtractAll(extractPath + "/" + extractFolderName);
				}
				amLogger.Log (extractFolderName+".zip successfully extracted to "+extractPath+extractFolderName);
			}
			catch (System.Exception e) {
				amLogger.LogException (e);
			}
		}
		
		public static string GetExtractFolderName(string filePath) 
		{
			string extractFolderName = "";
			int slashIndex = 0;
			//отделение имени архива от остального пути
			char[] path = filePath.ToCharArray (0, filePath.Length);
			for (int i = filePath.Length-1; i > -1; i--) {
				if (path[i] == symbols[1] || path[i] == symbols[2]) {
					slashIndex = i;
					break;
				}
			}
			int nameStartIndex = slashIndex + 1;
			int nameLenght = (filePath.Length - 5) - slashIndex;
			//отделение имени от типа архива для содания каталога распаковки
			char[] name = filePath.ToCharArray (nameStartIndex, nameLenght);
			foreach (char c in name)
				extractFolderName += c;
			return extractFolderName;
		}
		
		private static string GetExtractDefaultPath(string filePath) 
		{
			string defaultExtractPath = "";
			int slashIndex = 0;
			//отделение пути к архиву от его имени
			char[] path = filePath.ToCharArray (0, filePath.Length);
			for (int i = filePath.Length - 1; i > -1; i--) {
				if (path[i] == symbols[1] || path[i] == symbols[2]) {
					slashIndex = i;
					break;
				}
			}
			for (int j = 0; j < slashIndex + 1; j++)
				defaultExtractPath += path[j];
			return defaultExtractPath;
		}

		private static bool IsFile(string itemToZip) 
		{
			bool isFile = false;
			int dotIndex = 0;
			
			//поиск индексов последнего слэша в пути и точки перед типом файла
			char[] itemPath = itemToZip.ToCharArray (0, itemToZip.Length);
			for (int i = itemToZip.Length - 1; i > -1; i--) {
				if (itemPath[i] == symbols[0]) {
					dotIndex = i;
					break;
				}
			}
			if (dotIndex > 0) 
				isFile = true;
			
			return isFile;
		}

		private static string GetFirstItemName (string itemToZip) 
		{
			string firstItemName = "";
			int dotIndex = 0;
			int slashIndex = 0;
			
			//поиск индексов последнего слэша в пути и точки перед типом файла
			char[] itemPath = itemToZip.ToCharArray (0, itemToZip.Length);
			for (int i = itemToZip.Length - 1; i > -1; i--) {
				if (itemPath[i] == symbols[0])
					dotIndex = i;
				if (itemPath[i] == symbols[1] || itemPath[i] == symbols[2]) {
					slashIndex = i;
					break;
				}
			}
			char[] itemName;
			int itemNameStartIndex = slashIndex + 1;
			//если элемент - файл
			if (dotIndex > 0) {
				int itemNameLenght = dotIndex - itemNameStartIndex;
				itemName = itemToZip.ToCharArray (itemNameStartIndex, itemNameLenght);
			}
			//если элемент - каталог
			else {
				int itemNameLenght = itemToZip.Length - itemNameStartIndex;
				itemName = itemToZip.ToCharArray (itemNameStartIndex, itemNameLenght);
			}
			foreach (char c in itemName)
				firstItemName += c;
			return firstItemName;
		}
		
		private static string GetDriveLetter (string filePath)
		{
			int driveLetterIndex = 0;
			string driveLetter = "";
			
			//поиск индексов последнего слэша в пути и точки перед типом файла
			char[] path = filePath.ToCharArray (0, filePath.Length);
			for (int i = filePath.Length - 1; i > -1; i--) {
				if (path[i] == symbols[3]) {
					driveLetterIndex = i;
					driveLetter = path[i-1].ToString();
					break;
				}
			}			
			return driveLetter;
		}
*/
	}
}