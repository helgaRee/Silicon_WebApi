using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController(ApiContext context) : Controller
{

    private readonly ApiContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if(ModelState.IsValid)
        {
            var courses = await _context.Courses.ToListAsync();

            if (courses == null || !courses.Any())
                    return NotFound("No courses were found");
            return Ok(courses);
        }
        return BadRequest("Problems with getting the courses");
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetOne(int Id)
    {
        if (ModelState.IsValid)
        {
            var courses = await _context.Courses.FindAsync(Id);

            if (courses == null)
                return NotFound("No courses were found");
            return Ok(courses);
        }
        return BadRequest("Problems with getting the courses");
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse(CourseRegModel model)
    {
        //validera med modelstate
        if (ModelState.IsValid)
        {
            //registrera email om den inte redan finns
            //sök igenom om email redan finns, gå via subscribers som är reggad i ApiContext
            var exists = await _context.Courses.AnyAsync(x => x.Title == model.Title);
            //finns kursen redan , returnera conflict
            if (exists)
            {
                return Conflict("The course is already registererd.");
            }
            else
            {
                //annars registrera och lägg till en ny subscriberEntity där du stoppar in inkommande emailen
                _context.Add(new CourseEntity 
                { 
                    IsBestSeller = model.IsBestSeller,
                    Image = model.Image,
                    Title = model.Title,
                    Author = model.Author,
                    Price = model.Price,
                    DiscountPrice = model.DiscountPrice,
                    Hours = model.Hours,
                    LikesInNumers = model.LikesInNumers,
                    LikesInProcent = model.LikesInProcent,
                });
            }
            //spara öndringar i databasen
            await _context.SaveChangesAsync();
            //returnera OK
            return Ok();
        }
        return BadRequest("Ops, something went wrong. Couldnt save the new course to database.");
    }



    [HttpDelete]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        //validera med modelstate
        if (ModelState.IsValid)
        {
            //registrera email om den inte redan finns
            //sök igenom om email redan finns, gå via subscribers som är reggad i ApiContext
            var course = await _context.Courses.FindAsync(id);
            //finns kursen inte, returnera conflict
            if (course == null)
            {
                return Conflict("The course can not be found");
            }
            else
            {
                _context.Courses.Remove(course);
                //spara öndringar i databasen
                await _context.SaveChangesAsync();
                //returnera OK
                return Ok();
            }
        }
        return BadRequest("Ops, something went wrong. Couldnt save the new course to database.");
    }


}
