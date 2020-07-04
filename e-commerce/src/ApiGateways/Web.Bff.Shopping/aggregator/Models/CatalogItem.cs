namespace e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureUri { get; set; }
    }
}