using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Ardalis.Specification;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Domain.Attributes;

namespace ZeroFat.Application.Common.Specification;

//// See https://github.com/ardalis/Specification/issues/53
public static class SpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T> SearchBy<T>(this ISpecificationBuilder<T> query, BaseFilter filter)
        where T : class
        =>
        query
            .SearchByKeyword(filter.Keyword)
            .AdvancedSearch(filter.AdvancedSearch)
            .AdvancedFilter(filter.AdvancedFilter);

    public static ISpecificationBuilder<T> PaginateBy<T>(this ISpecificationBuilder<T> query, PaginationFilter filter)
    {
        if (filter.PageNumber <= 0)
        {
            filter.PageNumber = 1;
        }

        if (filter.PageSize <= 0)
        {
            filter.PageSize = 10;
        }

        if (filter.PageNumber > 1)
        {
            query = query.Skip((filter.PageNumber - 1) * filter.PageSize);
        }

        return query
            .Take(filter.PageSize)
            .OrderBy(filter.OrderBy);
    }

    public static ISpecificationBuilder<T> SearchByKeyword<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string? keyword)
        where T : class
        =>
        specificationBuilder.AdvancedSearch(new Search { Keyword = keyword });

    public static ISpecificationBuilder<T> AdvancedSearch<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Search? search)
        where T : class
    {
        if (!string.IsNullOrEmpty(search?.Keyword))
        {
            if (search.Fields?.Any() is true)
            {
                // search seleted fields (can contain deeper nested fields)
                foreach (string field in search.Fields)
                {
                    var paramExpr = Expression.Parameter(typeof(T));
                    MemberExpression propertyExpr = GetPropertyExpression(field, paramExpr);

                    specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
                }
            }
            else
            {
                var doesHasDeepSearch = typeof(T).GetCustomAttribute<ClassSupportDeepSearchAttribute>();

                if (doesHasDeepSearch != null)
                {
                    foreach (var property in typeof(T).GetProperties()
                        .Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is { } propertyType
                            && !propertyType.IsEnum))
                    {
                        var paramExpr = Expression.Parameter(typeof(T));
                        var propertyExpr = Expression.Property(paramExpr, property);

                        if (doesHasDeepSearch != null)
                        {
                            var pInfo = property.GetCustomAttribute<ColumnSupportDeepSearchAttribute>();

                            if (pInfo != null)
                            {
                                MemberExpression? insidePropertyExpr = null;

                                if (!IsSimpleType(property.PropertyType))
                                {
                                    foreach (string field in property.PropertyType.GetProperties().Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is { } propertyType && !propertyType.IsEnum && Type.GetTypeCode(propertyType) != TypeCode.Object).Select(x => x.Name))
                                    {
                                        insidePropertyExpr = GetPropertyExpression($"{property.Name}.{field}", paramExpr);

                                        specificationBuilder.AddSearchPropertyByKeyword(insidePropertyExpr, paramExpr, search.Keyword);
                                    }
                                }

                                specificationBuilder.AddSearchPropertyByKeyword(insidePropertyExpr ?? propertyExpr, paramExpr, search.Keyword);
                            }
                        }

                    }
                }
                else
                {
                    foreach (var property in typeof(T).GetProperties().Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is { } propertyType && !propertyType.IsEnum && Type.GetTypeCode(propertyType) != TypeCode.Object))
                    {
                        var paramExpr = Expression.Parameter(typeof(T));
                        var propertyExpr = Expression.Property(paramExpr, property);
                        specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
                    }
                }
            }
        }

        return specificationBuilder;
    }

    private static void AddSearchPropertyByKeyword<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression propertyExpr,
        ParameterExpression paramExpr,
        string keyword,
        string operatorSearch = FilterOperator.CONTAINS)
         where T : class
    {
        if (propertyExpr is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo property)
        {
            throw new ArgumentException("propertyExpr must be a property expression.", nameof(propertyExpr));
        }

        string searchTerm = operatorSearch switch
        {
            FilterOperator.STARTSWITH => $"{keyword}%",
            FilterOperator.ENDSWITH => $"%{keyword}",
            FilterOperator.CONTAINS => $"%{keyword}%",
            _ => throw new ArgumentException("operatorSearch is not valid.", nameof(operatorSearch))
        };

        // Generate lambda [ x => x.Property ] for string properties
        // or [ x => ((object)x.Property) == null ? null : x.Property.ToString() ] for other properties
        Expression selectorExpr =
            property.PropertyType == typeof(string)
                ? propertyExpr
                : Expression.Condition(
                    Expression.Equal(Expression.Convert(propertyExpr, typeof(object)), Expression.Constant(null, typeof(object))),
                    Expression.Constant(null, typeof(string)),
                    Expression.Call(propertyExpr, "ToString", null, null));

        var selector = Expression.Lambda<Func<T, string>>(selectorExpr, paramExpr);

        specificationBuilder.Search(selector, searchTerm, 1);

    }

    public static ISpecificationBuilder<T> AdvancedFilter<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Filter? filter)
        where T : class
    {
        if (filter is not null)
        {
            var parameter = Expression.Parameter(typeof(T));

            Expression binaryExpresioFilter;

            if (!string.IsNullOrEmpty(filter.Logic))
            {
                if (filter.Filters is null)
                    throw new CustomException("The Filters attribute is required when declaring a logic");
                binaryExpresioFilter = CreateFilterExpression(filter.Logic, filter.Filters, parameter);
            }
            else
            {
                var filterValid = GetValidFilter(filter);
                binaryExpresioFilter = CreateFilterExpression(filterValid.Field!, filterValid.Operator!, filterValid.Value, parameter);
            }

            ((List<WhereExpressionInfo<T>>)specificationBuilder.Specification.WhereExpressions)
                .Add(new WhereExpressionInfo<T>(Expression.Lambda<Func<T, bool>>(binaryExpresioFilter, parameter)));
        }

        return specificationBuilder;
    }

    private static Expression CreateFilterExpression(
        string logic,
        IEnumerable<Filter> filters,
        ParameterExpression parameter)
    {
        Expression filterExpression = default!;

        foreach (var filter in filters)
        {
            Expression bExpresionFilter;

            if (!string.IsNullOrEmpty(filter.Logic))
            {
                if (filter.Filters is null) throw new CustomException("The Filters attribute is required when declaring a logic");
                bExpresionFilter = CreateFilterExpression(filter.Logic, filter.Filters, parameter);
            }
            else
            {
                var filterValid = GetValidFilter(filter);
                bExpresionFilter = CreateFilterExpression(filterValid.Field!, filterValid.Operator!, filterValid.Value, parameter);
            }

            filterExpression = filterExpression is null ? bExpresionFilter : CombineFilter(logic, filterExpression, bExpresionFilter);
        }

        return filterExpression;
    }

    private static Expression CreateFilterExpression(
        string field,
        string filterOperator,
        object? value,
        ParameterExpression parameter)
    {
        var propertyExpresion = GetPropertyExpression(field, parameter);
        var valueExpresion = GeValuetExpression(field, value, propertyExpresion.Type);
        return CreateFilterExpression(propertyExpresion, valueExpresion, filterOperator);
    }

    private static Expression CreateFilterExpression(
        MemberExpression memberExpression,
        ConstantExpression constantExpression,
        string filterOperator)
    {
        return filterOperator switch
        {
            FilterOperator.EQ => Expression.Equal(memberExpression, constantExpression),
            FilterOperator.NEQ => Expression.NotEqual(memberExpression, constantExpression),
            FilterOperator.LT => Expression.LessThan(memberExpression, constantExpression),
            FilterOperator.LTE => Expression.LessThanOrEqual(memberExpression, constantExpression),
            FilterOperator.GT => Expression.GreaterThan(memberExpression, constantExpression),
            FilterOperator.GTE => Expression.GreaterThanOrEqual(memberExpression, constantExpression),
            FilterOperator.CONTAINS => Expression.Call(memberExpression, "Contains", null, constantExpression),
            FilterOperator.STARTSWITH => Expression.Call(memberExpression, "StartsWith", null, constantExpression),
            FilterOperator.ENDSWITH => Expression.Call(memberExpression, "EndsWith", null, constantExpression),
            _ => throw new CustomException("Filter Operator is not valid."),
        };
    }

    private static Expression CombineFilter(
        string filterOperator,
        Expression bExpresionBase,
        Expression bExpresion)
    {
        return filterOperator switch
        {
            "and" => Expression.And(bExpresionBase, bExpresion),
            "or" => Expression.Or(bExpresionBase, bExpresion),
            "xor" => Expression.ExclusiveOr(bExpresionBase, bExpresion),
            _ => throw new ArgumentException("FilterLogic is not valid.", nameof(FilterLogic)),
        };
    }

    private static MemberExpression GetPropertyExpression(
        string propertyName,
        ParameterExpression parameter)
    {
        Expression propertyExpression = parameter;
        foreach (string member in propertyName.Split('.'))
        {
            propertyExpression = Expression.PropertyOrField(propertyExpression, member);
        }

        return (MemberExpression)propertyExpression;
    }

    private static string GetStringFromJsonElement(object value)
        => ((JsonElement)value).GetString()!;

    private static ConstantExpression GeValuetExpression(
        string field,
        object? value,
        Type propertyType)
    {
        if (value == null) return Expression.Constant(null, propertyType);

        if (propertyType.IsEnum)
        {
            string? stringEnum = GetStringFromJsonElement(value);

            if (!Enum.TryParse(propertyType, stringEnum, true, out object? valueparsed)) throw new CustomException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }

        if (propertyType == typeof(Guid))
        {
            string? stringGuid = GetStringFromJsonElement(value);

            if (!Guid.TryParse(stringGuid, out Guid valueparsed)) throw new CustomException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }

        if (propertyType == typeof(string))
        {
            string? text = GetStringFromJsonElement(value);

            return Expression.Constant(text, propertyType);
        }

        if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            string? text = GetStringFromJsonElement(value);
            return Expression.Constant(ChangeType(text, propertyType), propertyType);
        }

        return Expression.Constant(ChangeType(((JsonElement)value).GetRawText(), propertyType), propertyType);
    }

    public static dynamic? ChangeType(object value, Type conversion)
    {
        var t = conversion;

        if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        {
            if (value == null)
            {
                return null;
            }

            t = Nullable.GetUnderlyingType(t);
        }

        return Convert.ChangeType(value, t!);
    }

    private static Filter GetValidFilter(Filter filter)
    {
        if (string.IsNullOrEmpty(filter.Field)) throw new CustomException("The field attribute is required when declaring a filter");
        if (string.IsNullOrEmpty(filter.Operator)) throw new CustomException("The Operator attribute is required when declaring a filter");
        return filter;
    }

    public static ISpecificationBuilder<T> OrderBy<T>(
         this ISpecificationBuilder<T> specificationBuilder,
         string[]? orderByFields)
    {
        IOrderedSpecificationBuilder<T> orderedBuilder = null!;

        if (orderByFields is not null)
        {
            foreach (var field in ParseOrderBy(orderByFields))
            {
                var paramExpr = Expression.Parameter(typeof(T));

                Expression propertyExpr = paramExpr;
                foreach (string member in field.Key.Split('.'))
                {
                    propertyExpr = Expression.PropertyOrField(propertyExpr, member);
                }

                var keySelector = Expression.Lambda<Func<T, object?>>(
                    Expression.Convert(propertyExpr, typeof(object)),
                    paramExpr);

                orderedBuilder = field.Value switch
                {
                    OrderTypeEnum.OrderBy => specificationBuilder.OrderBy(keySelector),
                    OrderTypeEnum.OrderByDescending => specificationBuilder.OrderByDescending(keySelector),
                    OrderTypeEnum.ThenBy => orderedBuilder.ThenBy(keySelector),
                    OrderTypeEnum.ThenByDescending => orderedBuilder.ThenByDescending(keySelector),
                    _ => throw new CustomException("OrderTypeEnum is not valid."),
                };
            }
        }

        return specificationBuilder;
    }


    private static Dictionary<string, OrderTypeEnum> ParseOrderBy(string[] orderByFields) =>
        new(orderByFields.Select((orderByfield, index) =>
        {
            string[] fieldParts = orderByfield.Split(' ');
            string field = fieldParts[0];
            bool descending = fieldParts.Length > 1 && fieldParts[1].StartsWith("Desc", StringComparison.OrdinalIgnoreCase);
            var orderBy = index == 0
                ? descending ? OrderTypeEnum.OrderByDescending
                                : OrderTypeEnum.OrderBy
                : descending ? OrderTypeEnum.ThenByDescending
                                : OrderTypeEnum.ThenBy;

            return new KeyValuePair<string, OrderTypeEnum>(field, orderBy);
        }));

    private static bool IsSimpleType(Type t)
    {
        return t.IsPrimitive || t.IsValueType || (t == typeof(string));
    }

}
