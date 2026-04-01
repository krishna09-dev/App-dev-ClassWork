using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssignmentApi.Data;
using AssignmentApi.Models;

namespace AssignmentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
    {
        var orders = await _context.Orders.ToListAsync();
        return Ok(orders);
    }

    // GET: /api/orders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    // GET: /api/orders/customer/{customerId}
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomerId(int customerId)
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .ToListAsync();

        return Ok(orders);
    }

    // POST: /api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(Order order)
    {
        var customerExists = await _context.Customers.AnyAsync(c => c.Id == order.CustomerId);

        if (!customerExists)
        {
            return BadRequest("CustomerId does not exist.");
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    // PATCH: /api/orders/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, Order updatedOrder)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        var customerExists = await _context.Customers.AnyAsync(c => c.Id == updatedOrder.CustomerId);

        if (!customerExists)
        {
            return BadRequest("CustomerId does not exist.");
        }

        order.CustomerId = updatedOrder.CustomerId;
        order.OrderDate = updatedOrder.OrderDate;
        order.TotalAmount = updatedOrder.TotalAmount;
        order.Status = updatedOrder.Status;

        await _context.SaveChangesAsync();

        return Ok(order);
    }

    // DELETE: /api/orders/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}