using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddCartItemsBulkCommandHandler : IRequestHandler<AddCartItemsBulkCommand, BulkCartResult>
    {
        private readonly ICartAggregateRepository _cartAggrRepository;
        private readonly IProductSearchService _productSearchService;
        private readonly IMediator _mediator;

        public AddCartItemsBulkCommandHandler(
            ICartAggregateRepository cartAggrRepository,
            IProductSearchService productSearchService,
            IMediator mediator)
        {
            _cartAggrRepository = cartAggrRepository;
            _productSearchService = productSearchService;
            _mediator = mediator;
        }

        public async Task<BulkCartResult> Handle(AddCartItemsBulkCommand request, CancellationToken cancellationToken)
        {
            var result = new BulkCartResult();
            var cartItemsToAdd = new List<NewCartItem>();

            // find missing skus
            var products = await FindProductsBySkuAsync(request);
            foreach (var item in request.CartItems)
            {
                var product = products.FirstOrDefault(x => x.Code == item.ProductSku);
                if (product != null)
                {
                    var newCartItem = new NewCartItem(product.Id, item.Quantity);
                    cartItemsToAdd.Add(newCartItem);
                }
                else
                {
                    var error = CartErrorDescriber.BulkInvalidProductError(nameof(CatalogProduct), item.ProductSku);
                    result.Errors.Add(error);
                }
            }

            // send Add to Cart command
            var command = new AddCartItemsCommand
            {
                CartId = request.CartId,
                StoreId = request.StoreId,
                CartType = request.CartType,
                CartName = request.CartName,
                UserId = request.UserId,
                CurrencyCode = request.CurrencyCode,
                CultureName = request.CultureName,
                CartItems = cartItemsToAdd.ToArray(),
            };

            var cartAggregate = await _mediator.Send(command, cancellationToken);

            result.Cart = cartAggregate;

            // transform cart product unavaliable validation erorrs to human readable
            var lineItemErrors = cartAggregate.ValidationErrors
                .OfType<CartValidationError>()
                .Where(x => x.ObjectType == "CartProduct" && x.ErrorCode == "CART_PRODUCT_UNAVAILABLE")
                .Select(x =>
                {
                    var sku = products.FirstOrDefault(p => p.Id == x.ObjectId)?.Code ?? x.ObjectId;
                    var error = CartErrorDescriber.BulkProductUnavailableError(nameof(CatalogProduct), sku);
                    return error;
                });

            result.Errors.AddRange(lineItemErrors);

            return result;
        }

        protected virtual async Task<IList<CatalogProduct>> FindProductsBySkuAsync(AddCartItemsBulkCommand request)
        {
            var productSkus = request.CartItems.Select(x => x.ProductSku).ToList();

            //find products
            var productSearchRequest = new ProductSearchCriteria
            {
                Take = productSkus.Count,
                Skus = productSkus,
                SearchInVariations = true,
                ResponseGroup = ItemResponseGroup.ItemInfo.ToString(),
            };

            var searchProductResult = await _productSearchService.SearchProductsAsync(productSearchRequest);
            return searchProductResult.Results;
        }
    }
}
