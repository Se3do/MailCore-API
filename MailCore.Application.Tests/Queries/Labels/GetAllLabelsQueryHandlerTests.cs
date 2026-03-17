using MailCore.Application.Queries.Labels.GetAllLabels;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;
using Xunit;

namespace MailCore.Application.Tests.Queries.Labels;

public class GetAllLabelsQueryHandlerTests
{
    private readonly Mock<ILabelRepository> _labelRepo = new();
    private readonly GetAllLabelsQueryHandler _sut;

    public GetAllLabelsQueryHandlerTests()
    {
        _sut = new GetAllLabelsQueryHandler(_labelRepo.Object);
    }

    [Fact]
    public async Task Handle_ReturnsLabelsMappedToDto()
    {
        var userId = Guid.NewGuid();
        var labels = new List<Label>
        {
            new Label { Id = Guid.NewGuid(), UserId = userId, Name = "Work", Color = "Blue" },
            new Label { Id = Guid.NewGuid(), UserId = userId, Name = "Personal", Color = "Green" }
        };

        _labelRepo.Setup(r => r.GetAllAsync(userId, default)).ReturnsAsync(labels);

        var result = await _sut.Handle(new GetAllLabelsQuery(userId), default);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, l => l.Name == "Work");
        Assert.Contains(result, l => l.Name == "Personal");
    }
}
