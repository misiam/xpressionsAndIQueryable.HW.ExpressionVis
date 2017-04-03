using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionsAndIQueryable.HW.ExpressionVis
{
    public class ReplaceParamsExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _expression;
        private IDictionary<string, object> _toReplace;



        public ReplaceParamsExpressionVisitor(Expression expression, IDictionary<string, object> toReplace)
        {
            _expression = expression;

            this._toReplace = toReplace;
        }

        public ReplaceParamsExpressionVisitor(Expression<Func<int, int, string, string>> expression, params Expression<Func<string, object>>[] toReplaceFunc)
        {
            _expression = expression;

            IDictionary<string, object> toReplace = new Dictionary<string, object>();
            foreach (var func in toReplaceFunc)
            {
                toReplace.Add(func.Parameters[0].Name, func.Compile().Invoke(""));
            }
            _toReplace = toReplace;
        }

        public LambdaExpression ReplaceWithConstant(/*Expression expression /*, IDictionary<string, object> toReplace*/)
            {
            return (LambdaExpression)Visit(_expression);
        }

        public override Expression Visit(Expression node)
        {
  
            return base.Visit(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var exp = (new SimplifyIncrementExpressionVisitor().VisitAndConvert(node, "simplify_increment_decrement"));
            Console.WriteLine(exp);
            var parameters = exp.Parameters.Where(p => !_toReplace.ContainsKey(p.Name )).ToList();
            return Expression.Lambda(Visit(exp.Body), parameters);

        }

        protected override Expression VisitParameter(ParameterExpression node)
        {

            if (node.NodeType == ExpressionType.Parameter)
            {
                var paramNode = node as ParameterExpression;
                if (_toReplace.ContainsKey(paramNode.Name))
                {
                    object obj = (object)_toReplace[paramNode.Name];
                    return Expression.Constant(obj);

                }
            }
            return base.VisitParameter(node);
        }

    }
}
