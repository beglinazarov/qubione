using Catalog.API.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using static CatalogApi.Catalog;

namespace Catalog.API.Grpc
{
    public class CatalogService : CatalogBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly CatalogSettings _settings;
        private readonly ILogger _logger;
        public CatalogService(CatalogContext dbContext, IOptions<CatalogSettings> settings, ILogger<CatalogService> logger)
        {
            _settings = settings.Value;
            _catalogContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger;
        }
    }
}