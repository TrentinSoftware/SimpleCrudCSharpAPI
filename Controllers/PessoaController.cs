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

    /// <summary>
    /// Endpoint de cadastro de dados de pessoa (nome, email, números de telefone).
    /// Retorna o JSON criado.
    /// </summary>
    /// <param name="nome">Nome da pessoa</param>
    /// <param name="email">Email da pessoa</param>
    /// <param name="numero">Lista de números de telefone</param>
    [HttpPost("cadastro")]
    public async Task<ActionResult<Pessoa>> PostPessoaSimples([FromForm] string nome, [FromForm] string email, [FromForm] List<string> numero)
    {
        var pessoa = new Pessoa
        {
            Nome = nome,
            Email = email,
            Telefones = numero.Select(n => new Telefone { Numero = n }).ToList()
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        // Retorna o JSON criado
        return CreatedAtAction(nameof(GetPessoas), new { id = pessoa.Id }, pessoa);
    }

    /// <summary>
    /// Atualiza nome e email de uma pessoa pelo ID.
    /// Não altera telefones.
    /// </summary>
    /// <param name="id">ID da pessoa</param>
    /// <param name="nome">Novo nome</param>
    /// <param name="email">Novo email</param>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPessoa(int id, [FromForm] string nome, [FromForm] string email)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);
        if (pessoa == null)
            return NotFound();

        pessoa.Nome = nome;
        pessoa.Email = email;

        _context.Entry(pessoa).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deleta uma pessoa e todos os seus telefones.
    /// </summary>
    /// <param name="id">ID da pessoa</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePessoa(int id)
    {
        var pessoa = await _context.Pessoas.Include(p => p.Telefones).FirstOrDefaultAsync(p => p.Id == id);
        if (pessoa == null)
            return NotFound();

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Endpoint para ler o cadastro de todas as pessoas.
    /// Retorna todas as pessoas cadastradas, com seus telefones.
    /// Pode receber um parâmetro opcional "limit" para limitar o número de resultados retornados.
    /// </summary>
    /// <param name="limit">Limite opcional de pessoas retornadas (ex: 10)</param>
    /// <returns>Lista de pessoas cadastradas</returns>
    [HttpGet("/api/all/pessoas")]
    public async Task<ActionResult<IEnumerable<Pessoa>>> GetPessoas([FromQuery] int? limit = null)
    {
        var query = _context.Pessoas.Include(p => p.Telefones).AsQueryable();
        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Busca os dados de uma pessoa específica pelo ID.
    /// </summary>
    /// <param name="id">ID da pessoa</param>
    /// <returns>Dados da pessoa encontrada ou 404 se não existir</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Pessoa>> GetPessoaById(int id)
    {
        var pessoa = await _context.Pessoas.Include(p => p.Telefones).FirstOrDefaultAsync(p => p.Id == id);
        if (pessoa == null)
            return NotFound();

        return pessoa;
    }
}