using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace NJsonApi.Utils
{
    public static class ExpressionUtils
    {
#if (NETCOREAPP1_0)
        // JObject.ToObject(value) method info
        private static readonly MethodInfo JObjectToObjectMethodInfo =
            typeof(JObject).GetMethods().Single(x => x.Name == "ToObject" && !x.ContainsGenericParameters && x.GetParameters().Length == 1);
        private static readonly MethodInfo JArrayToObjectMethodInfo =
            typeof(JArray).GetMethods().Single(x => x.Name == "ToObject" && !x.ContainsGenericParameters && x.GetParameters().Length == 1);
#else
        // JObject.ToObject(value) method info
        private static readonly MethodInfo JObjectToObjectMethodInfo =
            typeof(JObject).GetMethod("ToObject",
            BindingFlags.Instance | BindingFlags.Public,
            null,
            CallingConventions.HasThis,
            new[] { typeof(Type) }, null);

        private static readonly MethodInfo JArrayToObjectMethodInfo =
            typeof(JArray).GetMethod("ToObject",
            BindingFlags.Instance | BindingFlags.Public,
            null,
            CallingConventions.HasThis,
            new[] { typeof(Type) }, null);
#endif

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

        public static bool IsGenericType(Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static Delegate ToCompiledSetterDelegate(this PropertyInfo pi, Type tInstance, Type tValue)
        {
            if (!tValue.IsAssignableFrom(pi.PropertyType) && !pi.PropertyType.IsAssignableFrom(tValue))
                throw new InvalidOperationException($"Unsupported type combination: {tValue} and {pi.GetType()}.");

            var instanceParameter = Expression.Parameter(tInstance);
            var valueParameter = Expression.Parameter(typeof(object));

            Expression exp;
            if (Type.GetTypeCode(pi.PropertyType) == TypeCode.Object)
            {
                if (pi.PropertyType.GetInterfaces().Any(x => x == typeof(IEnumerable)))
                {
                    exp = CreateJArrayTypeSetterExpression(pi, instanceParameter, valueParameter);
                }
                else
                {
                    var canConvertToJObject = Expression.Equal(Expression.TypeAs(valueParameter, typeof(JObject)),
                        Expression.Constant(null));

                    exp = Expression.IfThenElse(canConvertToJObject,
                        CreateSimpleTypeSetterExpression(pi, instanceParameter, valueParameter),
                        CreateJObjectTypeSetterExpression(pi, instanceParameter, valueParameter));
                }
            }
            else
            {
                exp = CreateSimpleTypeSetterExpression(pi, instanceParameter, valueParameter);
            }
            return Expression.Lambda(exp, instanceParameter, valueParameter).Compile();
        }

        public static Action<TInstance, object> ToCompiledSetterAction<TInstance, TValue>(this PropertyInfo pi)
        {
            return (Action<TInstance, object>)ToCompiledSetterDelegate(pi, typeof(TInstance), typeof(TValue));
        }

        private static Expression CreateSimpleTypeSetterExpression(PropertyInfo pi, ParameterExpression instanceParameter, ParameterExpression valueParameter)
        {
            var mi = pi.GetSetMethod();
            Expression valueExpression = valueParameter;

            if (pi.PropertyType != valueParameter.Type)
                valueExpression = Expression.Convert(valueParameter, pi.PropertyType);

            var body = Expression.Call(instanceParameter, mi, valueExpression);

            return body;
        }

        private static Expression CreateJObjectTypeSetterExpression(PropertyInfo pi,
            ParameterExpression instanceParameter, ParameterExpression valueParameter)
        {
            return CreateJTokenSetterExpression<JObject>(pi, instanceParameter, valueParameter, JObjectToObjectMethodInfo);
        }

        private static Expression CreateJArrayTypeSetterExpression(PropertyInfo pi,
            ParameterExpression instanceParameter, ParameterExpression valueParameter)
        {
            return CreateJTokenSetterExpression<JArray>(pi, instanceParameter, valueParameter, JArrayToObjectMethodInfo);
        }

        private static Expression CreateJTokenSetterExpression<TJTokenType>(PropertyInfo pi,
            ParameterExpression instanceParameter, ParameterExpression valueParameter, MethodInfo method) where TJTokenType : JToken
        {
            // Use "(targetType) {JObject/JArray}.ToObject(value)" to get deserialized object
            var mi = pi.GetSetMethod();
            var typeConstant = Expression.Constant(pi.PropertyType);

            var convertToJObjectExpression = Expression.Convert(valueParameter, typeof(TJTokenType));
            var toObjectCall = Expression.Call(convertToJObjectExpression, method, typeConstant);
            var convertToTargetTypeExpression = Expression.Convert(toObjectCall, pi.PropertyType);
            var body = Expression.Call(instanceParameter, mi, convertToTargetTypeExpression);

            return body;
        }
    }

}
