// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class Demo
	{
		private static string TALISKER = "Talisker";

		[Fact]
		public void FillingRemovesInventoryIfInStock()
		{
			//setup - data
			var order = new Order(TALISKER, 50);
			var mock = new Mock<IWarehouse>();

			//setup - expectations
			mock.Setup(x => x.HasInventory(TALISKER, 50)).Returns(true);

			//exercise
			order.Fill(mock.Object);

			//verify state
			Assert.True(order.IsFilled);
			//verify interaction
			mock.VerifyAll();
		}

		[Fact]
		public void FillingDoesNotRemoveIfNotEnoughInStock()
		{
			//setup - data
			var order = new Order(TALISKER, 50);
			var mock = new Mock<IWarehouse>();

			//setup - expectations
			mock.Setup(x => x.HasInventory(It.IsAny<string>(), It.IsInRange(0, 100, Range.Inclusive))).Returns(false);
			mock.Setup(x => x.Remove(It.IsAny<string>(), It.IsAny<int>())).Throws(new InvalidOperationException());

			//exercise
			order.Fill(mock.Object);

			//verify
			Assert.False(order.IsFilled);
		}

		[Fact]
		public void TestPresenterSelection()
		{
			var mockView = new Mock<IOrdersView>();
			var presenter = new OrdersPresenter(mockView.Object);

			// Check that the presenter has no selection by default
			Assert.Null(presenter.SelectedOrder);

			// Finally raise the event with a specific arguments data
			mockView.Raise(mv => mv.OrderSelected += null, new OrderEventArgs { Order = new Order("moq", 500) });

			// Now the presenter reacted to the event, and we have a selected order
			Assert.NotNull(presenter.SelectedOrder);
			Assert.Equal("moq", presenter.SelectedOrder.ProductName);
		}

		public class OrderEventArgs : EventArgs
		{
			public Order Order { get; set; }
		}

		public interface IOrdersView
		{
			event EventHandler<OrderEventArgs> OrderSelected;
		}

		public class OrdersPresenter
		{
			public OrdersPresenter(IOrdersView view)
			{
				view.OrderSelected += (sender, args) => DoOrderSelection(args.Order);
			}

			public Order SelectedOrder { get; private set; }

			private void DoOrderSelection(Order selectedOrder)
			{
				// Do something when the view selects an order.
				SelectedOrder = selectedOrder;
			}
		}

		public interface IWarehouse
		{
			bool HasInventory(string productName, int quantity);
			void Remove(string productName, int quantity);
		}

		public class Order
		{
			public string ProductName { get; private set; }
			public int Quantity { get; private set; }
			public bool IsFilled { get; private set; }

			public Order(string productName, int quantity)
			{
				this.ProductName = productName;
				this.Quantity = quantity;
			}

			public void Fill(IWarehouse warehouse)
			{
				if (warehouse.HasInventory(ProductName, Quantity))
				{
					warehouse.Remove(ProductName, Quantity);
					IsFilled = true;
				}
			}
		}
	}
}
