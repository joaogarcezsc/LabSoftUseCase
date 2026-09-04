using System.ComponentModel.DataAnnotations;

namespace AppTask.Models;

public class CentroDeCusto
{
    public int Codigo { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "Código do centro")]
    public string CodigoCentro { get; set; } = null!;

    [Required, StringLength(100)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = null!;

    [StringLength(250)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Display(Name = "Departamento")]
    public int? DepartamentoId { get; set; }

    [Display(Name = "Responsável")]
    public int? ResponsavelId { get; set; }

    [Range(0, 999999999999.99)]
    [Display(Name = "Orçamento mensal")]
    public decimal? OrcamentoMensal { get; set; }

    [Required]
    [Display(Name = "Data de criação")]
    public DateTime DataCriacao { get; set; } = DateTime.Today;

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    public virtual Departamento? Departamento { get; set; }
    public virtual Funcionario? Responsavel { get; set; }
}
