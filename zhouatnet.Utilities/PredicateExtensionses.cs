using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace zhouatnet.Utilities
{
    public static class PredicateExtensionses
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp_left, Expression<Func<T, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "candidate");
            var parameterReplacer = new ParameterReplacer(candidateExpr);
            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);
            var body = Expression.And(left, right);
            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp_left, Expression<Func<T, bool>> exp_right)
        {

            var candidateExpr = Expression.Parameter(typeof(T), "candidate");
            var parameterReplacer = new ParameterReplacer(candidateExpr);
            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);
            var body = Expression.Or(left, right);
            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        public static Expression<Func<T, int, bool>> And<T>(this Expression<Func<T, int, bool>> exp_left, Expression<Func<T, int, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "candidate");
            var parameterReplacer = new ParameterReplacer(candidateExpr);
            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);

            var indexExpr = Expression.Parameter(typeof(int), "index");
            var parameterReplacer2 = new ParameterReplacer(indexExpr);
            left = parameterReplacer2.Replace(exp_left.Body);
            right = parameterReplacer2.Replace(exp_right.Body);
            var body = Expression.And(left, right);
            return Expression.Lambda<Func<T, int, bool>>(body, candidateExpr, indexExpr);
        }

        public static Expression<Func<T, int, bool>> Or<T>(this Expression<Func<T, int, bool>> exp_left, Expression<Func<T, int, bool>> exp_right)
        {
            var candidateExpr = Expression.Parameter(typeof(T), "candidate");
            var parameterReplacer = new ParameterReplacer(candidateExpr);
            var left = parameterReplacer.Replace(exp_left.Body);
            var right = parameterReplacer.Replace(exp_right.Body);

            var indexExpr = Expression.Parameter(typeof(int), "index");
            var parameterReplacer2 = new ParameterReplacer(indexExpr);
            left = parameterReplacer2.Replace(exp_left.Body);
            right = parameterReplacer2.Replace(exp_right.Body);
            var body = Expression.Or(left, right);
            return Expression.Lambda<Func<T, int, bool>>(body, candidateExpr, indexExpr);
        }
    }
}
