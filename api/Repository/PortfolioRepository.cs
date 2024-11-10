using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
            
        }
        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        //This method filters Portfolio entries for the current user, pulls StockId values, and retrieves Stock details through navigation properties, building a list of stock objects.
        {
            return await _context.Portfolios.Where(u => u.AppUserId == user.Id).Select(portfolio => new Stock //portfolio here represents Portfolio records related to the user, allowing retrieval of Stock data via navigation properties.
            {
                Id = portfolio.StockId,
                Symbol = portfolio.Stock.Symbol, 
                CompanyName = portfolio.Stock.CompanyName,
                Purchase = portfolio.Stock.Purchase,
                LastDiv = portfolio.Stock.LastDiv,
                Industry = portfolio.Stock.Industry,
                MarketCap = portfolio.Stock.MarketCap,

            }).ToListAsync();
        }
        //
    }
}
