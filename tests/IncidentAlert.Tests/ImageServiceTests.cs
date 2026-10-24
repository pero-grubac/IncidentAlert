using IncidentAlert.Data;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories.Implementation;
using IncidentAlert.Services.Implementation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace IncidentAlert.Tests;

public sealed class ImageServiceTests : IDisposable
{
    private readonly string _webRoot = Directory.CreateTempSubdirectory("incident-alert-tests-").FullName;

    public void Dispose() => Directory.Delete(_webRoot, recursive: true);

    [Theory]
    [InlineData("photo.jpg", "photo.jpg")]
    [InlineData("../../appsettings.json", "appsettings.json")]
    [InlineData(@"..\..\evil.png", "evil.png")]
    [InlineData("/etc/passwd", "passwd")]
    public void SafeFileName_KeepsOnlyTheLastSegment(string input, string expected) =>
        Assert.Equal(expected, ImageService.SafeFileName(input));

    [Theory]
    [InlineData("")]
    [InlineData("..")]
    [InlineData("folder/")]
    public void SafeFileName_RejectsNamesWithoutAFile(string input) =>
        Assert.Throws<FileEmptyException>(() => ImageService.SafeFileName(input));

    [Fact]
    public async Task Add_WritesTheFileInsideTheIncidentFolderEvenForATraversalName()
    {
        var options = TestData.NewDatabase();
        var incident = new Incident { Title = "t", Text = "t", DateTime = DateTime.UtcNow, Location = TestData.BanjaLuka() };
        TestData.Seed(options, db => db.Incidents.Add(incident));

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(_webRoot);

        using var context = new DataContext(options);
        var service = new ImageService(environment, new ImageRepository(context), new IncidentRepository(context),
            Substitute.For<IHttpContextAccessor>());

        await service.Add(new ImageData { FileName = "../../escape.jpg", Content = [1, 2, 3] }, incident.Id);

        var expected = Path.Combine(_webRoot, "uploads", incident.Id.ToString(), "escape.jpg");
        Assert.True(File.Exists(expected));
        Assert.False(File.Exists(Path.Combine(_webRoot, "..", "escape.jpg")));
        Assert.Equal($"/uploads/{incident.Id}/escape.jpg", context.Images.Single().FilePath);
    }
}
