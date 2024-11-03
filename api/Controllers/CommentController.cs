using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(ICommentRepository commentRepo,IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            var comments = await _commentRepo.GetAllAsync();
            var CommentsDto = comments.Select(s => s.ToCommentDto());
            return Ok(CommentsDto);

        }

        [HttpGet("{commentId:int}")]
        public async Task<IActionResult> GetById([FromRoute] int commentId)
        {
            if(!ModelState.IsValid) // this is inherited from ControllerBase
                return BadRequest(ModelState);
            var commentModel = await _commentRepo.GetByIdAsync(commentId);
            if (commentModel == null)
            {
                return NotFound();
            }
            return Ok(commentModel.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId,[FromBody] CreateCommentRequestDto commentDto)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            if(!await _stockRepo.StockExit(stockId))
            {
                return BadRequest("No stock found.");
            }
            var commentModel = await _commentRepo.CreateAsync(commentDto.ToCommentFromCreate(stockId));
            return CreatedAtAction(nameof(GetById), new {id = commentModel.Id}, commentModel.ToCommentDto());


        }

        [HttpPut("{commentId:int}")]
        public async Task<IActionResult> Update([FromRoute] int commentId, [FromBody] UpdateCommentRequestDto updateCommentDto)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            //remember to pass Comment type to UpdateAsync method.
            var commentModel = await _commentRepo.UpdateAsync(commentId,updateCommentDto.ToCommentFromUpdate());
            if (commentModel == null)
            {
                return NotFound("No comment founded.");
            }
            return Ok(commentModel.ToCommentDto());

        }

        [HttpDelete("{commentId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int commentId)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            var commentModel = await _commentRepo.DeleteAsync(commentId);
            if (commentModel == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}

/*
********************************************************************
Data Validation:
1. Add data validation to route parameters
Ensure route parameters have specified data types (e.g., commentId:int) for validation and error prevention.

2.How to add data validation in Dto:
2.1 Add data validation in Dto(eg. updateDto or createDto that need to receive data).（Dont directly add data validation in models as it will apply globally.）
2.2 In controller, explictly tell each route to use data annotation:            if(!ModelState.IsValid) return BadRequest(ModelState);
********************************************************************
>>>> Steps to Implement a One-to-Many Relationship and HttpGet:

To include related data (e.g., Commnent in Stock/CommentDto in StockDto), you followed these steps:

1.Define the Relationship in the Parent Model: Add public List<Comment> Comments { get; set; } = new List<Comment>(); to Stock to establish the one-to-many relationship with Comment. Additionally, in the Comment model, add public int? StockID { get; set; } as a foreign key and public Stock? Stock { get; set; } to link back to the parent Stock.

2.Add Related Data in the DTO Class: In StockDto, add a List<CommentDto> property to represent the comments in the data transfer object.

3.Map the Relationship in a Mapper Class: In ToStockDto within StockMappers, use .Select(c => c.ToCommentDto()).ToList() to convert each Comment to CommentDto.

4.Eagerly Load Related Entities in Repository: Use .Include(s => s.Comments) in StockRepository to load comments with each Stock query, reducing additional queries and improving performance.

5.Install Newstonsoft.Json and Microsoft.AspNetCore.Mvc.NewtonsoftJson and add that to the registered service to prevent object cycles

6.Prevent Object Cycles: In Program.cs, configure JSON serialization with ReferenceLoopHandling.Ignore to prevent serialization issues due to circular references.

********************************************************************

>>>> Steps to Add HttpPost (Create Comment)
1.Check Stock Existence:
    Verify if the stockId exists by implementing a StockExists method in the IStockRepository interface and StockRepository class.
    Inject StockRepository into CommentController.
    If Stock Not Found: Return BadRequest("No stock found.").
    If Stock Found: Proceed to the next step.
2.Map CreateCommentRequestDto to Comment Model:
    In the controller, use a mapper to convert CreateCommentRequestDto to a Comment model, adding stockId from the route.
    Save Comment in Repository:
3.Add a CreateAsync method in ICommentRepository and CommentRepository to save the new comment to the database.

My Mistake: Avoid including stockId in CreateCommentRequestDto; the DTO should only have user-provided fields (title and content). The stockId is passed through the route and added during mapping.
********************************************************************
>>>> Steps to Add HttpPut (Update Comment)
1.Check Comment Existence:
    In the controller, verify if a comment with the provided commentId exists in the database.
    If Comment Not Found: Return NotFound.
2.Map UpdateCommentRequestDto to Comment Model:
    Use a mapper to convert UpdateCommentRequestDto (containing only title and content) to a Comment model.
    Note: Do not pass UpdateCommentRequestDto directly to repository methods; always map it to the model first in the controller.
3.Update Comment in Repository:
    Implement an UpdateAsync method in ICommentRepository and CommentRepository to save the changes to the database.
********************************************************************
>>>> Steps to Add HttpDelete (Delete Comment)
1.Check Comment Existence:
    In the controller, check if the comment exists in the database by commentId.
    If Comment Not Found: Return NotFound.
2.Delete Comment in Repository:
    Add a DeleteAsync method in ICommentRepository and CommentRepository to remove the comment from the database.
********************************************************************
>>>> Key Points to Remember
1.DTO Mapping in Controller: Always map DTOs to models within the controller before passing them to repository methods. This keeps the repository focused solely on database operations and ensures that it receives complete, correctly structured data.

2.Minimal DTO Structure: DTOs should only include fields that the user directly provides (e.g., title and content for comments). Any additional IDs or relationships (like stockId) should be handled within the controller or through mapping logic.

*/