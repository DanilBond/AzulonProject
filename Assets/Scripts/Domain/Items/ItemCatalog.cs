using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Azulon.Domain.Items
{
    public sealed class ItemCatalog
    {
        private readonly ReadOnlyCollection<ItemData> _items;
        private readonly ReadOnlyCollection<ItemTagData> _tags;
        private readonly Dictionary<ItemId, ItemData> _itemsById;
        private readonly Dictionary<ItemTagId, ItemTagData> _tagsById;

        public ItemCatalog(IEnumerable<ItemTagData> tags, IEnumerable<ItemData> items)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var tagList = new List<ItemTagData>();
            _tagsById = new Dictionary<ItemTagId, ItemTagData>();
            foreach (var tag in tags)
            {
                if (tag == null)
                {
                    throw new ArgumentException("Catalog tags cannot contain null.", nameof(tags));
                }

                if (!_tagsById.TryAdd(tag.Id, tag))
                {
                    throw new ArgumentException($"Catalog contains duplicate tag ID '{tag.Id}'.", nameof(tags));
                }

                tagList.Add(tag);
            }

            if (tagList.Count == 0)
            {
                throw new ArgumentException("Catalog must contain at least one tag.", nameof(tags));
            }

            var itemList = new List<ItemData>();
            _itemsById = new Dictionary<ItemId, ItemData>();
            foreach (var item in items)
            {
                if (item == null)
                {
                    throw new ArgumentException("Catalog items cannot contain null.", nameof(items));
                }

                if (!_itemsById.TryAdd(item.Id, item))
                {
                    throw new ArgumentException($"Catalog contains duplicate item ID '{item.Id}'.", nameof(items));
                }

                foreach (var tagId in item.Tags)
                {
                    if (!_tagsById.ContainsKey(tagId))
                    {
                        throw new ArgumentException(
                            $"Item '{item.Id}' references unknown tag '{tagId}'.",
                            nameof(items));
                    }
                }

                itemList.Add(item);
            }

            if (itemList.Count == 0)
            {
                throw new ArgumentException("Catalog must contain at least one item.", nameof(items));
            }

            _tags = tagList.AsReadOnly();
            _items = itemList.AsReadOnly();
        }

        public IReadOnlyList<ItemData> Items => _items;

        public IReadOnlyList<ItemTagData> Tags => _tags;

        public bool TryGetItem(ItemId itemId, out ItemData item)
        {
            return _itemsById.TryGetValue(itemId, out item);
        }

        public bool TryGetTag(ItemTagId tagId, out ItemTagData tag)
        {
            return _tagsById.TryGetValue(tagId, out tag);
        }
    }
}
