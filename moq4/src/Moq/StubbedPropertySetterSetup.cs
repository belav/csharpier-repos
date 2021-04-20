// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   Setup used by <see cref="Mock.SetupAllProperties(Mock)"/> for property setters.
	/// </summary>
	internal sealed class StubbedPropertySetterSetup : Setup
	{
		private Action<object> setter;

		public StubbedPropertySetterSetup(Mock mock, LambdaExpression originalExpression, MethodInfo method, Action<object> setter)
			: base(originalExpression: null, mock, new InvocationShape(originalExpression, method, new Expression[] { It.IsAny(method.GetParameterTypes().Last()) }))
		{
			this.setter = setter;

			this.MarkAsVerifiable();
		}

		protected override void ExecuteCore(Invocation invocation)
		{
			this.setter.Invoke(invocation.Arguments[0]);
		}

		protected override void VerifySelf()
		{
		}
	}
}
