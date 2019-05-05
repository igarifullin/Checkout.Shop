using FluentAssertions;
using Moq;
using NUnit.Framework;
using Shop.Domain;
using Shop.Domain.CartAggregate;
using Shop.Domain.Dto;
using Shop.Domain.Repositories;
using Shop.Services.Impl;
using System.Collections.Generic;

namespace Services.Tests
{
    [TestFixture]
    public class CartManagerTests
    {
        private Mock<ICatalogItemRepository> _mockCatalogItemsRepository;
        private Mock<ICartRepository> _mockCartRepository;
        private CartManager _sut;

        [SetUp]
        public void Setup()
        {
            _mockCartRepository = new Mock<ICartRepository>(MockBehavior.Default);
            _mockCatalogItemsRepository = new Mock<ICatalogItemRepository>(MockBehavior.Strict);

            _sut = new CartManager(_mockCartRepository.Object, _mockCatalogItemsRepository.Object);
        }

        [Test]
        public void GetCartAsync_NotFound_Put()
        {
            // arrange
            var id = "SomeSuitableValue";
            Cart cart = new Cart();
            _mockCartRepository
                .Setup(x => x.PutCartAsync(It.IsAny<Cart>()))
                .Callback((Cart c) =>
                {
                    // just because we can't change the reference
                    // when callbacks body executes, mock captured the original cart reference
                    cart.Id = c.Id;
                    foreach (var i in c.Items)
                    {
                        cart.AddItem(i.CatalogItem, i.Quantity);
                    }
                })
                .ReturnsAsync(cart);

            // act
            Cart result = null;
            Assert.DoesNotThrowAsync(async () => result = await _sut.GetCartAsync(id));

            // asserts
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Items, Is.Empty);

            // verify mocks
            _mockCartRepository.Verify(x => x.GetCartAsync(id), Times.Once);
            _mockCartRepository.Verify(x => x.PutCartAsync(It.Is<Cart>(p => p.Id == id)), Times.Once);
        }

        [Test]
        public void GetCartAsync_ExistingEntry_Return()
        {
            // arrange
            var id = "SomeSuitableValue";
            var cart = new Cart { Id = id };
            _mockCartRepository.Setup(x => x.GetCartAsync(id)).ReturnsAsync(cart);
            Cart result = null;

            // act
            Assert.DoesNotThrowAsync(async () => result = await _sut.GetCartAsync(id));

            // asserts
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(cart.Id));
            Assert.That(result.Items, Is.EqualTo(cart.Items));

            // verify mocks
            _mockCartRepository.Verify(x => x.GetCartAsync(id), Times.Once);
            _mockCartRepository.Verify(x => x.PutCartAsync(It.IsAny<Cart>()), Times.Never);
        }

        [Test]
        public void UpdateCartAsync_NewItems_Updated()
        {
            // arrange
            var id = "SomeSuitableValue";
            var catalogItems = new List<CatalogItem>
            {
                new CatalogItem
                    {
                        Id = 1,
                        Price = 100.99m,
                        Name = "DVD star wars 1 episode"
                    },
                new CatalogItem
                    {
                        Id = 2,
                        Price = 103.99m,
                        Name = "DVD star wars 2 episode"
                    },
                new CatalogItem
                    {
                        Id = 3,
                        Price = 99.99m,
                        Name = "DVD star wars 3 episode"
                    }
            };

            catalogItems.ForEach(x => _mockCatalogItemsRepository.Setup(s => s.GetCatalogItemAsync(x.Id)).ReturnsAsync(x));

            var cart = new Cart { Id = id };
            cart.AddItem(catalogItems[0], 3);
            cart.AddItem(catalogItems[1], 1);
            _mockCartRepository.Setup(x => x.GetCartAsync(id)).ReturnsAsync(cart);

            _mockCartRepository
                .Setup(x => x.UpdateCartAsync(It.IsAny<Cart>()))
                .ReturnsAsync(cart);

            var request = new CustomerCartDto(id)
            {
                Items = new[]
                {
                    new CustomerCartItemDto
                    {
                        CatalogItemId = catalogItems[0].Id,
                        Quantity = 1
                    },
                    new CustomerCartItemDto
                    {
                        CatalogItemId = catalogItems[2].Id,
                        Quantity = 5
                    },
                }
            };

            // act
            Cart result = null;
            Assert.DoesNotThrowAsync(async () => result = await _sut.UpdateCartAsync(request));

            // asserts
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(cart.Id));

            result.Items.Should().BeEquivalentTo(new[]
            {
                new
                {
                    CatalogItem = catalogItems[0],
                    Quantity = 1,
                },
                new
                {
                    CatalogItem = catalogItems[2],
                    Quantity = 5,
                }
            });

            // verify mocks
            _mockCartRepository.Verify(x => x.UpdateCartAsync(cart), Times.Once);
            _mockCatalogItemsRepository.Verify(x => x.GetCatalogItemAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void DeleteCartAsync_CallRepository_Success()
        {
            // arrange
            var id = "SomeSuitableId";

            // act
            Assert.DoesNotThrowAsync(async() => await _sut.DeleteCartAsync(id));

            // assert
            // verify mocks
            _mockCartRepository.Verify(x => x.DeleteAsync(id), Times.Once);
        }
    }
}
