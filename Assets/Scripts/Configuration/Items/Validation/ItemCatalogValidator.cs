using System;
using System.Collections.Generic;
using Azulon.Configuration.Validation;
using Azulon.Domain.Items;

namespace Azulon.Configuration.Items.Validation
{
    public static class ItemCatalogValidator
    {
        public static ItemCatalogValidationResult Validate(ItemCatalogAsset catalog)
        {
            var issues = new List<CatalogValidationIssue>();
            if (catalog == null)
            {
                AddError(issues, "Item catalog is missing.");
                return new ItemCatalogValidationResult(issues);
            }

            var registeredTags = ValidateTags(catalog.TagDefinitions, issues);
            ValidateItems(catalog.ItemDefinitions, registeredTags, issues);
            return new ItemCatalogValidationResult(issues);
        }

        private static HashSet<ItemTagDefinition> ValidateTags(
            IReadOnlyList<ItemTagDefinition> tags,
            ICollection<CatalogValidationIssue> issues)
        {
            var registeredTags = new HashSet<ItemTagDefinition>();
            var ids = new HashSet<string>(StringComparer.Ordinal);

            if (tags.Count == 0)
            {
                AddError(issues, "Catalog must contain at least one tag definition.");
                return registeredTags;
            }

            for (var index = 0; index < tags.Count; index++)
            {
                var tag = tags[index];
                if (tag == null)
                {
                    AddError(issues, $"Tag entry at index {index} is missing.");
                    continue;
                }

                registeredTags.Add(tag);
                if (!ItemTagId.TryCreate(tag.Id, out _))
                {
                    AddError(issues, $"Tag '{tag.name}' has invalid ID '{tag.Id}'. Use lowercase letters, digits, and underscores.");
                }
                else if (!ids.Add(tag.Id))
                {
                    AddError(issues, $"Duplicate tag ID '{tag.Id}'.");
                }

                if (string.IsNullOrWhiteSpace(tag.DisplayName))
                {
                    AddError(issues, $"Tag '{tag.name}' has no display name.");
                }
            }

            return registeredTags;
        }

        private static void ValidateItems(
            IReadOnlyList<ItemDefinition> items,
            ISet<ItemTagDefinition> registeredTags,
            ICollection<CatalogValidationIssue> issues)
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (items.Count == 0)
            {
                AddError(issues, "Catalog must contain at least one item definition.");
                return;
            }

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                if (item == null)
                {
                    AddError(issues, $"Item entry at index {index} is missing.");
                    continue;
                }

                if (!ItemId.TryCreate(item.Id, out _))
                {
                    AddError(issues, $"Item '{item.name}' has invalid ID '{item.Id}'. Use lowercase letters, digits, and underscores.");
                }
                else if (!ids.Add(item.Id))
                {
                    AddError(issues, $"Duplicate item ID '{item.Id}'.");
                }

                if (string.IsNullOrWhiteSpace(item.DisplayName))
                {
                    AddError(issues, $"Item '{item.name}' has no display name.");
                }

                if (string.IsNullOrWhiteSpace(item.Description))
                {
                    AddError(issues, $"Item '{item.name}' has no description.");
                }

                if (item.Icon == null)
                {
                    AddError(issues, $"Item '{item.name}' has no icon.");
                }

                if (item.Price <= 0)
                {
                    AddError(issues, $"Item '{item.name}' must have a price greater than zero.");
                }

                if (item.Power < 0)
                {
                    AddError(issues, $"Item '{item.name}' cannot have negative power.");
                }

                if (!Enum.IsDefined(typeof(ItemRarity), item.Rarity))
                {
                    AddError(issues, $"Item '{item.name}' has an unknown rarity value.");
                }

                if (!Enum.IsDefined(typeof(ItemCategory), item.Category))
                {
                    AddError(issues, $"Item '{item.name}' has an unknown category value.");
                }

                ValidateItemTags(item, registeredTags, issues);
            }
        }

        private static void ValidateItemTags(
            ItemDefinition item,
            ISet<ItemTagDefinition> registeredTags,
            ICollection<CatalogValidationIssue> issues)
        {
            if (item.Tags.Count == 0)
            {
                AddError(issues, $"Item '{item.name}' must have at least one tag.");
                return;
            }

            var uniqueTags = new HashSet<ItemTagDefinition>();
            for (var index = 0; index < item.Tags.Count; index++)
            {
                var tag = item.Tags[index];
                if (tag == null)
                {
                    AddError(issues, $"Item '{item.name}' has a missing tag at index {index}.");
                    continue;
                }

                if (!uniqueTags.Add(tag))
                {
                    AddError(issues, $"Item '{item.name}' contains tag '{tag.name}' more than once.");
                }

                if (!registeredTags.Contains(tag))
                {
                    AddError(issues, $"Item '{item.name}' uses tag '{tag.name}' that is not registered in the catalog.");
                }
            }
        }

        private static void AddError(ICollection<CatalogValidationIssue> issues, string message)
        {
            issues.Add(new CatalogValidationIssue(CatalogValidationSeverity.Error, message));
        }
    }
}
