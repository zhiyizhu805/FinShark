using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using api.Data;
using api.Dtos.Stock;
using api.Models;


namespace api.Mappers
{
    public static class StockMappers
    {
        public static StockDto ToStokeDto(this Stock stockModel)
        {
            return new StockDto
            {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
            };
            
        }
    }
}

/* 
importing the top-level namespace (e.g., using api.Mappers;) allows access to all classes within that folder.

Extension Methods: 
Allow adding methods (like ToStockDto) to existing classes (like Stock) without modifying them directly.

this Stock stockModel: 
Specifies that ToStockDto is an extension method for Stock, so it can be called directly on Stock instances.

Manual Mapping: 
The ToStockDto method converts a Stock to StockDto by setting each property individually, providing precise control over the mapping process.
*/