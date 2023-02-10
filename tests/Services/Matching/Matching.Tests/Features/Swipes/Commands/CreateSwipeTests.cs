using Matching.Application.Features.Swipes.Commands;
using Matching.Tests.Base;
using System.Net.Http.Json;
using System.Net;

namespace Matching.Tests.Features.Swipes.Commands
{
    public class CreateSwipeTests : IntegrationTestBase
    {
        public CreateSwipeTests(IntegrationTestFactory factory) : base(factory)
        {
        }


        [Fact]
        public async Task CreateSwipe_WhenSwipeNotExists_StatusShouldReturn_Created()
        {
            var command = new CreateSwipeCommand
            {
                FromUserId = "aaa",
                ToUserId = "bbb",
                Type = "right"
            };

            var client = Factory.CreateClient();
            var createSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.Created, createSwipeResponse.StatusCode);
        }

        [Fact]
        public async Task CreateSwipe_WhenExists_StatusShouldReturn_UnprocessableEntity()
        {
            var command = new CreateSwipeCommand
            {
                FromUserId = "bbb",
                ToUserId = "ccc",
                Type = "right"
            };

            var client = Factory.CreateClient();
            var createSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.Created, createSwipeResponse.StatusCode);


            var createExistentSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, createExistentSwipeResponse.StatusCode);
        }

        [Fact]
        public async Task CreateSwipe_WhenInvalidInput_ShouldReturn_BadRequest()
        {
            var command = new CreateSwipeCommand
            {
                FromUserId = "ccc",
                ToUserId = "ddd",
                Type = "righggt"
            };

            var client = Factory.CreateClient();
            var createSwipeResponse = await client.PostAsJsonAsync($"/api/swipes", command);

            Assert.Equal(HttpStatusCode.BadRequest, createSwipeResponse.StatusCode);
        }
    }
}
