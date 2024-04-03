using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Helpers;
using ecommerce.Models;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Services
{
    public class BlogService : IBlogService
    {
        private readonly EcommerceContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;

        public BlogService(EcommerceContext context, IUnitOfWork unitOfWork,IUploadFilesService uploadFilesService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _uploadFilesService = uploadFilesService;
        }

        // CRUD operations for Blog

        public async Task<ApiResponse<bool>> CreateBlogAsync(BlogDto blogDto)
        {
            // update the image path
            var imagePath = await _uploadFilesService.UploadFileAsync(blogDto.Image, Contains.BlogImageFolder);
            if (!imagePath.Status)
            {
                return new ApiResponse<bool> { Data = false };
            }
            var newBlog = new Blog
            {
                Title = blogDto.Title,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = blogDto.CreatedBy,
                UpdatedBy = blogDto.UpdatedBy,
                Image = imagePath.Data,
                Slug = StringHelper.GenerateSlug(blogDto.Title)
            };

            foreach (var categoryId in blogDto.CategoryIds)
            {
                var category = await _context.BlogCategories.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
                if (category != null)
                {
                    newBlog.Categories.Add(category);
                }
            }

            await _context.Blogs.AddAsync(newBlog);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<bool> { Data = true, Status = true  };
        }

        public async Task<ApiResponse<bool>> UpdateBlogAsync(int id,BlogDto blog)
        {
            // update the image path
            var imagePath = await _uploadFilesService.UploadFileAsync(blog.Image, Contains.BlogImageFolder);
            if (!imagePath.Status)
            {
                return new ApiResponse<bool> { Data = false };
            }
            var existingBlog = await _context.Blogs.FirstOrDefaultAsync(x => x.BlogId == id);
            await _uploadFilesService.RemoveFileAsync(existingBlog.Image, Contains.BlogImageFolder);
            if (existingBlog != null)
            {
                existingBlog.Title = blog.Title;
                existingBlog.UpdatedAt = DateTime.Now;
                existingBlog.UpdatedBy = blog.UpdatedBy;
                existingBlog.Image = imagePath.Data;
                existingBlog.Slug = StringHelper.GenerateSlug(blog.Title);

                // remove image when the image is updated
                await _unitOfWork.SaveChangesAsync();
               
                return new ApiResponse<bool> { Data = true, Status = true };
            }
            return new ApiResponse<bool> { Data = true, Status = true  };
        }

        public async Task<ApiResponse<bool>> DeleteBlogAsync(int id)
        {
            var blogToDelete = await _context.Blogs.FindAsync(id);

            if (blogToDelete != null)
            {
                _context.Blogs.Remove(blogToDelete);
                await _unitOfWork.SaveChangesAsync();
                // remove image when the blog is deleted
                await _uploadFilesService.RemoveFileAsync(blogToDelete.Image, Contains.BlogImageFolder);
            }
            return new ApiResponse<bool> { Data = true , Status = true };
        }

        public async Task<ApiResponse<bool>> DeleteAllBlogsAsync()
        {
            var blogs = await _context.Blogs.ToListAsync();
            _context.Blogs.RemoveRange(blogs);
            await _unitOfWork.SaveChangesAsync();
            // remove all images when all blogs are deleted
            foreach (var blog in blogs)
            {
                await _uploadFilesService.RemoveFileAsync(blog.Image, Contains.BlogImageFolder);
            }
            
            return new ApiResponse<bool> { Data = true, Status = true  };
        }

        public async Task<ApiResponse<bool>> DeleteMultipleBlogsAsync(List<int> ids)
        {
            var blogsToDelete = await _context.Blogs.Where(blog => ids.Contains(blog.BlogId)).ToListAsync();

            if (blogsToDelete.Any())
            {
                _context.Blogs.RemoveRange(blogsToDelete);
                await _unitOfWork.SaveChangesAsync();
                // remove all images when multiple blogs are deleted
                foreach (var blog in blogsToDelete)
                {
                    await _uploadFilesService.RemoveFileAsync(blog.Image, Contains.BlogImageFolder);
                }
                return new ApiResponse<bool> { Data = true, Status = true };
            }
            return new ApiResponse<bool> { Data = false , Status = true };
        }

        public async Task<ApiResponse<List<BlogAllDto>>> GetAllBlogsAsync(int pageNumber, int pageSize)
        {
            var blogs = await _context.Blogs
                .Include(blog => blog.Categories)
                .Include(blog => blog.Details)
                .OrderByDescending(blog => blog.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            // get image url
            foreach (var blog in blogs)
            {
                blog.Image = _uploadFilesService.GetFilePath(blog.Image, Contains.BlogImageFolder);
            }
            var blogDtos = blogs.Select(blog => new BlogAllDto
            {
                Id = blog.BlogId,
                Title = blog.Title,
                CreatedBy = blog.CreatedBy,
                UpdatedBy = blog.UpdatedBy,
                Image = blog.Image,
                CategoryIds = blog.Categories.Select(category => category.CategoryId).ToList(),
                Categories = blog.Categories.Select(category => category.Name).ToList(),
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            }).ToList();
            return new ApiResponse<List<BlogAllDto>> { Data = blogDtos, Status = true  };
        }

        public async Task<ApiResponse<BlogAllDto>> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs.Include(u=>u.Categories).FirstOrDefaultAsync(x=>x.BlogId==id);
            blog.Image = _uploadFilesService.GetFilePath(blog.Image, Contains.BlogImageFolder);
            var blogDto = new BlogAllDto
            {
                Id = blog.BlogId,
                Title = blog.Title,
                CreatedBy = blog.CreatedBy,
                UpdatedBy = blog.UpdatedBy,
                Image = blog.Image,
                CategoryIds = blog.Categories.Select(category => category.CategoryId).ToList(),
                Categories = blog.Categories.Select(category => category.Name).ToList(),
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            };
            return new ApiResponse<BlogAllDto> { Data = blogDto, Status = true  };
        }

        public async Task<ApiResponse<List<BlogAllDto>>> SearchBlogsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var blogs = await _context.Blogs
                .Where(blog => blog.Title.Contains(searchTerm) || string.IsNullOrEmpty(searchTerm))
                .OrderByDescending(blog => blog.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            // get image url
            foreach (var blog in blogs)
            {
                blog.Image = _uploadFilesService.GetFilePath(blog.Image, Contains.BlogImageFolder);
            }
            var blogDtos = blogs.Select(blog => new BlogAllDto
            {
                Id = blog.BlogId,
                Title = blog.Title,
                CreatedBy = blog.CreatedBy,
                UpdatedBy = blog.UpdatedBy,
                Image = blog.Image,
                CategoryIds = blog.Categories.Select(category => category.CategoryId).ToList(),
                Categories = blog.Categories.Select(category => category.Name).ToList(),
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            }).ToList();

            return new ApiResponse<List<BlogAllDto>> { Data = blogDtos , Status = true };
        }



        public async Task<ApiResponse<bool>> CreateBlogDetailAsync(int blogId, BlogDetailDto detail)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(X=>X.BlogId == blogId);
            if (blog != null)
            {
                var newDetail = new BlogDetail
                {
                    Content = detail.Content,
                    Description = detail.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                blog.Details.Add(newDetail);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<bool> { Data = true, Status = true  };
            }
            return new ApiResponse<bool> { Data = false, Status = false  };
        }

        public async Task<ApiResponse<bool>> UpdateBlogDetailAsync(int id,BlogDetailDto detail)
        {
            var existingDetail = await _context.BlogDetails.FirstOrDefaultAsync(x => x.BlogDetailId == id);
            if (existingDetail != null)
            {
                existingDetail.Content = detail.Content;
                existingDetail.Description = detail.Description;
                existingDetail.UpdatedAt = DateTime.Now;
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<bool> { Data = true , Status = true };
            }
            return new ApiResponse<bool> { Data = false, Status = false  };
        }

        public async Task<ApiResponse<bool>> DeleteBlogDetailAsync(int id)
        {
            var detailToDelete = await _context.BlogDetails.FirstOrDefaultAsync(x => x.BlogDetailId == id);
            if (detailToDelete != null)
            {
                _context.BlogDetails.Remove(detailToDelete);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<bool> { Data = true , Status = true };
            }
            return new ApiResponse<bool> { Data = false, Status = false };
        }


        public async Task<ApiResponse<bool>> CreateBlogCategoryAsync(BlogCategoryDto category)
        {
            var newCategory = new BlogCategory
            {
                Name = category.Name,
                Description = category.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = category.CreatedBy,
                UpdatedBy = category.UpdatedBy
            };
            await _context.BlogCategories.AddAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<bool> { Data = true, Status = true  };
        }

        public async Task<ApiResponse<bool>> UpdateBlogCategoryAsync(int id,BlogCategoryDto category)
        {
            var existingCategory = await _context.BlogCategories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.UpdatedAt = DateTime.Now;
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<bool> { Data = true, Status = true  };
            }
            return new ApiResponse<bool> { Data = false , Status = false };
        }

        public async Task<ApiResponse<bool>> DeleteBlogCategoryAsync(int id)
        {
            var categoryToDelete = await _context.BlogCategories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (categoryToDelete != null)
            {
                _context.BlogCategories.Remove(categoryToDelete);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<bool> { Data = true, Status = true  };
            }
            return new ApiResponse<bool> { Data = false , Status = false };
        }

        public async Task<ApiResponse<List<BlogCategoryAllDto>>> GetAllBlogCategoriesAsync(PagingForBlogCategory? paging = null)
        {
            if(paging == null){
                // get all categories
                var categoriesNotPaging = await _context.BlogCategories.ToListAsync();
                var categoryDtoNotPagings = categoriesNotPaging.Select(category => new BlogCategoryAllDto
                {
                    Name = category.Name,
                    Description = category.Description,
                    CreatedBy = category.CreatedBy,
                    UpdatedBy = category.UpdatedBy,
                    CategoryId = category.CategoryId,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                }).ToList();
                return new ApiResponse<List<BlogCategoryAllDto>> { Data = categoryDtoNotPagings , Status = true };
            }
            var categories = _context.BlogCategories.AsQueryable();
            if (!string.IsNullOrEmpty(paging.Search) || !string.IsNullOrWhiteSpace(paging.Name))
            {
                categories = categories.Where(category => category.Name.Contains(paging.Search) || category.Name.Contains(paging.Name));
            }
            if(paging.SortByDate){
                categories = categories.OrderBy(category => category.CreatedAt);
            }
            else
            {
                categories = categories.OrderByDescending(category => category.CreatedAt);
            }
            if (paging.PageSize > 0)
            {
                categories = categories.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);
            }
            var categoryDtos = categories.Select(category => new BlogCategoryAllDto
            {
                Name = category.Name,
                Description = category.Description,
                CreatedBy = category.CreatedBy,
                UpdatedBy = category.UpdatedBy,
                CategoryId = category.CategoryId,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            }).ToList();
            return new ApiResponse<List<BlogCategoryAllDto>> { Data = categoryDtos , Status = true };
        }

        public async Task<ApiResponse<BlogCategoryAllDto>> GetBlogCategoryByIdAsync(int id)
        {
            var category = await _context.BlogCategories.FirstOrDefaultAsync(x=>x.CategoryId == id);
            if (category == null)
            {
                return new ApiResponse<BlogCategoryAllDto> { Data = null, Status =false };
            }
            var categoryDto = new BlogCategoryAllDto
            {
                Name = category.Name,
                Description = category.Description,
                CreatedBy = category.CreatedBy,
                UpdatedBy = category.UpdatedBy,
                CategoryId = category.CategoryId,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
            return new ApiResponse<BlogCategoryAllDto> { Data = categoryDto, Status = true };
        }

        public async Task<ApiResponse<bool>> DeleteMultipleBlogCategoriesAsync(List<int> ids)
        {
            var categoriesToDelete = await _context.BlogCategories.Where(category => ids.Contains(category.CategoryId)).ToListAsync();

            if (categoriesToDelete.Any())
            {
                _context.BlogCategories.RemoveRange(categoriesToDelete);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<bool> { Data = true, Status = true  };
            }
            return new ApiResponse<bool> { Data = false };
        }

        public async Task<ApiResponse<bool>> DeleteAllBlogCategoriesAsync()
        {
            var categoriesToDelete = await _context.BlogCategories.ToListAsync();

            if (categoriesToDelete.Any())
            {
                _context.BlogCategories.RemoveRange(categoriesToDelete);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<bool> { Data = true , Status = true };
            }
            return new ApiResponse<bool> { Data = false };
        }

        public async Task<ApiResponse<BlogAllDto>> GetBlogBySlugAsync(string slug)
        {
            var blog = await _context.Blogs.Include(u=>u.Categories).FirstOrDefaultAsync(blog => blog.Slug == slug);
            if (blog != null)
            {
                blog.Image = _uploadFilesService.GetFilePath(blog.Image, Contains.BlogImageFolder);
                var blogDto = new BlogAllDto
                {
                    Id = blog.BlogId,
                    Title = blog.Title,
                    CreatedBy = blog.CreatedBy,
                    UpdatedBy = blog.UpdatedBy,
                    Image = blog.Image,
                    CategoryIds = blog.Categories.Select(category => category.CategoryId).ToList(),
                    Categories = blog.Categories.Select(category => category.Name).ToList(),
                    CreatedAt = blog.CreatedAt,
                    UpdatedAt = blog.UpdatedAt
                };
                return new ApiResponse<BlogAllDto> { Data = blogDto, Status = true  };
            }
            return new ApiResponse<BlogAllDto> { Data = null , Status = false };
        }

        public async Task<ApiResponse<BlogDetailAllDto>> GetBlogDetailByIdAsync(int id)
        {
            var blogDetail = await _context.BlogDetails.FirstOrDefaultAsync(blogDetail => blogDetail.BlogDetailId == id);
            var blogDetailDto = new BlogDetailAllDto
            {
                BlogDetailId = blogDetail.BlogDetailId,
                Content = blogDetail.Content,
                Description = blogDetail.Description,
                CreatedAt = blogDetail.CreatedAt,
                UpdatedAt = blogDetail.UpdatedAt
            };
            return new ApiResponse<BlogDetailAllDto> { Data = blogDetailDto, Status = true  };

        }
    }
}
