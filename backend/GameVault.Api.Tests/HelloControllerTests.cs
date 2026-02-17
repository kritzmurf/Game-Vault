using Microsoft.AspNetCore.Mvc;
using GameVault.Controllers;

namespace GameVault.Api.Tests;

public class HelloControllerTests
{
    [Fact]
    public void Get_ReturnsOkResult()
    {
        // Arrange
        var controller = new HelloController();

        // Act
        var res = controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(res);
    }
}
