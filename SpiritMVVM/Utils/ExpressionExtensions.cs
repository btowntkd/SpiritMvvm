using System;
using System.Linq.Expressions;

namespace SpiritMVVM.Utils
{
    /// <summary>
    /// Provides extension methods for lambda expressions.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Retrieve the string name of the given property member expression.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>Returns the string name of the referred property.</returns>
        public static string PropertyName<TProperty>(this Expression<Func<TProperty>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression does not reference a valid member", "propertyExpression");

            return memberExpression.Member.Name;
        }
    }
}