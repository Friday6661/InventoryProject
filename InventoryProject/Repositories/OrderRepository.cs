using InventoryProject.Contracts;
using InventoryProject.Enum;
using InventoryProject.Models;
using InventoryProject.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static InventoryProject.Models.DTOs.ServiceResponse;

namespace InventoryProject.Repositories
{
    public class OrderRepository : IOrder
    {
        private readonly InventoryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderRepository(InventoryDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        

        public async Task<GeneralResponse> ApprovedOrderAsync(int id, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = await _context.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();
                    if (order is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Order not found");
                    }

                    var user = await _userManager.GetUserAsync(httpContext.User);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if(!userRoles.Contains("Admin") && !userRoles.Contains("Manager"))
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "You are not authorized to perform this action");
                    }

                    if (order.OrderStatus != OrderStatus.Requested)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid orrder status for this action");
                    }

                    order.OrderStatus = OrderStatus.Approved;
                    order.UpdatedAt = DateTime.Now;

                    var orderProductConsumables = await _context.OrderProductConsumables
                                                                .Where(opc => opc.OrderId == id && opc.OrderProductStatus == OrderProductStatus.Requested)
                                                                .ToListAsync();
                    foreach (var orderProductConsumable in orderProductConsumables)
                    {
                        orderProductConsumable.OrderProductStatus = OrderProductStatus.Approved;
                        orderProductConsumable.UpdatedAt = DateTime.Now;
                    }

                    var orderProductReusables = await _context.OrderProductReusables
                                                                .Where(opr => opr.OrderId == id && opr.OrderProductStatus == OrderProductStatus.Requested)
                                                                .ToListAsync();
                    foreach (var orderProductReusable in orderProductReusables)
                    {
                        orderProductReusable.OrderProductStatus = OrderProductStatus.Approved;
                        orderProductReusable.UpdatedAt = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return new GeneralResponse(true, "Approved order is successfully");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Error occured while approving order");
                    throw;
                }
            }
        }

        
        public async Task<GeneralResponse> CancelledOrderAsync(int id, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = await _context.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();
                    if (order is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Order not found");
                    }

                    var user = await _userManager.GetUserAsync(httpContext.User);
                    if (user == null) return new GeneralResponse(false, "User not authenticated");
                    
                    if (!(order.OrderStatus == OrderStatus.Requested || order.OrderStatus == OrderStatus.Approved))
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid order status for this action");
                    }
                    
                    order.OrderStatus = OrderStatus.Cancelled;
                    order.UpdatedAt = DateTime.Now;

                    var orderProductConsumables = await _context.OrderProductConsumables
                                                                .Where(opc => opc.OrderId == id)
                                                                .ToListAsync();
                    
                    

                    foreach (var orderProductConsumable in orderProductConsumables)
                    {
                        orderProductConsumable.OrderProductStatus = OrderProductStatus.Cancelled;
                        orderProductConsumable.UpdatedAt = DateTime.Now;
                        var product = await _context.Products
                                                    .Where(p => p.Id == orderProductConsumable.ProductId)
                                                    .FirstOrDefaultAsync();

                        product.StockQuantity++;
                        order.ProcessQuantity++;
                    }

                    var orderProductReusables = await _context.OrderProductReusables
                                                                .Where(opr => opr.OrderId == id)
                                                                .ToListAsync();
                    foreach (var orderProductReusable in orderProductReusables)
                    {
                        orderProductReusable.OrderProductStatus = OrderProductStatus.Cancelled;
                        orderProductReusable.UpdatedAt = DateTime.Now;
                        var product = await _context.Products
                                                   .Where(p => p.Id == orderProductReusable.ProductId)
                                                   .FirstOrDefaultAsync();
                        product.StockQuantity++;
                        order.ProcessQuantity++;
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return new GeneralResponse(true, "Cancelling order is successfully");
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Error occurred while cancelling order");
                }
            }
        }

        public async Task<GeneralResponse> CreateOrderAsync(ICollection<OrderHelperDTO> orderHelperDTOs, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userManager.GetUserAsync(httpContext.User);
                    if (user is null) return new GeneralResponse(false, "Invalid user");

                    var order = new Order
                    {
                        CreatedAt = DateTime.Now,
                        OrderStatus = OrderStatus.Requested,
                        CreatedBy = user
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    foreach (var orderHelperDTO in orderHelperDTOs)
                    {
                        var product = await _context.Products.FindAsync(orderHelperDTO.ProductId);
                        if (product is null)
                        {
                            transaction.Rollback();
                            return new GeneralResponse(false, "Product not found");
                        }

                        if (product.ProductType != ProductType.Reusable)
                        {
                            if (product.ProductType == ProductType.Consumable)
                            {
                                for (int i = 0; i < orderHelperDTO.Quantity; i++)
                                {
                                    var orderProductConsumable = new OrderProductConsumable
                                    {
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                        CreatedById = user.Id,
                                        OrderProductStatus = OrderProductStatus.Requested,
                                        OrderId = order.Id,
                                        ProductId = orderHelperDTO.ProductId
                                    };
                                    _context.OrderProductConsumables.Add(orderProductConsumable);
                                    product.StockQuantity--;
                                    order.TotalQuantity++;
                                    await _context.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                return new GeneralResponse(false, "Invalid product type");
                            }
                        }
                        else
                        {
                            for (int i = 0; i < orderHelperDTO.Quantity; i++)
                            {
                                var orderProductReusable = new OrderProductReusable
                                {
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    CreatedById = user.Id,
                                    OrderProductStatus = OrderProductStatus.Requested,
                                    OrderId = order.Id,
                                    ProductId = orderHelperDTO.ProductId
                                };

                                _context.OrderProductReusables.Add(orderProductReusable);
                                product.StockQuantity--;
                                order.TotalQuantity++;
                                await _context.SaveChangesAsync();
                            }
                        }

                    }

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return new GeneralResponse(true, "Created Order Product successfully");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Error occurred while creating order");
                }
            }
        }

        public async Task<ICollection<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                                .Include(o => o.CreatedBy)
                                .Include(o => o.OrderProductConsumables)
                                .Include(o => o.OrderProductReusables)
                                .ToListAsync();

            
            var orderDTOs = orders.Select(async order => new OrderDTO
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                OrderStatus = order.OrderStatus,
                // UserId = order.CreatedBy.Id,
                CreatedBy = new UserProfileDTO
                {
                    UserName = order.CreatedBy.UserName!,
                    FirstName = order.CreatedBy.FirstName!,
                    LastName = order.CreatedBy.LastName!,
                    Email = order.CreatedBy.Email!,
                    Role = (await _userManager.GetRolesAsync(order.CreatedBy)).FirstOrDefault()!
                },
                OrderProductConsumables = order.OrderProductConsumables.Select(opc => new OrderProductConsumableDTO
                {
                    Id = opc.Id,
                    CreatedAt = opc.CreatedAt,
                    UpdatedAt = opc.UpdatedAt,
                    CreatedById = opc.CreatedById,
                    OrderProductStatus = opc.OrderProductStatus,
                    ProductId = opc.ProductId
                }).ToList(),
                OrderProductReusables = order.OrderProductReusables.Select(opr => new OrderProductReuseableDTO
                {
                    Id = opr.Id,
                    CreatedAt = opr.CreatedAt,
                    UpdatedAt = opr.UpdatedAt,
                    CreatedById = opr.CreatedById,
                    OrderProductStatus = opr.OrderProductStatus,
                    ProductId = opr.ProductId
                }).ToList()
            }).ToList();

            return await Task.WhenAll(orderDTOs);
        }

        public async Task<DetailOrderResponse> GetOrderByIdAsync(int id, HttpContext httpContext)
        {
            var order = await _context.Orders.Where(o => o.Id == id)
                                            .Include(o => o.CreatedBy)
                                            .Include(o => o.OrderProductConsumables)
                                            .Include(o => o.OrderProductReusables)
                                            .FirstOrDefaultAsync();

            if (order is null) return new DetailOrderResponse(false, null!, "Specific order not found");

            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new DetailOrderResponse(false, null!, "Invalid Order Details");

            var orderDetail = new OrderDTO()
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                OrderStatus = order.OrderStatus,
                CreatedBy = new UserProfileDTO
                {
                    UserName = order.CreatedBy.UserName,
                    FirstName = order.CreatedBy.FirstName,
                    LastName = order.CreatedBy.LastName,
                    Email = order.CreatedBy.Email,
                    Role = (await _userManager.GetRolesAsync(order.CreatedBy)).FirstOrDefault()!

                },
                OrderProductConsumables = order.OrderProductConsumables.Select(opc => new OrderProductConsumableDTO
                {
                    Id = opc.Id,
                    CreatedAt = opc.CreatedAt,
                    UpdatedAt = opc.UpdatedAt,
                    CreatedById = opc.CreatedById,
                    OrderProductStatus = opc.OrderProductStatus,
                    ProductId = opc.ProductId
                }).ToList(),
                OrderProductReusables = order.OrderProductReusables.Select(opr => new OrderProductReuseableDTO
                {
                    Id = opr.Id,
                    CreatedAt = opr.CreatedAt,
                    UpdatedAt = opr.UpdatedAt,
                    CreatedById = opr.CreatedById,
                    OrderProductStatus = opr.OrderProductStatus,
                    ProductId = opr.ProductId
                }).ToList(),
            };
            return new DetailOrderResponse(true, orderDetail, "Load Detail Order is succesfully");
        }

        public async Task<GeneralResponse> OnHandOrderAsync(int id, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = await _context.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();
                    if (order is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Order not found");
                    }

                    var user = await _userManager.GetUserAsync(httpContext.User);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if(!userRoles.Contains("Admin") && !userRoles.Contains("Manager"))
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "You are not allowed to perform this action");
                    }

                    if (order.OrderStatus != OrderStatus.Approved)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Invalid order status for this action");
                    }
                    
                    // bool hasOrderProductReusables = order.OrderProductReusables.Any(); // Menggunakan Any() untuk memeriksa apakah ada elemen
                    // order.OrderStatus = hasOrderProductReusables ? OrderStatus.OnHand : OrderStatus.Completed;
                    // order.UpdatedAt = DateTime.Now;
                
                    var orderProductConsumables = await _context.OrderProductConsumables
                                                                .Where(opc => opc.OrderId == id && opc.OrderProductStatus == OrderProductStatus.Approved)
                                                                .ToListAsync();
                    foreach (var orderProductConsumable in orderProductConsumables)
                    {
                        orderProductConsumable.OrderProductStatus = OrderProductStatus.Consumed;
                        orderProductConsumable.UpdatedAt = DateTime.Now;
                        order.ProcessQuantity++;
                    }

                    var orderProductReusables = await _context.OrderProductReusables
                                                                .Where(opr => opr.OrderId == id && opr.OrderProductStatus == OrderProductStatus.Approved)
                                                                .ToListAsync();
                    foreach (var orderProductReusable in orderProductReusables)
                    {
                        orderProductReusable.OrderProductStatus = OrderProductStatus.OnHand;
                        orderProductReusable.UpdatedAt = DateTime.Now;
                    }

                    if (orderProductReusables.Count != 0)
                    {
                        order.OrderStatus = OrderStatus.OnHand;
                        order.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        order.OrderStatus = OrderStatus.Completed;
                        order.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return new GeneralResponse(true, "The processing order successfully");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error occurred while processing the order: {ex.Message}");
                    return new GeneralResponse(false, "Error occurred while processing the order");
                }
            }
        }

        public async Task<GeneralResponse> RejectedOrderAsync(int id, HttpContext httpContext)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = await _context.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();
                    if (order is null)
                    {
                        transaction.Rollback();
                        return new GeneralResponse(false, "Order not found");
                    }

                    var user = await _userManager.GetUserAsync(httpContext.User);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (!userRoles.Contains("Admin") && !userRoles.Contains("Manager"))
                    {
                        return new GeneralResponse(false, "You are not authorized to perform this action");
                    }

                    if (order.OrderStatus != OrderStatus.Requested)
                    {
                        return new GeneralResponse(false, "Invalid order status for this action: ");
                    }
                    order.OrderStatus = OrderStatus.Rejected;
                    order.UpdatedAt = DateTime.Now;

                    var orderProductConsumables = await _context.OrderProductConsumables
                                                                .Where(opc => opc.OrderId == id && opc.OrderProductStatus == OrderProductStatus.Requested)
                                                                .ToListAsync();
                    foreach (var orderProductConsumable in orderProductConsumables)
                    {
                        orderProductConsumable.OrderProductStatus = OrderProductStatus.Rejected;
                        orderProductConsumable.UpdatedAt = DateTime.Now;
                        var product = await _context.Products
                                                    .Where(p => p.Id == orderProductConsumable.ProductId)
                                                    .FirstOrDefaultAsync();
                        product.StockQuantity++;
                        order.ProcessQuantity++;
                    }

                    var orderProductReusables = await _context.OrderProductReusables
                                                                .Where(opr => opr.OrderId == id && opr.OrderProductStatus == OrderProductStatus.Requested)
                                                                .ToListAsync();
                    foreach (var orderProductReusable in orderProductReusables)
                    {
                        orderProductReusable.OrderProductStatus = OrderProductStatus.Rejected;
                        orderProductReusable.UpdatedAt = DateTime.Now;
                        var product = await _context.Products
                                                    .Where(p => p.Id == orderProductReusable.ProductId)
                                                    .FirstOrDefaultAsync();
                        product.StockQuantity++;
                        order.ProcessQuantity++;
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return new GeneralResponse(true, "Rejecting order is successfully");
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    return new GeneralResponse(false, "Error occurred while rejecting order");
                    throw;
                }
            }
        }

        public async Task<GeneralResponse> CompleteOrderAsync(int id, HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user is null) return new GeneralResponse(false, "Invalid user");

            var order = await _context.Orders.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (order is null) return new GeneralResponse(false, "Order not found");

            int countOrderProductReuseable = CountOrderProductReusables(order.Id);
            int countOrderProductConsumable = CountOrderProductConsumables(order.Id);
            int totalCount = countOrderProductConsumable + countOrderProductReuseable;
            
            if (order.TotalQuantity == totalCount)
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
                return new GeneralResponse(true, "Order completed");
            }
            else
            {
                return new GeneralResponse(false, "Order not completed");
            }
        }

        private bool HasMissingOrBrokenReusables(Order order)
        {
            return order.OrderProductReusables.Any(opr =>
                opr.OrderProductStatus == OrderProductStatus.Missing ||
                opr.OrderProductStatus == OrderProductStatus.Broken);
        }
        private int CountOrderProductReusables(int id)
        {
            return _context.Orders
                            .Where(o => o.Id == id)
                            .SelectMany(o => o.OrderProductReusables)
                            .Count(opr => opr.OrderProductStatus == OrderProductStatus.Returned
                                        || opr.OrderProductStatus == OrderProductStatus.Missing
                                        || opr.OrderProductStatus == OrderProductStatus.Broken);
        }

        private int CountOrderProductConsumables(int id)
        {
            return _context.Orders
                           .Where(o => o.Id == id)
                           .SelectMany(o => o.OrderProductConsumables)
                           .Count(opc => opc.OrderProductStatus == OrderProductStatus.Consumed);
        }

    }
}