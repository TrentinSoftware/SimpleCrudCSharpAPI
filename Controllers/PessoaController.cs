using Microsoft.AspNetCore.Mvc;
using SimpleCrudCSharpAPI.Models;
using SimpleCrudCSharpAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace SimpleCrudCSharpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PessoasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/pessoas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pessoa>>> GetPessoas()
    {
        return await _context.Pessoas.Include(p => p.Telefones).ToListAsync();
    }

    // POST: api/pessoas
    [HttpPost]
    public async Task<ActionResult<Pessoa>> PostPessoa(Pessoa pessoa)
    {
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPessoas), new { id = pessoa.Id }, pessoa);
    }
}