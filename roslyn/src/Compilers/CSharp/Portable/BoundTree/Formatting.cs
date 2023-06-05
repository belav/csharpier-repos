// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.CSharp
{
    partial internal abstract class BoundExpression
    {
        /// <summary>
        /// Returns a serializable object that is used for displaying this expression in a diagnostic message.
        /// </summary>
        public virtual object Display
        {
            get
            {
                Debug.Assert(this.Type is { }, $"Unexpected null type in {this.GetType().Name}");
                return this.Type;
            }
        }
    }

    partial internal sealed class BoundArgListOperator
    {
        public override object Display
        {
            get { return "__arglist"; }
        }
    }

    partial internal sealed class BoundLiteral
    {
        public override object Display
        {
            get
            {
                return ConstantValueOpt?.IsNull == true
                    ? MessageID.IDS_NULL.Localize()
                    : base.Display;
            }
        }
    }

    partial internal sealed class BoundLambda
    {
        public override object Display
        {
            get { return this.MessageID.Localize(); }
        }
    }

    partial internal sealed class UnboundLambda
    {
        public override object Display
        {
            get { return this.MessageID.Localize(); }
        }
    }

    partial internal sealed class BoundMethodGroup
    {
        public override object Display
        {
            get { return MessageID.IDS_MethodGroup.Localize(); }
        }
    }

    partial internal sealed class BoundThrowExpression
    {
        public override object Display
        {
            get { return MessageID.IDS_ThrowExpression.Localize(); }
        }
    }

    partial internal class BoundTupleExpression
    {
        public override object Display
        {
            get
            {
                var pooledBuilder = PooledStringBuilder.GetInstance();
                var builder = pooledBuilder.Builder;
                var arguments = this.Arguments;
                var argumentDisplays = new object[arguments.Length];

                builder.Append('(');
                builder.Append("{0}");
                argumentDisplays[0] = arguments[0].Display;

                for (int i = 1; i < arguments.Length; i++)
                {
                    builder.Append(", {" + i + "}");
                    argumentDisplays[i] = arguments[i].Display;
                }

                builder.Append(')');

                var format = pooledBuilder.ToStringAndFree();
                return FormattableStringFactory.Create(format, argumentDisplays);
            }
        }
    }

    partial internal sealed class BoundPropertyGroup
    {
        public override object Display
        {
            get { throw ExceptionUtilities.Unreachable(); }
        }
    }

    partial internal class OutVariablePendingInference
    {
        public override object Display
        {
            get { return string.Empty; }
        }
    }

    partial internal class OutDeconstructVarPendingInference
    {
        public override object Display
        {
            get { return string.Empty; }
        }
    }

    partial internal class BoundDiscardExpression
    {
        public override object Display
        {
            get { return (object?)this.Type ?? "_"; }
        }
    }

    partial internal class DeconstructionVariablePendingInference
    {
        public override object Display
        {
            get { throw ExceptionUtilities.Unreachable(); }
        }
    }

    partial internal class BoundDefaultLiteral
    {
        public override object Display
        {
            get { return (object?)this.Type ?? "default"; }
        }
    }

    partial internal class BoundStackAllocArrayCreation
    {
        public override object Display =>
            (Type is null)
                ? FormattableStringFactory.Create(
                    "stackalloc {0}[{1}]",
                    ElementType,
                    Count.WasCompilerGenerated ? null : Count.Syntax.ToString()
                )
                : base.Display;
    }

    partial internal class BoundUnconvertedSwitchExpression
    {
        public override object Display =>
            (Type is null) ? MessageID.IDS_FeatureSwitchExpression.Localize() : base.Display;
    }

    partial internal class BoundUnconvertedConditionalOperator
    {
        public override object Display =>
            (Type is null) ? MessageID.IDS_FeatureTargetTypedConditional.Localize() : base.Display;
    }

    partial internal class BoundPassByCopy
    {
        public override object Display => Expression.Display;
    }

    partial internal class BoundUnconvertedAddressOfOperator
    {
        public override object Display => FormattableStringFactory.Create("&{0}", Operand.Display);
    }

    partial internal class BoundUnconvertedObjectCreationExpression
    {
        public override object Display
        {
            get
            {
                var arguments = this.Arguments;
                if (arguments.Length == 0)
                {
                    return "new()";
                }

                var pooledBuilder = PooledStringBuilder.GetInstance();
                var builder = pooledBuilder.Builder;
                var argumentDisplays = new object[arguments.Length];

                builder.Append("new");
                builder.Append('(');
                builder.Append("{0}");
                argumentDisplays[0] = arguments[0].Display;

                for (int i = 1; i < arguments.Length; i++)
                {
                    builder.Append($", {{{i.ToString()}}}");
                    argumentDisplays[i] = arguments[i].Display;
                }

                builder.Append(')');

                var format = pooledBuilder.ToStringAndFree();
                return FormattableStringFactory.Create(format, argumentDisplays);
            }
        }
    }
}
