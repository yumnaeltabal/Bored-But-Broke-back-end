using Bored_But_Broke_back_end.Controllers;
using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.Models.Queries;
using Bored_But_Broke_back_end.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bored_But_Broke_back_end.UnitTests.Services
{
    internal class PlaceServiceTests
    {
        private Mock<IYelpClient> _mockYelpClient;
        private PlaceService _placeService;

        [SetUp]
        public void Setup()
        {
            _mockYelpClient = new Mock<IYelpClient>();
            _placeService = new PlaceService(_mockYelpClient.Object);
        }

        [Test]
        public async Task GetPlacesAsync_ShouldReturnEmptyList_WhenClientSuccessfullyReturnNoResults()
        {
            var query = new GetPlacesQuery
            { 
                Location = "London"
            };
            var token = CancellationToken.None;

            var response = new SearchResponse
            {
                Businesses = new List<Business>()
            };

            _mockYelpClient
                .Setup(m => m.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            var result = await _placeService.GetPlacesAsync(query, token);

            result.ShouldNotBeNull();
            result.ShouldBeEmpty();

            _mockYelpClient.Verify(m => m.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetPlacesAsync_ShouldReturnListOfPlaces_WhenClientSuccessfullyReturnPlaces()
        {
            var query = new GetPlacesQuery
            {
                Location = "London"
            };
            var token = CancellationToken.None;

            var response = new SearchResponse
            {
                Businesses = new List<Business>
                {
                    new Business { PlaceId = "1", PlaceName = "Sample Place 1" },
                    new Business { PlaceId = "2", PlaceName = "Sample Place 2" }
                }
            };

            _mockYelpClient
                .Setup(m => m.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _placeService.GetPlacesAsync(query, token);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldBeEquivalentTo(response.ToPlaces());

            _mockYelpClient.Verify(m => m.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        // TODO: Add more params test later
        [Test]
        public async Task GetPlacesAsync_ShouldPassCorrectQueryParamsToClient_WithNotEmptyQuery()
        {
            var location = "London";
            var limit = 20;

            var query = new GetPlacesQuery 
            { 
                Location = location,
                Limit = limit 
            };
            var token = CancellationToken.None;

            Dictionary<string, StringValues> queryParams = null!;

            _mockYelpClient
                .Setup(m => m.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .Callback<Dictionary<string, StringValues>, CancellationToken>((d, _) => queryParams = d)
                .ReturnsAsync(new SearchResponse());

            await _placeService.GetPlacesAsync(query, token);

            queryParams.ShouldNotBeNull();
            queryParams.Count.ShouldBe(2);
            queryParams["location"].ToString().ShouldBe(location);
            queryParams["limit"].ToString().ShouldBe(limit.ToString());

            _mockYelpClient.Verify(m => m.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetPlacesAsync_ShouldThrowsException_WhenClientFails()
        {
            var query = new GetPlacesQuery 
            { 
                Location = "London", 
            };
            var token = CancellationToken.None;

            _mockYelpClient
                .Setup(c => c.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Yelp API unavailable"));

            var asyncAction = async () => await _placeService.GetPlacesAsync(query, token);

            asyncAction()
                .ShouldThrowAsync<HttpRequestException>()
                .Result.Message.ShouldBe("Yelp API unavailable");

            var ex = await Should.ThrowAsync<Exception>(_placeService.GetPlacesAsync(query, token));
            ex.Message.ShouldBe("Yelp API unavailable");
        }

        [Test]
        public async Task GetPlacesAsync_ShouldForwardsCancellationToken()
        {
            using var cts = new CancellationTokenSource();
            var token = cts.Token;

            GetPlacesQuery query = new GetPlacesQuery
            {
                Location = "London"
            };

            _mockYelpClient
                .Setup(m => m.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse());

            await _placeService.GetPlacesAsync(query, token);

            _mockYelpClient.Verify(m => m.GetPlacesAsync(It.IsAny<Dictionary<string, StringValues>>(), token), Times.Once());
        }
    }
}