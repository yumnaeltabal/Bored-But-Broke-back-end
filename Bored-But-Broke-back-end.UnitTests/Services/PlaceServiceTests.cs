using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.ExternalApis.Yelp.Extensions;
using Bored_But_Broke_back_end.ExternalApis.Yelp.Responses;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Models.Queries;
using Bored_But_Broke_back_end.Services;
using Microsoft.Extensions.Primitives;
using Moq;
using Shouldly;

namespace Bored_But_Broke_back_end.UnitTests.Services
{
    internal class PlaceServiceTests
    {
        private Mock<IYelpClient> _mockYelpClient;
        private Mock<ILocationService> _mockLocationService;
        private PlaceService _placeService;

        [SetUp]
        public void Setup()
        {
            _mockYelpClient = new Mock<IYelpClient>();
            _mockLocationService = new Mock<ILocationService>();
            _placeService = new PlaceService(_mockYelpClient.Object, _mockLocationService.Object);
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
                Businesses = new List<YelpBusiness>()
            };

            _mockLocationService
                .Setup(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Coordinates());

            _mockYelpClient
                .Setup(m => m.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            var result = await _placeService.GetPlacesAsync(query, token);

            result.ShouldNotBeNull();
            result.ShouldBeEmpty();

            _mockLocationService.Verify(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _mockYelpClient.Verify(m => m.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()), Times.Once());
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
                Businesses = new List<YelpBusiness>
                {
                    new YelpBusiness { PlaceId = "1", PlaceName = "Sample Place 1" },
                    new YelpBusiness { PlaceId = "2", PlaceName = "Sample Place 2" }
                }
            };

            _mockLocationService
                .Setup(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Coordinates());

            _mockYelpClient
                .Setup(m => m.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _placeService.GetPlacesAsync(query, token);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldBeEquivalentTo(response.ToPlaces());

            _mockLocationService.Verify(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _mockYelpClient.Verify(m => m.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        // TODO: Add more params test later
        [Test]
        public async Task GetPlacesAsync_ShouldPassCorrectQueryParamsToClient_WithNotEmptyQuery()
        {
            var location = "London";
            var radius = 2000;
            Price[] price = [Price.Cheap, Price.Moderate];
            var limit = 20;
            var latitude = 1.1;
            var longitude = 2.2;

            var query = new GetPlacesQuery 
            { 
                Location = location,
                Radius = radius,
                Budget = price,
                Limit = limit 
            };
            var coordinates = new Coordinates
            {
                Latitude = latitude,
                Longitude = longitude
            };
            var token = CancellationToken.None;

            Dictionary<string, StringValues> queryParams = null!;

            _mockLocationService
                .Setup(m => m.GetCoordinatesFromAddressAsync(location, It.IsAny<CancellationToken>()))
                .ReturnsAsync(coordinates);

            _mockYelpClient
                .Setup(m => m.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .Callback<Dictionary<string, StringValues>, CancellationToken>((d, _) => queryParams = d)
                .ReturnsAsync(new SearchResponse());

            await _placeService.GetPlacesAsync(query, token);

            queryParams.ShouldNotBeNull();
            queryParams.Count.ShouldBe(5);
            queryParams["latitude"].ToString().ShouldBe(latitude.ToString());
            queryParams["longitude"].ToString().ShouldBe(longitude.ToString());
            queryParams["radius"].ToString().ShouldBe(radius.ToString());
            queryParams["price"].ToString().ShouldBe(String.Join(",", price.Cast<int>()));
            queryParams["limit"].ToString().ShouldBe(limit.ToString());

            _mockLocationService.Verify(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
            _mockYelpClient.Verify(m => m.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task GetPlacesAsync_ShouldThrowsException_WhenClientFails()
        {
            var query = new GetPlacesQuery 
            { 
                Location = "London", 
            };
            var token = CancellationToken.None;

            _mockLocationService
                .Setup(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Coordinates());

            _mockYelpClient
                .Setup(c => c.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
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

            _mockLocationService
                .Setup(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Coordinates());

            _mockYelpClient
                .Setup(m => m.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse());

            await _placeService.GetPlacesAsync(query, token);

            _mockLocationService.Verify(m => m.GetCoordinatesFromAddressAsync(It.IsAny<string>(), token), Times.Once());
            _mockYelpClient.Verify(m => m.BusinessesSearchAsync(It.IsAny<Dictionary<string, StringValues>>(), token), Times.Once());
        }
    }
}