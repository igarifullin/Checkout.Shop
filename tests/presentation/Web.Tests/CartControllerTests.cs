using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Shop.Web.Models;
using Shop.Web.ViewModels;

namespace Web.Tests
{
    [TestFixture]
    public class CartControllerTests : BaseWebTests
    {
        [Test]
        public void GetBasketByIdAsync_AnyIdValue()
        {
            // arrange
            var id = "SomeSuitableId";
            var client = _testServer.CreateClient();

            // act
            HttpResponseMessage response = null;

            Assert.DoesNotThrowAsync(async () =>
            {
                response = await client.GetAsync($"/api/cart/{id}");
            });

            Action act = () => response.EnsureSuccessStatusCode();

            // asserts
            act.Should().NotThrow();  // Status Code 200-299

            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            CustomerCartViewModel responseModel = null;
            Assert.DoesNotThrowAsync(async () => responseModel = JsonConvert.DeserializeObject<CustomerCartViewModel>(await response.Content.ReadAsStringAsync()));
            Assert.That(responseModel, Is.Not.Null);
            Assert.That(responseModel.Items, Is.Empty);
            Assert.That(responseModel.Total, Is.EqualTo(0));
        }

        [Test]
        public void UpdateBasketAsync_SkippedUnknownItem()
        {
            // arrange
            var id = "SomeSuitableId";
            var client = _testServer.CreateClient();
            var request = new CustomerCartModel
            {
                Id = id,
                Items = new[]
                {
                    new CustomerCartItemModel
                    {
                        CatalogItemId = 1,
                        Quantity = 2
                    },
                    new CustomerCartItemModel
                    {
                        CatalogItemId = 2,
                        Quantity = 1
                    },
                    new CustomerCartItemModel
                    {
                        // not existing
                        CatalogItemId = 999999,
                        Quantity = 1
                    }
                }
            };

            // act
            HttpResponseMessage response = null;

            Assert.DoesNotThrowAsync(async () =>
            {
                response = await client.PostAsJsonAsync($"/api/cart", request);
            });

            Action act = () => response.EnsureSuccessStatusCode();

            // asserts
            act.Should().NotThrow();  // Status Code 200-299

            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            CustomerCartViewModel responseModel = null;
            Assert.DoesNotThrowAsync(async () => responseModel = JsonConvert.DeserializeObject<CustomerCartViewModel>(await response.Content.ReadAsStringAsync()));
            Assert.That(responseModel, Is.Not.Null);

            responseModel.Items.Should().BeEquivalentTo(new[]
            {
                new
                {
                    CatalogItemId = 1,
                    Quantity = 2
                },
                new
                {
                    CatalogItemId = 2,
                    Quantity = 1
                }
            });
        }

        [Test]
        public void UpdateBasketAsync_ValidationCheck()
        {
            // arrange
            var id = "SomeSuitableId";
            var client = _testServer.CreateClient();
            var request = new CustomerCartModel
            {
                Id = id,
                Items = new[]
                {
                    new CustomerCartItemModel
                    {
                        CatalogItemId = -2,
                        Quantity = -50
                    }
                }
            };

            // act
            HttpResponseMessage response = null;

            Assert.DoesNotThrowAsync(async () =>
            {
                response = await client.PostAsJsonAsync($"/api/cart", request);
            });

            // asserts
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
