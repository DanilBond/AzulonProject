using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Azulon.Configuration.Editor
{
    internal static class ContentEditorAssetUtility
    {
        public static T CreateAsset<T>(T asset, string folder, string fileName)
            where T : ScriptableObject
        {
            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            EnsureFolderExists(folder);
            var path = AssetDatabase.GenerateUniqueAssetPath(
                $"{folder}/{fileName}.asset");
            AssetDatabase.CreateAsset(asset, path);
            Undo.RegisterCreatedObjectUndo(asset, $"Create {typeof(T).Name}");
            return asset;
        }

        public static void AppendObjectReference(
            ScriptableObject owner,
            string listPropertyName,
            Object value,
            string undoName)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Undo.RecordObject(owner, undoName);
            var serializedOwner = new SerializedObject(owner);
            var list = serializedOwner.FindProperty(listPropertyName);
            if (list == null || !list.isArray)
            {
                throw new InvalidOperationException(
                    $"Asset '{owner.name}' has no array property '{listPropertyName}'.");
            }

            var index = list.arraySize;
            list.InsertArrayElementAtIndex(index);
            list.GetArrayElementAtIndex(index).objectReferenceValue = value;
            serializedOwner.ApplyModifiedProperties();
            EditorUtility.SetDirty(owner);
        }

        public static IReadOnlyList<T> FindAssets<T>(string folder) where T : Object
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                return Array.Empty<T>();
            }

            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder });
            var paths = new List<string>(guids.Length);
            foreach (var guid in guids)
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            paths.Sort(StringComparer.Ordinal);
            var assets = new List<T>(paths.Count);
            foreach (var path in paths)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets.AsReadOnly();
        }

        public static void EnsureFolderExists(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            var parts = folder.Split('/');
            var current = parts[0];
            for (var index = 1; index < parts.Length; index++)
            {
                var next = $"{current}/{parts[index]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }

        public static void SetObjectReferences<T>(
            SerializedProperty list,
            IReadOnlyList<T> values)
            where T : Object
        {
            list.arraySize = values.Count;
            for (var index = 0; index < values.Count; index++)
            {
                list.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
        }
    }
}
