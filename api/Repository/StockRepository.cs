using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace api.Repository
{
    public class StockRepository : IStockRepository 
    {
        private readonly ApplicationDBContext _context;
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if(stockModel == null)
            {
                return null;
            }
            //no need to add Async adding to a remove method. why?
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            // return  _context.Stocks.Include(s => s.Comments).ToListAsync();
              var stocks = _context.Stocks.Include(s => s.Comments).ThenInclude(a=>a.AppUser).AsQueryable();
              if (!string.IsNullOrWhiteSpace(query.CompanyName))
              {
                   stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
              }
              
              if (!string.IsNullOrWhiteSpace(query.Symbol))
              {
                  stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
              }

              if (!string.IsNullOrWhiteSpace(query.SortBy))
              {
                //.Equals() can take one parameter for default equality or two parameters to specify a comparison type, like case-insensitive matching with StringComparison.OrdinalIgnoreCase.
                //StringComparison.OrdinalIgnoreCase is used to make the comparison case-insensitive, meaning it will match "symbol", "SYMBOL", or any other case variation.
                if(query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase)) 
                {
                    stocks = query.IsDescending? stocks.OrderByDescending(s=>s.Symbol) : stocks.OrderBy(s=>s.Symbol);
                }
              }

              var skipNumber = (query.PageNumber - 1) * query.PageSize;
              
              return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
            //   return await stocks.ToListAsync();

        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.Include(s => s.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s=>s.Symbol.ToLower() == symbol.ToLower());
        }

        public async Task<bool> StockExit(int id)
        {
            return  await _context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateDto)
        {
            var existingStock = _context.Stocks.FirstOrDefault(x => x.Id == id);
            if (existingStock == null)
            {
                return null;
            }
            existingStock.Symbol = updateDto.Symbol;
            existingStock.CompanyName = updateDto.CompanyName;
            existingStock.Purchase = updateDto.Purchase;
            existingStock.LastDiv = updateDto.LastDiv;
            existingStock.Industry = updateDto.Industry;
            existingStock.MarketCap = updateDto.MarketCap;
            await _context.SaveChangesAsync();
            return existingStock;
        }

        
    }
}

/*
1.Task<Stock?> with FirstOrDefault:
Task<Stock?> represents an asynchronous method that returns either a Stock object or null. The nullable (?) type indicates that FirstOrDefault might return null if no match is found.

2.Why AddScoped in Program.cs:
AddScoped<IStockRepository, StockRepository>() registers IStockRepository with StockRepository as a scoped service, creating a new instance per HTTP request.
Using IStockRepository allows flexibility, enabling a different implementation without modifying the controller.

3. alt + . auto implement interface

=== Steps to Refactor Database Logic into Repository ===

1.Create Repository Interface:
Defined IStockRepository to specify the contract for methods like GetAllAsync and GetByIdAsync.
Implement Repository Class:

2.Created StockRepository, implemented IStockRepository, and injected ApplicationDBContext for database interactions.
Inject Repository into Controller:

3.Used IStockRepository in StockController, replacing direct database calls, improving modularity.
Register in Program.cs:

4.Added builder.Services.AddScoped<IStockRepository, StockRepository>(); to make StockRepository available as a DI service.

*/