using System;
using UnityEditor;
using UnityEngine;

namespace Azulon.Configuration.Editor
{
    public sealed class GuildContentValidationPostprocessor : AssetPostprocessor
    {
        private const string ContentRoot = "Assets/Data/";
        private static bool _validationScheduled;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!ContainsContentAsset(importedAssets) &&
                !ContainsContentAsset(deletedAssets) &&
                !ContainsContentAsset(movedAssets) &&
                !ContainsContentAsset(movedFromAssetPaths))
            {
                return;
            }

            ScheduleValidation();
        }

        private static bool ContainsContentAsset(string[] paths)
        {
            foreach (var path in paths)
            {
                if (path.StartsWith(ContentRoot, StringComparison.Ordinal) &&
                    path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ScheduleValidation()
        {
            if (_validationScheduled)
            {
                return;
            }

            _validationScheduled = true;
            EditorApplication.delayCall += RunValidation;
        }

        private static void RunValidation()
        {
            _validationScheduled = false;
            try
            {
                ContentValidationService.ValidateAllProjectConfigurations(false);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
