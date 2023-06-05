// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Operations;

namespace Microsoft.CodeAnalysis.CSharp
{
    partial internal class BoundObjectCreationExpression : IBoundInvalidNode
    {
        internal static ImmutableArray<BoundExpression> GetChildInitializers(
            BoundExpression? objectOrCollectionInitializer
        )
        {
            var objectInitializerExpression =
                objectOrCollectionInitializer as BoundObjectInitializerExpression;
            if (objectInitializerExpression != null)
            {
                return objectInitializerExpression.Initializers;
            }

            var collectionInitializerExpression =
                objectOrCollectionInitializer as BoundCollectionInitializerExpression;
            if (collectionInitializerExpression != null)
            {
                return collectionInitializerExpression.Initializers;
            }

            return ImmutableArray<BoundExpression>.Empty;
        }

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren =>
            CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(
                receiverOpt: null,
                Arguments,
                InitializerExpressionOpt
            );
    }

    partial internal sealed class BoundObjectInitializerMember : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren =>
            StaticCast<BoundNode>.From(Arguments);
    }

    partial internal sealed class BoundCollectionElementInitializer : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren =>
            CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(
                ImplicitReceiverOpt,
                Arguments
            );
    }

    partial internal sealed class BoundDeconstructionAssignmentOperator : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Left, this.Right);
    }

    partial internal class BoundBadExpression : IBoundInvalidNode
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(this.ChildBoundNodes);

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren =>
            StaticCast<BoundNode>.From(this.ChildBoundNodes);
    }

    partial internal class BoundCall : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren =>
            CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(
                ReceiverOpt,
                Arguments
            );
    }

    partial internal class BoundIndexerAccess : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren =>
            CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(
                ReceiverOpt,
                Arguments
            );
    }

    partial internal class BoundDynamicIndexerAccess
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(this.Arguments.Insert(0, this.Receiver));
    }

    partial internal class BoundAnonymousObjectCreationExpression
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(this.Arguments);
    }

    partial internal class BoundAttribute
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(
                this.ConstructorArguments.AddRange(
                    StaticCast<BoundExpression>.From(this.NamedArguments)
                )
            );
    }

    partial internal class BoundQueryClause
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Value);
    }

    partial internal class BoundArgListOperator
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(this.Arguments);
    }

    partial internal class BoundNameOfOperator
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Argument);
    }

    partial internal class BoundPointerElementAccess
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Expression, this.Index);
    }

    partial internal class BoundRefTypeOperator
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Operand);
    }

    partial internal class BoundDynamicMemberAccess
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Receiver);
    }

    partial internal class BoundMakeRefOperator
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Operand);
    }

    partial internal class BoundRefValueOperator
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Operand);
    }

    partial internal class BoundDynamicInvocation
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(this.Arguments.Insert(0, this.Expression));
    }

    partial internal class BoundFixedLocalCollectionInitializer
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Expression);
    }

    partial internal class BoundStackAllocArrayCreationBase
    {
        internal static ImmutableArray<BoundExpression> GetChildInitializers(
            BoundArrayInitialization? arrayInitializer
        )
        {
            return arrayInitializer?.Initializers ?? ImmutableArray<BoundExpression>.Empty;
        }
    }

    partial internal class BoundStackAllocArrayCreation
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(
                GetChildInitializers(this.InitializerOpt).Insert(0, this.Count)
            );
    }

    partial internal class BoundConvertedStackAllocExpression
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(
                GetChildInitializers(this.InitializerOpt).Insert(0, this.Count)
            );
    }

    partial internal class BoundDynamicObjectCreationExpression
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(
                this.Arguments.AddRange(
                    BoundObjectCreationExpression.GetChildInitializers(
                        this.InitializerExpressionOpt
                    )
                )
            );
    }

    partial class BoundThrowExpression
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Expression);
    }

    partial internal abstract class BoundMethodOrPropertyGroup
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.ReceiverOpt);
    }

    partial internal class BoundSequence
    {
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(this.SideEffects.Add(this.Value));
    }

    partial internal class BoundStatementList
    {
        protected override ImmutableArray<BoundNode?> Children =>
            (this.Kind == BoundKind.StatementList || this.Kind == BoundKind.Scope)
                ? StaticCast<BoundNode?>.From(this.Statements)
                : ImmutableArray<BoundNode?>.Empty;
    }

    partial internal class BoundPassByCopy
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Expression);
    }

    partial internal class BoundImplicitIndexerAccess
    {
        protected override ImmutableArray<BoundNode?> Children =>
            ImmutableArray.Create<BoundNode?>(this.Receiver, Argument);
    }

    partial internal class BoundFunctionPointerInvocation : IBoundInvalidNode
    {
        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren =>
            CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(
                receiverOpt: this.InvokedExpression,
                Arguments
            );
        protected override ImmutableArray<BoundNode?> Children =>
            StaticCast<BoundNode?>.From(((IBoundInvalidNode)this).InvalidNodeChildren);
    }
}
