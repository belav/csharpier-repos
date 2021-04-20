// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class OutRefFixture
	{
		[Fact]
		public void ExpectsOutArgument()
		{
			var mock = new Mock<IFoo>();
			var expected = "ack";

			mock.Setup(m => m.Execute("ping", out expected)).Returns(true);

			string actual;
			var ok = mock.Object.Execute("ping", out actual);

			Assert.True(ok);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ExpectsOutEagerlyEvaluates()
		{
			var mock = new Mock<IFoo>();
			string expected = "ack";

			mock.Setup(m => m.Execute("ping", out expected)).Returns(true);

			expected = "foo";

			string actual;
			bool ok = mock.Object.Execute("ping", out actual);

			Assert.True(ok);
			Assert.Equal("ack", actual);
		}

		[Fact]
		public void ExpectsRefArgument()
		{
			var mock = new Mock<IFoo>();
			string expected = "ack";

			mock.Setup(m => m.Echo(ref expected)).Returns<string>(s => s);

			string actual = mock.Object.Echo(ref expected);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void RefOnlyMatchesSameInstance()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			string expected = "ack";

			mock.Setup(m => m.Echo(ref expected)).Returns<string>(s => s);

			string actual = null;
			Assert.Throws<MockException>(() => mock.Object.Echo(ref actual));
		}

		[Fact]
		public void RefTakesGuidParameter()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			var expected = Guid.NewGuid();

			mock.Setup(m => m.GuidMethod(ref expected)).Returns(true);

			Assert.True(mock.Object.GuidMethod(ref expected));
		}

		[Fact]
		public void RefWorksWithOtherValueTypes()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			var expected = 5;

			mock.Setup(m => m.IntMethod(ref expected)).Returns(true);

			Assert.True(mock.Object.IntMethod(ref expected));
		}

		[Fact]
		public void SetupSequence_setups_assign_out_parameters()
		{
			const string expected = "expected";

			var valueForActual = expected;
			var mock = new Mock<IFoo>();
			mock.SetupSequence(m => m.Execute(It.IsAny<string>(), out valueForActual));

			mock.Object.Execute(default, out var actual);

			Assert.Equal(expected, actual);
		}

		// ThrowsIfOutIsNotConstant
		// ThrowsIfRefIsNotConstant

		public interface IFoo
		{
			T Echo<T>(ref T value);
			bool Execute(string command, out string result);
			void Submit(string command, ref string result);
			int Value { get; set; }
			bool GuidMethod(ref Guid guid);
			bool IntMethod(ref int value);
		}
	}
}
