using System;
using System.Collections.Generic;
using Azulon.Configuration.Items.Validation;
using Azulon.Domain.Items;

namespace Azulon.Configuration.Items
{
    public static class ItemCatalogMapper
    {
        public static ItemCatalog ToDomain(ItemCatalogAsset catalogAsset)
        {
            var validation = ItemCatalogValidator.Validate(catalogAsset);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(
                    $"Cannot build item catalog because its configuration is invalid:{Environment.NewLine}{validation.FormatErrors()}");
            }

            var tags = new List<ItemTagData>(catalogAsset.TagDefinitions.Count);
            foreach (var tagDefinition in catalogAsset.TagDefinitions)
            {
                tags.Add(new ItemTagData(
                    new ItemTagId(tagDefinition.Id),
                    tagDefinition.DisplayName));
            }

            var items = new List<ItemData>(catalogAsset.ItemDefinitions.Count);
            foreach (var itemDefinition in catalogAsset.ItemDefinitions)
            {
                var tagIds = new List<ItemTagId>(itemDefinition.Tags.Count);
                foreach (var tagDefinition in itemDefinition.Tags)
                {
                    tagIds.Add(new ItemTagId(tagDefinition.Id));
                }

                items.Add(new ItemData(
                    new ItemId(itemDefinition.Id),
                    itemDefinition.DisplayName,
                    itemDefinition.Description,
                    itemDefinition.Price,
                    itemDefinition.Power,
                    itemDefinition.Rarity,
                    itemDefinition.Category,
                    tagIds));
            }

            return new ItemCatalog(tags, items);
        }
    }
}
