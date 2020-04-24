using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace zhouatnet.Utilities
{
    internal class ParameterReplacer : ExpressionVisitor
    {
        public ParameterReplacer(ParameterExpression paramExpr)
        {
            this.ParameterExpression = paramExpr;
        }

        public ParameterExpression ParameterExpression { get; private set; }

        public Expression Replace(Expression expr)
        {
            return this.Visit(expr);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (p.Type == ParameterExpression.Type)
            {
                return this.ParameterExpression;
            }

            return p;
        }
    }
}
