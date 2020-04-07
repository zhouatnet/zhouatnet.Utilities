using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace zhouatnet.Utilities
{
    /// <summary>
    /// 表达式树 想象者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpressionTreeVisualizer<T>
    {
        /// <summary>
        /// 创建简单排序表达式树
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> CreateSimpleSortExpression(string field)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), field);

            Expression<Func<T, object>> exp = Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(parameterExpression, typeof(T).GetProperty(field)), typeof(object)), new ParameterExpression[]
            {
                parameterExpression
            });

            return exp;
        }


        public static Expression<Func<T, S>> CreateSimpleSortExpression<S>(string field)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), field);

            Expression<Func<T, S>> exp = Expression.Lambda<Func<T, S>>(Expression.Convert(Expression.Property(parameterExpression, typeof(T).GetProperty(field)), typeof(S)), new ParameterExpression[]
            {
                parameterExpression
            });

            return exp;
        }
    }
}
