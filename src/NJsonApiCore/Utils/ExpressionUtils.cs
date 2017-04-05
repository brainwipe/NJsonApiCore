using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace NJsonApi.Utils
{
    public static class ExpressionUtils
    {
        // JObject.ToObject(value) method info
        private static readonly MethodInfo JObjectToObjectMethodInfo =
            typeof(JObject).GetMethod("ToObject",
            BindingFlags.Instance | BindingFlags.Public,
            null,
            CallingConventions.HasThis,
            new[] {typeof(Type)}, null);

        public static PropertyInfo GetPropertyInfo(this LambdaExpression propertyExpression)
        {
            var expression = propertyExpression.Body;
            if (expression is UnaryExpression)
                expression = ((UnaryExpression)expression).Operand;

            var me = expression as MemberExpression;

            if (me == null || !(me.Member is PropertyInfo))
                throw new NotSupportedException("Only simple property accessors are supported");

            return (PropertyInfo)me.Member;
        }

        public static Func<object, object> CompileToObjectTypedFunction<T>(Expression<Func<T, object>> expression)
        {
            ParameterExpression p = Expression.Parameter(typeof(object));
            Expression<Func<object, object>> convertedExpression = Expression.Lambda<Func<object, object>>
            (
                Expression.Invoke(expression, Expression.Convert(p, typeof(T))),
                p
            );

            return convertedExpression.Compile();
        }

        public static Expression<Action<object, object>> ConvertToObjectTypeExpression<T>(Expression<Action<T, object>> expression)
        {
            ParameterExpression p = Expression.Parameter(typeof(object));
            Expression<Action<object, object>> convertedExpression = Expression.Lambda<Action<object, object>>
            (
                Expression.Invoke(expression, Expression.Convert(p, typeof(T))),
                p
            );

            return convertedExpression;
        }

        public static Expression<Func<TResource, object>> CompileToObjectTypedExpression<TResource, TNested>(Expression<Func<TResource, TNested>> expression)
        {
            ParameterExpression p = Expression.Parameter(typeof(object));
            Expression<Func<TResource, object>> convertedExpression = Expression.Lambda<Func<TResource, object>>
            (
                Expression.Invoke(expression, Expression.Convert(p, typeof(TResource))),
                p
            );

            return convertedExpression;
        }

        public static Delegate ToCompiledGetterDelegate(this PropertyInfo pi, Type tInstance, Type tResult)
        {
            var mi = pi.GetGetMethod();
            var parameter = Expression.Parameter(tInstance);
            return Expression.Lambda(Expression.Call(parameter, mi), parameter).Compile();
        }

        public static Func<TInstance, TResult> ToCompiledGetterFunc<TInstance, TResult>(this PropertyInfo pi)
        {
            return (Func<TInstance, TResult>)ToCompiledGetterDelegate(pi, typeof(TInstance), typeof(TResult));
        }

        public static Delegate ToCompiledSetterDelegate(this PropertyInfo pi, Type tInstance, Type tValue)
        {
            if (!tValue.IsAssignableFrom(pi.PropertyType) && !pi.PropertyType.IsAssignableFrom(tValue))
                throw new InvalidOperationException($"Unsupported type combination: {tValue} and {pi.GetType()}.");

             
            if (Type.GetTypeCode(pi.PropertyType) == TypeCode.Object)
            {
                // Complex types
                // Use "(targetType) JObject.ToObject(value)" to get deserialized object
                var mi = pi.GetSetMethod();

                var instanceParameter = Expression.Parameter(tInstance);
                var valueParameter = Expression.Parameter(tValue);
                var typeConstant = Expression.Constant(pi.PropertyType);

                var convertToJObjectExpression = Expression.Convert(valueParameter, typeof(JObject));
                var toObjectCall = Expression.Call(convertToJObjectExpression, JObjectToObjectMethodInfo, typeConstant);
                var convertToTargetTypeExpression = Expression.Convert(toObjectCall, pi.PropertyType);
                var body = Expression.Call(instanceParameter, mi, convertToTargetTypeExpression);

                return Expression.Lambda(body, instanceParameter, valueParameter).Compile();
            }
            else
            {
                // Simple types
                var mi = pi.GetSetMethod();

                var instanceParameter = Expression.Parameter(tInstance);
                var valueParameter = Expression.Parameter(tValue);
                Expression valueExpression = valueParameter;

                if (pi.PropertyType != tValue)
                    valueExpression = Expression.Convert(valueExpression, pi.PropertyType);

                var body = Expression.Call(instanceParameter, mi, valueExpression);

                return Expression.Lambda(body, instanceParameter, valueParameter).Compile();
            }
        }

        public static Action<TInstance, TValue> ToCompiledSetterAction<TInstance, TValue>(this PropertyInfo pi)
        {
            return (Action<TInstance, TValue>)ToCompiledSetterDelegate(pi, typeof(TInstance), typeof(TValue));
        }
    }

}
