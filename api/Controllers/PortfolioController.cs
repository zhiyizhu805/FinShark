using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepo, IPortfolioRepository portfolioRepo)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername(); // the User is inherited from the controlerbase
            var appUser = await _userManager.FindByNameAsync(username); 
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var stockModel = await _stockRepo.GetBySymbolAsync(symbol);
            if (stockModel == null) return BadRequest("No stock found!");
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);
            if(userPortfolio.Any(p=>p.Symbol.ToLower() == symbol.ToLower())) return BadRequest("Symbol existed! Fail to add!");
            var portfolioModel = new Portfolio
            {
                AppUserId = appUser.Id,
                StockId = stockModel.Id,
            };

            await _portfolioRepo.CreateAsync(portfolioModel);
            return Created();

        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);
            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();
            if (filteredStock.Count() == 1)
            {
                var portfolioModel = await _portfolioRepo.DeleteAsync(appUser,symbol);
            }
            else
            {
                return BadRequest("Fail to delete, no matched stock found in your portfolio!");
            }
            return NoContent();
        }
        
    }
}

//[Authorize] ensures only authenticated users can access this endpoint. ASP.NET validates the JWT token and checks user claims for authorization.
//User Property: User is available in ControllerBase, representing the authenticated user's identity from ClaimsPrincipal in HttpContext. It contains details like username and roles, derived from the JWT token.
//to get the userPortfolio, we will reach the database,the portfolio table, we are going to pull out all records that are associated with the given user that is logged in with the data we got from the claims and we are going to return all these stocks taht are associated with that user, how do we do that?
    //1.create interface IPortfolioRepository Task<List<Stock>> GetUserPortfolio(AppUser user);
    //2.implement it