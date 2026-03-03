using FluentValidation;
using MailCore.Application.Common.Behaviors;
using MailCore.Application.Exceptions;
using MediatR;
using Moq;

namespace MailCore.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    private record TestRequest(string Value) : IRequest<string>;

    private static Task<string> Next() => Task.FromResult("ok");

    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
   var behavior = new ValidationBehavior<TestRequest, string>([]);

        var result = await behavior.Handle(new TestRequest("x"), _ => Next(), default);

        Assert.Equal("ok", result);
  }

    [Fact]
    public async Task Handle_ValidRequest_CallsNext()
    {
        var validator = new InlineValidator<TestRequest>();
        validator.RuleFor(x => x.Value).NotEmpty();

        var behavior = new ValidationBehavior<TestRequest, string>([validator]);

        var result = await behavior.Handle(new TestRequest("valid"), _ => Next(), default);

        Assert.Equal("ok", result);
    }

    [Fact]
 public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
      var validator = new InlineValidator<TestRequest>();
validator.RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required.");

        var behavior = new ValidationBehavior<TestRequest, string>([validator]);

   var ex = await Assert.ThrowsAsync<Exceptions.ValidationException>(
            () => behavior.Handle(new TestRequest(""), _ => Next(), default));

        Assert.Contains("Value is required.", ex.Message);
    }

    [Fact]
    public async Task Handle_MultipleFailures_CombinesMessages()
    {
        var validator = new InlineValidator<TestRequest>();
        validator.RuleFor(x => x.Value).NotEmpty().WithMessage("Empty.");
        validator.RuleFor(x => x.Value).MinimumLength(10).WithMessage("Too short.");

        var behavior = new ValidationBehavior<TestRequest, string>([validator]);

      var ex = await Assert.ThrowsAsync<Exceptions.ValidationException>(
            () => behavior.Handle(new TestRequest(""), _ => Next(), default));

        Assert.Contains("Empty.", ex.Message);
    }

  [Fact]
    public async Task Handle_MultipleValidators_AllRun()
    {
var v1 = new InlineValidator<TestRequest>();
        v1.RuleFor(x => x.Value).NotEmpty().WithMessage("From v1.");

        var v2 = new InlineValidator<TestRequest>();
     v2.RuleFor(x => x.Value).MinimumLength(20).WithMessage("From v2.");

    var behavior = new ValidationBehavior<TestRequest, string>([v1, v2]);

        var ex = await Assert.ThrowsAsync<Exceptions.ValidationException>(
   () => behavior.Handle(new TestRequest(""), _ => Next(), default));

        Assert.Contains("From v1.", ex.Message);
    }
}
