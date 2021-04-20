// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;

using Moq.Protected;

using Xunit;

namespace Moq.Tests
{
	public class ProtectedAsMockFixture
	{
		private Mock<Foo> mock;
		private IProtectedAsMock<Foo, Fooish> protectedMock;

		public ProtectedAsMockFixture()
		{
			this.mock = new Mock<Foo>();
			this.protectedMock = this.mock.Protected().As<Fooish>();
		}

		[Fact]
		public void Setup_throws_when_expression_null()
		{
			var actual = Record.Exception(() =>
			{
				this.protectedMock.Setup(null);
			});

			Assert.IsType<ArgumentNullException>(actual);
		}

		[Fact]
		public void Setup_throws_ArgumentException_when_expression_contains_nonexistent_method()
		{
			var actual = Record.Exception(() =>
			{
				this.protectedMock.Setup(m => m.NonExistentMethod());
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Contains("does not have matching protected member", actual.Message);
		}

		[Fact]
		public void Setup_throws_ArgumentException_when_expression_contains_nonexistent_property()
		{
			var actual = Record.Exception(() =>
			{
				this.protectedMock.Setup(m => m.NonExistentProperty);
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Contains("does not have matching protected member", actual.Message);
		}

		[Fact]
		public void Setup_TResult_throws_when_expression_null()
		{
			var actual = Record.Exception(() => this.protectedMock.Setup<object>(null));

			Assert.IsType<ArgumentNullException>(actual);
		}

		[Fact]
		public void Setup_can_setup_simple_method()
		{
			bool doSomethingImplInvoked = false;
			this.protectedMock.Setup(m => m.DoSomethingImpl()).Callback(() => doSomethingImplInvoked = true);

			this.mock.Object.DoSomething();

			Assert.True(doSomethingImplInvoked);
		}

		[Fact]
		public void Setup_TResult_can_setup_simple_method_with_return_value()
		{
			this.protectedMock.Setup(m => m.GetSomethingImpl()).Returns(() => 42);

			var actual = this.mock.Object.GetSomething();

			Assert.Equal(42, actual);
		}

		[Fact]
		public void Setup_can_match_exact_arguments()
		{
			bool doSomethingImplInvoked = false;
			this.protectedMock.Setup(m => m.DoSomethingImpl(1)).Callback(() => doSomethingImplInvoked = true);

			this.mock.Object.DoSomething(0);
			Assert.False(doSomethingImplInvoked);

			this.mock.Object.DoSomething(1);
			Assert.True(doSomethingImplInvoked);
		}

		[Fact]
		public void Setup_can_involve_matchers()
		{
			bool doSomethingImplInvoked = false;
			this.protectedMock.Setup(m => m.DoSomethingImpl(It.Is<int>(i => i == 1))).Callback(() => doSomethingImplInvoked = true);

			this.mock.Object.DoSomething(0);
			Assert.False(doSomethingImplInvoked);

			this.mock.Object.DoSomething(1);
			Assert.True(doSomethingImplInvoked);
		}

		[Fact]
		public void Setup_can_setup_generic_method()
		{
			var handledMessages = new List<string>();

			var mock = new Mock<MessageHandlerBase>();
			mock.Protected().As<MessageHandlerBaseish>()
				.Setup(m => m.HandleImpl(It.IsAny<string>()))
				.Callback((string message) =>
				{
					handledMessages.Add(message);
				});

			mock.Object.Handle("Hello world.", 3);

			Assert.Contains("Hello world.", handledMessages);
			Assert.Equal(3, handledMessages.Count);
		}

		[Fact]
		public void SetupGet_can_setup_readonly_property()
		{
			this.protectedMock.SetupGet(m => m.ReadOnlyPropertyImpl).Returns(42);

			var actual = this.mock.Object.ReadOnlyProperty;

			Assert.Equal(42, actual);
		}

		[Fact]
		public void SetupGet_can_setup_readwrite_property()
		{
			this.protectedMock.SetupGet(m => m.ReadWritePropertyImpl).Returns(42);

			var actual = this.mock.Object.ReadWriteProperty;

			Assert.Equal(42, actual);
		}

		[Fact]
		public void SetupProperty_can_setup_readwrite_property()
		{
			this.protectedMock.SetupProperty(m => m.ReadWritePropertyImpl, 42);

			var actualBeforeSetting = this.mock.Object.ReadWriteProperty;
			this.mock.Object.ReadWriteProperty = 17;
			var actualAfterSetting = this.mock.Object.ReadWriteProperty;

			Assert.Equal(42, actualBeforeSetting);
			Assert.Equal(17, actualAfterSetting);
		}

		[Fact]
		public void SetupProperty_cannot_setup_readonly_property()
		{
			var exception = Record.Exception(() =>
			{
				this.protectedMock.SetupProperty(m => m.ReadOnlyPropertyImpl);
			});

			Assert.NotNull(exception);
		}

		[Fact]
		public void SetupSequence_TResult_can_setup_property()
		{
			this.protectedMock.SetupSequence(m => m.ReadOnlyPropertyImpl)
				.Returns(1)
				.Throws(new InvalidOperationException())
				.Returns(3);

			var actual = new List<int>();
			actual.Add(this.mock.Object.ReadOnlyProperty);
			var exception = Record.Exception(() =>
			{
				actual.Add(this.mock.Object.ReadOnlyProperty);
			});
			actual.Add(this.mock.Object.ReadOnlyProperty);

			Assert.Equal(new[] { 1, 3 }, actual);
			Assert.IsType<InvalidOperationException>(exception);
		}

		[Fact]
		public void SetupSequence_can_setup_actions()
		{
			this.protectedMock.SetupSequence(m => m.DoSomethingImpl())
				.Pass()
				.Pass().
				Throws(new InvalidOperationException());

			this.mock.Object.DoSomething();
			this.mock.Object.DoSomething();
			var exception = Record.Exception(() =>
			{
				this.mock.Object.DoSomething();
			});
			this.mock.Object.DoSomething();

			Assert.IsType<InvalidOperationException>(exception);
		}

		[Fact]
		public void Verify_can_verify_method_invocations()
		{
			this.mock.Object.DoSomething();

			this.protectedMock.Verify(m => m.DoSomethingImpl());
		}

		[Fact]
		public void Verify_throws_if_invocation_not_occurred()
		{
			var exception = Record.Exception(() =>
			{
				this.protectedMock.Verify(m => m.DoSomethingImpl());
			});

			Assert.IsType<MockException>(exception);
		}

		[Fact]
		public void Verify_can_verify_method_invocations_times()
		{
			this.mock.Object.DoSomething();
			this.mock.Object.DoSomething();
			this.mock.Object.DoSomething();

			this.protectedMock.Verify(m => m.DoSomethingImpl(), Times.Exactly(3));
		}

		[Fact]
		public void Verify_includes_failure_message_in_exception()
		{
			this.mock.Object.DoSomething();
			this.mock.Object.DoSomething();

			var exception = Record.Exception(() =>
			{
				this.protectedMock.Verify(m => m.DoSomethingImpl(), Times.Exactly(3), "Wasn't called three times.");
			});

			Assert.IsType<MockException>(exception);
			Assert.Contains("Wasn't called three times.", exception.Message);
		}

		[Fact]
		public void Verify_can_verify_generic_method()
		{
			var mock = new Mock<MessageHandlerBase>();

			mock.Object.Handle("Hello world.", 3);

			mock.Protected().As<MessageHandlerBaseish>().Verify(m => m.HandleImpl("Hello world."), Times.Exactly(3));
		}

		[Fact]
		public void VerifyGet_can_verify_properties()
		{
			var _ = this.mock.Object.ReadOnlyProperty;

			this.protectedMock.VerifyGet(m => m.ReadOnlyPropertyImpl);
		}

		[Fact]
		public void VerifyGet_throws_if_property_query_not_occurred()
		{
			var _ = this.mock.Object.ReadOnlyProperty;

			var exception = Record.Exception(() =>
			{
				this.protectedMock.VerifyGet(m => m.ReadOnlyPropertyImpl, Times.AtLeast(2));
			});
		}

		[Fact]
		public void VerifyGet_includes_failure_message_in_exception()
		{
			var exception = Record.Exception(() =>
			{
				this.protectedMock.VerifyGet(m => m.ReadOnlyPropertyImpl, Times.Once(), "Was not queried.");
			});

			Assert.IsType<MockException>(exception);
			Assert.Contains("Was not queried.", exception.Message);
		}

		public abstract class Foo
		{
			protected Foo()
			{
			}

			public int ReadOnlyProperty => this.ReadOnlyPropertyImpl;

			public int ReadWriteProperty
			{
				get => this.ReadWritePropertyImpl;
				set => this.ReadWritePropertyImpl = value;
			}

			public void DoSomething()
			{
				this.DoSomethingImpl();
			}

			public void DoSomething(int arg)
			{
				this.DoSomethingImpl(arg);
			}

			public int GetSomething()
			{
				return this.GetSomethingImpl();
			}

			protected abstract int ReadOnlyPropertyImpl { get; }

			protected abstract int ReadWritePropertyImpl { get; set; }

			protected abstract void DoSomethingImpl();

			protected abstract void DoSomethingImpl(int arg);

			protected abstract int GetSomethingImpl();
		}

		public interface Fooish
		{
			int ReadOnlyPropertyImpl { get; }
			int ReadWritePropertyImpl { get; set; }
			int NonExistentProperty { get; }
			void DoSomethingImpl();
			void DoSomethingImpl(int arg);
			int GetSomethingImpl();
			void NonExistentMethod();
		}

		public abstract class MessageHandlerBase
		{
			public void Handle<TMessage>(TMessage message, int count)
			{
				for (int i = 0; i < count; ++i)
				{
					this.HandleImpl(message);
				}
			}

			protected abstract void HandleImpl<TMessage>(TMessage message);
		}

		public interface MessageHandlerBaseish
		{
			void HandleImpl<TMessage>(TMessage message);
		}
	}
}
