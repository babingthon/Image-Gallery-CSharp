using ImageGallery.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Data;

public class ImageGalleryDbContext : DbContext
{
    public ImageGalleryDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ImageModel> Image { get; set; }
}
