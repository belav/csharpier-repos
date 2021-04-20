using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    public class DateTimeModelBinderProviderTest
    {
        private readonly DateTimeModelBinderProvider _provider = new DateTimeModelBinderProvider();

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(DateTimeOffset))]
        [InlineData(typeof(DateTimeOffset?))]
        [InlineData(typeof(TimeSpan))]
        public void Create_ForNonDateTime_ReturnsNull(Type modelType)
        {
            // Arrange
            var context = new TestModelBinderProviderContext(modelType);

            // Act
            var result = _provider.GetBinder(context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ForDateTime_ReturnsBinder()
        {
            // Arrange
            var context = new TestModelBinderProviderContext(typeof(DateTime));

            // Act
            var result = _provider.GetBinder(context);

            // Assert
            Assert.IsType<DateTimeModelBinder>(result);
        }

        [Fact]
        public void Create_ForNullableDateTime_ReturnsBinder()
        {
            // Arrange
            var context = new TestModelBinderProviderContext(typeof(DateTime?));

            // Act
            var result = _provider.GetBinder(context);

            // Assert
            Assert.IsType<DateTimeModelBinder>(result);
        }
    }
}
