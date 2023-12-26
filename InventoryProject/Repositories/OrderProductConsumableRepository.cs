using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using InventoryProject.Contracts;
using InventoryProject.Enum;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Repositories
{
    public class OrderProductConsumableRepository : IOrderProductConsumable
    {
        private readonly InventoryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderProductConsumableRepository(InventoryDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<GeneralResponse> CancelledOrderProductConsumableAsync(int id, HttpContext httpContext)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(httpContext.User);
                    if (user is null) return new GeneralResponse(false, "You are not authorized for this action");
                    
                    var orderProductConsumable = await _context.OrderProductConsumables
                                                                .Where(opc => opc.Id == id)
                                                                .FirstOrDefaultAsync();
                    if (orderProductConsumable is null) return new GeneralResponse(false, "Invalid order product consumable");
                    bool status = orderProductConsumable.OrderProductStatus == OrderProductStatus.Requested
                                    || orderProductConsumable.OrderProductStatus == OrderProductStatus.Approved;
                    
                    if (!status) return new GeneralResponse(false, "Invalid order product consumable status");
                    orderProductConsumable.OrderProductStatus = OrderProductStatus.Cancelled;
                    await _context.SaveChangesAsync();

                    var product = await _context.Products.FindAsync(orderProductConsumable.ProductId);
                    if (product is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid product id");
                    }
                    product.StockQuantity++;
                    await _context.SaveChangesAsync();

                    var order = await _context.Orders.FindAsync(orderProductConsumable.ProductId);
                    if (order is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid order");
                    }
                    order.TotalQuantity--;
                    order.ProcessQuantity++;
                    await _context.SaveChangesAsync();
                    
                    transaction.Commit();
                    return new GeneralResponse(true, "Cancelling order product consumable is successfully");
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Error occurred while cancelling order product");
                }
            }
        }

        public async Task<OrderProductConsumableResponse> GetAllOrderProductConsumablesAsync(HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new OrderProductConsumableResponse(false, null!, "You are not authorized for this action");
            var userRoles = await _userManager.GetRolesAsync(user);

            if (!(userRoles.Contains("Admin") || userRoles.Contains("Manager")))
            {
                var orderProductConsumables = await _context.OrderProductConsumables
                                                            .Where(opc => opc.CreatedById == user.Id)
                                                            .ToListAsync();
                var orderProductConsumablesDTOs = orderProductConsumables.Select(opc => new OrderProductConsumableDTO
                {
                    Id = opc.Id,
                    CreatedAt = opc.CreatedAt,
                    UpdatedAt = opc.UpdatedAt,
                    CreatedById = opc.CreatedById,
                    OrderProductStatus = opc.OrderProductStatus,
                    ProductId = opc.ProductId
                }).ToList();
                return new OrderProductConsumableResponse(true, orderProductConsumablesDTOs, "Load successfully");
                                                            
            }
            else
            {
                var orderProductConsumables = await _context.OrderProductConsumables.ToListAsync();
                var orderProductConsumablesDTOs = orderProductConsumables.Select(opc => new OrderProductConsumableDTO
                {
                    Id = opc.Id,
                    CreatedAt = opc.CreatedAt,
                    UpdatedAt = opc.UpdatedAt,
                    CreatedById = opc.CreatedById,
                    OrderProductStatus = opc.OrderProductStatus,
                    ProductId = opc.ProductId
                }).ToList();
                return new OrderProductConsumableResponse(true, orderProductConsumablesDTOs, "Load successfully");
            }


        }

        public async Task<DetailOrderProductConsumableResponse> GetOrderProductConsumableByIdAsync(int id, HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new DetailOrderProductConsumableResponse(false, null!, "You are not authorize for this action");
            var userRole = await _userManager.GetRolesAsync(user);

            if (!(userRole.Contains("Admin") || userRole.Contains("Manager")))
            {
                var orderProductConsumable = await _context.OrderProductConsumables
                                                            .Where(opc => opc.Id == id && opc.CreatedById == user.Id)
                                                            .FirstOrDefaultAsync();
                if (orderProductConsumable is null) return new DetailOrderProductConsumableResponse(false, null!, "Invalid order product consumable");
                var orderProductConsumableDTO = new OrderProductConsumableDTO()
                {
                    Id = orderProductConsumable.Id,
                    CreatedAt = orderProductConsumable.CreatedAt,
                    UpdatedAt = orderProductConsumable.UpdatedAt,
                    CreatedById = orderProductConsumable.CreatedById,
                    OrderProductStatus = orderProductConsumable.OrderProductStatus,
                    ProductId = orderProductConsumable.ProductId
                };

                return new DetailOrderProductConsumableResponse(true, orderProductConsumableDTO, "Load detail order product consumable is successfully");
            }
            else
            {
                var orderProductConsumable = await _context.OrderProductConsumables
                                                            .Where(opc => opc.Id == id)
                                                            .FirstOrDefaultAsync();
                if (orderProductConsumable is null) return new DetailOrderProductConsumableResponse(false, null!, "Invalid order product consumable");
                var orderProductConsumableDTO = new OrderProductConsumableDTO()
                {
                    Id = orderProductConsumable.Id,
                    CreatedAt = orderProductConsumable.CreatedAt,
                    UpdatedAt = orderProductConsumable.UpdatedAt,
                    CreatedById = orderProductConsumable.CreatedById,
                    OrderProductStatus = orderProductConsumable.OrderProductStatus,
                    ProductId = orderProductConsumable.ProductId
                };

                return new DetailOrderProductConsumableResponse(true, orderProductConsumableDTO, "Load detail order product consumable is successfully");
            }

        }

        public async Task<OrderProductConsumableResponse> GetOrderProductConsumableByOrderIdAsync(int orderId, HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new OrderProductConsumableResponse(false, null!, "You are not authorize for this action");
            var userRole = await _userManager.GetRolesAsync(user);

            if (!(userRole.Contains("Admin") || userRole.Contains("Manager")))
            {
                var orderProductConsumables = await _context.OrderProductConsumables
                                                            .Where(opc => opc.OrderId == orderId && opc.CreatedById == user.Id)
                                                            .ToListAsync();
                var orderProductConsumableDTOs = orderProductConsumables.Select(opc => new OrderProductConsumableDTO
                {
                    Id = opc.Id,
                    CreatedAt = opc.CreatedAt,
                    UpdatedAt = opc.UpdatedAt,
                    CreatedById = opc.CreatedById,
                    OrderProductStatus = opc.OrderProductStatus,
                    ProductId = opc.ProductId
                }).ToList();
                return new OrderProductConsumableResponse(true, orderProductConsumableDTOs, "Load order products consumables by orderId successfully");
            }
            else
            {
                var orderProductConsumables = await _context.OrderProductConsumables
                                                            .Where(opc => opc.OrderId == orderId)
                                                            .ToListAsync();
                var orderProductConsumableDTOs = orderProductConsumables.Select(opc => new OrderProductConsumableDTO
                {
                    Id = opc.Id,
                    CreatedAt = opc.CreatedAt,
                    UpdatedAt = opc.UpdatedAt,
                    CreatedById = opc.CreatedById,
                    OrderProductStatus = opc.OrderProductStatus,
                    ProductId = opc.ProductId
                }).ToList();
                return new OrderProductConsumableResponse(true, orderProductConsumableDTOs, "Load order products consumables by orderId successfully");
            }
        }

        public async Task<GeneralResponse> RejectedOrderProductConsumableAsync(int id, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(httpContext.User);
                    var userRole = await _userManager.GetRolesAsync(user);
                    
                    var orderProductConsumable = await _context.OrderProductConsumables
                                                                .Where(opc => opc.Id == id)
                                                                .FirstOrDefaultAsync();
                    if (orderProductConsumable is null) return new GeneralResponse(false, "Invalid order product consumable");
                    bool status = orderProductConsumable.OrderProductStatus == OrderProductStatus.Requested;
                    if (!status) return new GeneralResponse(false, "Invalid order product consumable status");
                    orderProductConsumable.OrderProductStatus = OrderProductStatus.Rejected;
                    await _context.SaveChangesAsync();

                    var product = await _context.Products.FindAsync(orderProductConsumable.ProductId);
                    if (product is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid product");
                    }
                    product.StockQuantity++;
                    await _context.SaveChangesAsync();

                    var order = await _context.Orders.FindAsync(orderProductConsumable.OrderId);
                    if (order is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid order");
                    }
                    order.TotalQuantity--;
                    order.ProcessQuantity++;
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return new GeneralResponse(true, "Rejecting order product consumable is successfully");
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Error occurred while rejecting order product");
                }
            }
        }
    }
}