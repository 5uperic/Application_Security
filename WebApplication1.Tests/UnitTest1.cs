using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class SystemTests
{
    private readonly HttpClient _client;

    public SystemTests()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5282")  // Replace with your application's URL
        };
    }

    [Fact]
    public async Task HomePage_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task LoginPage_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/Login");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}