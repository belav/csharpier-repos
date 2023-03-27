﻿namespace AutoMapper.Internal.Mappers;
public class ConstructorMapper : IObjectMapper
{
    public bool IsMatch(TypePair context) => GetConstructor(context.SourceType, context.DestinationType) != null;
    private static ConstructorInfo GetConstructor(Type sourceType, Type destinationType) => 
        destinationType.GetConstructor(TypeExtensions.InstanceFlags, null, new[] { sourceType }, null);
    public Expression MapExpression(IGlobalConfiguration configuration, ProfileMap profileMap, MemberMap memberMap, Expression sourceExpression, Expression destExpression)
    {
        var constructor = GetConstructor(sourceExpression.Type, destExpression.Type);
        return New(constructor, ToType(sourceExpression, constructor.FirstParameterType()));
    }
}