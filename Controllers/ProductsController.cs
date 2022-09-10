using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS.Data;
using IMS.Models;
using IMS.Models.DTO;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using Microsoft.CodeAnalysis.Operations;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace IMS.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {

            if (_context.Branch != null)
            {
                var branch = from b in _context.Branch
                             select new DTOBranch()
                             {
                                 BranchId = b.BranchId,
                                 BranchName = b.BranchName
                             };

                ViewData["Branch"] = await branch.ToListAsync();
            }
            else
                ViewData["Product"] = Problem("Entity set 'ApplicationDbContext.Branch'  is null.");


            if (_context.Product != null)
            {
                ViewData["Product"] = new List<Product>();
                ViewData["Product"] = await _context.Product.ToListAsync(); 
            }
            else
                ViewData["Product"] = Problem("Entity set 'ApplicationDbContext.Product'  is null.");
    
            return View();
                    
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpGet]
        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create(string ProductName, string ProductCode,string ?Barcode,int CurrentQuantity,string Description, IFormFile? ProductImageUrl, double? DefaultBuyingPrice, double? DefaultSellingPrice, int? LowStockLevel, bool IsActive,int BranchId)
        {
            if (ModelState.IsValid)
            {
                Product p = new Product();

            if (ProductImageUrl !=null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ProductImageUrl.CopyToAsync(memoryStream);


                    if (memoryStream.Length < 2097152)
                    {
                        p.ProductImageUrl = memoryStream.GetBuffer();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
            }else
            {
                    p.ProductImageUrl = null;
            }

            p.ProductName = ProductName;      
            p.ProductCode = ProductCode;
            p.Barcode = Barcode;
            p.Description = Description;
            p.CurrentQuantity = CurrentQuantity;
            p.DefaultBuyingPrice = DefaultBuyingPrice;
            p.DefaultSellingPrice = DefaultSellingPrice;
            p.LowStockLevel = LowStockLevel;
            p.IsActive = IsActive;
            p.BranchId = BranchId;
         
                _context.Add(p);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ProductId,string ProductName, string ProductCode, string? Barcode, int CurrentQuantity, string Description,IFormFile? ProductImageUrl, double? DefaultBuyingPrice, double? DefaultSellingPrice, int? LowStockLevel, bool IsActive, int BranchId)
        {
            Product p = _context.Product.FirstOrDefault(item => item.ProductId == ProductId);
            if (ModelState.IsValid)
            {
                
                if (ProductImageUrl != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ProductImageUrl.CopyToAsync(memoryStream);


                        if (memoryStream.Length < 2097152)
                        {
                            p.ProductImageUrl = memoryStream.GetBuffer();
                        }
                        else
                        {
                            ModelState.AddModelError("File", "The file is too large.");
                        }
                    }
                }
                p.ProductName = ProductName;
                p.ProductCode = ProductCode;
                p.Barcode = Barcode;
                p.Description = Description;
                p.CurrentQuantity = CurrentQuantity;
                p.DefaultBuyingPrice = DefaultBuyingPrice;
                p.DefaultSellingPrice = DefaultSellingPrice;
                p.LowStockLevel = LowStockLevel;
                p.IsActive = IsActive;
                p.BranchId = BranchId;


                try
                { 
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(p.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
