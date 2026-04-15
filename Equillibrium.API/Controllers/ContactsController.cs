using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Equillibrium.Infrastructure.Data; // Adjust to your actual namespace
using Equillibrium.Core.Entities;
[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ContactsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/contacts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetContacts() 
        => await _context.Contacts.ToListAsync();

    // GET: api/contacts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetContact(Guid id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        return contact == null ? NotFound() : contact;
    }

    // POST: api/contacts
    [HttpPost]
    // POST: api/contacts
[HttpPost]
public async Task<ActionResult<Contact>> CreateContact(Contact contact) 
{
    // EF now knows exactly what table and columns to use
    _context.Contacts.Add(contact);
    await _context.SaveChangesAsync();

    // This returns a 201 Created status and a link to the new record
    return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
}


    // DELETE: api/contacts/{id}
    [HttpDelete("{id}")]
public async Task<IActionResult> DeleteContact(Guid id)
{
    var contact = await _context.Contacts.FindAsync(id);
    if (contact == null) return NotFound();

    // SOFT DELETE: Just flip the flag
    contact.IsDeleted = true;
    
    await _context.SaveChangesAsync();
    return NoContent();
}
// DELETE: api/contacts/{id}/permanent
[HttpDelete("{id}/permanent")]
public async Task<IActionResult> ForceDeleteContact(Guid id)
{
    // Use .IgnoreQueryFilters() so we can find the record even if it was already soft-deleted
    var contact = await _context.Contacts
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(c => c.Id == id);

    if (contact == null) return NotFound();

    // HARD DELETE: Actually removes the row from Postgres
    _context.Contacts.Remove(contact);
    
    await _context.SaveChangesAsync();
    return NoContent();
}

}
