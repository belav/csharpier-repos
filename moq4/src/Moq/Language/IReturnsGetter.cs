// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Returns</c> verb for property get setups.
	/// </summary>
	/// <typeparam name="TMock">Mocked type.</typeparam>
	/// <typeparam name="TProperty">Type of the property.</typeparam>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturnsGetter<TMock, TProperty> : IFluentInterface
		where TMock : class
	{
		/// <summary>
		/// Specifies the value to return.
		/// </summary>
		/// <param name="value">The value to return, or <see langword="null"/>.</param>
		/// <example>
		/// Return a <c>true</c> value from the property getter call:
		/// <code>
		/// mock.SetupGet(x => x.Suspended)
		///     .Returns(true);
		/// </code>
		/// </example>
		IReturnsResult<TMock> Returns(TProperty value);

		/// <summary>
		/// Specifies a function that will calculate the value to return for the property.
		/// </summary>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		/// <example>
		/// Return a calculated value when the property is retrieved:
		/// <code>
		/// mock.SetupGet(x => x.Suspended)
		///     .Returns(() => returnValues[0]);
		/// </code>
		/// The lambda expression to retrieve the return value is lazy-executed, 
		/// meaning that its value may change depending on the moment the property  
		/// is retrieved and the value the <c>returnValues</c> array has at 
		/// that moment.
		/// </example>
		IReturnsResult<TMock> Returns(Func<TProperty> valueFunction);

		/// <summary>
		/// Calls the real property of the object and returns its return value.
		/// </summary>
		/// <returns>The value calculated by the real property of the object.</returns>
		IReturnsResult<TMock> CallBase();
	}
}
