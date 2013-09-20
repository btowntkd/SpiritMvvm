using System;
using System.Linq.Expressions;

namespace SpiritMVVM.PropertyMapping
{
    /// <summary>
    /// Maps a list of dependencies to a specified property using a Fluent interface.
    /// </summary>
    public interface IPropertyMapBuilder
    {
        /// <summary>
        /// Add a dependency to the currently-targeted property.
        /// </summary>
        /// <param name="propertyName">The name of the dependency.</param>
        /// <returns>Returns the current fluent syntax object for mapping
        /// additional dependencies.</returns>
        IPropertyMapBuilder DependsOn(string propertyName);

        /// <summary>
        /// Add a dependency to the currently-targeted property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the added dependency.</typeparam>
        /// <param name="propertyExpression">The name of the dependency.</param>
        /// <returns>Returns the current fluent syntax object for mapping
        /// additional dependencies.</returns>
        IPropertyMapBuilder DependsOn<TProperty>(Expression<Func<TProperty>> propertyExpression);
    }
}