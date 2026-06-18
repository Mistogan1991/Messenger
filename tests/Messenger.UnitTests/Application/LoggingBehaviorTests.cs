using Messenger.Application.Common.Behaviors;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Messenger.UnitTests.Application;

public class LoggingBehaviorTests
{
    public sealed record SampleRequest(string Value);

    private static LoggingBehavior<SampleRequest, string> NewBehavior() =>
        new(NullLogger<LoggingBehavior<SampleRequest, string>>.Instance);

    [Fact]
    public async Task Returns_the_inner_handler_response()
    {
        var result = await NewBehavior().Handle(
            new SampleRequest("x"), _ => Task.FromResult("handled"), CancellationToken.None);

        Assert.Equal("handled", result);
    }

    [Fact]
    public async Task Propagates_exceptions_from_the_inner_handler()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            NewBehavior().Handle(
                new SampleRequest("x"),
                _ => throw new InvalidOperationException("boom"),
                CancellationToken.None));
    }
}
