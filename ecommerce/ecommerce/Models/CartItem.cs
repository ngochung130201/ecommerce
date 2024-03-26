using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("cart_item")]
    public class CartItem
    {
        [Key]
        [Column("cart_item_id")]
        public int CartItemId { get; set; }
        [Column("cart_id")]
        public int CartId { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("total_price", TypeName = "DECIMAL(20,7)")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}
