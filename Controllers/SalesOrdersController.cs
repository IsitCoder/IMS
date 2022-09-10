using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS.Data;
using IMS.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Security.Claims;

namespace IMS.Controllers
{
    public class SalesOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SalesOrders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SalesOrder.Include(s => s.Branch).Include(s => s.Customer).Include(s => s.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SalesOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SalesOrder == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder
                .Include(s => s.Branch)
                .Include(s => s.Customer)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // GET: SalesOrders/Create
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName");
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName");
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName");
            return View();
        }


        //Check Product Stock
        public bool CheckProductStock(int currentquantity,int orderquantity)
        {
            if (currentquantity < orderquantity)
            {          
                return false;
            }
            return true;
        }

        // POST: SalesOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesOrderId,CustomerId,BranchId,ProductId,OrderDate,DeliveryDate,Price,Quantity,Total,Remarks")] SalesOrder salesOrder)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            salesOrder.AdminId = user;
            if (ModelState.IsValid)
            {
                var ProductStock = await _context.Product.FindAsync(salesOrder.ProductId);
                if(ProductStock == null)
                    return NotFound();
                if (!CheckProductStock(ProductStock.CurrentQuantity,salesOrder.Quantity))
                {
                    String s = String.Format("The stock for {0} this are not enough!!!" +
                        "\nOrder {2} units have exceed {1} units in Inventory",
                         ProductStock.ProductName, ProductStock.CurrentQuantity,salesOrder.Quantity);
                    ViewData["Message"] = s;
                    ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", salesOrder.BranchId);
                    ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", salesOrder.CustomerId);
                    ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", salesOrder.ProductId);
                    return View(salesOrder);
                }

                _context.Add(salesOrder);
                ProductStock.CurrentQuantity-=salesOrder.Quantity;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", salesOrder.BranchId);
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", salesOrder.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", salesOrder.ProductId);
            return View(salesOrder);
        }

        // GET: SalesOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SalesOrder == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder.FindAsync(id);
            if (salesOrder == null)
            {
                return NotFound();
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", salesOrder.BranchId);
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", salesOrder.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", salesOrder.ProductId);
            return View(salesOrder);
        }

        // POST: SalesOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int SalesOrderId, int BranchId, int CustomerId, int ProductId, int Quantity, double Price, double Amount, DateTimeOffset OrderDate, DateTimeOffset DeliveryDate, string? Remarks, string? InvoiceId, string AdminId)
        {
            if (SalesOrderId==null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                SalesOrder sales = _context.SalesOrder.FirstOrDefault(item => item.SalesOrderId == SalesOrderId);
                if (sales == null)
                    return NotFound();
                if (sales.ProductId == ProductId)
                {
                    var UpdateProduct = await _context.Product.FindAsync(ProductId);
                    if (UpdateProduct == null)
                        return NotFound();
                    if (sales.Quantity > Quantity)
                    {

                        var increaseQuantity = sales.Quantity - Quantity;
                        UpdateProduct.CurrentQuantity += increaseQuantity;
                    }
                    if (sales.Quantity < Quantity)
                    {
                        if(!CheckProductStock(UpdateProduct.CurrentQuantity,Quantity))
                        {
                            String s = String.Format("The stock for {0} this are not enough!!!" +
                        "\nOrder {2} units have exceed {1} units in Inventory",
                         UpdateProduct.ProductName, UpdateProduct.CurrentQuantity,Quantity);
                            ViewData["Message"] = s;
                            SalesOrder so = new SalesOrder();
                            so.SalesOrderId = SalesOrderId;
                            so.BranchId = BranchId;
                            so.CustomerId=CustomerId;
                            so.ProductId = ProductId;
                            so.Quantity = Quantity;
                            so.Price=Price;
                            so.Amount=Amount;
                            so.OrderDate = OrderDate;
                            so.DeliveryDate = DeliveryDate;
                            so.Remarks = Remarks;
                            so.InvoiceId=InvoiceId;
                            so.AdminId = AdminId; 
                            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", so.BranchId);
                            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", so.CustomerId);
                            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", so.ProductId);
                            return View(so);
                        }
                        var decreaseQuantity = Quantity - sales.Quantity;
                        UpdateProduct.CurrentQuantity -= decreaseQuantity;
                    }

                }
                else
                {
                    var UpdateProduct = await _context.Product.FindAsync(sales.ProductId);
                    UpdateProduct.CurrentQuantity += sales.Quantity;

                    var NewUpdateProduct = await _context.Product.FindAsync(ProductId);
                    NewUpdateProduct.CurrentQuantity -= Quantity;


                }
                sales.BranchId = BranchId;
                sales.CustomerId = CustomerId;
                sales.ProductId = ProductId;
                sales.Quantity = Quantity;
                sales.Price = Price;
                sales.Amount = Amount;
                sales.OrderDate = OrderDate;
                sales.DeliveryDate = DeliveryDate;
                sales.Remarks = Remarks;
                sales.InvoiceId = InvoiceId;
                sales.AdminId = AdminId;
                try
                {
                    _context.Update(sales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderExists(sales.SalesOrderId))
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

            SalesOrder salesOrder = new SalesOrder();
            salesOrder.SalesOrderId = SalesOrderId;
            salesOrder.BranchId = BranchId;
            salesOrder.CustomerId = CustomerId;
            salesOrder.ProductId = ProductId;
            salesOrder.Quantity = Quantity;
            salesOrder.Price = Price;
            salesOrder.Amount = Amount;
            salesOrder.OrderDate = OrderDate;
            salesOrder.DeliveryDate = DeliveryDate;
            salesOrder.Remarks = Remarks;
            salesOrder.InvoiceId = InvoiceId;
            salesOrder.AdminId = AdminId;
            ViewData["BranchId"] = new SelectList(_context.Branch, "BranchId", "BranchName", salesOrder.BranchId);
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", salesOrder.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "ProductName", salesOrder.ProductId);
            return View(salesOrder);
        }

        // GET: SalesOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SalesOrder == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrder
                .Include(s => s.Branch)
                .Include(s => s.Customer)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.SalesOrderId == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // POST: SalesOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SalesOrder == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SalesOrder'  is null.");
            }
            var salesOrder = await _context.SalesOrder.FindAsync(id);
            if (salesOrder != null)
            {
                var UpdateProduct = await _context.Product.FindAsync(salesOrder.ProductId);
                if (UpdateProduct == null)
                    return NotFound();
                {
                    UpdateProduct.CurrentQuantity += salesOrder.Quantity;
                }
                _context.SalesOrder.Remove(salesOrder);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrderExists(int id)
        {
          return (_context.SalesOrder?.Any(e => e.SalesOrderId == id)).GetValueOrDefault();
        }
    }
}
