using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;

namespace dotnetCoreApi.TestProject1
{
    public class Tests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client; 

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task GetWeatherForecast_ShouldReturnData()
        {
            // 模拟对 GetWeatherForecast 端点的请求
            var response = await _client.GetAsync("/WeatherForecast");

            // 验证响应是否成功
            response.EnsureSuccessStatusCode();

            // 获取响应内容并反序列化为 WeatherForecast 数组
            var responseData = await response.Content.ReadAsStringAsync();
            var weatherForecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(responseData);
            Console.WriteLine(responseData);

            // 验证返回的数据不为空
            Assert.IsNotNull(weatherForecasts);

            // 验证返回的数据数量是否符合预期
            Assert.AreEqual(5, weatherForecasts.Length);

            // 可以根据需要添加更多的验证
        }

    }
}