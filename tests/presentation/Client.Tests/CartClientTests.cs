using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using Shop.Client;
using Shop.Client.Responses;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Tests
{
    [TestFixture]
    public class CartClientTests
    {
        private CartClient _sut;
        private Mock<HttpMessageHandler> _handlerMock;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var uri = new Uri("http://localhost:5000");

            _handlerMock = new Mock<HttpMessageHandler>();
            _handlerMock
                .Protected()
           // Setup the PROTECTED method to mock
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           // prepare the expected response of the mocked http call
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent("[{'id':1,'value':'1'}]"),
           });
            _client = new HttpClient(_handlerMock.Object);

            _sut = new CartClient(_client, uri);
        }

        [Test]
        public void AddProductAsync_Success()
        {
            // arrange
            var id = "SomeSuitableId";
            var catalogId = 1;
            var quantity = 10;
            var expectedResult = new CartResponse
            {
                Id = id,
                Items = new[]
                {
                    new CartItem
                    {
                        CatalogItemId = catalogId,
                        Quantity = quantity
                    },
                    new CartItem
                    {
                        CatalogItemId = catalogId+1,
                        Quantity = 10
                    }
                }
            };
            SetupHttpResponse(HttpMethod.Get, HttpStatusCode.OK, new CartResponse
            {
                Id = id,
                Items = new[]
                {
                    new CartItem
                    {
                        CatalogItemId = catalogId+1,
                        Quantity = 10
                    }
                }
            });
            SetupHttpResponse(HttpMethod.Post, HttpStatusCode.OK);

            // act
            CartResponse response = null;
            Assert.DoesNotThrowAsync(async () => response = await _sut.AddProductAsync(id, catalogId, quantity));

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Items, Is.Not.Empty);
            response.Items.Should().BeEquivalentTo(expectedResult.Items);
        }

        [Test]
        public void AddProductAsync_Error()
        {
            // arrange
            var id = "SomeSuitableId";
            var catalogId = 1;
            var quantity = 10;
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal server error")
            }, HttpMethod.Get);

            // act
            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.AddProductAsync(id, catalogId, quantity));

            // assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void ClearCartAsync_Success()
        {
            // arrange
            var id = "SomeSuitableId";
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent,
                Content = new StringContent("")
            }, HttpMethod.Delete);

            // act
            Assert.DoesNotThrowAsync(async () => await _sut.ClearCartAsync(id));
        }

        [Test]
        public void ClearCartAsync_Error()
        {
            // arrange
            var id = "SomeSuitableId";
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal server error")
            }, HttpMethod.Delete);

            // act
            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.ClearCartAsync(id));

            // assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void CreateCartAsync_Success()
        {
            // arrange
            var id = "SomeSuitableId";
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{'id': 'SomeSuitableId', 'items': [], 'total': '0'}")
            }, HttpMethod.Get);

            // act
            CartResponse response = null;
            Assert.DoesNotThrowAsync(async () => response = await _sut.CreateCartAsync(id));

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Total, Is.EqualTo(0));
            Assert.That(response.Items, Is.Empty);
        }

        [Test]
        public void CreateCartAsync_Error()
        {
            // arrange
            var id = "SomeSuitableId";
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal server error")
            }, HttpMethod.Get);

            // act
            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.CreateCartAsync(id));

            // assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void GetCartAsync_Success()
        {
            // arrange
            var id = "SomeSuitableId";
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{'id': 'SomeSuitableId', 'items': [], 'total': '0'}")
            }, HttpMethod.Get);

            // act
            CartResponse response = null;
            Assert.DoesNotThrowAsync(async () => response = await _sut.GetCartAsync(id));

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Total, Is.EqualTo(0));
            Assert.That(response.Items, Is.Empty);
        }

        [Test]
        public void GetCartAsync_Error()
        {
            // arrange
            var id = "SomeSuitableId";
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal server error")
            }, HttpMethod.Get);

            // act
            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.GetCartAsync(id));

            // assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void RemoveProductAsync_AllQuantity_Success()
        {
            // arrange
            var id = "SomeSuitableId";

            var catalogId = 1;
            var obj = new CartResponse
            {
                Id = id,
                Items = new[]
                {
                    new CartItem
                    {
                        CatalogItemId = catalogId,
                        Quantity = 10
                    }
                }
            };
            SetupHttpResponse(HttpMethod.Get, HttpStatusCode.OK, obj);
            SetupHttpResponse(HttpMethod.Post, HttpStatusCode.OK);

            // act
            CartResponse response = null;
            Assert.DoesNotThrowAsync(async () => response = await _sut.RemoveProductAsync(id, catalogId));

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Total, Is.EqualTo(0));
            Assert.That(response.Items, Is.Empty);
        }

        [Test]
        public void RemoveProductAsync_OneItem_Success()
        {
            // arrange
            var id = "SomeSuitableId";
            var catalogId = 1;
            var obj = new CartResponse
            {
                Id = id,
                Items = new[]
                {
                    new CartItem
                    {
                        CatalogItemId = catalogId,
                        Quantity = 10
                    },
                    new CartItem
                    {
                        CatalogItemId = catalogId+1,
                        Quantity = 10
                    }
                }
            };
            SetupHttpResponse(HttpMethod.Get, HttpStatusCode.OK, obj);
            SetupHttpResponse(HttpMethod.Post, HttpStatusCode.OK);

            // act
            CartResponse response = null;
            Assert.DoesNotThrowAsync(async () => response = await _sut.RemoveProductAsync(id, catalogId));

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Total, Is.EqualTo(0));
            Assert.That(response.Items, Is.Not.Empty);
            response.Items.Should().BeEquivalentTo(obj.Items.Where(x => x.CatalogItemId != catalogId));
        }

        [Test]
        public void RemoveProductAsync_Error()
        {
            // arrange
            var id = "SomeSuitableId";
            var catalogId = 10;
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal server error")
            }, HttpMethod.Get);

            // act
            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.RemoveProductAsync(id, catalogId));

            // assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void SetProductsQuantityAsync_AllQuantity_Success()
        {
            // arrange
            var id = "SomeSuitableId";

            var catalogId = 1;
            var quantity = 0;
            var obj = new CartResponse
            {
                Id = id,
                Items = new[]
                {
                    new CartItem
                    {
                        CatalogItemId = catalogId,
                        Quantity = 100
                    }
                }
            };
            SetupHttpResponse(HttpMethod.Get, HttpStatusCode.OK, obj);
            SetupHttpResponse(HttpMethod.Post, HttpStatusCode.OK);

            // act
            CartResponse response = null;
            Assert.DoesNotThrowAsync(async () => response = await _sut.SetProductsQuantityAsync(id, catalogId, quantity));

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Total, Is.EqualTo(0));
            Assert.That(response.Items, Is.Empty);
        }

        [Test]
        public void SetProductsQuantityAsync_OneItem_Success()
        {
            // arrange
            var id = "SomeSuitableId";

            var catalogId = 1;
            var quantity = 5;
            var expectedQuantity = 1;
            var obj = new CartResponse
            {
                Id = id,
                Items = new[]
                {
                    new CartItem
                    {
                        CatalogItemId = catalogId,
                        Quantity = quantity
                    }
                }
            };
            SetupHttpResponse(HttpMethod.Get, HttpStatusCode.OK, obj);
            SetupHttpResponse(HttpMethod.Post, HttpStatusCode.OK);

            // act
            CartResponse response = null;
            Assert.DoesNotThrowAsync(async () => response = await _sut.SetProductsQuantityAsync(id, catalogId, expectedQuantity));

            // assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Items, Is.Not.Empty);
            Assert.That(response.Items.Count, Is.EqualTo(obj.Items.Count()));
            Assert.That(response.Items[0].Quantity, Is.EqualTo(expectedQuantity));
        }

        [Test]
        public void SetProductsQuantityAsync_Error()
        {
            // arrange
            var id = "SomeSuitableId";
            var catalogId = 10;
            var quantity = 10;
            SetupHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal server error")
            }, HttpMethod.Get);

            // act
            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await _sut.SetProductsQuantityAsync(id, catalogId, quantity));

            // assert
            Assert.That(exception, Is.Not.Null);
        }

        private void SetupHttpResponse(HttpResponseMessage response, HttpMethod method)
        {
            _handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.Is<HttpRequestMessage>(r => r.Method == method),
                   ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(response);
        }

        private void SetupHttpResponse(HttpMethod method, HttpStatusCode httpStatusCode, object obj = null)
        {
            HttpContent content = null;
            _handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.Is<HttpRequestMessage>(r => r.Method == method),
                   ItExpr.IsAny<CancellationToken>()
                )
                .Callback((HttpRequestMessage message, CancellationToken token) =>
                {
                    content = message.Content;
                })
                // prepare the expected response of the mocked http call
                .ReturnsAsync(() =>
                {
                    var result = obj != null
                    ? new StringContent(JsonConvert.SerializeObject(obj))
                    : content;


                    return new HttpResponseMessage
                    {
                        StatusCode = httpStatusCode,
                        Content = result
                    };
                });
        }
    }
}