using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Knot.Localization.Editor
{
    public class KnotBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (!preloadedAssets.Contains(KnotLocalization.ProjectSettings))
                PlayerSettings.SetPreloadedAssets(preloadedAssets.Append(KnotLocalization.ProjectSettings).ToArray());
        }
    }
}

