﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpiritMVVM.PropertyMapping
{
    /// <summary>
    /// Defines an interface through which an object can track its properties' 
    /// Change-tracking dependencies.
    /// </summary>
    internal interface IMapDependencies
    {
        /// <summary>
        /// Begin mapping a new property's dependencies using fluent syntax.
        /// </summary>
        /// <param name="propertyName">The property for which to add dependencies.</param>
        /// <returns>Returns a fluent property dependency builder.</returns>
        IPropertyMapBuilder Property(string propertyName);

        /// <summary>
        /// Begin mapping a new property's dependencies using fluent syntax.
        /// </summary>
        /// <typeparam name="TProperty">The type of the target property.</typeparam>
        /// <param name="propertyExpression">The property for which to add dependencies.</param>
        /// <returns>Returns a fluent property dependency builder.</returns>
        IPropertyMapBuilder Property<TProperty>(Expression<Func<TProperty>> propertyExpression);

        /// <summary>
        /// Get the list of all properties which depend on the given property.
        /// </summary>
        /// <remarks>The resulting list should include indirect dependants
        /// as well as direct ones (i.e. if A depends on B, and B depends on C,
        /// then the list of dependants from C will include both A and B).</remarks>
        /// <param name="propertyName">The property for which to gather all dependants.</param>
        /// <returns>Returns the list of all properties which are dependant on the given property.</returns>
        IEnumerable<string> GetDependantsFor(string propertyName);

        /// <summary>
        /// Get the list of all properties which depend on the given property.
        /// </summary>
        /// <remarks>The resulting list should include indirect dependants
        /// as well as direct ones (i.e. if A depends on B, and B depends on C,
        /// then the list of dependants from C will include both A and B).</remarks>
        /// <typeparam name="TProperty">The type of the target property.</typeparam>
        /// <param name="propertyExpression">The property for which to gather all dependants.</param>
        /// <returns>Returns the list of all properties which are dependant on the given property.</returns>
        IEnumerable<string> GetDependantsFor<TProperty>(Expression<Func<TProperty>> propertyExpression);
    }
}
