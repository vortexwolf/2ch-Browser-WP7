using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DvachBrowser.Assets.Extensions
{
    public static class PropertyNameExtensions
    {
        public static string GetPropertyName<TP>(Expression<Func<TP>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            MemberExpression memberExpression;

            if (expression.Body is UnaryExpression)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            else
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression", "expression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property", "expression");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property", "expression");
            }

            return memberExpression.Member.Name;
        }
    }
}
