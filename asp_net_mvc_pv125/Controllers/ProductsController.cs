﻿using BusinessLogic.Services;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace asp_net_mvc_pv125.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductsService productsService;
        private readonly ShopDbContext context;
        public ProductsController(IProductsService productsService, ShopDbContext context)
        {
            this.productsService = productsService;
            this.context = context;
        }


        private void LoadCategories()
        {
            // ViewData is nothing but a dictionary of objects and it is accessible by string as key
            // ViewData["List"] = new List<int>() { 1, 2, 3 };

            // ViewBag is a dynamic property (dynamic keyword which is introduced in .net framework 4.0)
            ViewBag.CategoryList = new SelectList(productsService.GetCategories(), nameof(Category.Id), nameof(Category.Name));
        }

        // GET: ~/Products/Index
        [AllowAnonymous]
        public IActionResult Index(string searchString)
        {
            ViewData["SearchString"] = searchString;
            var cars = from m in context.Products
                       select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(s => s.Name.StartsWith(searchString));
            }

            return View(cars);
        }

        // GET: ~/Products/Details/{id}
        [AllowAnonymous]
        public IActionResult Details(int id, string returnUrl = null)
        {
            if (id < 0) return BadRequest(); // error 400

            // get product by id
            var product = productsService.Get(id);

            if (product == null) return NotFound(); // error 404

            ViewBag.ReturnUrl = returnUrl;
            return View(product);
        }

        // GET: ~/Products/Create
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }

        // POST: ~/Products/Create
        [HttpPost]
        public IActionResult Create(Product product) // product - model
        {
            // validations
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(product);
            }

            productsService.Create(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: ~/Products/Edit/{id}
        public IActionResult Edit(int id)
        {
            // get element by id
            var product = productsService.Get(id);

            if (product == null) return NotFound();

            LoadCategories();
            return View(product);
        }

        // POST: ~/Products/Edit
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            // validations
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return View(product);
            }

            productsService.Edit(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: ~/Products/Delete/{id}
        public IActionResult Delete(int id)
        {
            productsService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
