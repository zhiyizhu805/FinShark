using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Mappers;
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
            var stocks = _context.Stocks.ToList().Select(s => s.ToStokeDto());
            //Select will return an immutable array/list
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

            return Ok(stock.ToStokeDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromStockDto();
            _context.Stocks.Add(stockModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id}, stockModel.ToStokeDto());
        }


    }
}

/*
Steps to create Dtos and use Mapper to map the model class to a Dto class:
1. Create DTO for a Model Class:
   - Define a Data Transfer Object (DTO) class with properties that match or are derived from those in the model class.
   -This class usually has only the necessary fields for data transfer and may exclude sensitive or unnecessary data.
2. Create Mapper as an Extension Method for the Model Class:
   - Write a static extension method in a separate static class (e.g., StockMappers) to map the model to the DTO.
   - Use this ModelType model as the first parameter to create an extension method, allowing you to call it directly on instances of the model class.
3. Map the Model to DTO in the Controller:
   - In the controller, when you retrieve a model instance (e.g., Stock), call the extension method (e.g., stock.ToStockDto()) to map the model to a DTO before returning it.
********************************************************************
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