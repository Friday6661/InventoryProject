using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryProject.Models.DTOs
{
    public class ServiceResponse
    {
        public record class GeneralResponse(bool Flag, string Message);
        public record class LoginResponse(bool Flag, string Token, string Message);
        public record class UserProfileResponse(bool Flag, UserProfileDTO UserProfile, string Message);
        public record class DetailProductCategoryResponse(bool Flag, DetailProductCategoryDTO DetailProductCategory, string Message);
        public record class UpdateProductCategoryResponse(bool Flag, string Message);
        public record class DetailProductResponse(bool Flag, DetailProductDTO DetailProduct, string Message);
        public record class DetailOrderResponse(bool Flag, OrderDTO DetailOrder, string Message);
        public record class DetailOrderProductConsumableResponse(bool Flag, OrderProductConsumableDTO DetailOrderProduct, string Message);
        public record class OrderProductConsumableResponse(bool Flag, ICollection<OrderProductConsumableDTO> DetailOrderProduct, string Message);
        public record class DetailOrderProductReuseableResponse(bool Flag, OrderProductReuseableDTO DetailOrderProduct, string Message);
        public record class OrderProductReuseableResponse(bool Flag, ICollection<OrderProductReuseableDTO> DetailOrderProduct, string Message);
    }
}