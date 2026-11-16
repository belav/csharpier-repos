//
// Tests for System.Windows.Threading.DispatcherTimer
//
// Author:
//    Konrad M. Kruczynski
//

using System;
using System.Windows.Threading;
using NUnit.Framework;

namespace MonoTests.System.Windows.Threading
{
    [TestFixture]
    public class DispatcherTimerTest
    {
        [Test]
        public void IntervalSettingTest()
        {
            var timer = new DispatcherTimer();
            var second = new TimeSpan(0, 0, 0, 1);
            timer.Interval = second;
            var timerInterval = timer.Interval;
            Assert.AreEqual(second, timerInterval, "Interval given to timer was not preserved.");
        }
    }
}
