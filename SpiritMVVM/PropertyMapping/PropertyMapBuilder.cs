using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SpiritMVVM.Utils;

namespace SpiritMVVM.PropertyMapping
{
    internal class PropertyMapBuilder : IPropertyMapBuilder
    {
        private readonly Action<string> _addDependantAction;

        /// <summary>
        /// Create a new instance of the <see cref="PropertyMapBuilder"/>,
        /// passing in a delegate to execute when reverse-mapping property dependencies.
        /// </summary>
        /// <param name="addDependantAction"></param>
        public PropertyMapBuilder(Action<string> addDependantAction)
        {
            if (addDependantAction == null)
                throw new ArgumentNullException("addDependantAction");
            
            _addDependantAction = addDependantAction;
        }

        /// <summary>
        /// Add a dependency to the currently-targeted property.
        /// </summary>
        /// <param name="propertyName">The name of the dependency.</param>
        /// <returns>Returns the current fluent syntax object for mapping
        /// additional dependencies.</returns>
        public IPropertyMapBuilder DependsOn(string propertyName)
        {
            _addDependantAction(propertyName);
            return this;
        }

        /// <summary>
        /// Add a dependency to the currently-targeted property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the added dependency.</typeparam>
        /// <param name="propertyExpression">The name of the dependency.</param>
        /// <returns>Returns the current fluent syntax object for mapping
        /// additional dependencies.</returns>
        public IPropertyMapBuilder DependsOn<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            string propertyName = propertyExpression.PropertyName();
            return DependsOn(propertyName);
        }
    }
}
