using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssignmentApi.Data;
using AssignmentApi.Models;

namespace AssignmentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrderItemsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/orderitems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetAllOrderItems()
    {
        var orderItems = await _context.OrderItems.ToListAsync();
        return Ok(orderItems);
    }

    // GET: /api/orderitems/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItem>> GetOrderItemById(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
        {
            return NotFound();
        }

        return Ok(orderItem);
    }

    // GET: /api/orderitems/order/{orderId}
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItemsByOrderId(int orderId)
    {
        var orderItems = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        return Ok(orderItems);
    }

    // POST: /api/orderitems
    [HttpPost]
    public async Task<ActionResult<OrderItem>> CreateOrderItem(OrderItem orderItem)
    {
        var orderExists = await _context.Orders.AnyAsync(o => o.Id == orderItem.OrderId);

        if (!orderExists)
        {
            return BadRequest("OrderId does not exist.");
        }

        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderItemById), new { id = orderItem.Id }, orderItem);
    }

    // PATCH: /api/orderitems/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateOrderItem(int id, OrderItem updatedOrderItem)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
        {
            return NotFound();
        }

        var orderExists = await _context.Orders.AnyAsync(o => o.Id == updatedOrderItem.OrderId);

        if (!orderExists)
        {
            return BadRequest("OrderId does not exist.");
        }

        orderItem.OrderId = updatedOrderItem.OrderId;
        orderItem.ProductName = updatedOrderItem.ProductName;
        orderItem.Quantity = updatedOrderItem.Quantity;
        orderItem.UnitPrice = updatedOrderItem.UnitPrice;

        await _context.SaveChangesAsync();

        return Ok(orderItem);
    }

    // DELETE: /api/orderitems/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItem(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null)
        {
            return NotFound();
        }

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}