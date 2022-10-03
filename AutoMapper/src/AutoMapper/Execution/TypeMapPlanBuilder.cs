namespace AutoMapper.Execution;
public struct TypeMapPlanBuilder
{
    private static readonly MethodInfo MappingError = typeof(TypeMapPlanBuilder).GetStaticMethod(nameof(MemberMappingError));
    private readonly IGlobalConfiguration _configuration;
    private readonly ParameterExpression _destination;
    private readonly ParameterExpression _initialDestination;
    private readonly ParameterExpression[] _parameters;
    private readonly TypeMap _typeMap;
    private readonly ParameterExpression _source;
    private List<ParameterExpression> _variables;
    private List<Expression> _expressions;
    private CatchBlock[] _catches;
    public TypeMapPlanBuilder(IGlobalConfiguration configuration, TypeMap typeMap)
    {
        _configuration = configuration;
        _typeMap = typeMap;
        _source = Parameter(typeMap.SourceType, "source");
        _initialDestination = Parameter(typeMap.DestinationType, "destination");
        _destination = Variable(typeMap.DestinationType, "typeMapDestination");
        _variables = configuration.Variables;
        _expressions = configuration.Expressions;
        _catches = configuration.Catches;
        _parameters = _configuration.Parameters ?? new ParameterExpression[] { null, null, ContextParameter };
    }
    public Type DestinationType => _destination.Type;
    private static AutoMapperMappingException MemberMappingError(Exception innerException, MemberMap memberMap) => new("Error mapping types.", innerException, memberMap);
    ParameterExpression[] GetParameters(ParameterExpression first = null, ParameterExpression second = null)
    {
        _parameters[0] = first ?? _source;
        _parameters[1] = second ?? _destination;
        return _parameters;
    }
    public LambdaExpression CreateMapperLambda()
    {
        var parameters = GetParameters(second: _initialDestination);
        var customExpression = _typeMap.TypeConverter?.GetExpression(_configuration, parameters);
        if (customExpression != null)
        {
            return Lambda(customExpression, parameters);
        }
        _variables ??= new();
        _expressions ??= new();
        _catches ??= new CatchBlock[1];
        var typeMapsPath = _configuration.TypeMapsPath;
        Clear(ref typeMapsPath);
        CheckForCycles(_configuration, _typeMap, typeMapsPath);
        var createDestinationFunc = CreateDestinationFunc();
        var assignmentFunc = CreateAssignmentFunc(createDestinationFunc);
        var mapperFunc = CreateMapperFunc(assignmentFunc);
        _variables.Clear();
        _expressions.Clear();
        if (_typeMap.IncludedMembersTypeMaps.Count > 0)
        {
            IncludeMembers();
        }
        var checkContext = CheckContext(_typeMap);
        if (checkContext != null)
        {
            _expressions.Add(checkContext);
        }
        _expressions.Add(mapperFunc);
        _variables.Add(_destination);
        return Lambda(Block(_variables, _expressions), GetParameters(second: _initialDestination));
        static void Clear(ref HashSet<TypeMap> typeMapsPath)
        {
            if (typeMapsPath == null)
            {
                typeMapsPath = new HashSet<TypeMap>();
            }
            else
            {
                typeMapsPath.Clear();
            }
        }
    }
    void IncludeMembers()
    {
        var configuration = _configuration;
        _variables.AddRange(_typeMap.IncludedMembersTypeMaps.Select(i => i.Variable));
        var source = _source;
        _expressions.AddRange(_variables.Zip(_typeMap.IncludedMembersTypeMaps, (v, i) => Assign(v, configuration.ReplaceParameters(i.MemberExpression, source).NullCheck(null))));
    }
    private static void CheckForCycles(IGlobalConfiguration configuration, TypeMap typeMap, HashSet<TypeMap> typeMapsPath)
    {
        typeMapsPath.Add(typeMap);
        foreach (var memberMap in MemberMaps())
        {
            var memberTypeMap = ResolveMemberTypeMap(memberMap);
            if (memberTypeMap == null || memberTypeMap.HasTypeConverter)
            {
                continue;
            }
            if (memberMap.Inline && (memberTypeMap.PreserveReferences || typeMapsPath.Count == configuration.MaxExecutionPlanDepth))
            {
                memberMap.Inline = false;
                TraceInline(typeMap, memberMap);
            }
            if (memberTypeMap.PreserveReferences || memberTypeMap.MapExpression != null)
            {
                continue;
            }
            if (typeMapsPath.Contains(memberTypeMap))
            {
                if (memberTypeMap.SourceType.IsValueType)
                {
                    if (memberTypeMap.MaxDepth == 0)
                    {
                        memberTypeMap.MaxDepth = 10;
                    }
                    continue;
                }
                memberTypeMap.PreserveReferences = true;
                Trace(typeMap, memberTypeMap, memberMap);
                if (memberMap.Inline)
                {
                    memberMap.Inline = false;
                    TraceInline(typeMap, memberMap);
                }
                foreach (var derivedTypeMap in configuration.GetIncludedTypeMaps(memberTypeMap))
                {
                    derivedTypeMap.PreserveReferences = true;
                    Trace(typeMap, derivedTypeMap, memberMap);
                }
            }
            CheckForCycles(configuration, memberTypeMap, typeMapsPath);
        }
        typeMapsPath.Remove(typeMap);
        return;
        IEnumerable<MemberMap> MemberMaps()
        {
            var memberMaps = typeMap.MemberMaps;
            return typeMap.HasDerivedTypesToInclude ?
                memberMaps.Concat(configuration.GetIncludedTypeMaps(typeMap).SelectMany(tm => tm.MemberMaps)) :
                memberMaps;
        }
        TypeMap ResolveMemberTypeMap(MemberMap memberMap)
        {
            if (!memberMap.CanResolveValue)
            {
                return null;
            }
            var types = memberMap.Types();
            return types.ContainsGenericParameters ? null : configuration.ResolveAssociatedTypeMap(types);
        }
        [Conditional("DEBUG")]
        static void Trace(TypeMap typeMap, TypeMap memberTypeMap, MemberMap memberMap) =>
            Debug.WriteLine($"Setting PreserveReferences: {memberMap.DestinationName} {typeMap.SourceType} - {typeMap.DestinationType} => {memberTypeMap.SourceType} - {memberTypeMap.DestinationType}");
        [Conditional("DEBUG")]
        static void TraceInline(TypeMap typeMap, MemberMap memberMap) =>
            Debug.WriteLine($"Resetting Inline: {memberMap.DestinationName} in {typeMap.SourceType} - {typeMap.DestinationType}");
    }
    private Expression CreateDestinationFunc()
    {
        var newDestFunc = CreateNewDestinationFunc();
        var getDest = DestinationType.IsValueType ? newDestFunc : Coalesce(_initialDestination, ToType(newDestFunc, DestinationType));
        var destinationFunc = Assign(_destination, getDest);
        return _typeMap.PreserveReferences ?
            Block(destinationFunc, Call(ContextParameter, CacheDestinationMethod, _source, Constant(DestinationType), _destination), _destination) :
            destinationFunc;
    }
    Expression ReplaceParameters(LambdaExpression lambda) => _configuration.ReplaceParameters(lambda, GetParameters());
    private Expression CreateAssignmentFunc(Expression createDestination)
    {
        List<Expression> actions = new() { createDestination };
        Expression typeMapExpression = null;
        var hasMaxDepth = _typeMap.MaxDepth > 0;
        if (hasMaxDepth)
        {
            typeMapExpression = Constant(_typeMap);
            actions.Add(Call(ContextParameter, IncTypeDepthInfo, typeMapExpression));
        }
        var beforeMap = _typeMap.BeforeMapActions;
        if (beforeMap.Count > 0)
        {
            foreach (var beforeMapAction in beforeMap)
            {
                actions.Add(ReplaceParameters(beforeMapAction));
            }
        }
        var propertyMaps = _typeMap.OrderedPropertyMaps();
        if (propertyMaps != null)
        {
            foreach (var propertyMap in propertyMaps)
            {
                if (propertyMap.CanResolveValue)
                {
                    var property = TryPropertyMap(propertyMap);
                    if (_typeMap.ConstructorParameterMatches(propertyMap.DestinationName))
                    {
                        property = _initialDestination.IfNullElse(_configuration.Default(property.Type), property);
                    }
                    actions.Add(property);
                }
            }
        }
        var pathMaps = _typeMap.PathMaps;
        if (pathMaps.Count > 0)
        {
            foreach (var pathMap in pathMaps)
            {
                if (!pathMap.Ignored)
                {
                    actions.Add(TryPathMap(pathMap));
                }
            }
        }
        var afterMap = _typeMap.AfterMapActions;
        if (afterMap.Count > 0)
        {
            foreach (var afterMapAction in afterMap)
            {
                actions.Add(ReplaceParameters(afterMapAction));
            }
        }
        if (hasMaxDepth)
        {
            actions.Add(Call(ContextParameter, DecTypeDepthInfo, typeMapExpression));
        }
        actions.Add(_destination);
        return Block(actions);
    }
    private Expression TryPathMap(PathMap pathMap)
    {
        var destination = ((MemberExpression)_configuration.ConvertReplaceParameters(pathMap.DestinationExpression, _destination)).Expression;
        var configuration = _configuration;
        var createInnerObjects = CreateInnerObjects(destination);
        var setFinalValue = CreatePropertyMapFunc(pathMap, destination, pathMap.MemberPath.Last);
        var pathMapExpression = Block(createInnerObjects, setFinalValue);
        return TryMemberMap(pathMap, pathMapExpression);
        Expression CreateInnerObjects(Expression destination)
        {
            return Block(destination.GetMemberExpressions().Select(NullCheck).Append(ExpressionBuilder.Empty));
            Expression NullCheck(MemberExpression memberExpression)
            {
                var setter = GetSetter(memberExpression);
                var ifNull = setter == null
                    ? Throw(Constant(new NullReferenceException($"{memberExpression} cannot be null because it's used by ForPath.")), memberExpression.Type)
                    : (Expression)Assign(setter, ObjectFactory.GenerateConstructorExpression(memberExpression.Type, configuration));
                return memberExpression.IfNullElse(ifNull, configuration.Default(memberExpression.Type));
            }
            static Expression GetSetter(MemberExpression memberExpression) => memberExpression.Member switch
            {
                PropertyInfo { CanWrite: true } property => Property(memberExpression.Expression, property),
                FieldInfo { IsInitOnly: false } field => Field(memberExpression.Expression, field),
                _ => null,
            };
        }
    }
    private Expression CreateMapperFunc(Expression assignmentFunc)
    {
        var mapperFunc = assignmentFunc;
        var overMaxDepth = OverMaxDepth(_typeMap);
        if (overMaxDepth != null)
        {
            mapperFunc = Condition(overMaxDepth, _configuration.Default(DestinationType), mapperFunc);
        }
        mapperFunc = _configuration.NullCheckSource(_typeMap.Profile, _source, _initialDestination, mapperFunc, null);
        return CheckReferencesCache(mapperFunc);
    }
    private Expression CheckReferencesCache(Expression valueBuilder)
    {
        if(!_typeMap.PreserveReferences)
        {
            return valueBuilder;
        }
        var getCachedDestination = Call(ContextParameter, GetDestinationMethod, _source, Constant(DestinationType));
        return Coalesce(ToType(getCachedDestination, DestinationType), valueBuilder);
    }
    private Expression CreateNewDestinationFunc() => _typeMap switch
    {
        { CustomCtorFunction: LambdaExpression constructUsingFunc } => _configuration.ReplaceParameters(constructUsingFunc, GetParameters(second: ContextParameter)),
        { ConstructorMap: { CanResolve: true } constructorMap } => ConstructorMapping(constructorMap),
        { DestinationType: { IsInterface: true } interfaceType } => Throw(Constant(new AutoMapperMappingException("Cannot create interface "+interfaceType, null, _typeMap)), interfaceType),
        _ => ObjectFactory.GenerateConstructorExpression(DestinationType, _configuration)
    };
    private Expression ConstructorMapping(ConstructorMap constructorMap)
    {
        var ctorArgs = constructorMap.CtorParams.Select(CreateConstructorParameterExpression);
        var variables = constructorMap.Ctor.GetParameters().Select(parameter => Variable(parameter.ParameterType, parameter.Name)).ToArray();
        var body = variables.Zip(ctorArgs, (variable, expression) => (Expression)Assign(variable, ToType(expression, variable.Type)))
            .Append(CheckReferencesCache(New(constructorMap.Ctor, variables)));
        return Block(variables, body);
    }
    private Expression CreateConstructorParameterExpression(ConstructorParameterMap ctorParamMap)
    {
        var defaultValue = ctorParamMap.DefaultValue(_configuration);
        var customSource = GetCustomSource(ctorParamMap);
        var resolvedExpression = BuildValueResolverFunc(ctorParamMap, customSource, defaultValue);
        var resolvedValue = Variable(resolvedExpression.Type, "resolvedValue");
        var mapMember = MapMember(ctorParamMap, defaultValue, resolvedValue);
        _variables.Clear();
        _variables.Add(resolvedValue);
        _expressions.Clear();
        _expressions.Add(Assign(resolvedValue, resolvedExpression));
        _expressions.Add(mapMember);
        return TryMemberMap(ctorParamMap, Block(_variables, _expressions));
    }
    private Expression TryPropertyMap(PropertyMap propertyMap)
    {
        var propertyMapExpression = CreatePropertyMapFunc(propertyMap, _destination, propertyMap.DestinationMember);
        return TryMemberMap(propertyMap, propertyMapExpression);
    }
    private Expression TryMemberMap(MemberMap memberMap, Expression memberMapExpression)
    {
        var newException = Call(MappingError, ExceptionParameter, Constant(memberMap));
        _catches[0] = Catch(ExceptionParameter, Throw(newException, memberMapExpression.Type));
        return TryCatch(memberMapExpression, _catches);
    }
    private Expression CreatePropertyMapFunc(MemberMap memberMap, Expression destination, MemberInfo destinationMember)
    {
        Expression destinationMemberAccess, destinationMemberGetter;
        bool destinationMemberReadOnly;
        if (destinationMember is PropertyInfo destinationProperty)
        {
            destinationMemberAccess = Property(destination, destinationProperty);
            destinationMemberReadOnly = !destinationProperty.CanWrite;
            destinationMemberGetter = destinationProperty.CanRead ? destinationMemberAccess : _configuration.Default(memberMap.DestinationType);
        }
        else
        {
            var destinationField = (FieldInfo)destinationMember;
            destinationMemberAccess = Field(destination, destinationField);
            destinationMemberReadOnly = destinationField.IsInitOnly;
            destinationMemberGetter = destinationMemberAccess;
        }
        var customSource = GetCustomSource(memberMap);
        var valueResolver = BuildValueResolverFunc(memberMap, customSource, destinationMemberGetter);
        var resolvedValueVariable = Variable(valueResolver.Type, "resolvedValue");
        var destinationMemberValue = DestinationMemberValue(memberMap, destinationMemberGetter, destinationMemberReadOnly);
        var mappedMember = MapMember(memberMap, destinationMemberValue, resolvedValueVariable);
        var mappedMemberVariable = SetVariables(valueResolver, resolvedValueVariable, mappedMember);
        var mapperExpr = destinationMemberReadOnly ? (Expression)mappedMemberVariable : Assign(destinationMemberAccess, mappedMemberVariable);
        if (memberMap.Condition != null)
        {
            _expressions.Add(IfThen(
                _configuration.ConvertReplaceParameters(memberMap.Condition, new[] { customSource, _destination, mappedMemberVariable, destinationMemberGetter, ContextParameter }),
                mapperExpr));
        }
        else if (!destinationMemberReadOnly)
        {
            _expressions.Add(mapperExpr);
        }
        if (memberMap.PreCondition != null)
        {
            Precondition(memberMap, customSource);
        }
        return Block(_variables, _expressions);
    }
    Expression DestinationMemberValue(MemberMap memberMap, Expression destinationMemberGetter, bool destinationMemberReadOnly)
    {
        if (destinationMemberReadOnly || memberMap.UseDestinationValue is true)
        {
            return destinationMemberGetter;
        }
        var defaultValue = _configuration.Default(memberMap.DestinationType);
        return DestinationType.IsValueType ? defaultValue : Condition(ReferenceEqual(_initialDestination, Null), defaultValue, destinationMemberGetter);
    }
    void Precondition(MemberMap memberMap, ParameterExpression customSource)
    {
        var preCondition = _configuration.ConvertReplaceParameters(memberMap.PreCondition, GetParameters(first: customSource));
        var ifThen = IfThen(preCondition, Block(_expressions));
        _expressions.Clear();
        _expressions.Add(ifThen);
    }
    ParameterExpression SetVariables(Expression valueResolver, ParameterExpression resolvedValueVariable, Expression mappedMember)
    {
        _expressions.Clear();
        _variables.Clear();
        _variables.Add(resolvedValueVariable);
        _expressions.Add(Assign(resolvedValueVariable, valueResolver));
        ParameterExpression mappedMemberVariable;
        if (mappedMember == resolvedValueVariable)
        {
            mappedMemberVariable = resolvedValueVariable;
        }
        else
        {
            mappedMemberVariable = Variable(mappedMember.Type, "mappedValue");
            _variables.Add(mappedMemberVariable);
            _expressions.Add(Assign(mappedMemberVariable, mappedMember));
        }
        return mappedMemberVariable;
    }
    Expression MapMember(MemberMap memberMap, Expression destinationMemberValue, ParameterExpression resolvedValue)
    {
        var typePair = memberMap.Types();
        var profile = _typeMap.Profile;
        var mapMember = memberMap.Inline ?
            _configuration.MapExpression(profile, typePair, resolvedValue, memberMap, destinationMemberValue) :
            _configuration.NullCheckSource(profile, resolvedValue, destinationMemberValue, ContextMap(typePair, resolvedValue, destinationMemberValue, memberMap), memberMap);
        return memberMap.ApplyTransformers(mapMember, _configuration);
    }
    private Expression BuildValueResolverFunc(MemberMap memberMap, Expression customSource, Expression destValueExpr)
    {
        var valueResolverFunc = memberMap.Resolver?.GetExpression(_configuration, memberMap, customSource, _destination, destValueExpr) ?? destValueExpr;
        if (memberMap.NullSubstitute != null)
        {
            valueResolverFunc = memberMap.NullSubstitute(valueResolverFunc);
        }
        else if (!memberMap.AllowsNullDestinationValues)
        {
            var toCreate = memberMap.SourceType;
            if (!toCreate.IsAbstract && toCreate.IsClass && !toCreate.IsArray)
            {
                var ctor = ObjectFactory.GenerateConstructorExpression(toCreate, _configuration);
                valueResolverFunc = Coalesce(valueResolverFunc, ToType(ctor, valueResolverFunc.Type));
            }
        }
        return valueResolverFunc;
    }
    private ParameterExpression GetCustomSource(MemberMap memberMap) => memberMap.IncludedMember?.Variable ?? _source;
}
public interface IValueResolver
{
    Expression GetExpression(IGlobalConfiguration configuration, MemberMap memberMap, Expression source, Expression destination, Expression destinationMember);
    MemberInfo GetSourceMember(MemberMap memberMap);
    Type ResolvedType { get; }
    string SourceMemberName => null;
    LambdaExpression ProjectToExpression => null;
    IValueResolver CloseGenerics(TypeMap typeMap) => this;
}
public class MemberPathResolver : IValueResolver
{
    private readonly MemberInfo[] _members;
    public MemberPathResolver(MemberInfo[] members) => _members = members;
    public Type ResolvedType => _members?[^1].GetMemberType();
    public Expression GetExpression(IGlobalConfiguration configuration, MemberMap memberMap, Expression source, Expression destination, Expression destinationMember)
    {
        var expression = _members.Chain(source);
        return memberMap.IncludedMember == null && _members.Length < 2 ? expression : expression.NullCheck(configuration, memberMap, destinationMember);
    }
    public MemberInfo GetSourceMember(MemberMap memberMap) => _members.Length == 1 ? _members[0] : null;
    public LambdaExpression ProjectToExpression => _members.Lambda();
    public IValueResolver CloseGenerics(TypeMap typeMap) => _members[0].DeclaringType.ContainsGenericParameters ?
        new MemberPathResolver(ReflectionHelper.GetMemberPath(typeMap.SourceType, Array.ConvertAll(_members, m => m.Name), typeMap)) : this;
}
public abstract class LambdaValueResolver
{
    public LambdaExpression Lambda { get; }
    public Type ResolvedType => Lambda.ReturnType;
    protected LambdaValueResolver(LambdaExpression lambda) => Lambda = lambda;
}
public class FuncResolver : LambdaValueResolver, IValueResolver
{
    public FuncResolver(LambdaExpression lambda) : base(lambda) { }
    public Expression GetExpression(IGlobalConfiguration configuration, MemberMap memberMap, Expression source, Expression destination, Expression destinationMember) =>
        configuration.ConvertReplaceParameters(Lambda, new[] { source, destination, destinationMember, ContextParameter });
    public MemberInfo GetSourceMember(MemberMap _) => null;
}
public class ExpressionResolver : LambdaValueResolver, IValueResolver
{
    public ExpressionResolver(LambdaExpression lambda) : base(lambda) { }
    public Expression GetExpression(IGlobalConfiguration configuration, MemberMap memberMap, Expression source, Expression _, Expression destinationMember)
    {
        var mapFrom = configuration.ReplaceParameters(Lambda, source);
        var nullCheckedExpression = mapFrom.NullCheck(configuration, memberMap, destinationMember);
        if (nullCheckedExpression != mapFrom)
        {
            return nullCheckedExpression;
        }
        var defaultExpression = configuration.Default(mapFrom.Type);
        return TryCatch(mapFrom, Catch(typeof(NullReferenceException), defaultExpression), Catch(typeof(ArgumentNullException), defaultExpression));
    }
    public MemberInfo GetSourceMember(MemberMap _) => Lambda.GetMember();
    public LambdaExpression ProjectToExpression => Lambda;
}
public abstract class ValueResolverConfig
{
    private protected readonly Expression _instance;
    public Type ConcreteType { get; }
    public Type InterfaceType { get; }
    public LambdaExpression SourceMemberLambda { get; init; }
    protected ValueResolverConfig(Type concreteType, Type interfaceType, Expression instance = null)
    {
        ConcreteType = concreteType;
        InterfaceType = interfaceType;
        _instance = instance;
    }
    protected ValueResolverConfig(object instance, Type interfaceType)
    {
        _instance = Constant(instance);
        InterfaceType = interfaceType;
    }
    public string SourceMemberName { get; init; }
    public Type ResolvedType => InterfaceType.GenericTypeArguments[^1];
}
public class ValueConverter : ValueResolverConfig, IValueResolver
{
    public ValueConverter(Type concreteType, Type interfaceType) : base(concreteType, interfaceType, ServiceLocator(concreteType)) { }
    public ValueConverter(object instance, Type interfaceType) : base(instance, interfaceType) { }
    public Expression GetExpression(IGlobalConfiguration configuration, MemberMap memberMap, Expression source, Expression _, Expression destinationMember)
    {
        var sourceMemberType = InterfaceType.GenericTypeArguments[0];
        var sourceMember = this switch
        {
            { SourceMemberLambda: { } } => configuration.ReplaceParameters(SourceMemberLambda, source),
            { SourceMemberName: { } } => PropertyOrField(source, SourceMemberName),
            _ when memberMap.SourceMembers.Length > 0 => memberMap.ChainSourceMembers(configuration, source, destinationMember),
            _ => Throw(Constant(BuildExceptionMessage()), sourceMemberType)
        };
        return Call(ToType(_instance, InterfaceType), InterfaceType.GetMethod("Convert"), ToType(sourceMember, sourceMemberType), ContextParameter);
        AutoMapperConfigurationException BuildExceptionMessage()
            => new($"Cannot find a source member to pass to the value converter of type {ConcreteType}. Configure a source member to map from.");
    }
    public MemberInfo GetSourceMember(MemberMap memberMap) => this switch
    {
        { SourceMemberLambda: { } lambda } => lambda.GetMember(),
        { SourceMemberName: { } } => null,
        _ => memberMap.SourceMembers.Length == 1 ? memberMap.SourceMembers[0] : null
    };
}
public class ClassValueResolver : ValueResolverConfig, IValueResolver
{
    public ClassValueResolver(Type concreteType, Type interfaceType) : base(concreteType, interfaceType) { }
    public ClassValueResolver(object instance, Type interfaceType) : base(instance, interfaceType) { }
    public Expression GetExpression(IGlobalConfiguration configuration, MemberMap memberMap, Expression source, Expression destination, Expression destinationMember)
    {
        var typeMap = memberMap.TypeMap;
        var resolverInstance = _instance ?? ServiceLocator(typeMap.MakeGenericType(ConcreteType));
        Expression sourceMember;
        if (SourceMemberLambda == null)
        {
            sourceMember = SourceMemberName == null ? null : PropertyOrField(source, SourceMemberName);
        }
        else
        {
            sourceMember = configuration.ReplaceParameters(SourceMemberLambda, source);
        }
        var iValueResolver = InterfaceType;
        if (iValueResolver.ContainsGenericParameters)
        {
            var typeArgs =
                iValueResolver.GenericTypeArguments.Zip(new[] { typeMap.SourceType, typeMap.DestinationType, sourceMember?.Type, destinationMember.Type }.Where(t => t != null),
                    (declaredType, runtimeType) => declaredType.ContainsGenericParameters ? runtimeType : declaredType).ToArray();
            iValueResolver = iValueResolver.GetGenericTypeDefinition().MakeGenericType(typeArgs);
        }
        var parameters = new[] { source, destination, sourceMember, destinationMember }.Where(p => p != null)
            .Zip(iValueResolver.GenericTypeArguments, ToType)
            .Append(ContextParameter)
            .ToArray();
        return Call(ToType(resolverInstance, iValueResolver), "Resolve", parameters);
    }
    public MemberInfo GetSourceMember(MemberMap _) => SourceMemberLambda?.GetMember();
}
public abstract class TypeConverter
{
    public abstract Expression GetExpression(IGlobalConfiguration configuration, ParameterExpression[] parameters);
    public virtual void CloseGenerics(TypeMapConfiguration openMapConfig, TypePair closedTypes) { }
    public virtual LambdaExpression ProjectToExpression => null;
}
public class LambdaTypeConverter : TypeConverter
{
    public LambdaTypeConverter(LambdaExpression lambda) => Lambda = lambda;
    public LambdaExpression Lambda { get; }
    public override Expression GetExpression(IGlobalConfiguration configuration, ParameterExpression[] parameters) => 
        configuration.ConvertReplaceParameters(Lambda, parameters);
}
public class ExpressionTypeConverter : LambdaTypeConverter
{
    public ExpressionTypeConverter(LambdaExpression lambda) : base(lambda){}
    public override LambdaExpression ProjectToExpression => Lambda;
}
public class ClassTypeConverter : TypeConverter
{
    public ClassTypeConverter(Type converterType, Type converterInterface)
    {
        ConverterType = converterType;
        ConverterInterface = converterInterface;
    }
    public Type ConverterType { get; private set; }
    public Type ConverterInterface { get; }
    public override Expression GetExpression(IGlobalConfiguration configuration, ParameterExpression[] parameters) =>
        Call(ToType(ServiceLocator(ConverterType), ConverterInterface), "Convert", parameters);
    public override void CloseGenerics(TypeMapConfiguration openMapConfig, TypePair closedTypes)
    {
        var typeParams = (openMapConfig.SourceType.IsGenericTypeDefinition ? closedTypes.SourceType.GenericTypeArguments : Type.EmptyTypes)
            .Concat(openMapConfig.DestinationType.IsGenericTypeDefinition ? closedTypes.DestinationType.GenericTypeArguments : Type.EmptyTypes);
        var neededParameters = ConverterType.GenericParametersCount();
        ConverterType = ConverterType.MakeGenericType(typeParams.Take(neededParameters).ToArray());
    }
}