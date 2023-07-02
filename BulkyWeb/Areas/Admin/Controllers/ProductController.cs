using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ProductController
        public ActionResult Index()
        {
            List<Product> product = _unitOfWork.ProductRepository.GetAll(includeproperties:"Category").ToList();
         
            return View(product);
        }

        // GET: ProductController/Create
        public ActionResult Upsert(int? id)
        {
           // ViewBag.CategoryList = CategoryList;
            ProductVM productVM = new ()
            {
                CategoryList = _unitOfWork.CategoryRepository
                 .GetAll()
                 .Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString()
                 }),
                Product = new Product()
            };

            if(id is not null || id > 0)
            {
                //Update
                productVM.Product = _unitOfWork.ProductRepository.Get(u => u.Id == id); 
            }
            //Create
            return View(productVM);
        }
        // POST: ProductController/Create
        [HttpPost]
        public ActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
      
            if (ModelState.IsValid)
            {
                string wwwRootpath = _webHostEnvironment.WebRootPath;
                if(file is not null)
                {
                    string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootpath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootpath,productVM.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using(var fileStream = new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.CategoryRepository
                 .GetAll()
                 .Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString()
                 });
                return View(productVM);
            }
        }
        


        //// GET: ProductController/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id is null || id is 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? productFromDb = _unitOfWork.ProductRepository.Get(u => u.Id == id);
        //    if (productFromDb is null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}

        //// POST: ProductController/Edit/product
        //[HttpPost]
        //public ActionResult Edit(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.ProductRepository.Update(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product edited successfully";
        //        return RedirectToAction("Index");
        //    }

        //    return View();
        //}

        // GET: ProductController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id is null || id is 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            if (productFromDb is null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        public ActionResult Delete(int? id, Product product)
        {
            if (id is null || id is 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            if (productFromDb is null)
            {
                return NotFound();
            }

            _unitOfWork.ProductRepository.Remove(productFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
