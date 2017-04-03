using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionsAndIQueryable.HW.Mapper
{
    public class MappingGenerator
    {
        public Mapper<TSource, TDestination> Generate<TSource, TDestination>() where TDestination : new()
        {
            var sourceParam = Expression.Parameter(typeof (TSource));
            var destType = typeof (TDestination);
            var ctor = Expression.New(destType);
            var initializedCtor = Expression.MemberInit(ctor, CreateMemberBindings(destType, sourceParam));

            var mapFunction =
                Expression.Lambda<Func<TSource, TDestination>>(initializedCtor, sourceParam);

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }


        private IList<MemberBinding> CreateMemberBindings(Type ctorType, ParameterExpression param)
        {
            var sourceType = param.Type;

            var sourceProperties = sourceType.GetProperties().Where(x => x.CanRead).ToList();
            var destinationProperties = ctorType.GetProperties().Where(x => x.CanWrite).ToList();

            IList<MemberBinding> memberBindings = new List<MemberBinding>();

            foreach (var sourceProp in sourceProperties)
            {
                var propName = sourceProp.Name;
                var destProp = destinationProperties.FirstOrDefault(x => x.Name == propName);
                if (destProp == null)
                {
                    continue;
                }

                var accessMember = Expression.MakeMemberAccess(param, sourceProp);
                var assignMember = Expression.Bind(destProp, accessMember);
                memberBindings.Add(assignMember);
            }

            return memberBindings;
        }
    }
}