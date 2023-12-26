using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Contracts;
using InventoryProject.Enum;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Repositories
{
    public class OrderProductReuseableRepository : IOrderProductReuseable
    {
        private readonly InventoryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public OrderProductReuseableRepository(InventoryDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        public async Task<GeneralResponse> CancelledOrderProductReuseableAsync(int id, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(httpContext.User);
                    if (user is null) return new GeneralResponse(false, "Invalid User");
                    
                    var orderProductReuseable = await _context.OrderProductReusables
                                                                .Where(opc => opc.Id == id)
                                                                .FirstOrDefaultAsync();
                    
                    if (orderProductReuseable is null) return new GeneralResponse(false, "Invalid order product reusable");
                    bool status = orderProductReuseable.OrderProductStatus == OrderProductStatus.Requested
                                    || orderProductReuseable.OrderProductStatus == OrderProductStatus.Approved;
                    if (!status) return new GeneralResponse(false, "Invalid order product reusable status");
                    orderProductReuseable.OrderProductStatus = OrderProductStatus.Cancelled;
                    await _context.SaveChangesAsync();

                    var product = await _context.Products.FindAsync(orderProductReuseable.ProductId);
                    if (product is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid product for this order");
                    }
                    product.StockQuantity++;
                    await _context.SaveChangesAsync();
                    
                    var order = await _context.Orders.FindAsync(orderProductReuseable.OrderId);
                    if (order is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid order");
                    }
                    // order.TotalQuantity--;
                    order.ProcessQuantity++;
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return new GeneralResponse(true, "Cancelling order product reuseable successfully");
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Cancelling order product reuseable failed");
                }
            }
        }

        public async Task<OrderProductReuseableResponse> GetAllOrderProductReuseablesAsync(HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new OrderProductReuseableResponse(false, null!,"Invalid user");
            var userRoles = await _userManager.GetRolesAsync(user);

            if (!(userRoles.Contains("Admin")) && userRoles.Contains("Manager"))
            {
                var orderProductReusables = await _context.OrderProductReusables
                                                            .Where(opr => opr.CreatedById == user.Id)
                                                            .ToListAsync();
                var orderProductReusablesDTOs = orderProductReusables.Select(opr => new OrderProductReuseableDTO
                {
                    Id = opr.Id,
                    CreatedAt = opr.CreatedAt,
                    UpdatedAt = opr.UpdatedAt,
                    CreatedById = opr.CreatedById,
                    OrderProductStatus = opr.OrderProductStatus,
                    ProductId = opr.ProductId
                }).ToList();
                return new OrderProductReuseableResponse(true, orderProductReusablesDTOs, "Load order products reuseables is successfully");
            }
            else
            {
                var orderProductReusables = await _context.OrderProductReusables
                                                           .ToListAsync();
                var orderProductReusablesDTOs = orderProductReusables.Select(opr => new OrderProductReuseableDTO
                {
                    Id = opr.Id,
                    CreatedAt = opr.CreatedAt,
                    UpdatedAt = opr.UpdatedAt,
                    CreatedById = opr.CreatedById,
                    OrderProductStatus = opr.OrderProductStatus,
                    ProductId = opr.ProductId
                }).ToList();
                return new OrderProductReuseableResponse(true, orderProductReusablesDTOs, "Load order products reuseables is successfully");
            }
        }

        public async Task<DetailOrderProductReuseableResponse> GetOrderProductReuseableByIdAsync(int id, HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new DetailOrderProductReuseableResponse(false, null!,"Invalid user");
            var userRoles = await _userManager.GetRolesAsync(user);

            if (!(userRoles.Contains("Admin") || userRoles.Contains("Manager")))
            {
                var orderProductReusable = await _context.OrderProductReusables
                                                            .Where(opr => opr.Id == id && opr.CreatedById == user.Id)
                                                            .FirstOrDefaultAsync();
                if (orderProductReusable is null) return new DetailOrderProductReuseableResponse(false, null!, "Invalid order product reuseable");
                var orderProductReusablesDTO = new OrderProductReuseableDTO()
                {
                    Id = orderProductReusable.Id,
                    CreatedAt = orderProductReusable.CreatedAt,
                    UpdatedAt = orderProductReusable.UpdatedAt,
                    CreatedById = orderProductReusable.CreatedById,
                    OrderProductStatus = orderProductReusable.OrderProductStatus,
                    ProductId = orderProductReusable.ProductId
                };
                return new DetailOrderProductReuseableResponse(true, orderProductReusablesDTO, "load order product reuseable successfully");
            }
            else
            {
                var orderProductReusable = await _context.OrderProductReusables
                                                            .FirstOrDefaultAsync();
                if (orderProductReusable is null) return new DetailOrderProductReuseableResponse(false, null!, "Invalid order product reuseable");
                var orderProductReusablesDTO = new OrderProductReuseableDTO()
                {
                    Id = orderProductReusable.Id,
                    CreatedAt = orderProductReusable.CreatedAt,
                    UpdatedAt = orderProductReusable.UpdatedAt,
                    CreatedById = orderProductReusable.CreatedById,
                    OrderProductStatus = orderProductReusable.OrderProductStatus,
                    ProductId = orderProductReusable.ProductId
                };
                return new DetailOrderProductReuseableResponse(true, orderProductReusablesDTO, "load order product reuseable successfully");
            }
        }

        public async Task<OrderProductReuseableResponse> GetOrderProductReuseableByOrderIdAsync(int orderId, HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new OrderProductReuseableResponse(false, null!,"Invalid user");
            var userRoles = await _userManager.GetRolesAsync(user);

            if(!(userRoles.Contains("Admin") || userRoles.Contains("Manager")))
            {
                var orderProductReusable = await _context.OrderProductReusables
                                                           .Where(opr => opr.OrderId == orderId && opr.CreatedById == user.Id)
                                                           .ToListAsync();
                if (orderProductReusable is null) return new OrderProductReuseableResponse(false, null!, "Invalid order product reuseable");
                var orderProductReusablesDTOs = orderProductReusable.Select(opr => new OrderProductReuseableDTO
                {
                    Id = opr.Id,
                    CreatedAt = opr.CreatedAt,
                    UpdatedAt = opr.UpdatedAt,
                    CreatedById = opr.CreatedById,
                    OrderProductStatus = opr.OrderProductStatus,
                    ProductId = opr.ProductId
                }).ToList();
                return new OrderProductReuseableResponse(true, orderProductReusablesDTOs, "load order product reuseable successfully");
            }
            else
            {
                var orderProductReusable = await _context.OrderProductReusables
                                                           .Where(opr => opr.OrderId == orderId)
                                                           .ToListAsync();
                if (orderProductReusable is null) return new OrderProductReuseableResponse(false, null!, "Invalid order product reuseable");
                var orderProductReusablesDTOs = orderProductReusable.Select(opr => new OrderProductReuseableDTO
                {
                    Id = opr.Id,
                    CreatedAt = opr.CreatedAt,
                    UpdatedAt = opr.UpdatedAt,
                    CreatedById = opr.CreatedById,
                    OrderProductStatus = opr.OrderProductStatus,
                    ProductId = opr.ProductId
                }).ToList();
                return new OrderProductReuseableResponse(true, orderProductReusablesDTOs, "load order product reuseable successfully");
            }
        }

        public async Task<GeneralResponse> RejectedOrderProductReuseableAsync(int id, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(httpContext.User);
                    if (user is null) return new GeneralResponse(false, "Invalid user");

                    var orderProductReusable = await _context.OrderProductReusables
                                                                .Where(opr => opr.Id == id)
                                                                .FirstOrDefaultAsync();
                    if (orderProductReusable is null) return new GeneralResponse(false, "Invalid order product reuseable");
                    bool status = orderProductReusable.OrderProductStatus == OrderProductStatus.Requested;
                    if (!status) return new GeneralResponse(false, "Invalid order product status");
                    orderProductReusable.OrderProductStatus = OrderProductStatus.Rejected;
                    await _context.SaveChangesAsync();

                    var product = await _context.Products.FindAsync(orderProductReusable.ProductId);
                    if (product is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid product");
                    }
                    product.StockQuantity++;
                    await _context.SaveChangesAsync();

                    var order = await _context.Orders.FindAsync(orderProductReusable.OrderId);
                    if (order is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid order");
                    }
                    // order.TotalQuantity--;
                    order.ProcessQuantity++;
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return new GeneralResponse(true, "Rejecting order product consumable is successfully");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Error occurred while rejecting order product");
                }
            }

        }

        public async Task<GeneralResponse> ReportOrderProductReuseableAsync(int id, ReportIssueDTO reportIssueDTO, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(httpContext.User);
                    if (user is null) return new GeneralResponse(false, "Invalid user");
                    
                    var orderProductReusable = await _context.OrderProductReusables
                                                                .Where(opr => opr.Id == id)
                                                                .FirstOrDefaultAsync();
                    if (orderProductReusable is null) return new GeneralResponse(false, "Invalid order product reusable");
                    bool status = orderProductReusable.OrderProductStatus == OrderProductStatus.OnHand;
                    if (!status) return new GeneralResponse(false, "Invalid order product status");
                    if (reportIssueDTO.Status != IssueStatus.Broken)
                    {
                        if (reportIssueDTO.Status == IssueStatus.Missing)
                        {
                            orderProductReusable.OrderProductStatus = OrderProductStatus.Missing;
                        }
                    }
                    else
                    {
                        orderProductReusable.OrderProductStatus = OrderProductStatus.Broken;
                    }

                    string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    imageFileName += Path.GetExtension(reportIssueDTO.ImageFile.FileName);
                    string imagesFolder = _env.WebRootPath + "/images/productIssues";

                    using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
                    {
                        reportIssueDTO.ImageFile.CopyTo(stream);
                    }

                    var productIssue = new ProductIssue
                    {
                        CreatedAt = DateTime.Now,
                        Status = reportIssueDTO.Status,
                        Quantity = 1,
                        ImageFile = imageFileName,
                        CreatedBy = user,
                        ProductId = orderProductReusable.ProductId
                    };
                    _context.ProductIssues.Add(productIssue);

                    var order = await _context.Orders.FindAsync(orderProductReusable.OrderId);
                    if (order is null) return new GeneralResponse(false, "Invalid order");
                    order.ProcessQuantity++;

                    if (order.TotalQuantity == order.ProcessQuantity)
                    {
                        if (HasMissingOrBrokenReusables(order))
                        {
                            order.OrderStatus = OrderStatus.CompletedWithIssue;
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            order.OrderStatus = OrderStatus.Completed;
                            await _context.SaveChangesAsync();
                        }
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return new GeneralResponse(true, "Reported order product reuseable successfully");
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Error occurred while reporting order product reuseable");
                }
            }
        }

        public async Task<GeneralResponse> ReturnedOrderProductReuseableAsync(int id, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(httpContext.User);
                    if (user is null) return new GeneralResponse(false, "Invalid User");

                    var orderProductReusable = await _context.OrderProductReusables
                                                                .Where(opr => opr.Id == id)
                                                                .FirstOrDefaultAsync();
                    if (orderProductReusable is null) return new GeneralResponse(false, "Invalid order product reuseable");
                    bool status = orderProductReusable.OrderProductStatus == OrderProductStatus.OnHand;
                    if (!status) return new GeneralResponse(false, "Invalid order product status");
                    orderProductReusable.OrderProductStatus = OrderProductStatus.Returned;
                    await _context.SaveChangesAsync();

                    var product = await _context.Products.FindAsync(orderProductReusable.ProductId);
                    if(product is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false,"Invalid product");
                    }
                    product.StockQuantity++;
                    await _context.SaveChangesAsync();

                    var order = await _context.Orders.FindAsync(orderProductReusable.OrderId);
                    if (order is null) return new GeneralResponse(false, "Invalid order");
                    order.ProcessQuantity++;

                    if (order.TotalQuantity == order.ProcessQuantity)
                    {
                        if (HasMissingOrBrokenReusables(order))
                        {
                            order.OrderStatus = OrderStatus.CompletedWithIssue;
                        }
                        else
                        {
                            order.OrderStatus = OrderStatus.Completed;
                        }
                        await _context.SaveChangesAsync();
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return new GeneralResponse(true, "Order product returned successfully");
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Order product returned failed");
                    throw;
                }
            }
        }
        
        private bool HasMissingOrBrokenReusables(Order order)
        {
            return _context.OrderProductReusables.Any(opr =>
                opr.OrderProductStatus == OrderProductStatus.Missing ||
                opr.OrderProductStatus == OrderProductStatus.Broken);
        }

    }
}