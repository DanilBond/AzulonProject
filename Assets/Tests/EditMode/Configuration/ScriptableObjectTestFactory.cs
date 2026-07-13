using System;
using System.Collections.Generic;
using Azulon.Configuration.Items;
using Azulon.Domain.Items;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Azulon.Tests.EditMode.Configuration
{
    internal sealed class ScriptableObjectTestFactory : IDisposable
    {
        private readonly List<Object> _createdObjects = new List<Object>();

        public ItemTagDefinition CreateTag(string id, string displayName)
        {
            var definition = Track(ScriptableObject.CreateInstance<ItemTagDefinition>());
            definition.name = $"Tag_{id}";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_id").stringValue = id;
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public ItemDefinition CreateItem(
            string id,
            string displayName,
            string description,
            Sprite icon,
            int price,
            int power,
            ItemRarity rarity,
            ItemCategory category,
            params ItemTagDefinition[] tags)
        {
            var definition = Track(ScriptableObject.CreateInstance<ItemDefinition>());
            definition.name = $"Item_{id}";

            var serializedObject = new SerializedObject(definition);
            serializedObject.FindProperty("_id").stringValue = id;
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_description").stringValue = description;
            serializedObject.FindProperty("_icon").objectReferenceValue = icon;
            serializedObject.FindProperty("_price").intValue = price;
            serializedObject.FindProperty("_power").intValue = power;
            serializedObject.FindProperty("_rarity").intValue = (int)rarity;
            serializedObject.FindProperty("_category").intValue = (int)category;
            SetObjectArray(serializedObject.FindProperty("_tags"), tags);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }

        public ItemCatalogAsset CreateCatalog(
            ItemTagDefinition[] tags,
            ItemDefinition[] items)
        {
            var catalog = Track(ScriptableObject.CreateInstance<ItemCatalogAsset>());
            catalog.name = "ItemCatalog_Test";

            var serializedObject = new SerializedObject(catalog);
            SetObjectArray(serializedObject.FindProperty("_tagDefinitions"), tags);
            SetObjectArray(serializedObject.FindProperty("_itemDefinitions"), items);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return catalog;
        }

        public Sprite CreateIcon()
        {
            var texture = Track(new Texture2D(1, 1, TextureFormat.RGBA32, false));
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            return Track(Sprite.Create(
                texture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f)));
        }

        public void Dispose()
        {
            for (var index = _createdObjects.Count - 1; index >= 0; index--)
            {
                if (_createdObjects[index] != null)
                {
                    Object.DestroyImmediate(_createdObjects[index]);
                }
            }

            _createdObjects.Clear();
        }

        private T Track<T>(T instance) where T : Object
        {
            _createdObjects.Add(instance);
            return instance;
        }

        private static void SetObjectArray<T>(SerializedProperty property, IReadOnlyList<T> values)
            where T : Object
        {
            property.arraySize = values.Count;
            for (var index = 0; index < values.Count; index++)
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
        }
    }
}
