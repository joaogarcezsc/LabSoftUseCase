using System;
using System.Collections.Generic;

namespace AppTaskZero.Models;

public partial class Funcionario
{
    public int Codigo { get; set; }

    public string Nome { get; set; } = null!;

    public string Cargo { get; set; } = null!;

    public virtual ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();
}
