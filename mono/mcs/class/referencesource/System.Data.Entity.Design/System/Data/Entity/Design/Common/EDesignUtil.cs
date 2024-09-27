//------------------------------------------------------------------------------
// <copyright file="EDesignUtil.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// @owner       Microsoft
// @backupOwner Microsoft
//------------------------------------------------------------------------------

namespace System.Data.Entity.Design.Common
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;

    internal static class EDesignUtil
    {
        ////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        //
        // Helper Functions
        //
        internal static string GetMessagesFromEntireExceptionChain(Exception e)
        {
            // get the full error message list from the inner exceptions
            string message = e.Message;
            int count = 0;
            for (Exception inner = e.InnerException; inner != null; inner = inner.InnerException)
            {
                count++;
                string indent = string.Empty.PadLeft(count, '\t');
                message += Environment.NewLine + indent;
                message += inner.Message;
            }
            return message;
        }

        internal static T CheckArgumentNull<T>(T value, string parameterName)
            where T : class
        {
            if (null == value)
            {
                throw ArgumentNull(parameterName);
            }
            return value;
        }

        internal static void CheckStringArgument(string value, string parameterName)
        {
            // Throw ArgumentNullException when string is null
            CheckArgumentNull(value, parameterName);

            // Throw ArgumentException when string is empty
            if (value.Length == 0)
            {
                throw InvalidStringArgument(parameterName);
            }
        }

        internal static LanguageOption CheckLanguageOptionArgument(
            LanguageOption value,
            string paramName
        )
        {
            if (
                value == LanguageOption.GenerateCSharpCode
                || value == LanguageOption.GenerateVBCode
            )
            {
                return value;
            }
            throw ArgumentOutOfRange(paramName);
        }

        internal static ArgumentException SingleStoreEntityContainerExpected(string parameterName)
        {
            ArgumentException e = new ArgumentException(
                Strings.SingleStoreEntityContainerExpected,
                parameterName
            );
            return e;
        }

        internal static ArgumentException InvalidStoreEntityContainer(
            string entityContainerName,
            string parameterName
        )
        {
            ArgumentException e = new ArgumentException(
                Strings.InvalidNonStoreEntityContainer(entityContainerName),
                parameterName
            );
            return e;
        }

        internal static ArgumentException InvalidStringArgument(string parameterName)
        {
            ArgumentException e = new ArgumentException(
                Strings.InvalidStringArgument(parameterName)
            );
            return e;
        }

        internal static ArgumentException EdmReservedNamespace(string namespaceName)
        {
            ArgumentException e = new ArgumentException(Strings.ReservedNamespace(namespaceName));
            return e;
        }

        internal static ArgumentNullException ArgumentNull(string parameter)
        {
            ArgumentNullException e = new ArgumentNullException(parameter);
            return e;
        }

        internal static ArgumentException Argument(string parameter)
        {
            ArgumentException e = new ArgumentException(parameter);
            return e;
        }

        internal static ArgumentException Argument(string message, Exception inner)
        {
            ArgumentException e = new ArgumentException(message, inner);
            return e;
        }

        internal static InvalidOperationException InvalidOperation(string error)
        {
            InvalidOperationException e = new InvalidOperationException(error);
            return e;
        }

        // SSDL Generator
        static internal StrongTypingException StrongTyping(string error, Exception innerException)
        {
            StrongTypingException e = new StrongTypingException(error, innerException);
            return e;
        }

        internal static StrongTypingException StonglyTypedAccessToNullValue(
            string columnName,
            string tableName,
            Exception innerException
        )
        {
            return StrongTyping(
                Strings.StonglyTypedAccessToNullValue(columnName, tableName),
                innerException
            );
        }

        internal static InvalidOperationException EntityStoreGeneratorSchemaNotLoaded()
        {
            return InvalidOperation(Strings.EntityStoreGeneratorSchemaNotLoaded);
        }

        internal static InvalidOperationException EntityModelGeneratorSchemaNotLoaded()
        {
            return InvalidOperation(Strings.EntityModelGeneratorSchemaNotLoaded);
        }

        internal static InvalidOperationException NonSerializableType(BuiltInTypeKind kind)
        {
            return InvalidOperation(Strings.Serialization_UnknownGlobalItem(kind));
        }

        internal static InvalidOperationException MissingGenerationPatternForType(
            BuiltInTypeKind kind
        )
        {
            return InvalidOperation(Strings.ModelGeneration_UnGeneratableType(kind));
        }

        internal static ArgumentException InvalidNamespaceNameArgument(string namespaceName)
        {
            return new ArgumentException(Strings.InvalidNamespaceNameArgument(namespaceName));
        }

        internal static ArgumentException InvalidEntityContainerNameArgument(
            string entityContainerName
        )
        {
            return new ArgumentException(
                Strings.InvalidEntityContainerNameArgument(entityContainerName)
            );
        }

        internal static ArgumentException DuplicateEntityContainerName(
            string newModelEntityContainerName,
            string storeEntityContainer
        )
        {
            return new ArgumentException(
                Strings.DuplicateEntityContainerName(
                    newModelEntityContainerName,
                    storeEntityContainer
                )
            );
        }

        internal static ProviderIncompatibleException ProviderIncompatible(string message)
        {
            return new ProviderIncompatibleException(message);
        }

        internal static ProviderIncompatibleException ProviderIncompatible(
            string message,
            Exception inner
        )
        {
            return new ProviderIncompatibleException(message, inner);
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName);
        }

        internal static void CheckTargetEntityFrameworkVersionArgument(
            Version targetEntityFrameworkVersion,
            string parameterName
        )
        {
            EDesignUtil.CheckArgumentNull(targetEntityFrameworkVersion, parameterName);
            if (!EntityFrameworkVersions.IsValidVersion(targetEntityFrameworkVersion))
            {
                throw EDesignUtil.Argument(parameterName);
            }
        }
    }
}
