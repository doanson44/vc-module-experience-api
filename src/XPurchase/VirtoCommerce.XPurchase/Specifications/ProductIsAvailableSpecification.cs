namespace VirtoCommerce.XPurchase
{
    public class ProductIsAvailableSpecification
    {
        public virtual bool IsSatisfiedBy(CartProduct product, long requestedQuantity)
        {
            var result = new ProductIsBuyableSpecification().IsSatisfiedBy(product);

            if (result && product.Product.TrackInventory.GetValueOrDefault(false) && product.Inventory != null)
            {
                result = product.Inventory.AllowPreorder ||
                              product.Inventory.AllowBackorder ||
                              product.AvailableQuantity >= requestedQuantity;
            }

            return result;
        }

    }
}
