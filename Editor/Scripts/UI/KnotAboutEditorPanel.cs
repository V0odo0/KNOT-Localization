using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Knot.Localization.Editor
{
    public class KnotAboutEditorPanel : KnotEditorPanel
    {
        ///todo: Update links
        const string OnlineManualLink = "https://vd3v.com/assets/knot_localization/docs/articles/introduction.html";
        const string OfflineManualFileName = "KNOT Localization - Manual.pdf";
        const string UnityForumThreadLink = "https://forum.unity.com/threads/knot-localization.1042891/";
        const string VersionCheckLink = "https://vd3v.com/assets/knot_localization/latest_version.txt";
        const string UpdateVersionLink = "";
        const string ContactDevEmail = "contact@vd3v.com";


        private readonly VisualElement LogoImage;
        private readonly Label VersionLabel;
        private readonly Button OnlineManualButton;
        private readonly Button OfflineManualButton;
        private readonly Button UnityForumThreadButton;
        private readonly Button UpdateButton;
        private readonly TextField ContactDevEmailLabel;


        public KnotAboutEditorPanel() : base(nameof(KnotAboutEditorPanel))
        {
            if (KnotEditorUtils.IsUpmPackage)
            {
                Debug.Log(KnotEditorUtils.UpmPackageInfo.version);
            }

            LogoImage = Root.Q<VisualElement>(nameof(LogoImage));
            LogoImage.style.backgroundImage = new StyleBackground(KnotEditorUtils.CoreIcon as Texture2D);

            VersionLabel = Root.Q<Label>(nameof(VersionLabel));
            //VersionLabel.text = $"v{KnotLocalization.Version}-beta";

            OnlineManualButton = Root.Q<Button>(nameof(OnlineManualButton));
            OnlineManualButton.clicked += OpenOnlineManual;
            OnlineManualButton.SetEnabled(!string.IsNullOrEmpty(OnlineManualLink));

            OfflineManualButton = Root.Q<Button>(nameof(OfflineManualButton));
            OfflineManualButton.clicked += OpenOfflineManual;
            if (string.IsNullOrEmpty(GetOfflineManualPath()))
                OfflineManualButton.RemoveFromHierarchy();

            UnityForumThreadButton = Root.Q<Button>(nameof(UnityForumThreadButton));
            UnityForumThreadButton.SetEnabled(!string.IsNullOrEmpty(UnityForumThreadLink));
            UnityForumThreadButton.clicked += OpenUnityForumThread;

            UpdateButton = Root.Q<Button>(nameof(UpdateButton));
            UpdateButton.SetEnabled(false);

            ContactDevEmailLabel = Root.Q<TextField>(nameof(ContactDevEmailLabel));
            ContactDevEmailLabel.value = ContactDevEmail;

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

        static string GetOfflineManualPath()
        {
            return Directory.GetFiles(Application.dataPath, OfflineManualFileName, SearchOption.AllDirectories).FirstOrDefault();
        }


        public static void OpenOnlineManual()
        {
            Application.OpenURL(OnlineManualLink);
        }

        public static void OpenOfflineManual()
        {
            Application.OpenURL(GetOfflineManualPath());
        }

        public static void OpenUnityForumThread()
        {
            Application.OpenURL(UnityForumThreadLink);
        }
    }
}