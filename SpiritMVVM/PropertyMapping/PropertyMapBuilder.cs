using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SpiritMVVM.Utils;

namespace SpiritMVVM.PropertyMapping
{
    internal class PropertyMapBuilder : IPropertyMapBuilder
    {
        private readonly List<string> _dependencies;

        public PropertyMapBuilder(List<string> dependencies)
        {
            _dependencies = dependencies;
        }

        public IPropertyMapBuilder DependsOn(string propertyName)
        {
            if (!_dependencies.Contains(propertyName))
            {
                _dependencies.Add(propertyName);
            }
            return this;
        }

        public IPropertyMapBuilder DependsOn<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            string propertyName = propertyExpression.PropertyName();
            return DependsOn(propertyName);
        }
    }
}
