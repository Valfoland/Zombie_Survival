#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using Ionic.Zip;

namespace AMEditor
{
    public class AMEditorGit
    {
        public bool printDebug = false;
		public bool hideAllLogs = false;

        private struct GitProject
        {
            public float id;
            public string name;
            public string path;
        }
        List<GitProject> gitProjectsList;

        private struct GitGroup
        {
            public float id;
            public string name;
            public string path;
        }
        GitGroup projectsGroup;

        private string urlForApi = @"";
        private string url = @"";
        private string token = "";
        private bool authFailed = true;
        private bool downloadComplete = false;
		private bool forceSearchConfigs = false;
        private List<AMEditorNetwork.UrlParameter> urlParameters;

		private const string CONFIG_NAME = "ameditor_plugins.json";
		private const string CONFIG_COMMIT_MESSAGE = "Update ameditor_plugins.json with AM Editor";

        private string branch = "";
        private string version = "";
        
        private string pathFolder = "";

        private string fileName = "";
        public List<string> configList;

        static BackgroundWorker ArchiveWorkerBW;
        public BackgroundWorker ConfigSearchBW;
		static BackgroundWorker PushConfigBW;

        public delegate void DownloadDelegate (bool downloadComplete);
        public delegate void AuthDelegate (bool authFailed);
        public delegate void StatusDelegate (string downloadStatus, float step);
        public delegate void ConfigDelegate (List<string> pluginsConfigsList);
		public delegate void PusgingDelegate (bool success);
		public delegate void ErrorDelegate (string error);

        public event DownloadDelegate ArchiveDownloadComplete;
        public event DownloadDelegate UnitypackageDownloadComplete;
        public event AuthDelegate AuthFailed;
        public event StatusDelegate ChangeStatus;
        public event ConfigDelegate ConfigWorkComplete;
		public event PusgingDelegate PushWorkComplete;
        public event ErrorDelegate ErrorHappened;

        public AMEditorGit ()
        {
            ArchiveWorkerBW = new BackgroundWorker ();
            ArchiveWorkerBW.WorkerSupportsCancellation = true;
            ArchiveWorkerBW.DoWork += new DoWorkEventHandler (ArchiveWorkerBW_DoWork);
            ArchiveWorkerBW.RunWorkerCompleted += new RunWorkerCompletedEventHandler (ArchiveWorkerBW_RunWorkerCompleted);

            ConfigSearchBW = new BackgroundWorker ();
            ConfigSearchBW.WorkerSupportsCancellation = true;
            ConfigSearchBW.DoWork += new DoWorkEventHandler (ConfigSearchBW_DoWork);
            ConfigSearchBW.RunWorkerCompleted += new RunWorkerCompletedEventHandler (ConfigSearchBW_RunWorkerCompleted);

			PushConfigBW = new BackgroundWorker ();
			PushConfigBW.WorkerSupportsCancellation = true;
			PushConfigBW.DoWork += new DoWorkEventHandler (PushConfigBW_DoWork);
			PushConfigBW.RunWorkerCompleted += new RunWorkerCompletedEventHandler (PushConfigBW_RunWorkerCompleted);

            configList = new List<string> ();
            urlParameters = new List<AMEditorNetwork.UrlParameter> ();

			AMEditorNetwork.printDebug = printDebug;
        }

        public string AuthByLoginAndPass (string urlAddress, string userLogin, string userPass)
        {
            authFailed = true;
            string pt = "";

            if (!urlAddress.Equals (@"https://gitlab.digital-ecosystems.ru") && !urlAddress.Equals (@"http://pgit.digital-ecosystems.ru"))
                Debug.LogError ("Invalid url!");
            else if (userLogin.Equals (""))
                Debug.LogError ("Type Login!");
            else if (userPass.Equals (""))
                Debug.LogError ("Type Password!");
            else
            {
                urlForApi = urlAddress + "/api/v3";
                url = urlAddress;

                urlParameters.Add (new AMEditorNetwork.UrlParameter
                {
                    name = "login", 
                    value = userLogin, 
                });
                urlParameters.Add (new AMEditorNetwork.UrlParameter
                {
                    name = "password", 
                    value = userPass, 
                });

                string authResponse = AMEditorNetwork.PostRequest (urlForApi + "/session", urlParameters);

                if (ResponseIsValid (authResponse, "private_token", false))
                {
					Hashtable sessionInfo = AMEditorJSON.JsonDecode (authResponse) as Hashtable;

                    pt = (string)sessionInfo["private_token"];
                    token = "private_token=" + pt;
					
					authFailed = false;
					if (!hideAllLogs)
                    	Debug.Log ("Authorization Success");
                }
                else
                {
                    authFailed = true;
                }
                if (AuthFailed != null)
                    AuthFailed (authFailed);
            }
            return pt;
        }

        public void AuthByPT (string urlAddress, string userPT)
        {
            authFailed = true;

            if (!urlAddress.Equals (@"https://gitlab.digital-ecosystems.ru") && !urlAddress.Equals (@"http://pgit.digital-ecosystems.ru"))
                Debug.LogError ("Invalid url!");
            urlForApi = urlAddress + "/api/v3";
            url = urlAddress;
            try
            {
                string userInfoResponse = GetUserInfo (userPT);

                if (ResponseIsValid (userInfoResponse, "id", false))
                {
                    token = "private_token=" + userPT;
                    authFailed = false;
					if (!hideAllLogs)
                    	Debug.Log ("Authorization Success");
                }
                else
                {
                    authFailed = true;
                }
                if (AuthFailed != null)
                    AuthFailed (authFailed);
            }
            catch (WebException ex)
            {
                AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("", ex.ToString ());
                if (ErrorHappened != null)
                    ErrorHappened (":" + ex.ToString ());
            }
        }

        public string GetUserInfo (string userPT)
        {
            token = "private_token=" + userPT;
            string userInfo = AMEditorNetwork.GetRequest (urlForApi + "/user", "?" + token);

           if (printDebug && !hideAllLogs)
                Debug.Log ("Current user info : " + userInfo);

            return userInfo;
        }

        public void SetParameters (string[] pluginName, string pluginVersion, string groupName, string branchName)
        {
            branch = branchName;
            version = pluginVersion;

			projectsGroup = new GitGroup ();
			projectsGroup.name = groupName;

			gitProjectsList = new List<GitProject> ();

            foreach (var name in pluginName)
            {
                gitProjectsList.Add (new GitProject
                {
                    name = name, 
                    path = "", 
                    id = 0, 
                });
            }

           if (printDebug && !hideAllLogs)
                Debug.Log ("Project info was inserted");

            ProjectSearch ();
        }

        public void GetProjectFromArchive (string[] projectName, string pluginVersion, string groupName, string branchName, string pathToDownload)
        {
            SetParameters (projectName, pluginVersion, groupName, branchName);

            pathFolder = pathToDownload;

            ArchiveWorkerBW.RunWorkerAsync ();
        }

		public string GetProjectsOfAGroup ()
		{
			string projectsOfAGroup = "";
			bool hasGroup = false;
			
			if (ChangeStatus != null)
				ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._GettingGroupsList, 0.2f);
			
			string foundUnityPluginsGroupResponse = AMEditorNetwork.GetRequest (urlForApi + "/groups", "?search=unity%20plugins&" + token);
						
			if (printDebug && !hideAllLogs)
				Debug.Log ("Founded group : " + foundUnityPluginsGroupResponse);
			
			if (ResponseIsValid (foundUnityPluginsGroupResponse, "jsonarray", false))
			{
				ArrayList allUserGroupsList = AMEditorJSON.JsonDecode (foundUnityPluginsGroupResponse) as ArrayList;
				
				float step = 0.8f / (allUserGroupsList.Count + 1);
				foreach (var item in allUserGroupsList)
				{
					if (ChangeStatus != null)
						ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._SearchingNeededGroup + projectsGroup.name, step);
					
					Hashtable userGroup = item as Hashtable;
					string groupName = (string)userGroup["name"];
					
					if (groupName.Equals (projectsGroup.name))
					{
						try
						{
							projectsGroup.id = float.Parse (userGroup["id"].ToString ());
						}
						catch (Exception) { }
						projectsGroup.path = (string)userGroup["path"];
						
						if (ChangeStatus != null)
							ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._GettingGroupProjects + projectsGroup.name, step);
						
						string currentGroupResponse = AMEditorNetwork.GetRequest (urlForApi + "/groups/" + projectsGroup.id, "?" + token);
						Hashtable currentGroup = AMEditorJSON.JsonDecode (currentGroupResponse) as Hashtable;
						projectsOfAGroup = AMEditorJSON.JsonEncode (currentGroup["projects"]);
						
						hasGroup = true;
					}
				}
				if (hasGroup == false)
					Debug.LogError ("Group not found!");
			}
			return projectsOfAGroup;
		}
		
		public void ProjectSearch ()
		{
			if (printDebug && !hideAllLogs)
				Debug.Log ("\"" + gitProjectsList[0].name + "\" project");
			
			bool hasProject = false;
			
			string projectsListResponse = AMEditorNetwork.GetRequest (urlForApi + "/projects/search/" + gitProjectsList[0].name, "?per_page=100&" + token);

           if (printDebug && !hideAllLogs)
                Debug.Log ("Searched project (s) : " + projectsListResponse);

			if (ResponseIsValid (projectsListResponse, "jsonarray", false))
			{
				ArrayList projectsList = AMEditorJSON.JsonDecode (projectsListResponse) as ArrayList;

				foreach (var item in projectsList)
				{
					Hashtable currentProjectInfo = item as Hashtable;

					Hashtable projectNamespace = currentProjectInfo["namespace"] as Hashtable;
					string projectGroup = (string)projectNamespace["name"];
					
					if (!projectGroup.Equals (projectsGroup.name))
					{
						continue;
					}

					string currentProjectName = (string)currentProjectInfo["name"];
					float currentProjectId = -1;
					try
					{
						currentProjectId = float.Parse (currentProjectInfo["id"].ToString ());
					}
					catch (Exception) { }
					string currentProjectPath = (string)currentProjectInfo["path"];
					for (int p = 0; p < gitProjectsList.Count; p++)
					{
						if (currentProjectName.Equals (gitProjectsList[p].name))
						{
							hasProject = true;
							var gitProject = gitProjectsList[p];
							gitProject.id = currentProjectId;
							gitProject.path = currentProjectPath;
							gitProjectsList[p] = gitProject;
						}
					}
				}
				if (hasProject == false)
				{
					AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("102", AMEditor.AMEditorSystem.WebError._102 (gitProjectsList[0].name));
					if (ErrorHappened != null)
						ErrorHappened ("102:" + AMEditor.AMEditorSystem.WebError._102 (gitProjectsList[0].name));
				}
			}
        }

        private bool BranchCheck (float projectId, string branchName)
        {
            bool hasBranch = false;

            string projectBranchListResponse = AMEditorNetwork.GetRequest (urlForApi + "/projects/" + projectId + "/repository/branches", "?" + token);

           if (printDebug && !hideAllLogs)
                Debug.Log ("Project branches : " + projectBranchListResponse);

            if (ResponseIsValid (projectBranchListResponse, "jsonarray", false))
            {
                ArrayList projectBranchList = AMEditorJSON.JsonDecode (projectBranchListResponse) as ArrayList;
                foreach (var item in projectBranchList)
                {
                    string projectBranchName = (string)((item as Hashtable)["name"]);

                    if (projectBranchName.Equals (branchName))
                        hasBranch = true;
                }
            }
            return hasBranch;
        }

        private string GetLastCommit (float projectId, string branchName)
        {
            string commitSHA = "";

            string commitsListResponse = AMEditorNetwork.GetRequest (urlForApi + "/projects/" + projectId + "/repository/commits", "?ref_name=" + branchName + "&" + token);

           if (printDebug && !hideAllLogs)
                Debug.Log ("Commits of a project : " + commitsListResponse);

            if (ResponseIsValid (commitsListResponse, "jsonarray", false))
            {
                ArrayList commitList = AMEditorJSON.JsonDecode (commitsListResponse) as ArrayList;
                Hashtable commit = commitList[0] as Hashtable;

                commitSHA = (string)commit["id"];
            }
            return commitSHA;
        }

        private string GetTagCommit (float id, string pluginVersion)
        {
            string versionForTag = "v" + pluginVersion;
            string commitSHA = "";
            bool match = false;

            string tagsListResponse = AMEditorNetwork.GetRequest (urlForApi + "/projects/" + id + "/repository/tags", "?" + token);

           if (printDebug && !hideAllLogs)
                Debug.Log ("Tags of a project : " + tagsListResponse);

            if (ResponseIsValid (tagsListResponse, "jsonarray", false))
            {
                ArrayList tagsList = AMEditorJSON.JsonDecode (tagsListResponse) as ArrayList;

                foreach (var tag in tagsList)
                {
                    Hashtable tagHT = tag as Hashtable;
                    if (versionForTag == (string)tagHT["name"])
                    {
                        Hashtable commitHT = tagHT["commit"] as Hashtable;
                        commitSHA = (string)commitHT["id"];
                        match = true;
                        break;
                    }
                    else
                        match = false;
                }
                if (match == false)
                {
                    Debug.LogWarning ("Tag not found!");
                }
            }
            return commitSHA;
        }

        public static event System.Action EndDownload;

        private void ArchiveWorkerBW_DoWork (object sender, DoWorkEventArgs e)
        {
            downloadComplete = false;

            string commit = "";
            string archiveName = "";
            string downloadData = "";
			string projectRepoFolder = "";

			AMEditorNetwork.printDebug = printDebug;

            foreach (var project in gitProjectsList)
            {
                if (CancelDownload (e))
                    break;
                if (ChangeStatus != null)
					ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._CheckingBranch, 0.20f);

                bool projectHasBranch = BranchCheck (project.id, "master");//TODO: убрать проверку веток

                if (CancelDownload (e))
                    break;

                if (projectHasBranch)
                {
                    if (CancelDownload (e))
                        break;

                    if (version != string.Empty)
                    {
                        if (ChangeStatus != null)
                        {
							ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._GettingTags, 0.20f);
                        }
                        commit = GetTagCommit (project.id, version);
                    }
                    else
                    {
                        if (ChangeStatus != null)
                        {
							ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._GettingLastCommit, 0.20f);
                        }
                        commit = GetLastCommit (project.id, branch);
                    }

                    archiveName = project.path + "-" + commit + ".zip";//для GitLab ".tar.gz" вместо ".zip"
                    downloadData = pathFolder + "/" + archiveName;

                    if (CancelDownload (e))
                        break;

                    if (!Directory.Exists (pathFolder))
                    {
                        try
                        {
                            Directory.CreateDirectory (pathFolder);
                        }
                        catch (Exception)
                        {
                            AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("203", AMEditor.AMEditorSystem.FileSystemError._203 (project.name));
                            if (ErrorHappened != null)
								ErrorHappened (string.Empty);
                        }
                    }

                    if (CancelDownload (e))
                        break;
                    if (ChangeStatus != null)
						ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._DownloadingArchive, 0.20f);

                    string downloadArchiveMessage = string.Empty;
					if (version != string.Empty && commit == string.Empty)
                    {
						string projectName = gitProjectsList [0].name.ToLower ().Replace (" ", "-");
						downloadArchiveMessage = AMEditorNetwork.FileGetRequest (url + "/unity-plugins/" + projectName + "/repository/archive.zip", 
																				"?ref=v" + version + "&" + token, downloadData);
                    }
					else
					{
						downloadArchiveMessage = AMEditorNetwork.FileGetRequest (urlForApi + "/projects/" + project.id + "/repository/archive.zip", 
																				"?sha=" + commit + "&" + token, downloadData);
					}

                    if (downloadArchiveMessage != string.Empty)
                    {
                       if (printDebug && !hideAllLogs)
                            Debug.Log (downloadArchiveMessage);
						if (!hideAllLogs)
	                        Debug.Log ("Archive was successfully downloaded to " + pathFolder);
                    }
                    else
                    {
                        AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("105", AMEditor.AMEditorSystem.WebError._105 (project.name));

                        if (ErrorHappened != null)
							ErrorHappened (string.Empty);
                    }

                    if (CancelDownload (e))
                        break;
                    if (ChangeStatus != null)
						ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._ExtractingArchive, 0.20f);

					projectRepoFolder = (version != string.Empty && version != "?" && version != " ") ? project.name + " " + version : project.name;

                    int renameCount = 0;
					using (ZipFile archive = ZipFile.Read (downloadData))
                    {
						string newEntryFirstDir = project.name.ToLower ().Replace (" ", "-");

						List<ZipEntry> zipEntriesList = archive.Entries.ToList ();
						for (int i = 0; i < zipEntriesList.Count; i++)
						{
							ZipEntry entry = zipEntriesList [i];

							string oldEntryFirstDir = entry.FileName.Substring (0, entry.FileName.IndexOf ('/'));

							entry.FileName = entry.FileName.Replace (oldEntryFirstDir, newEntryFirstDir);
							renameCount++;
						}
                        archive.Save ();
                    }
                    using (ZipFile archive = ZipFile.Read (downloadData))
                    {
						if (Directory.Exists (pathFolder + "/" + projectRepoFolder))
                        {
                            try
                            {
								Directory.Delete (pathFolder + "/" + projectRepoFolder, true);
                            }
                            catch (Exception)
                            {
                                AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("202", AMEditor.AMEditorSystem.FileSystemError._202 (project.name));
                                if (ErrorHappened != null)
									ErrorHappened (string.Empty);
                            }
                        }
						if (!hideAllLogs)
							Debug.Log (pathFolder + "/" + projectRepoFolder);
                        try
                        {
                            archive.ExtractAll (pathFolder + "/" + projectRepoFolder);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError ("EX: "+ex.ToString ());
                            AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("200", AMEditor.AMEditorSystem.FileSystemError._200 (projectRepoFolder));
                            if (ErrorHappened != null)
                                ErrorHappened (string.Empty);
                        }
                    }
					if (!hideAllLogs)
						Debug.Log (project.name + " archive was successfully extracted to " + pathFolder + "/" + projectRepoFolder);

                    if (CancelDownload (e))
                        break;
                    if (ChangeStatus != null)
                        ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._DeletingArchive, 0.20f);

                    try
                    {
                        File.Delete (downloadData);
						if (!hideAllLogs)
							Debug.Log (projectRepoFolder + " archive was deleted");
                    }
                    catch (Exception)
                    {
						AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("201", AMEditor.AMEditorSystem.FileSystemError._201 (projectRepoFolder));
                        if (ErrorHappened != null)
							ErrorHappened (string.Empty);
                    }
                    downloadComplete = true;
                }
                else
                {
                    AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("103", AMEditor.AMEditorSystem.WebError._103 (project.name, branch));
                    if (ErrorHappened != null)
						ErrorHappened (string.Empty);
                }
            }
        }

        private void ArchiveWorkerBW_RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                AMEditorNetwork.stopDownload = false;
				downloadComplete = false;

                Debug.LogWarning ("Archive work canceled!");
            }
            else if (e.Error == null)
            {
				if (!hideAllLogs)
               		Debug.Log ("Archive work completed");
            }
            else
            {
				downloadComplete = false;

                Debug.LogError ("Archive work failed! " + e.Error.ToString ());
                if (ErrorHappened != null)
					ErrorHappened (string.Empty);
            }
			if (ArchiveDownloadComplete != null) ArchiveDownloadComplete (downloadComplete);
            if (EndDownload != null)
                EndDownload ();
        }

		public void ConfigSearch (string configFileName, string groupName, string branchName, bool forceSearch = false)
        {
            fileName = configFileName;

            projectsGroup = new GitGroup ();
            projectsGroup.name = groupName;

            branch = branchName;

			forceSearchConfigs = forceSearch;

            ConfigSearchBW.RunWorkerAsync ();
        }

		private void ConfigSearchBW_DoWork (object sender, DoWorkEventArgs e)
        {
            List<GitProject> compatibleProjects = new List<GitProject> ();
            
            string projectsOfAGroup = "";

            if (CancelDownload (e))
                return;
            projectsOfAGroup = GetProjectsOfAGroup ();

           if (printDebug && !hideAllLogs)
                Debug.Log ("Projects in \"" + projectsGroup.name + "\" : " + projectsOfAGroup);

            if (CancelDownload (e))
                return;
            ArrayList groupProjectsList = AMEditorJSON.JsonDecode (projectsOfAGroup) as ArrayList;

            if (CancelDownload (e))
                return;

            if (ChangeStatus != null)
                ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._SearchingCompatibleProjects, 0);
            foreach (var proj in groupProjectsList)
            {
                Hashtable project = proj as Hashtable;
                int projectId = -1;
                try
                {
                    projectId = int.Parse (project["id"].ToString ());
                }
                catch (Exception)
                { }
                string projectName = (string)project["name"];
                string projectPath = (string)project["path"];

                if (CancelDownload (e))
                    break;
                bool projectHasBranch = BranchCheck (projectId, branch);

                float step = 1f / groupProjectsList.Count;
                if (projectHasBranch)
                {
                    if (ChangeStatus != null)
                        ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._SearchingCompatibleProjects + ": " + projectName, step);

                    if (CancelDownload (e))
                        break;
                    string mainConfigResponse = AMEditorNetwork.GetRequest (url + "/" + projectsGroup.path + "/" + projectPath + "/raw/" + branch + "/" + fileName, "?" + token);
                    if (CancelDownload (e))
                        break;

                    if (ResponseIsValid (mainConfigResponse, "mandatory", true))
                    {
                        compatibleProjects.Add (new GitProject
                        {
                            id = projectId, 
                            name = projectName, 
                            path = projectPath, 
                        });
                    }
                }
                else
                {
                   if (printDebug && !hideAllLogs)
                        Debug.LogError ("\"" + projectName + "\" project has no \"" + branch + "\" branch");
                    if (ChangeStatus != null)
                        ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._SearchingCompatibleProjects + ": " + projectName, step);
                }
            }
            if (ChangeStatus != null)
                ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._WorkingWithCompatible, 0);
            if (compatibleProjects != null && compatibleProjects.Count > 0)
            {
				for (int i = 0; i < compatibleProjects.Count; i++)
				{
					var project = compatibleProjects [i];

                    if (CancelDownload (e))
                        break;
                    string tagsListResponse = AMEditorNetwork.GetRequest (urlForApi + "/projects/" + project.id + "/repository/tags", "?" + token);
                    if (CancelDownload (e))
                        break;

					List<Hashtable> tagsList = new List<Hashtable> ();
					
                    if (ResponseIsValid (tagsListResponse, "jsonarray", false))
                    {
						ArrayList tagsArrayList = AMEditorJSON.JsonDecode (tagsListResponse) as ArrayList;

						float step = 0;
						if (forceSearchConfigs)
						{
							step = (tagsArrayList.Count == 0) ? 1f / (compatibleProjects.Count * 3) : (1f / compatibleProjects.Count) / ((tagsArrayList.Count * 2) + 1);
						}
						else
						{
							step = (1f / compatibleProjects.Count) / 3;
						}

						if (ChangeStatus != null)
							ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._Plugin + project.name + ". " + AMEditor.AMEditorSystem.ContentStatuses._GettingTags, step);

						if (tagsArrayList.Count == 0)
						{
							if (ChangeStatus != null)
								ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._Plugin + project.name + ". " + AMEditor.AMEditorSystem.ContentStatuses._GettingTags, step);
							if (ChangeStatus != null)
								ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._Plugin + project.name + ". " + AMEditor.AMEditorSystem.ContentStatuses._GettingTags, step);
						}
						else
						{
							for (int index = 0; index < tagsArrayList.Count; index++)
							{
								if (CancelDownload (e))
									break;
								tagsList.Add (tagsArrayList[index] as Hashtable);
							}
								
							tagsList.Sort ((t1, t2) => {
								var n1 = (string)t1["name"];
								var n2 = (string)t2["name"];
								n1 = n1.Substring (1);
								n2 = n2.Substring (1);
								n1 = n1.Contains ("-") ? n1.Substring (0, n1.IndexOf ("-")) : n1;
								n2 = n2.Contains ("-") ? n2.Substring (0, n2.IndexOf ("-")) : n2;
								return (new Version (n2).CompareTo (new Version (n1)));
							});
						}

                        if (CancelDownload (e))
                            break;
						for (int j = 0; j < tagsList.Count; j++)
						{
							if (!forceSearchConfigs && j > 0)
								continue;

							var tag = tagsList[j];
							Hashtable tagHT = tag as Hashtable;
							version = (string)tagHT["name"];
							
                            Hashtable commitHT = tagHT["commit"] as Hashtable;
                            string commitSHA = (string)commitHT["id"];

                            if (ChangeStatus != null)
                                ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._Plugin + project.name + " " + version + ". " + AMEditor.AMEditorSystem.ContentStatuses._SearchingPluginConfig, step);

                            if (CancelDownload (e))
                                return;
                            string commitConfigResponse = AMEditorNetwork.GetRequest (url + "/" + projectsGroup.path + "/" + project.path + "/raw/" + commitSHA + "/" + fileName, "?" + token);
                            if (CancelDownload (e))
                                return;

                            if (ResponseIsValid (commitConfigResponse, "mandatory", true))
                            {
                                if (ChangeStatus != null)
                                    ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._Plugin + project.name + " " + version + ". " + AMEditor.AMEditorSystem.ContentStatuses._GettingPluginConfig, step);
                                if (CancelDownload (e))
                                    return;

                               if (printDebug && !hideAllLogs)
                                    Debug.Log (fileName + " file content : " + commitConfigResponse);
                                if (!configList.Contains (commitConfigResponse))
                                    configList.Add (commitConfigResponse);
                            }
                            else
                            {
                                if (ChangeStatus != null)
                                    ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._Plugin + project.name + " " + version + ". " + AMEditor.AMEditorSystem.ContentStatuses._SearchingPluginConfig, step);
                            }
                        }
                    }
                }
            }
           if (printDebug && !hideAllLogs)
                Debug.Log ("Config files reading completed");
        }

        private void ConfigSearchBW_RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                AMEditorNetwork.stopDownload = false;

                Debug.LogWarning ("Config work canceled!");
                if (ConfigWorkComplete != null)
                    ConfigWorkComplete (new List<string> ());
            }
            else if (e.Error == null)
            {
				if (!hideAllLogs)
                	Debug.Log ("Config work completed");
                if (ConfigWorkComplete != null)
                    ConfigWorkComplete (configList);
            }
            else
            {
                Debug.LogError ("Config work failed! " + e.Error);
                if (ErrorHappened == null)
                    AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("", e.Error.ToString ());
                if (ConfigWorkComplete != null)
                    ConfigWorkComplete (null);
            }
        }

		public void PushConfig (string content, string message, string branchName, string userPT)
		{
			if (ChangeStatus != null)
				ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._PushingConfig, 0.25f);

			configContent = content;
			commitMessage = message;
			token = "private_token=" + userPT;

			SetParameters (new string[]{"AM Editor Plugins"}, "", AMEditor.AMEditorSystem.Git._GroupName, branchName);

			PushConfigBW.RunWorkerAsync ();
		}

		string configContent = string.Empty;
		string commitMessage = string.Empty;
		private void PushConfigBW_DoWork (object sender, DoWorkEventArgs e)
		{
			bool pushingComplete = false;
			string tmpBranch = branch;

			if (string.IsNullOrEmpty (commitMessage))
			{
				commitMessage = CONFIG_COMMIT_MESSAGE;
			}

			if (CancelDownload (e))
				return;				

			List<AMEditorNetwork.UrlParameter> requestParameters = new List<AMEditorNetwork.UrlParameter> ();

			requestParameters.Add (new AMEditorNetwork.UrlParameter
			{
				name = "file_path", 
				value = CONFIG_NAME, 
			});
			if (CONFIG_NAME == "ameditor_plugins.json")
			{
				switch (branch)
				{
				case "develop":
					tmpBranch = "dev";
					break;
				case "release":
					tmpBranch = "rc";
					break;
				default:
					break;
				}
			}
			requestParameters.Add (new AMEditorNetwork.UrlParameter
			{
				name = "branch_name", 
				value = tmpBranch, 
			});
			requestParameters.Add (new AMEditorNetwork.UrlParameter
			{
				name = "content", 
				value = configContent, 
			});
			requestParameters.Add (new AMEditorNetwork.UrlParameter
			{
				name = "commit_message", 
				value = commitMessage, 
			});
			requestParameters.Add (new AMEditorNetwork.UrlParameter
			{
				name = token.Substring (0, token.IndexOf ('=')), 
				value = token.Substring (token.IndexOf ("=") + 1), 
			});

			if (CancelDownload (e))
				return;

			if (ChangeStatus != null)
				ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._PushingConfig, 0.25f);
			
			try
			{
				if (CancelDownload (e))
					return;
				
				string configPushResponse = AMEditorNetwork.PutRequest ("http://pgit.digital-ecosystems.ru/api/v3/projects/" + gitProjectsList[0].id + "/repository/files/", requestParameters);

				if (ResponseIsValid (configPushResponse, "file_path", false))
				{
					if (printDebug && !hideAllLogs)
						Debug.Log ("Push config response : " + configPushResponse);

					if (ChangeStatus != null)
						ChangeStatus (AMEditor.AMEditorSystem.ContentProgressBar._PushingConfig, 0.25f);

					pushingComplete = true;
				}
				if (PushWorkComplete != null) 
					PushWorkComplete (pushingComplete);
			}
			catch (WebException ex)
			{
				AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("", ex.ToString ());
				if (ErrorHappened != null)
					ErrorHappened (":" + ex.ToString ());
			}
		}

		private void PushConfigBW_RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled == true)
			{
				Debug.LogWarning ("Config pushing canceled!");
			}
			else if (e.Error == null)
			{
				if (!hideAllLogs)
					Debug.Log ("Config pushing completed");
			}
			else
			{
				Debug.LogError ("Config pushing failed! " + e.Error.ToString ());
				if (ErrorHappened != null)
					ErrorHappened (string.Empty);
				else if (ErrorHappened == null)
					AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("", e.Error.ToString ());
			}
		}

        public void UnitypackageDownload (string downloadUrl, string pathToDownload)
        {
			bool groupAccess = false;
			string groupCheckResponse = AMEditorNetwork.GetRequest (urlForApi + "/groups", "?" + token);
			if (ResponseIsValid (groupCheckResponse, "jsonarray", true))
			{
				ArrayList userGroups = AMEditorJSON.JsonDecode (groupCheckResponse) as ArrayList;
				foreach (var gr in userGroups)
				{
					Hashtable group = gr as Hashtable;
					string name = (string)group["name"];
					if (name.Equals ("Unity Plugins"))
					{
						groupAccess = true;
						fileName = pathToDownload.Substring (pathToDownload.LastIndexOf ('/') + 1);
						pathFolder = pathToDownload.Substring (0, pathToDownload.LastIndexOf ('/'));

						string nameForProgressBar = fileName.Substring (0, fileName.LastIndexOf ('.')).Replace ("_", " ");

						if (UnityEditor.EditorUtility.DisplayCancelableProgressBar (AMEditor.AMEditorSystem.ContentProgressBar._Update, AMEditor.AMEditorSystem.ContentProgressBar._Plugin + nameForProgressBar + ". " + AMEditor.AMEditorSystem.ContentStatuses._DownloadingUnitypackage, 0.6f))
						{
							UnityEditor.EditorUtility.ClearProgressBar ();
						}

						string downloadMessage = string.Empty;

						downloadMessage = AMEditorNetwork.FileGetRequest (downloadUrl, "?" + token, pathFolder + Path.DirectorySeparatorChar + fileName);

						if (downloadMessage != string.Empty)
						{
							if (UnityEditor.EditorUtility.DisplayCancelableProgressBar (AMEditor.AMEditorSystem.ContentProgressBar._Update, AMEditor.AMEditorSystem.ContentProgressBar._Plugin + nameForProgressBar + ". " + AMEditor.AMEditorSystem.ContentStatuses._ImportingUnitypackage, 0.9f))
							{
								UnityEditor.EditorUtility.ClearProgressBar ();
							}

							if (printDebug && !hideAllLogs)
								Debug.Log (downloadMessage);
							if (!hideAllLogs)
								Debug.Log ("External plugin was successfully downloaded to " + pathFolder);

							if (UnitypackageDownloadComplete != null)
								UnitypackageDownloadComplete (true);
						}
						else
						{
							AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("105", AMEditor.AMEditorSystem.WebError._105 (nameForProgressBar));
							if (!hideAllLogs)
								Debug.Log ("External plugin download failed");
							if (UnitypackageDownloadComplete != null)
								UnitypackageDownloadComplete (false);
						}
						break;
					}
				}
				if (!groupAccess)
				{
					if (UnityEditor.EditorUtility.DisplayDialog (AMEditor.AMEditorSystem.ContentPlugin._ExternalPluginsUnavailableTitleDialog, 
																	AMEditor.AMEditorSystem.ContentPlugin._ExternalPluginsUnavailableMessage, 
																	AMEditor.AMEditorSystem.ContentStandardButton._Ok))
					{
						UnityEditor.EditorUtility.ClearProgressBar ();
					}
				}
			}
        }

		public void GetProjectForCI (string[] projectName, string pluginVersion, string groupName, string branchName, string pathToDownload)
		{
			SetParameters (projectName, pluginVersion, groupName, branchName);
			
			pathFolder = pathToDownload;
			
			downloadComplete = false;
			
			string commit = "";
			string archiveName = "";
			string downloadData = "";
			
			foreach (var project in gitProjectsList)
			{
				if (ChangeStatus != null)
					ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._CheckingBranch, 0);
				
				bool projectHasBranch = BranchCheck (project.id, branch);
				
				if (projectHasBranch)
				{
					if (version != string.Empty)
					{
						if (ChangeStatus != null)
						{
							ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._GettingTags, 0);
						}
						commit = GetTagCommit (project.id, version);
					}
					else
					{
						if (ChangeStatus != null)
						{
							ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._GettingLastCommit, 0);
						}
						commit = GetLastCommit (project.id, branch);
					}
					
					archiveName = project.path + "-" + commit + ".zip";//для GitLab ".tar.gz" вместо ".zip"
					downloadData = pathFolder + "/" + archiveName;

					if (!Directory.Exists (pathFolder))
					{
						try
						{
							Directory.CreateDirectory (pathFolder);
						}
						catch (Exception)
						{
							AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("203", AMEditor.AMEditorSystem.FileSystemError._203 (project.name));
							if (ErrorHappened != null)
								ErrorHappened ("203:" + AMEditor.AMEditorSystem.FileSystemError._203 (project.name));
						}
					}
					if (ChangeStatus != null)
						ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._DownloadingArchive, 0);
					
					string downloadArchiveMessage = string.Empty;
					if (version != string.Empty && commit == string.Empty)
					{
						string projName = gitProjectsList[0].name.ToLower ().Replace (" ", "-");
						downloadArchiveMessage = AMEditorNetwork.FileGetRequest (url + "/unity-plugins/" + projName + "/repository/archive.zip", 
						                                                        "?ref=v" + version + "&" + token, downloadData);
					}
					else
						downloadArchiveMessage = AMEditorNetwork.FileGetRequest (urlForApi + "/projects/" + project.id + "/repository/archive.zip", 
						                                                        "?sha=" + commit + "&" + token, downloadData);
					
					if (downloadArchiveMessage != string.Empty)
					{
						if (printDebug && !hideAllLogs)
							Debug.Log (downloadArchiveMessage);
						if (!hideAllLogs)
							Debug.Log ("Archive was successfully downloaded to " + pathFolder);
					}
					else
					{
						AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("105", AMEditor.AMEditorSystem.WebError._105 (project.name));
						
						if (ErrorHappened != null)
							ErrorHappened ("105:" + AMEditor.AMEditorSystem.FileSystemError._203 (project.name));
					}
					if (ChangeStatus != null)
						ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._ExtractingArchive, 0);
					
					using (ZipFile archive = ZipFile.Read (downloadData))
					{
						if (Directory.Exists (pathFolder + "/" + project.name + " " + version))
						{
							try
							{
								Directory.Delete (pathFolder + "/" + project.name + " " + version, true);
							}
							catch (Exception)
							{
								AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("202", AMEditor.AMEditorSystem.FileSystemError._202 (project.name));
								if (ErrorHappened != null)
									ErrorHappened ("202:" + AMEditor.AMEditorSystem.FileSystemError._202 (project.name));
							}
						}
						if (!hideAllLogs)
							Debug.Log (pathFolder + "/" + project.name + " " + version);
						try
						{
							archive.ExtractAll (pathFolder + "/" + project.name + " " + version);
						}
						catch (Exception)
						{
							AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("200", AMEditor.AMEditorSystem.FileSystemError._200 (project.name + " " + version));
							if (ErrorHappened != null)
								ErrorHappened ("200:" + AMEditor.AMEditorSystem.FileSystemError._200 (project.name + " " + version));
						}
					}
					if (!hideAllLogs)
						Debug.Log (project.name + " archive was successfully extracted to " + pathFolder + "/" + project.name + " " + version);
					
					if (ChangeStatus != null)
						ChangeStatus (AMEditor.AMEditorSystem.ContentStatuses._DeletingArchive, 0);
					
					try
					{
						File.Delete (downloadData);
						if (!hideAllLogs)
							Debug.Log (project.name + " " + version + " archive was deleted");
					}
					catch (Exception)
					{
						AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("201", AMEditor.AMEditorSystem.FileSystemError._201 (project.name + " " + version));
						if (ErrorHappened != null)
							ErrorHappened ("201:" + AMEditor.AMEditorSystem.FileSystemError._201 (project.name + " " + version));
					}
					downloadComplete = true;
					
					if (ArchiveDownloadComplete != null) ArchiveDownloadComplete (downloadComplete);
				}
				else
				{
					AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("103", AMEditor.AMEditorSystem.WebError._103 (project.name, branch));
					if (ErrorHappened != null)
						ErrorHappened ("103:" + AMEditor.AMEditorSystem.WebError._103 (project.name, branch));
				}
			}
			if (EndDownload != null)
				EndDownload ();
		}
		
		public static string RequestGet (string data)
        {
            try
            {
                return AMEditorNetwork.GetRequest (data, "");
            }
            catch (Exception)
            {
                return "{}";
            }
        }

        private bool ResponseIsValid (string response, string checkValue, bool ignore404)
        {
            if (response.Contains ("Not Found") && ignore404)
            {
                return false;
            }
            if (checkValue.Equals ("jsonarray"))
            {
                ArrayList responseArrayList = AMEditorJSON.JsonDecode (response) as ArrayList;

                if (responseArrayList != null)
                    return true;
            }
            else 
            {
                Hashtable responseHashtable = AMEditorJSON.JsonDecode (response) as Hashtable;

                if (responseHashtable != null)
                {
                    if (responseHashtable.ContainsKey (checkValue))
                    {
                        return true;
                    }
                    else if (responseHashtable.ContainsKey ("message"))
                    {
                        AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("", responseHashtable["message"].ToString ());

                        if (ErrorHappened != null)
							ErrorHappened (":" + responseHashtable["message"].ToString ());
                    }
                }
                else
                {
                    AMEditor.AMEditorPopupErrorWindow.ShowErrorPopup ("", response);

                    if (ErrorHappened != null)
                        ErrorHappened (":" + response);
                }
            }
            return false;
        }

        private bool CancelDownload (DoWorkEventArgs e)
        {
            if (ArchiveWorkerBW.CancellationPending)
            {
                e.Cancel = true;
                return true;
            }
            if (ConfigSearchBW.CancellationPending)
            {
                e.Cancel = true;
                return true;
            }
			if (PushConfigBW.CancellationPending)
			{
				e.Cancel = true;
				return true;
			}
            return false;
        }

        public void StopDownload ()
        {
            if (ArchiveWorkerBW.WorkerSupportsCancellation)
                ArchiveWorkerBW.CancelAsync ();
            if (ConfigSearchBW.WorkerSupportsCancellation)
                ConfigSearchBW.CancelAsync ();
            AMEditorNetwork.StopDownload ();
        }

        void OnDestroy ()
        {
            gitProjectsList = null;

            ArchiveWorkerBW = null;
            ConfigSearchBW = null;
        }
    }
}
#endif