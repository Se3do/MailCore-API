using MailCore.Application.Common.Pagination;
using MailCore.Application.Validators;

namespace MailCore.Application.Tests.Validators;

public class CursorPaginationValidatorTests
{
    private readonly CursorPaginationValidator _sut = new();

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(50)]
    public void ValidPageSize_Passes(int pageSize)
    {
      var result = _sut.Validate(new CursorPaginationQuery(null, pageSize));
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ZeroOrNegativePageSize_Fails(int pageSize)
    {
   var result = _sut.Validate(new CursorPaginationQuery(null, pageSize));
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(51)]
    [InlineData(100)]
    public void PageSizeExceeds50_Fails(int pageSize)
    {
  var result = _sut.Validate(new CursorPaginationQuery(null, pageSize));
        Assert.False(result.IsValid);
     Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("50"));
    }
}
