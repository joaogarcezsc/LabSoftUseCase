using System;

namespace GestaoTarefas;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=================================");
        Console.WriteLine("   SISTEMA DE GESTÃO DE TAREFAS  ");
        Console.WriteLine("=================================\n");

        Console.Write("Digite o nome da tarefa: ");
        string nome = Console.ReadLine() ?? "Sem nome";

        Console.Write("Digite o nome do funcionário responsável: ");
        string funcionario = Console.ReadLine() ?? "Não informado";

        Console.Write("Digite a data de início (dd/mm/aaaa): ");
        string dataInicioStr = Console.ReadLine() ?? "";
        DateTime dataInicio = DateTime.Parse(dataInicioStr);

        Console.Write("Digite a data de término (dd/mm/aaaa): ");
        string dataFimStr = Console.ReadLine() ?? "";
        DateTime dataFim = DateTime.Parse(dataFimStr);

        Tarefa tarefa = new Tarefa(nome, funcionario, dataInicio, dataFim);

        Console.WriteLine("\n--- RESUMO DA TAREFA ---");
        Console.WriteLine($"Tarefa: {tarefa.Nome}");
        Console.WriteLine($"Responsável: {tarefa.NomeFuncionario}");
        Console.WriteLine($"Início: {tarefa.DataInicio:dd/MM/yyyy}");
        Console.WriteLine($"Término: {tarefa.DataFim:dd/MM/yyyy}");
        Console.WriteLine($"Duração: {tarefa.ObterQuantidadeDias()} dias");
    }
}