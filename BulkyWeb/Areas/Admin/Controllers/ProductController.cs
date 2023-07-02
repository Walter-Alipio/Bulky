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
            List<Product> productList = _unitOfWork.ProductRepository.GetAll(includeproperties:"Category").ToList();
         
            return View(productList);
        }

        // GET: ProductController/Create
        // GET: ProductController/Update
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
        // POST: ProductController/Update
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

        #region API CALLS
        [HttpGet]
        public ActionResult GetAll() 
        {
            List<Product> productList = _unitOfWork.ProductRepository.GetAll(includeproperties:"Category").ToList();
            return Json(new {data = productList });
        }
        [HttpDelete]
        public ActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            if (productToBeDeleted is null)
            {
                return Json(new { succes = false, message = "Error while deleting" });
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.Trim('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.ProductRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { succes = true, message = "Delete Successful" });
        }
        #endregion
    }
}
