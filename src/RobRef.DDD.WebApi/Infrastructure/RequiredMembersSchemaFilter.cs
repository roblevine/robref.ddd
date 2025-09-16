using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RobRef.DDD.WebApi.Infrastructure;

/// <summary>
/// Ensures that data annotation required members and non-nullable reference types
/// surface in generated OpenAPI documents for minimal APIs.
/// </summary>
public sealed class RequiredMembersSchemaFilter : ISchemaFilter
{
    private static readonly NullabilityInfoContext NullabilityContext = new();

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null || schema.Properties.Count == 0)
        {
            return;
        }

        var requiredMembers = context.Type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => IsRequired(property))
            .Select(property => ToCamelCase(property.Name))
            .Where(propertyName => schema.Properties.ContainsKey(propertyName))
            .ToArray();

        if (requiredMembers.Length == 0)
        {
            return;
        }

        schema.Required ??= new HashSet<string>(StringComparer.Ordinal);
        foreach (var member in requiredMembers)
        {
            if (!schema.Required.Contains(member))
            {
                schema.Required.Add(member);
            }
        }
    }

    private static bool IsRequired(PropertyInfo property)
    {
        if (property.GetCustomAttributes(typeof(RequiredAttribute), inherit: true).Any())
        {
            return true;
        }

        if (!property.PropertyType.IsValueType)
        {
            var nullabilityInfo = NullabilityContext.Create(property);
            return nullabilityInfo.WriteState == NullabilityState.NotNull;
        }

        return false;
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value, 0))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
