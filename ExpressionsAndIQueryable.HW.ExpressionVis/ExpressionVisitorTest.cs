using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionsAndIQueryable.HW.ExpressionVis
{
    [TestClass]
    public class ExpressionVisitorTest
    {
        [TestMethod]
        public void AddToIncrementTransformTest()
        {
            Expression<Func<int, int, string, string>> source_exp = (a,x, name) => a + (a - 1) * (a + 5) * (a + 1)+x+name;
            Console.WriteLine(source_exp + " " + source_exp.Compile().Invoke(3, 2, "Foo"));

            var replacedParamsExpression = (new ReplaceParamsExpressionVisitor(source_exp, a=>3, x=>2, name=>"Bar").ReplaceWithConstant());
            Console.WriteLine(replacedParamsExpression + " " + replacedParamsExpression.Compile().DynamicInvoke() );

        }

    }
}
