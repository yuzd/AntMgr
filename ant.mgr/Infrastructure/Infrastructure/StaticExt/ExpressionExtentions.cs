using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.StaticExt
{
    public class ExpressionExtentions
    {
        public static Expression<Func<TModel, T>> GenerateMemberExpression<TModel, T>(string fieldName)
        {
            var fieldPropertyInfo = typeof(TModel).GetProperty(fieldName);
            var entityParam = Expression.Parameter(typeof(TModel), "r"); // {e}
            var columnExpr = Expression.MakeMemberAccess(entityParam, fieldPropertyInfo); // {e.fieldName}
            var lambda = Expression.Lambda(columnExpr, entityParam) as Expression<Func<TModel, T>>; // {e => e.column}
            return lambda;
        }


        public static Expression<Func<TObject, bool>> ApplyFilter<TObject, TValue>(String filterField, FilterOperation filterOper, TValue filterValue)
        {
            var type = typeof(TObject);
            ExpressionType operation;
            if (type.GetProperty(filterField) == null && type.GetField(filterField) == null)
                throw new MissingMemberException(type.Name, filterField);
            if (!operationMap.TryGetValue(filterOper, out operation))
                throw new ArgumentOutOfRangeException("filterOper", filterOper, "Invalid filter operation");

            var parameter = Expression.Parameter(type);

            var fieldAccess = Expression.PropertyOrField(parameter, filterField);
            var value = Expression.Constant(filterValue, filterValue.GetType());

            // let's perform the conversion only if we really need it
            var converted = value.Type != fieldAccess.Type
                ? (Expression)Expression.Convert(value, fieldAccess.Type)
                : (Expression)value;

            var body = Expression.MakeBinary(operation, fieldAccess, converted);

            var expr = Expression.Lambda<Func<TObject, bool>>(body, parameter);
            return expr;
        }

        // to restrict the allowable range of operations
        public enum FilterOperation
        {
            Equal,
            NotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual,
        }

        // we could have used reflection here instead since they have the same names
        static Dictionary<FilterOperation, ExpressionType> operationMap = new Dictionary<FilterOperation, ExpressionType>
        {
            { FilterOperation.Equal,                ExpressionType.Equal },
            { FilterOperation.NotEqual,             ExpressionType.NotEqual },
            { FilterOperation.LessThan,             ExpressionType.LessThan },
            { FilterOperation.LessThanOrEqual,      ExpressionType.LessThanOrEqual },
            { FilterOperation.GreaterThan,          ExpressionType.GreaterThan },
            { FilterOperation.GreaterThanOrEqual,   ExpressionType.GreaterThanOrEqual },
        };
    }
}
