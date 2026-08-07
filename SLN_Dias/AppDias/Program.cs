using System;
using System.Globalization;

namespace GestaoTarefas;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=================================");
        Console.WriteLine(" SoftwareHouse - SISTEMA DE GESTÃO DE TAREFAS - Versão 1.0.0");
        Console.WriteLine("=================================\n");

        Console.Write("Digite o nome da tarefa: ");
        string nome = Console.ReadLine() ?? "Sem nome";

        Console.Write("Digite o nome do funcionário responsável: ");
        string funcionario = Console.ReadLine() ?? "Não informado";

        DateTime dataInicio = LerDataValida("Digite a data de início (dd/mm/aaaa): ");
        DateTime dataFim = LerDataValida("Digite a data de término (dd/mm/aaaa): ");

        while (dataFim < dataInicio)
        {
            Console.WriteLine("\n⚠️ Ops! A data de término não pode ser anterior à data de início.");
            dataFim = LerDataValida("Digite novamente a data de término (dd/mm/aaaa): ");
        }

        Tarefa tarefa = new Tarefa(nome, funcionario, dataInicio, dataFim);

        Console.WriteLine("\n--- RESUMO DA TAREFA ---");
        Console.WriteLine($"Tarefa: {tarefa.Nome}");
        Console.WriteLine($"Responsável: {tarefa.NomeFuncionario}");
        Console.WriteLine($"Início: {tarefa.DataInicio:dd/MM/yyyy}");
        Console.WriteLine($"Término: {tarefa.DataFim:dd/MM/yyyy}");
        Console.WriteLine($"Duração: {tarefa.ObterQuantidadeDias()} dias");
    }

    /// <summary>
    /// Método auxiliar para ler e validar a data digitada pelo usuário.
    /// Evita crashes do sistema ao passar formatos incorretos.
    /// </summary>
    private static DateTime LerDataValida(string mensagemPrompt)
    {
        DateTime dataResultado;
        string[] formatosAceitos = { "dd/MM/yyyy", "d/M/yyyy" };

        while (true)
        {
            Console.Write(mensagemPrompt);
            string entrada = Console.ReadLine() ?? "";

            if (DateTime.TryParseExact(entrada, formatosAceitos, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataResultado))
            {
                return dataResultado;
            }

            Console.WriteLine("❌ Data em formato inválido! Por favor utilize o formato dd/mm/aaaa (Ex: 25/12/2024).\n");
        }
    }
}