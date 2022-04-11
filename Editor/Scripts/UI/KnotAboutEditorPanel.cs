using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotAboutEditorPanel : KnotEditorPanel
    {
        const string OnlineManualLink = "https://vd3v.com/assets/knot_localization/docs/articles/introduction.html";
        const string AssetStoreLink = "https://assetstore.unity.com/packages/tools/localization/knot-localization-187603";
        const string GitHubLink = "https://github.com/V0odo0/KNOT-Localization";
        const string OfflineManualFileName = "KNOT Localization - Manual.pdf";
        const string UnityForumThreadLink = "https://forum.unity.com/threads/knot-localization.1042891/";

        
        protected readonly VisualElement LogoImage;
        protected readonly Label VersionLabel;


        public KnotAboutEditorPanel() : base(nameof(KnotAboutEditorPanel))
        {
            LogoImage = Root.Q<VisualElement>(nameof(LogoImage));
            LogoImage.style.backgroundImage = new StyleBackground(KnotEditorUtils.CoreIcon as Texture2D);

            VersionLabel = Root.Q<Label>(nameof(VersionLabel));
            if (KnotEditorUtils.IsUpmPackage)
                VersionLabel.text = KnotEditorUtils.UpmPackageInfo.version;
            else VersionLabel.RemoveFromHierarchy();

            Root.Q<Button>("AssetStoreButton").clicked += () => OpenUrl(AssetStoreLink);
            Root.Q<Button>("OnlineManualButton").clicked += () => OpenUrl(OnlineManualLink);
            Root.Q<Button>("GitHubButton").clicked += () => OpenUrl(GitHubLink);
            Root.Q<Button>("UnityForumThreadButton").clicked += () => OpenUrl(UnityForumThreadLink);

            var offlineManualButton = Root.Q<Button>("OfflineManualButton");
            offlineManualButton.clicked += () => OpenUrl(GetOfflineManualUrl());
            if (string.IsNullOrEmpty(GetOfflineManualUrl()))
                offlineManualButton.RemoveFromHierarchy();

            GetLatestVersion(v =>
            {
                /*
                bool newVersionAvailable = v > KnotLocalization.Version;

                if (!string.IsNullOrEmpty(UpdateVersionLink))
                {
                    UpdateButton.SetEnabled(newVersionAvailable);
                    if (newVersionAvailable)
                        UpdateButton.text = $"Update to {v}";
                }
                */
            });
        }


        static void GetLatestVersion(Action<Version> versionLoaded)
        {
            /*var request = UnityWebRequest.Get(VersionCheckLink);
            request.SendWebRequest().completed += operation =>
            {
                string versionText = request.downloadHandler.text;

                versionLoaded?.Invoke(string.IsNullOrEmpty(versionText)
                    ? KnotLocalization.Version
                    : new Version(request.downloadHandler.text));
            };*/
        }

        static string GetOfflineManualUrl()
        {
            return Directory.GetFiles(Application.dataPath, OfflineManualFileName, SearchOption.AllDirectories).FirstOrDefault();
        }
        
        static void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
    }
}