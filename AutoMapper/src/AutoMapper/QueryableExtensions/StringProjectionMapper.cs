using System.ComponentModel;
using System.Linq.Expressions;

using AutoMapper.Execution;
using AutoMapper.Internal;

namespace AutoMapper.QueryableExtensions.Impl
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class StringProjectionMapper : IProjectionMapper
    {
        public bool IsMatch(
            MemberMap memberMap,
            TypeMap memberTypeMap,
            Expression resolvedSource
        ) => memberMap.DestinationType == typeof(string);

        public Expression Project(
            IGlobalConfiguration configuration,
            MemberMap memberMap,
            TypeMap memberTypeMap,
            in ProjectionRequest request,
            Expression resolvedSource,
            LetPropertyMaps letPropertyMaps
        ) => Expression.Call(resolvedSource, ExpressionBuilder.ObjectToString);
    }
}
