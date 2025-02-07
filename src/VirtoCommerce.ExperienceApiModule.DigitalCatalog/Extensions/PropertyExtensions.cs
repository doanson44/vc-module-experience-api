using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class PropertyExtensions
    {
        /// <summary>
        /// Flattens the tree-like structure of Property-PropertyValues into flat list of Properties,
        /// with each Property having a single PropertyValue in its Values collection
        /// </summary>
        public static IList<Property> ExpandByValues(this IEnumerable<Property> properties, string cultureName)
        {
            return properties.SelectMany(property =>
            {
                var propertyValues = property.Dictionary
                    // Group by Alias for dictionary properties
                    ? property.Values
                        .GroupBy(propertyValue => propertyValue.Alias)
                        .Select(aliasGroup
                            => aliasGroup.FirstOrDefault(propertyValue => propertyValue.LanguageCode.EqualsInvariant(cultureName))
                            // If localization not found build default value
                            ?? aliasGroup.Select(propertyValue =>
                            {
                                var clonedValue = (PropertyValue)propertyValue.Clone();
                                clonedValue.Value = aliasGroup.Key;
                                return clonedValue;
                            }).First()
                        )
                    : property.Values.Where(x => x.LanguageCode.EqualsInvariant(cultureName) || x.LanguageCode.IsNullOrEmpty());

                // wrap each PropertyValue into a Property
                return propertyValues
                    .Select(propertyValue => propertyValue.CopyPropertyWithValue(property))
                    .DefaultIfEmpty(property.CopyPropertyWithoutValues());
            }).ToList();
        }

        public static Property CopyPropertyWithValue(this PropertyValue propertyValue, Property property)
        {
            var clonedProperty = (Property)property.Clone();
            clonedProperty.Values = new List<PropertyValue> { propertyValue };
            return clonedProperty;
        }

        private static Property CopyPropertyWithoutValues(this Property property)
        {
            var clonedProperty = (Property)property.Clone();
            clonedProperty.Values = Array.Empty<PropertyValue>();
            return clonedProperty;
        }
    }
}
