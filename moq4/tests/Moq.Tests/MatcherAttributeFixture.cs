// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	[Obsolete("This fixture contains tests related to `" + nameof(MatcherAttribute) + "`, which is obsolete.")]
	public class MatcherAttributeFixture
	{
		public interface IFoo
		{
			void Bar(string p);
		}

		[Fact]
		public void ShouldFindGenericMethodMatcher()
		{
			var foo = new Mock<IFoo>();

			foo.Object.Bar("asdf");

			foo.Verify(f => f.Bar(Any<string>()));
		}

		[Matcher]
		public T Any<T>()
		{
			return default(T);
		}

		public bool Any<T>(T value)
		{
			return true;
		}

		[Fact]
		public void ShouldNotFindPrivateMethodMatcher()
		{
			var foo = new Mock<IFoo>();

			foo.Object.Bar("asd");

			Assert.Throws<MissingMethodException>(() => foo.Verify(f => f.Bar(OddLength())));
		}

		[Matcher]
		private static string OddLength()
		{
			return default(string);
		}

		private static bool OddLength(string value)
		{
			return value.Length % 2 == 0;
		}

		[Fact]
		public void ShouldTranslateToUseMatcherImplementation()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(IsMagicString()));
			IsMagicStringCalled = false;
			mock.Object.Bar("magic");
			Assert.True(IsMagicStringCalled);
		}

		[Fact]
		//[ExpectedException] not used so IsMagicStringCalled can be verified
		public void ShouldTranslateToUseMatcherImplementation2()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(IsMagicString()));
			IsMagicStringCalled = false;
			Exception expectedException = null;
			try
			{
				mock.Object.Bar("no-magic");
			}
			catch (Exception e)
			{
				expectedException = e;
			}

			Assert.True(IsMagicStringCalled);
			Assert.NotNull(expectedException);
		}

		private static bool IsMagicStringCalled;

		[Matcher]
		public static string IsMagicString()
		{
			return null;
		}

		public static bool IsMagicString(string arg)
		{
			IsMagicStringCalled = true;
			return arg == "magic";
		}

		[Fact]
		public void ShouldUseAditionalArguments()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(StartsWith("ma")));
			mock.Object.Bar("magic");
		}

		[Fact]
		public void ShouldUseAditionalArguments2()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(StartsWith("ma")));
			Assert.Throws<MockException>(() => mock.Object.Bar("no-magic"));
		}

		[Matcher]
		public static string StartsWith(string prefix)
		{
			return null;
		}

		public static bool StartsWith(string arg, string prefix)
		{
			return arg.StartsWith(prefix);
		}

		[Fact]
		public void ExpectMissingMatcherMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			Assert.Throws<MissingMethodException>(() => mock.Setup(x => x.Bar(MatcherHookWithoutMatcherMethod())));
		}

		[Matcher]
		public static string MatcherHookWithoutMatcherMethod()
		{
			return null;
		}

		[Fact]
		public void ExpectMissingMatcherWithArgsMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			Assert.Throws<MissingMethodException>(() => mock.Setup(x => x.Bar(MatcherHook2WithoutMatcherMethod(6))));
		}

		[Matcher]
		public static string MatcherHook2WithoutMatcherMethod(int a)
		{
			return null;
		}

		[Fact]
		public void UseCurrentInstanceAsContext()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(NonStaticMatcherHook()));
			NonStaticMatcherHookExpectedArg = "Do It";

			mock.Object.Bar("Do It");
		}

		[Matcher]
		public string NonStaticMatcherHook()
		{
			return null;
		}

		public bool NonStaticMatcherHook(string arg)
		{
			return arg == NonStaticMatcherHookExpectedArg;
		}

		private string NonStaticMatcherHookExpectedArg;

		[Fact]
		public void ExpectMissingNonStaticMatcherMethod()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			Assert.Throws<MissingMethodException>(() => mock.Setup(x => x.Bar(NonStaticMatcherHookWithoutMatcherMethod())));
		}

		[Matcher]
		public string NonStaticMatcherHookWithoutMatcherMethod()
		{
			return null;
		}

		[Fact]
		public void AllowStaticMethodsInHelperClassAsMatcherHook()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(x => x.Bar(A.NotNull()));
			mock.Object.Bar("a");
		}

		public static class A
		{
			[Matcher]
			public static string NotNull()
			{
				return null;
			}

			public static bool NotNull(string arg)
			{
				return arg != null;
			}
		}

		[Fact]
		public void AllowHelperClassInstance()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			var b = new B();
			mock.Setup(x => x.Bar(b.NotNull()));
			mock.Object.Bar("a");
		}

		public class B
		{
			[Matcher]
			public string NotNull()
			{
				return null;
			}

			public bool NotNull(string arg)
			{
				return arg != null;
			}
		}
	}
}
