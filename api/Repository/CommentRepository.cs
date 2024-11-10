using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;
        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> DeleteAsync(int commentId)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (commentModel == null)
            {
                return null;
            }
            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;

        }

        public Task<List<Comment>> GetAllAsync()
        {
            return _context.Comments.Include(a=>a.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            // return await _context.Comments.FindAsync(id);
            return await _context.Comments.Include(a=>a.AppUser).FirstOrDefaultAsync(c=>c.Id == id);

        }

        public async Task<Comment?> UpdateAsync(int commentId, Comment comment)
        {
            var existingComment = await _context.Comments.FindAsync(commentId);
            if (existingComment == null)
            {
                return null;
            }
            existingComment.Title = comment.Title;
            existingComment.Content = comment.Content;
            await _context.SaveChangesAsync();
            return existingComment;
        }
    }
}