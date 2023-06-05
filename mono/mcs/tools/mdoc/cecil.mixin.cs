using System;
using System.Collections.Generic;
using System.Linq;

namespace Mono.Cecil
{
    partial static class Mixin
    {
        public static bool IsTypeSpecification(this TypeReference type)
        {
            return type is GenericParameter || type is TypeSpecification;
        }

        public static IEnumerable<MethodDefinition> GetConstructors(this TypeDefinition type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.HasMethods)
                return Array.Empty<MethodDefinition>();

            return type.Methods.Where(method => method.IsConstructor);
        }
    }
}
