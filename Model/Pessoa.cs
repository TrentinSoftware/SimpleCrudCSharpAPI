namespace SimpleCrudCSharpAPI.Models;

public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public List<Telefone> Telefones { get; set; } = new();
}

public class Telefone
{
    public int Id { get; set; }
    public string Numero { get; set; }
    public int PessoaId { get; set; }
}