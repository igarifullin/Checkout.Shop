using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Shop.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Web.Tests
{
    [TestFixture]
    public class CatalogControllerTests : BaseWebTests
    {
        [Test]
        public void GetAllItems()
        {
            var client = _testServer.CreateClient();

            // act
            HttpResponseMessage response = null;

            Assert.DoesNotThrowAsync(async () =>
            {
                response = await client.GetAsync($"/api/catalog/items");
            });

            Action act = () => response.EnsureSuccessStatusCode();

            // asserts
            act.Should().NotThrow();  // Status Code 200-299

            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");

            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            IEnumerable<CatalogItemViewModel> responseModel = null;
            Assert.DoesNotThrowAsync(async () => responseModel = JsonConvert.DeserializeObject<IEnumerable<CatalogItemViewModel>>(await response.Content.ReadAsStringAsync()));
            Assert.That(responseModel, Is.Not.Null);
            Assert.That(responseModel, Is.Not.Empty);
        }
    }
}