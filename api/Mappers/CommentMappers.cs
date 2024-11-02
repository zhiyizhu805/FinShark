using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Repository;

namespace api.Mappers
{
    public static class CommentMappers
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreateOn = commentModel.CreateOn,
                StockID = commentModel.StockID,
            };
        }

        public static Comment ToCommentFromCreate(this CreateCommentRequestDto commentDto, int stockId)
        {
            return new Comment{
                Title = commentDto.Title,
                Content = commentDto.Content,
                StockID = stockId,
            };
        }

        public static Comment ToCommentFromUpdate(this UpdateCommentRequestDto updateCommentDto)
        {
            return new Comment{
                Title = updateCommentDto.Title,
                Content = updateCommentDto.Content,
            };
        }

    }
}