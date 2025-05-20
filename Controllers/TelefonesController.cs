using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCrudCSharpAPI.Data;
using SimpleCrudCSharpAPI.Models;

namespace SimpleCrudCSharpAPI.Controllers;

[ApiController]
[Route("api/telefones")]
public class TelefonesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TelefonesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Atualiza o número de telefone pelo ID do telefone.
    /// </summary>
    /// <param name="id">ID do telefone</param>
    /// <param name="numero">Novo número</param>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTelefone(int id, [FromForm] string numero)
    {
        var telefone = await _context.Telefones.FindAsync(id);
        if (telefone == null)
            return NotFound();

        telefone.Numero = numero;
        _context.Entry(telefone).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Adiciona um novo telefone para uma pessoa existente.
    /// </summary>
    /// <param name="pessoaId">ID da pessoa</param>
    /// <param name="numero">Número do telefone</param>
    [HttpPost]
    public async Task<ActionResult<Telefone>> PostTelefone([FromForm] int pessoaId, [FromForm] string numero)
    {
        var pessoa = await _context.Pessoas.Include(p => p.Telefones).FirstOrDefaultAsync(p => p.Id == pessoaId);
        if (pessoa == null)
            return NotFound();

        var telefone = new Telefone { Numero = numero, PessoaId = pessoaId };
        pessoa.Telefones.Add(telefone);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(PostTelefone), new { id = telefone.Id }, telefone);
    }

    /// <summary>
    /// Deleta um telefone de uma pessoa existente.
    /// </summary>
    /// <param name="id">ID do telefone</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTelefone(int id)
    {
        var telefone = await _context.Telefones.FindAsync(id);
        if (telefone == null)
            return NotFound();

        _context.Telefones.Remove(telefone);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}