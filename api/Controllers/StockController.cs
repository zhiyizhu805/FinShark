using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc;


namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var stocks = _context.Stocks.ToList();
            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var stock = _context.Stocks.Find(id);

            if (stock == null)
            {
                return NotFound(); 
            }

            return Ok(stock);
        }


    }
}

/*
ControllerBase and IActionResult:

ControllerBase is a lightweight base class for API controllers in ASP.NET Core, providing core HTTP response methods.
IActionResult is a flexible return type that lets you return different HTTP responses (Ok(), NotFound(), etc.).
Ok() and Ok(stocks):

Ok() returns HTTP 200, indicating success.
Ok(stocks) returns HTTP 200 with stocks as the response data.
Deferred Execution with ToList():

ToList() triggers immediate execution of the query, converting data to a list, which ends deferred execution (where the query runs only when needed).
Find Method:

Find(id) is optimal for retrieving items by primary key, using caching to reduce database calls if the item was recently accessed.
NotFound():

Returns HTTP 404, indicating the item wasn’t found in the database, commonly used when Find returns null.

*/