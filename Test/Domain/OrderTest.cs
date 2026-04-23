using AutoMapper;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.Order;
using DesignPattern.Domain.ValueObjects.Payment;
using DesignPattern.Infrastructure.Mongo.Mappings;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Test.Domain;

public class OrderTests
{
  private static ShippingInfo CreateShippingInfo()
  {
    var addressResult = Address.Create(
      receiverName: "Phuoc Phung",
      phoneNumber: "0901234567",
      country: "VN",
      province: "HCM",
      district: "1",
      ward: "Ben Nghe",
      street: "1 Nguyen Hue",
      provinceCode: "79",
      districtCode: "760",
      wardCode: "26734");

    addressResult.IsSuccess.Should().BeTrue(addressResult.Error.Message);

    var shippingInfoResult = ShippingInfo.Create(ShippingType.Standard, addressResult.Value);
    shippingInfoResult.IsSuccess.Should().BeTrue(shippingInfoResult.Error.Message);

    return shippingInfoResult.Value;
  }

  private static List<OrderItem> CreateOrderItems(int count = 2)
  {
    var attrs = new List<AttributeItem>
    {
      new("Color", "Red"),
      new("Size", "M")
    };

    var items = new List<OrderItem>();
    for (var i = 0; i < count; i++)
    {
      var productNameResult = Name.Create($"Product {i + 1}");
      productNameResult.IsSuccess.Should().BeTrue(productNameResult.Error.Message);

      var skuResult = StandardText.Create($"SKU-{i + 1}");
      skuResult.IsSuccess.Should().BeTrue(skuResult.Error.Message);

      var sellingPriceResult = Price.Create(100_000m + (10_000m * i));
      sellingPriceResult.IsSuccess.Should().BeTrue(sellingPriceResult.Error.Message);

      var purchasePriceResult = Price.Create(80_000m + (10_000m * i));
      purchasePriceResult.IsSuccess.Should().BeTrue(purchasePriceResult.Error.Message);

      var itemResult = OrderItem.Create(
        productId: Guid.NewGuid(),
        productName: productNameResult.Value,
        quantity: 1 + i,
        sku: skuResult.Value,
        sellingPrice: sellingPriceResult.Value,
        purchasePrice: purchasePriceResult.Value,
        thumbnailUrl: $"https://img.local/{i + 1}.png",
        attributes: attrs);

      itemResult.IsSuccess.Should().BeTrue(itemResult.Error.Message);
      items.Add(itemResult.Value);
    }

    return items;
  }

  [Fact]
  public void Create_Should_ReturnSuccess_WhenInputIsValid()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var items = CreateOrderItems();
    var shippingInfo = CreateShippingInfo();

    var payment = CreditCard.Create(
      cardNumber: "4111111111111111",
      cardHolder: "PHUOC PHUNG",
      cardType: "VISA");
    payment.IsSuccess.Should().BeTrue(payment.Error.Message);

    var discountCode = Code.Create("OD001");
    discountCode.IsSuccess.Should().BeTrue(discountCode.Error.Message);

    var note = Note.Create("Test order");
    note.IsSuccess.Should().BeTrue(note.Error.Message);

    var pointsUsed = Quantity.Create(1).Value;

    // Act
    var result = Order.Create(
      userId: userId,
      items: items,
      shippingInfo: shippingInfo,
      paymentInfo: payment.Value,
      discountCode: discountCode.Value,
      pointsUsed: pointsUsed,
      note: note.Value);

    // Assert
    result.IsSuccess.Should().BeTrue(result.Error.Message);

    var order = result.Value;
    order.UserId.Should().Be(userId);
    order.Items.Should().HaveCount(items.Count);
    order.ShippingInfo.Type.Should().Be(ShippingType.Standard);
    order.DiscountCode.Should().NotBeNull();
    order.DiscountCode!.Value.Should().Be("OD001");
    order.PointsUsed.Value.Should().Be(1);
    order.PaymentInfo.Should().BeOfType<CreditCard>();
    order.Note!.Value.Should().Be("Test order");

    // Defaults
    order.PaymentStatus.Should().Be(PaymentStatusEnum.Pending);
    order.Status.Should().Be(OrderStatusEnum.Pending);
  }

  [Fact]
  public void Create_Should_ReturnFailure_WhenOrderHasNoItems()
  {
    // Arrange
    var userId = Guid.NewGuid();
    var shippingInfo = CreateShippingInfo();
    var payment = Cash.Create().Value;

    var pointsUsed = Quantity.Create(1).Value;

    // Act
    var result = Order.Create(
      userId: userId,
      items: [],
      shippingInfo: shippingInfo,
      paymentInfo: payment,
      discountCode: null,
      pointsUsed: pointsUsed,
      note: null);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().Be("OrderItems.HaveNotItems");
  }

  [Theory]
  [InlineData("")]
  [InlineData("BAD")]
  public void Code_Create_Should_ReturnFailure_WhenDiscountCodeIsInvalid(string discountCode)
  {
    // Act
    var result = Code.Create(discountCode);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().StartWith("Domain.Code.");
  }

  [Theory]
  [InlineData(-1)]
  public void Quantity_Create_Should_ReturnFailure_WhenPointsUsedIsNonPositive(int pointsUsed)
  {
    // Act
    var result = Quantity.Create(pointsUsed);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().Be("Domain.Quantity.NonPositive");
  }

  [Fact]
  public void Quantity_Create_Should_ReturnSuccess_WhenValueIsZero()
  {
    // Act
    var result = Quantity.Create(0);

    // Assert
    result.IsSuccess.Should().BeTrue(result.Error.Message);
    result.Value.Value.Should().Be(0);
  }

  [Theory]
  [InlineData(PaymentType.Cash)]
  [InlineData(PaymentType.CreditCard)]
  [InlineData(PaymentType.BankTransfer)]
  public void OrderMappingProfile_Should_Map_Order_RoundTrip(PaymentType paymentType)
  {
    // Arrange
    var config = new MapperConfiguration(
      cfg => cfg.AddMaps(typeof(OrderMappingProfile).Assembly),
      NullLoggerFactory.Instance);
    config.AssertConfigurationIsValid();
    var mapper = config.CreateMapper();

    var userId = Guid.NewGuid();
    var items = CreateOrderItems(2);
    var shippingInfo = CreateShippingInfo();

    PaymentInfo payment = paymentType switch
    {
      PaymentType.Cash => Cash.Create().Value,
      PaymentType.CreditCard => CreditCard.Create("4111111111111111", "PHUOC PHUNG", "VISA").Value,
      PaymentType.BankTransfer => BankTransfer.Create("MB Bank", "0987654321").Value,
      _ => throw new InvalidOperationException("Unsupported payment type")
    };

    var pointsUsed = Quantity.Create(1).Value;
    var discountCode = Code.Create("OD001").Value;
    var note = Note.Create("Mapping test").Value;

    var order = Order.Create(
      userId: userId,
      items: items,
      shippingInfo: shippingInfo,
      paymentInfo: payment,
      discountCode: discountCode,
      pointsUsed: pointsUsed,
      note: note).Value;

    order.SetOrderStatus(
      OrderStatusEnum.Processing,
      "Order is processing",
      changedBy: Guid.NewGuid(),
      changedByRole: UserRole.Manager);
    order.SetPaymentStatus(PaymentStatusEnum.Paid);
    order.AddHistory(
      OrderStatusEnum.Processing,
      changedBy: Guid.NewGuid(),
      changedByRole: UserRole.Customer,
      note: "Order is processing");

    var infraAssembly = typeof(OrderMappingProfile).Assembly;
    var orderDocType = infraAssembly.GetType("DesignPattern.Infrastructure.Mongo.Documents.OrderDocument", throwOnError: true)!;

    // Act (Domain -> Document)
    var orderDoc = mapper.Map(order, typeof(Order), orderDocType);

    // Assert (document shape)
    orderDoc.Should().NotBeNull();

    GetProp<Guid>(orderDoc!, "UserId").Should().Be(userId);
    GetProp<int>(orderDoc, "PointsUsed").Should().Be(1);
    GetProp<string>(orderDoc, "Note").Should().Be("Mapping test");
    GetProp<OrderStatusEnum>(orderDoc, "Status").Should().Be(OrderStatusEnum.Processing);
    GetProp<PaymentStatusEnum>(orderDoc, "PaymentStatus").Should().Be(PaymentStatusEnum.Paid);

    var paymentInfoDoc = GetProp<object>(orderDoc, "PaymentInfo");
    paymentInfoDoc.Should().NotBeNull();
    GetProp<PaymentType>(paymentInfoDoc!, "Type").Should().Be(paymentType);

    if (paymentType == PaymentType.CreditCard)
    {
      GetProp<string?>(paymentInfoDoc, "CardNumber").Should().Be("4111111111111111");
      GetProp<string?>(paymentInfoDoc, "CardHolder").Should().Be("PHUOC PHUNG");
      GetProp<string?>(paymentInfoDoc, "CardType").Should().Be("VISA");
      GetProp<string?>(paymentInfoDoc, "BankName").Should().BeNull();
      GetProp<string?>(paymentInfoDoc, "AccountNumber").Should().BeNull();
    }

    if (paymentType == PaymentType.BankTransfer)
    {
      GetProp<string?>(paymentInfoDoc, "BankName").Should().Be("MB Bank");
      GetProp<string?>(paymentInfoDoc, "AccountNumber").Should().Be("0987654321");
      GetProp<string?>(paymentInfoDoc, "CardNumber").Should().BeNull();
      GetProp<string?>(paymentInfoDoc, "CardHolder").Should().BeNull();
      GetProp<string?>(paymentInfoDoc, "CardType").Should().BeNull();
    }

    if (paymentType == PaymentType.Cash)
    {
      GetProp<string?>(paymentInfoDoc, "BankName").Should().BeNull();
      GetProp<string?>(paymentInfoDoc, "AccountNumber").Should().BeNull();
      GetProp<string?>(paymentInfoDoc, "CardNumber").Should().BeNull();
      GetProp<string?>(paymentInfoDoc, "CardHolder").Should().BeNull();
      GetProp<string?>(paymentInfoDoc, "CardType").Should().BeNull();
    }

    // Act (Document -> Domain)
    var mappedBack = (Order)mapper.Map(orderDoc, orderDocType, typeof(Order));

    // Assert (round-trip)
    mappedBack.UserId.Should().Be(userId);
    mappedBack.PointsUsed.Value.Should().Be(1);
    mappedBack.DiscountCode.Should().NotBeNull();
    mappedBack.DiscountCode!.Value.Should().Be("OD001");
    mappedBack.Note!.Value.Should().Be("Mapping test");
    mappedBack.Items.Should().HaveCount(2);
    mappedBack.Histories.Should().HaveCount(2);
    mappedBack.Histories.Should().OnlyContain(x => x.Status == OrderStatusEnum.Processing);

    mappedBack.PaymentInfo.Type.Should().Be(paymentType);

    if (paymentType == PaymentType.CreditCard)
    {
      var cc = (CreditCard)mappedBack.PaymentInfo;
      cc.CardNumber.Value.Should().Be("4111111111111111");
      cc.CardHolder.Value.Should().Be("PHUOC PHUNG");
      cc.CardType.Value.Should().Be("VISA");
    }

    if (paymentType == PaymentType.BankTransfer)
    {
      var bt = (BankTransfer)mappedBack.PaymentInfo;
      bt.BankName.Value.Should().Be("MB Bank");
      bt.AccountNumber.Value.Should().Be("0987654321");
    }

    if (paymentType == PaymentType.Cash)
    {
      mappedBack.PaymentInfo.Should().BeOfType<Cash>();
    }
  }

  [Fact]
  public async Task ConfirmOrder_Should_NotifyAttachedObservers_WhenNotifyAsyncIsCalled()
  {
    // Arrange
    var order = CreateValidOrder();
    var firstObserver = new SpyOrderObserver();
    var secondObserver = new SpyOrderObserver();

    order.Attach(firstObserver);
    order.Attach(secondObserver);

    // Act
    Result confirmResult = order.ConfirmOrder(changedBy: Guid.NewGuid());
    await order.NotifyAsync();

    // Assert
    confirmResult.IsSuccess.Should().BeTrue(confirmResult.Error.Message);
    order.Status.Should().Be(OrderStatusEnum.Processing);
    firstObserver.CallCount.Should().Be(1);
    secondObserver.CallCount.Should().Be(1);
    firstObserver.LastOrderId.Should().Be(order.Id);
    secondObserver.LastOrderId.Should().Be(order.Id);
  }

  [Fact]
  public void ConfirmOrder_Should_Fail_WhenStatusIsNotPending()
  {
    // Arrange
    var order = CreateValidOrder();
    order.SetOrderStatus(
      OrderStatusEnum.Processing,
      "Move to processing for test",
      changedBy: Guid.NewGuid(),
      changedByRole: UserRole.Manager);
    var observer = new SpyOrderObserver();
    order.Attach(observer);

    // Act
    Result confirmResult = order.ConfirmOrder(changedBy: Guid.NewGuid());

    // Assert
    confirmResult.IsFailure.Should().BeTrue();
    confirmResult.Error.Code.Should().Be("Order.Confirm.InvalidStatus");
    observer.CallCount.Should().Be(0);
  }

  private static Order CreateValidOrder()
  {
    var userId = Guid.NewGuid();
    var items = CreateOrderItems(1);
    var shippingInfo = CreateShippingInfo();
    var payment = Cash.Create().Value;

    var orderResult = Order.Create(
      userId: userId,
      items: items,
      shippingInfo: shippingInfo,
      paymentInfo: payment,
      discountCode: Code.Create("OD001").Value,
      pointsUsed: Quantity.Create(1).Value,
      note: Note.Create("Observer test").Value);

    orderResult.IsSuccess.Should().BeTrue(orderResult.Error.Message);
    return orderResult.Value;
  }

  private sealed class SpyOrderObserver : IOrderObserver
  {
    public int CallCount { get; private set; }

    public Guid? LastOrderId { get; private set; }

    public Task Update(Order order)
    {
      CallCount++;
      LastOrderId = order.Id;
      return Task.CompletedTask;
    }
  }

  private static T GetProp<T>(object obj, string propName)
  {
    var prop = obj.GetType().GetProperty(propName);
    prop.Should().NotBeNull($"Property '{propName}' should exist on {obj.GetType().FullName}");
    var value = prop!.GetValue(obj);

    if (value is null)
      return default!;

    value.Should().BeAssignableTo<T>();
    return (T)value;
  }
}
