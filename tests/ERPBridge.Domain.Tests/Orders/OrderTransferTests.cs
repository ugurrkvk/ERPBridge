using ERPBridge.Domain.Orders;
using Xunit;

namespace ERPBridge.Domain.Tests.Orders;

public sealed class OrderTransferTests
{
    private const string ValidHash = "0123456789abcdef0123456789ABCDEF0123456789abcdef0123456789ABCDEF";

    [Fact]
    public void Create_WithValidInputs_InitializesAllFields()
    {
        var before = DateTime.UtcNow;
        var transfer = CreateTransfer();
        var after = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, transfer.Id);
        Assert.Equal("Marketplace", transfer.SourceSystem);
        Assert.Equal("seller-1", transfer.SourceSellerId);
        Assert.Equal("order-1", transfer.ExternalOrderId);
        Assert.Equal(ValidHash, transfer.RequestHash);
        Assert.Equal(OrderTransferStatus.Pending, transfer.Status);
        Assert.Equal(0, transfer.AttemptCount);
        Assert.Null(transfer.ErpOrderId);
        Assert.Null(transfer.ErpOrderNumber);
        Assert.Null(transfer.LastError);
        Assert.Null(transfer.LastAttemptAtUtc);
        Assert.Null(transfer.CompletedAtUtc);
        AssertUtcWithin(transfer.CreatedAtUtc, before, after);
    }

    [Fact]
    public void Create_AssignsDifferentIdsToSeparateTransfers()
    {
        Assert.NotEqual(CreateTransfer().Id, CreateTransfer().Id);
    }

    [Fact]
    public void Create_TrimsInputsAndPreservesCase()
    {
        var transfer = OrderTransfer.Create(" \tMarketplace\r\n", " seller-1 ", " order-1 ", $" \t{ValidHash}\r\n");

        Assert.Equal("Marketplace", transfer.SourceSystem);
        Assert.Equal("seller-1", transfer.SourceSellerId);
        Assert.Equal("order-1", transfer.ExternalOrderId);
        Assert.Equal(ValidHash, transfer.RequestHash);
    }

    public static TheoryData<string, string?> InvalidCreationInputs
    {
        get
        {
            var data = new TheoryData<string, string?>();
            foreach (var parameter in new[] { "sourceSystem", "sourceSellerId", "externalOrderId", "requestHash" })
            foreach (var value in new string?[] { null, "", " ", "\t\r\n" })
                data.Add(parameter, value);
            data.Add("sourceSystem", new string('s', 31));
            data.Add("sourceSellerId", new string('s', 51));
            data.Add("externalOrderId", new string('o', 101));
            data.Add("requestHash", new string('a', 63));
            data.Add("requestHash", new string('a', 65));
            data.Add("requestHash", new string('a', 63) + "g");
            data.Add("requestHash", "G" + new string('a', 63));
            data.Add("requestHash", new string('a', 31) + " " + new string('a', 32));
            data.Add("requestHash", new string('a', 63) + "é");
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCreationInputs))]
    public void Create_WithInvalidInput_ThrowsForCorrectParameter(string parameter, string? value)
    {
        var exception = Assert.Throws<ArgumentException>(() => OrderTransfer.Create(
            parameter == "sourceSystem" ? value! : "Marketplace",
            parameter == "sourceSellerId" ? value! : "seller-1",
            parameter == "externalOrderId" ? value! : "order-1",
            parameter == "requestHash" ? value! : ValidHash));

        Assert.Equal(parameter, exception.ParamName);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(30, 50, 100)]
    public void Create_AcceptsBoundaryLengthsAfterTrimming(int systemLength, int sellerLength, int orderLength)
    {
        var transfer = OrderTransfer.Create($" {new string('s', systemLength)} ",
            $" {new string('s', sellerLength)} ", $" {new string('o', orderLength)} ", $" {ValidHash} ");

        Assert.Equal(systemLength, transfer.SourceSystem.Length);
        Assert.Equal(sellerLength, transfer.SourceSellerId.Length);
        Assert.Equal(orderLength, transfer.ExternalOrderId.Length);
        Assert.Equal(ValidHash, transfer.RequestHash);
    }

    [Fact]
    public void StartAttempt_FromPending_UpdatesOnlyAttemptFields()
    {
        var transfer = CreateTransfer();
        var original = Snapshot(transfer);
        var before = DateTime.UtcNow;

        transfer.StartAttempt();

        Assert.Equal(OrderTransferStatus.Processing, transfer.Status);
        Assert.Equal(1, transfer.AttemptCount);
        AssertUtcWithin(transfer.LastAttemptAtUtc, before, DateTime.UtcNow);
        Assert.Equal(original with
        {
            Status = OrderTransferStatus.Processing,
            AttemptCount = 1,
            LastAttemptAtUtc = transfer.LastAttemptAtUtc
        }, Snapshot(transfer));
    }

    [Theory]
    [InlineData(OrderTransferStatus.Processing)]
    [InlineData(OrderTransferStatus.Succeeded)]
    public void StartAttempt_FromDisallowedState_ThrowsWithoutMutation(OrderTransferStatus status)
    {
        var transfer = CreateInState(status);
        var original = Snapshot(transfer);

        Assert.Throws<InvalidOperationException>(() => transfer.StartAttempt());

        Assert.Equal(original, Snapshot(transfer));
    }

    [Theory]
    [InlineData(1, " ERP-1 ", "ERP-1")]
    [InlineData(int.MaxValue, "\tERP-2\r\n", "ERP-2")]
    [InlineData(42, "A", "A")]
    public void MarkSucceeded_FromProcessing_SetsResultAndPreservesOtherFields(int id, string number, string expectedNumber)
    {
        var transfer = CreateInState(OrderTransferStatus.Processing);
        var original = Snapshot(transfer);
        var before = DateTime.UtcNow;

        transfer.MarkSucceeded(id, number);

        AssertUtcWithin(transfer.CompletedAtUtc, before, DateTime.UtcNow);
        Assert.Equal(original with
        {
            Status = OrderTransferStatus.Succeeded,
            ErpOrderId = id,
            ErpOrderNumber = expectedNumber,
            CompletedAtUtc = transfer.CompletedAtUtc,
            LastError = null
        }, Snapshot(transfer));
    }

    [Fact]
    public void MarkSucceeded_AcceptsFiftyCharacterNumberAfterTrimming()
    {
        var transfer = CreateInState(OrderTransferStatus.Processing);
        var number = new string('n', 50);

        transfer.MarkSucceeded(1, $" {number} ");

        Assert.Equal(number, transfer.ErpOrderNumber);
        Assert.Equal(OrderTransferStatus.Succeeded, transfer.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void MarkSucceeded_WithInvalidId_ThrowsWithoutMutation(int id)
    {
        var transfer = CreateInState(OrderTransferStatus.Processing);
        var original = Snapshot(transfer);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => transfer.MarkSucceeded(id, "ERP-1"));

        Assert.Equal("erpOrderId", exception.ParamName);
        Assert.Equal(original, Snapshot(transfer));
    }

    public static TheoryData<string?> InvalidOrderNumbers => new()
    {
        null, "", " ", "\t\r\n", new string('n', 51), $" {new string('n', 51)} "
    };

    [Theory]
    [MemberData(nameof(InvalidOrderNumbers))]
    public void MarkSucceeded_WithInvalidNumber_ThrowsWithoutMutation(string? number)
    {
        var transfer = CreateInState(OrderTransferStatus.Processing);
        var original = Snapshot(transfer);

        var exception = Assert.Throws<ArgumentException>(() => transfer.MarkSucceeded(1, number!));

        Assert.Equal("erpOrderNumber", exception.ParamName);
        Assert.Equal(original, Snapshot(transfer));
    }

    [Theory]
    [InlineData(OrderTransferStatus.Pending, true)]
    [InlineData(OrderTransferStatus.Pending, false)]
    [InlineData(OrderTransferStatus.Failed, true)]
    [InlineData(OrderTransferStatus.Failed, false)]
    [InlineData(OrderTransferStatus.Succeeded, true)]
    [InlineData(OrderTransferStatus.Succeeded, false)]
    public void Completion_FromNonProcessingState_ThrowsWithoutMutation(OrderTransferStatus status, bool succeed)
    {
        var transfer = CreateInState(status);
        var original = Snapshot(transfer);

        Assert.Throws<InvalidOperationException>(() =>
        {
            if (succeed) transfer.MarkSucceeded(1, "ERP-1");
            else transfer.MarkFailed("Connection failed");
        });

        Assert.Equal(original, Snapshot(transfer));
    }

    [Fact]
    public void MarkFailed_FromProcessing_SetsTrimmedErrorAndPreservesOtherFields()
    {
        var transfer = CreateInState(OrderTransferStatus.Processing);
        var original = Snapshot(transfer);

        transfer.MarkFailed(" \tConnection failed\r\n ");

        Assert.Equal(original with { Status = OrderTransferStatus.Failed, LastError = "Connection failed" }, Snapshot(transfer));
        Assert.Null(transfer.CompletedAtUtc);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\r\n")]
    public void MarkFailed_WithEmptyError_ThrowsWithoutMutation(string? error)
    {
        var transfer = CreateInState(OrderTransferStatus.Processing);
        var original = Snapshot(transfer);

        var exception = Assert.Throws<ArgumentException>(() => transfer.MarkFailed(error!));

        Assert.Equal("error", exception.ParamName);
        Assert.Equal(original, Snapshot(transfer));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1999)]
    [InlineData(2000)]
    [InlineData(2001)]
    [InlineData(4000)]
    public void MarkFailed_TrimsThenLimitsErrorToFirstTwoThousandCharacters(int length)
    {
        var transfer = CreateInState(OrderTransferStatus.Processing);
        var error = new string('a', length - 1) + "Z";

        transfer.MarkFailed($" \t{error}\r\n ");

        Assert.Equal(error[..Math.Min(length, 2000)], transfer.LastError);
        Assert.Equal(OrderTransferStatus.Failed, transfer.Status);
    }

    [Fact]
    public void StartAttempt_FromFailed_IncrementsCountClearsErrorAndUpdatesTimestamp()
    {
        var transfer = CreateInState(OrderTransferStatus.Failed);
        var original = Snapshot(transfer);
        var before = DateTime.UtcNow;

        transfer.StartAttempt();

        AssertUtcWithin(transfer.LastAttemptAtUtc, before, DateTime.UtcNow);
        Assert.True(transfer.LastAttemptAtUtc >= original.LastAttemptAtUtc);
        Assert.Equal(original with
        {
            Status = OrderTransferStatus.Processing,
            AttemptCount = 2,
            LastError = null,
            LastAttemptAtUtc = transfer.LastAttemptAtUtc
        }, Snapshot(transfer));
    }

    [Fact]
    public void RepeatedFailuresThenSuccess_PreserveIdentityAndTrackAttemptsConsistently()
    {
        var transfer = CreateTransfer();
        var original = Snapshot(transfer);

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            transfer.StartAttempt();
            Assert.Equal(OrderTransferStatus.Processing, transfer.Status);
            Assert.Equal(attempt, transfer.AttemptCount);
            Assert.Null(transfer.LastError);
            transfer.MarkFailed($"Failure {attempt}");
            Assert.Equal(OrderTransferStatus.Failed, transfer.Status);
            Assert.Equal($"Failure {attempt}", transfer.LastError);
            Assert.Null(transfer.CompletedAtUtc);
            Assert.Null(transfer.ErpOrderId);
            Assert.Null(transfer.ErpOrderNumber);
        }

        transfer.StartAttempt();
        var lastAttempt = transfer.LastAttemptAtUtc;
        transfer.MarkSucceeded(42, " ERP-42 ");

        Assert.NotNull(transfer.CompletedAtUtc);
        Assert.Equal(original with
        {
            Status = OrderTransferStatus.Succeeded,
            AttemptCount = 4,
            LastAttemptAtUtc = lastAttempt,
            CompletedAtUtc = transfer.CompletedAtUtc,
            ErpOrderId = 42,
            ErpOrderNumber = "ERP-42"
        }, Snapshot(transfer));
    }

    [Fact]
    public void InvalidCompletionInputs_AllowCorrectionWithinSameAttempt()
    {
        var transfer = CreateInState(OrderTransferStatus.Processing);

        Assert.Throws<ArgumentException>(() => transfer.MarkSucceeded(1, " "));
        Assert.Throws<ArgumentException>(() => transfer.MarkFailed(" "));
        transfer.MarkSucceeded(1, "ERP-1");

        Assert.Equal(OrderTransferStatus.Succeeded, transfer.Status);
        Assert.Equal(1, transfer.AttemptCount);
        Assert.Equal(1, transfer.ErpOrderId);
        Assert.Equal("ERP-1", transfer.ErpOrderNumber);
        Assert.Null(transfer.LastError);
        Assert.NotNull(transfer.CompletedAtUtc);
    }

    private static OrderTransfer CreateTransfer() =>
        OrderTransfer.Create("Marketplace", "seller-1", "order-1", ValidHash);

    private static OrderTransfer CreateInState(OrderTransferStatus status)
    {
        var transfer = CreateTransfer();
        if (status == OrderTransferStatus.Pending) return transfer;
        transfer.StartAttempt();
        if (status == OrderTransferStatus.Failed) transfer.MarkFailed("Connection failed");
        if (status == OrderTransferStatus.Succeeded) transfer.MarkSucceeded(42, "ERP-42");
        return transfer;
    }

    // Bound timestamps around the operation; no sleeps or assumed clock resolution.
    private static void AssertUtcWithin(DateTime? value, DateTime before, DateTime after)
    {
        Assert.NotNull(value);
        Assert.Equal(DateTimeKind.Utc, value.Value.Kind);
        Assert.InRange(value.Value, before, after);
    }

    private static TransferSnapshot Snapshot(OrderTransfer transfer) => new(
        transfer.Id, transfer.SourceSystem, transfer.SourceSellerId, transfer.ExternalOrderId,
        transfer.RequestHash, transfer.Status, transfer.AttemptCount, transfer.ErpOrderId,
        transfer.ErpOrderNumber, transfer.LastError, transfer.CreatedAtUtc,
        transfer.LastAttemptAtUtc, transfer.CompletedAtUtc);

    private sealed record TransferSnapshot(
        Guid Id, string SourceSystem, string SourceSellerId, string ExternalOrderId,
        string RequestHash, OrderTransferStatus Status, int AttemptCount, int? ErpOrderId,
        string? ErpOrderNumber, string? LastError, DateTime CreatedAtUtc,
        DateTime? LastAttemptAtUtc, DateTime? CompletedAtUtc);
}
