using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class SubscribeController : Controller
{
    private readonly ApiContext _context;

    public SubscribeController(ApiContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(SubscribersEntity entity)
    {
        //validera med modelstate
        if (ModelState.IsValid)
        {
            //registrera email om den inte redan finns
            //sök igenom om email redan finns, gå via subscribers som är reggad i ApiContext
            var exists = await _context.Subscribers.AnyAsync(x => x.Email == entity.Email);
            //finns addressen redan , returnera conflict
            if (exists)
            {
                return Conflict("The email is already registererd.");
            }
            else
            {
                //annars registrera och lägg till en ny subscriberEntity där du stoppar in inkommande emailen
                _context.Add(entity);
            }
            //spara öndringar i databasen
            await _context.SaveChangesAsync();
            //returnera OK
            return Ok();
        }
        return BadRequest("Ops, something went wrong. Couldnt save the new Email to database.");
    }

    [HttpDelete]
    public async Task<IActionResult> UnSubscribe(string email)
    {

        //validera med modelstate
        if (ModelState.IsValid)
        {
            //sök efter entiteten och matcha på email
            //om den är null, returnera NotFound
            var subscriberEntity = await _context.Subscribers.FirstOrDefaultAsync(y => y.Email == email);
            if (subscriberEntity == null)
            {
                return NotFound("Couldn't find the email address in database.");
            }
            //annars ta bort subscriberentitý.
            else
            {
                _context.Remove(subscriberEntity);
                await _context.SaveChangesAsync();
                //returnera OK
                return Ok();
            }
        }
        return BadRequest("Something went wrong, couldn't remove the email address. Try again.");
    }
}
