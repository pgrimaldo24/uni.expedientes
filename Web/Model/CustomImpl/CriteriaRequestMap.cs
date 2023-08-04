using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Models.Cal;

namespace Unir.Expedientes.WebUi.Model.CustomImpl
{
    public class CriteriaRequestMap<TClass> : CriteriaRequest
        where TClass : QueryParameters
    {
        protected internal Dictionary<string, string> WhereFieldsMap;

        public CriteriaRequestMap()
        {
            WhereFieldsMap = new Dictionary<string, string>();

            AutoMapWhereFields();
        }

        public void MapWhereField<TMember>(Expression<Func<TClass, TMember>> expression, string fieldName)
        {
            var member = GetMemberExpression(expression.Body) as PropertyInfo;

            if (member == null)
                throw new InvalidOperationException($"No se encontró ls Propiedad '{expression}' en el tipo '{typeof(TClass).Name}'.");

            WhereFieldsMap[member.Name] = fieldName;
        }

        public TClass GetQueryParameters()
        {
            var result = (TClass)Activator.CreateInstance(typeof(TClass));
            if (result == null)
            {
                throw new InvalidOperationException($"No se pudo crear una instancia del tipo: {typeof(TClass).FullName}");
            }

            foreach (var propertyInfo in typeof(TClass).GetProperties())
            {
                if (!WhereFieldsMap.TryGetValue(propertyInfo.Name, out var fieldName))
                {
                    continue;
                }

                SetPropertyValue(result, propertyInfo, fieldName);
            }

            result.Offset = Offset;
            result.Limit = Limit;

            return result;
        }

        protected void AutoMapWhereFields()
        {
            foreach (var propertyInfo in typeof(TClass).GetProperties())
            {
                WhereFieldsMap[propertyInfo.Name] =
                    char.ToLowerInvariant(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1);
            }
        }

        protected internal virtual void SetPropertyValue(TClass instance, PropertyInfo propertyInfo, string fieldName)
        {
            var whereField = Where?.FirstOrDefault(c => c.Field == fieldName);

            if (whereField?.Value == null)
            {
                return;
            }
            
            
            if (propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(instance, whereField.Value?.ToString());
            }
            else if (propertyInfo.PropertyType == typeof(int?))
            {
                if (!int.TryParse(whereField.Value?.ToString(), out var parse))
                {
                    return;
                }
                propertyInfo.SetValue(instance, parse);
            }
            else if (propertyInfo.PropertyType == typeof(float?))
            {
                if (!float.TryParse(whereField.Value?.ToString(), out var parse))
                {
                    return;
                }
                propertyInfo.SetValue(instance, parse);
            }
            else if (propertyInfo.PropertyType == typeof(double?))
            {
                if (!double.TryParse(whereField.Value?.ToString(), out var parse))
                {
                    return;
                }
                propertyInfo.SetValue(instance, parse);
            }
            else if (propertyInfo.PropertyType == typeof(decimal?))
            {
                if (!decimal.TryParse(whereField.Value?.ToString(), out var parse))
                {
                    return;
                }
                propertyInfo.SetValue(instance, parse);
            }
            else if (propertyInfo.PropertyType == typeof(bool?))
            {
                if (!bool.TryParse(whereField.Value?.ToString(), out var parse))
                {
                    return;
                }
                propertyInfo.SetValue(instance, parse);
            }
            else if (propertyInfo.PropertyType == typeof(DateTime?))
            {
                var value = DateTime.Parse(whereField.Value.ToString() ?? string.Empty);
                propertyInfo.SetValue(instance, value);
            }
            else
            {
                var jsonValue = ((JsonElement)whereField.Value).ToString();
                if (jsonValue == null) return;
                var value = JsonSerializer.Deserialize(jsonValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(instance, value);
            }
        }

        protected internal virtual MemberInfo GetMemberExpression(Expression expression)
        {
            var memberExpression = (MemberExpression)null;
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpression = ((UnaryExpression)expression).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpression = expression as MemberExpression;
                    break;
            }
            return memberExpression?.Member;
        }
    }
}
