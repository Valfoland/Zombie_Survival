#if UNITY_EDITOR
using System.IO;
using System.Collections;

namespace AMEditor
{
	public class AppInfoAPI
	{
		public static void CreateFile (string path)
		{
			if (!AMEditorFileStorage.FileExist (AMEditorSystem.FileNames._ConfigAMEditor)) 
			{
				File.WriteAllText (AMEditorSystem.FileNames._ConfigAMEditor, "[]");
			}

			ArrayList listCurrentPlugin = AMEditorJSON.JsonDecode (AMEditorFileStorage.ReadTextFile (AMEditorSystem.FileNames._ConfigAMEditor)) as ArrayList;
			
			ArrayList unity_plugins = new ArrayList ();
			
			foreach (var item in listCurrentPlugin) 
			{
				var currentplugin = new Plugin (item as Hashtable);
				
				AMEditorConflictAPI.GetConflict (currentplugin);
				AMEditor.WindowMain.CheckingMissFilesAndHash (currentplugin);
				AMEditor.WindowMain.CheckVersionActuality (currentplugin);
				var errors = new ArrayList ();
				
				if (currentplugin.errors.conflict)
				{
					var error = new Hashtable ();
					error.Add ("code", 304);
					error.Add ("message", AMEditorSystem.ContentError._304 (currentplugin.name));
					errors.Add (error);
				}
				if (currentplugin.errors.oldVersion)
				{
					var error = new Hashtable ();
					error.Add ("code", 305);
					error.Add ("message", AMEditorSystem.ContentError._305 (currentplugin.name));
					errors.Add (error);
				}
				if (currentplugin.errors.missingFiles)
				{
					var error = new Hashtable ();
					error.Add ("code", 306);
					error.Add ("message", AMEditorSystem.ContentError._306 (currentplugin.name));
					errors.Add (error);
				}
				if (currentplugin.errors.missingFilesHash)
				{
					var error = new Hashtable ();
					error.Add ("code", 307);
					error.Add ("message", AMEditorSystem.ContentError._307 (currentplugin.name));
					errors.Add (error);
				}
				
				var pluginjson = new Hashtable ();
				pluginjson.Add ("name", currentplugin.name);
				pluginjson.Add ("version", currentplugin.version);
				pluginjson.Add ("errors", errors);
				
				unity_plugins.Add (pluginjson);
			}
			var globalErrors = new ArrayList ();
			int code = CustomResourceManagerAPI.CheckCorrect ();
			switch (code) 
			{
			case 302:
				var error302 = new Hashtable ();
				error302.Add ("code", 302);
				error302.Add ("message", AMEditorSystem.ContentError._302);
				globalErrors.Add (error302);
				break;
			case 303:
				var error303 = new Hashtable ();
				error303.Add ("code", 303);
				error303.Add ("message", AMEditorSystem.ContentError._303);
				globalErrors.Add (error303);
				break;
			case 313:
				var error313 = new Hashtable ();
				error313.Add ("code", 313);
				error313.Add ("message", AMEditorSystem.ContentError._313);
				globalErrors.Add (error313);
				break;
			case 314:
				var error314 = new Hashtable ();
				error314.Add ("code", 314);
				error314.Add ("message", AMEditorSystem.ContentError._314);
				globalErrors.Add (error314);
				break;
			default:
				break;
			}
			
			var appinfo = new Hashtable ();

			appinfo.Add ("unity_plugins", unity_plugins);
			appinfo.Add ("errors", globalErrors);
			appinfo.Add ("unity_version", UnityEngine.Application.unityVersion);
			appinfo.Add ("scene_with_crm", CustomResourceManagerAPI.FindSceneWithCRM ());

			#if CUSTOM_CODE_ASSET_BUNDLES
			AMEditorAssetBundleAPI.InitReflection ();
			string infoFilePath = Path.Combine (AMEditorAssetBundleAPI.getAssetBundlePathMethodInfo.Invoke (null, new System.Object[]{true}).ToString (), AMEditorAssetBundleAPI.infoFileNameFieldValue);
			if (File.Exists (infoFilePath))
			{
				var assetBundleInfo = AMEditorJSON.JsonDecode (File.ReadAllText (infoFilePath)) as Hashtable;
				if (assetBundleInfo != null)
				{
					try
					{
						appinfo.Add ("scenes_asset_bundle", assetBundleInfo);
					}
					catch (System.Exception)
					{}
				}
			}
			#endif

			File.WriteAllText (path +"/app_info.json", AMEditorJSON.FormatJson (AMEditorJSON.JsonEncode (appinfo)));
		}
	}
}
#endif