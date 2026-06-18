using FluentValidation;
using Messenger.Application.Common.Behaviors;
using Xunit;
using AppValidationException = Messenger.Application.Common.Exceptions.ValidationException;

namespace Messenger.UnitTests.Application;

public class ValidationBehaviorTests
{
    public sealed record SampleCommand(string Name);

    public sealed class SampleValidator : AbstractValidator<SampleCommand>
    {
        public SampleValidator() => RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }

    [Fact]
    public async Task Throws_when_request_is_invalid()
    {
        var behavior = new ValidationBehavior<SampleCommand, string>([new SampleValidator()]);

        var ex = await Assert.ThrowsAsync<AppValidationException>(() =>
            behavior.Handle(new SampleCommand(""), _ => Task.FromResult("ok"), CancellationToken.None));

        Assert.Contains("Name is required.", ex.Errors);
    }

    [Fact]
    public async Task Passes_through_when_request_is_valid()
    {
        var behavior = new ValidationBehavior<SampleCommand, string>([new SampleValidator()]);

        var result = await behavior.Handle(
            new SampleCommand("Ada"), _ => Task.FromResult("ok"), CancellationToken.None);

        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Passes_through_when_no_validators_registered()
    {
        var behavior = new ValidationBehavior<SampleCommand, string>([]);

        var result = await behavior.Handle(
            new SampleCommand(""), _ => Task.FromResult("ok"), CancellationToken.None);

        Assert.Equal("ok", result);
    }
}
