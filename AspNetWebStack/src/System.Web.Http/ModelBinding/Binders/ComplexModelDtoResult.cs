// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Web.Http.Validation;

namespace System.Web.Http.ModelBinding.Binders
{
    public sealed class ComplexModelDtoResult
    {
        public ComplexModelDtoResult(object model, ModelValidationNode validationNode)
        {
            if (validationNode == null)
            {
                throw Error.ArgumentNull("validationNode");
            }

            Model = model;
            ValidationNode = validationNode;
        }

        public object Model { get; private set; }

        public ModelValidationNode ValidationNode { get; private set; }
    }
}
