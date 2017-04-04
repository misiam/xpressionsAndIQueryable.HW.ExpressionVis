using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Sample03
{
    public class ExpressionToFTSRequestTranslator : ExpressionVisitor
    {
        StringBuilder resultString;
        List<string> queries = new List<string>(); 

        public string Translate(Expression exp)
        {
            resultString = new StringBuilder();
            Visit(exp);

            return resultString.ToString();
        }
        public IList<string> TranslateAnd(Expression exp)
        {
            resultString = new StringBuilder();
            queries = new List<string>();
            Visit(exp);

            string toAdd = resultString.ToString();

            if (!string.IsNullOrWhiteSpace(toAdd))
            {
                queries.Add(toAdd);
            }
            return queries;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof (Queryable))
            {
                if (node.Method.Name == "Where")
                {
                    var predicate = node.Arguments[1];
                    Visit(predicate);

                    return node;
                }
            }

            IList<string> allowedMethods = new List<string> { "StartsWith" , "EndsWith", "Contains" };
            if (allowedMethods.Contains(node.Method.Name))
            {
                string patternComparison = "{0}";

                switch (node.Method.Name)
                {
                    case "StartsWith":

                        patternComparison = "{0}*";
                        break;
                    case "EndsWith":

                        patternComparison = "*{0}";
                        break;
                    case "Contains":

                        patternComparison = "*{0}*";
                        break;
                }
                ConstantExpression left = Expression.Constant(string.Format(patternComparison, (node.Arguments[0] as ConstantExpression).Value));
                var right = node.Object as MemberExpression;
                var comparison = Expression.Equal(left, right);

                Visit(comparison);
                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Expression nodeLeft = node.Left;
            Expression nodeRight = node.Right;
            switch (node.NodeType) 
            {
                case ExpressionType.Equal:

                    

                    if (nodeLeft.NodeType != ExpressionType.MemberAccess)
                    {

                        nodeLeft = node.Right;
                        nodeRight = node.Left;
                    }

                    if (nodeLeft.NodeType != ExpressionType.MemberAccess)
                        throw new NotSupportedException(string.Format("Left operand should be property or field", node.NodeType));

                    if (nodeRight.NodeType != ExpressionType.Constant)
                        throw new NotSupportedException(string.Format("Right operand should be constant", node.NodeType));

                    Visit(nodeLeft);
                    resultString.Append("(");
                    Visit(nodeRight);
                    resultString.Append(")");
                    break;

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    var leftTranslator = new ExpressionToFTSRequestTranslator();
                    var leftResult = leftTranslator.TranslateAnd(nodeLeft);
                    queries.AddRange(leftResult);

                    var rightTranslator = new ExpressionToFTSRequestTranslator();
                    var rightResult = rightTranslator.TranslateAnd(nodeRight);
                    queries.AddRange(rightResult);

                    break;
                default:
                    throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            resultString.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            resultString.Append(node.Value);

            return node;
        }
    }

}
