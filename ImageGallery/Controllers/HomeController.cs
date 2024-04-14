using ImageGallery.Data;
using ImageGallery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace ImageGallery.Controllers;
public class HomeController : Controller
{
    private readonly ImageGalleryDbContext _context;
    private readonly string _pathToSaveImages;

    public HomeController(ImageGalleryDbContext context, IWebHostEnvironment system)
    {
        _context = context;
        _pathToSaveImages = system.WebRootPath;
    }

    [HttpGet]
    public async Task<ActionResult<List<ImageModel>>> Index()
    {
        var images = await _context.Image.ToListAsync();
        return View(images);
    }

    [HttpGet]
    public ActionResult ImportImage()
    {
        return View();
    }

    [HttpGet]
    public async Task<ActionResult<ImageModel>> EditImage(int id)
    {
        var image = await _context.Image.FirstOrDefaultAsync(i => i.Id == id);
        return View(image);
    }

    [HttpGet]
    public async Task<ActionResult<ImageModel>> RemoveImage(int id)
    {
        var image = await _context.Image.FirstOrDefaultAsync(i => i.Id == id);
        return View(image);
    }

    [HttpPost]
    public async Task<ActionResult> RemoveImage(ImageModel image)
    {
        var imageExists = await _context.Image.FirstOrDefaultAsync(i => i.Id == image.Id);

        if (imageExists != null)
        {
            var pathExistingImage = _pathToSaveImages + "\\imagem\\" + imageExists.PathImage;

            if (System.IO.File.Exists(pathExistingImage))
            {
                System.IO.File.Delete(pathExistingImage);
            }

            _context.Remove(imageExists);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        else
        {
            TempData["MensagemErro"] = "Erro ao realizar o processo de exclusão!";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<ActionResult> ImportImage(IFormFile photo, string name)
    {
        try
        {
            if (photo != null && name != null)
            {
                var namePathImage = GenerateImagePath(photo);

                var newImage = new ImageModel
                {
                    PathImage = namePathImage,
                    Name = name,
                };

                _context.Add(newImage);
                await _context.SaveChangesAsync();

                return Redirect("Index");
            }
            else
            {
                TempData["MensagemErro"] = "É necessário incluir a imagem e o título!";
                return View();
            }


        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ImageModel>> EditImage(IFormFile photo, string name, int id)
    {
        var image = await _context.Image.FirstOrDefaultAsync(i => i.Id == id);

        try
        {
            if (photo != null && name != null)
            {
                var pathImageExists = _pathToSaveImages + "\\images\\" + image.PathImage;

                if (System.IO.File.Exists(pathImageExists))
                {
                    System.IO.File.Delete(pathImageExists);
                }

                var namePathImage = GenerateImagePath(photo);

                image.Name = name;
                image.PathImage = namePathImage;

                _context.Update(image);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");

            }
            else
            {
                TempData["MensagemErro"] = "É necessário incluir a imagem e o título!";
                return View(image);
            }

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }


    private string GenerateImagePath(IFormFile photo)
    {
        var uniqueCode = Guid.NewGuid().ToString().ToString();
        var namePathImage = photo.FileName.Replace(" ", "").ToLower() + uniqueCode + ".png";

        string pathToSaveImages = _pathToSaveImages + "\\images\\";

        if (!Directory.Exists(pathToSaveImages))
        {
            Directory.CreateDirectory(pathToSaveImages);
        }

        using (var stream = System.IO.File.Create(pathToSaveImages + namePathImage))
        {
            photo.CopyToAsync(stream).Wait();
        }

        return namePathImage;
    }

}
