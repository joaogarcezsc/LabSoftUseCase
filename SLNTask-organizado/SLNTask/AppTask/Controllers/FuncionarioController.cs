using AppTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppTask.Controllers;

public class FuncionarioController : Controller
{
    private readonly DbTasksContext _context;

    public FuncionarioController(DbTasksContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var funcionarios = await _context.Funcionarios
            .Include(f => f.Departamento)
            .Include(f => f.Gerente)
            .ToListAsync();

        return View(funcionarios);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var funcionario = await _context.Funcionarios
            .Include(f => f.Departamento)
            .Include(f => f.Gerente)
            .FirstOrDefaultAsync(f => f.Codigo == id);

        return funcionario == null ? NotFound() : View(funcionario);
    }

    public async Task<IActionResult> Create()
    {
        await CarregarDepartamentos();
        // Sem departamento selecionado ainda, o select de gerente começa vazio
        // e é preenchido via JS (ObterGerentesPorDepartamento) quando o usuário escolhe o departamento.
        ViewBag.CodigoGerente = new SelectList(Enumerable.Empty<Funcionario>(), "Codigo", "Nome");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Codigo,Nome,Cargo,DepartamentoId,CodigoGerente,EhGerente")] Funcionario funcionario)
    {
        ModelState.Remove(nameof(Funcionario.Departamento));
        ModelState.Remove(nameof(Funcionario.Gerente));
        ModelState.Remove(nameof(Funcionario.Subordinados));
        ModelState.Remove(nameof(Funcionario.Tarefas));

        if (funcionario.DepartamentoId <= 0)
            ModelState.AddModelError(nameof(Funcionario.DepartamentoId), "Selecione um departamento.");

        if (funcionario.EhGerente)
        {
            funcionario.CodigoGerente = null;
        }
        else if (funcionario.CodigoGerente is int codigoGerenteSelecionado && codigoGerenteSelecionado <= 0)
        {
            funcionario.CodigoGerente = null;
        }

        // NOVO: gerente precisa ser do mesmo departamento do funcionário
        if (funcionario.CodigoGerente.HasValue)
        {
            var gerenteValido = await _context.Funcionarios.AnyAsync(f =>
                f.Codigo == funcionario.CodigoGerente.Value &&
                f.EhGerente &&
                f.DepartamentoId == funcionario.DepartamentoId);

            if (!gerenteValido)
                ModelState.AddModelError(nameof(Funcionario.CodigoGerente), "O gerente selecionado precisa ser do mesmo departamento do funcionário.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Funcionarios.Add(funcionario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
            }
        }

        await CarregarDepartamentos(funcionario.DepartamentoId);
        await CarregarGerentes(funcionario.CodigoGerente, departamentoId: funcionario.DepartamentoId);
        return View(funcionario);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null) return NotFound();
        await CarregarDepartamentos(funcionario.DepartamentoId);
        await CarregarGerentes(funcionario.CodigoGerente, funcionario.Codigo, funcionario.DepartamentoId);
        return View(funcionario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Codigo,Nome,Cargo,DepartamentoId,CodigoGerente,EhGerente")] Funcionario funcionario)
    {
        if (id != funcionario.Codigo) return NotFound();

        ModelState.Remove(nameof(Funcionario.Departamento));
        ModelState.Remove(nameof(Funcionario.Gerente));
        ModelState.Remove(nameof(Funcionario.Subordinados));
        ModelState.Remove(nameof(Funcionario.Tarefas));

        if (funcionario.DepartamentoId <= 0)
            ModelState.AddModelError(nameof(Funcionario.DepartamentoId), "Selecione um departamento.");

        if (funcionario.CodigoGerente == funcionario.Codigo)
            ModelState.AddModelError(nameof(Funcionario.CodigoGerente), "Um funcionário não pode ser gerente de si mesmo.");

        if (!funcionario.EhGerente)
        {
            var temSubordinados = await _context.Funcionarios.AnyAsync(f => f.CodigoGerente == funcionario.Codigo);
            if (temSubordinados)
                ModelState.AddModelError(nameof(Funcionario.EhGerente), "Este funcionário é gerente de outros — desmarque-os primeiro.");
        }

        if (funcionario.EhGerente)
        {
            funcionario.CodigoGerente = null;
        }

        // NOVO: gerente precisa ser do mesmo departamento do funcionário
        if (funcionario.CodigoGerente.HasValue)
        {
            var gerenteValido = await _context.Funcionarios.AnyAsync(f =>
                f.Codigo == funcionario.CodigoGerente.Value &&
                f.EhGerente &&
                f.DepartamentoId == funcionario.DepartamentoId);

            if (!gerenteValido)
                ModelState.AddModelError(nameof(Funcionario.CodigoGerente), "O gerente selecionado precisa ser do mesmo departamento do funcionário.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Funcionarios.Update(funcionario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
            }
        }

        await CarregarDepartamentos(funcionario.DepartamentoId);
        await CarregarGerentes(funcionario.CodigoGerente, funcionario.Codigo, funcionario.DepartamentoId);
        return View(funcionario);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var funcionario = await _context.Funcionarios
            .Include(f => f.Departamento)
            .Include(f => f.Gerente)
            .FirstOrDefaultAsync(f => f.Codigo == id);
        return funcionario == null ? NotFound() : View(funcionario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null) return NotFound();

        var possuiTarefas = await _context.Tarefas.AnyAsync(t => t.FuncionarioId == id);
        if (possuiTarefas)
        {
            TempData["Erro"] = "Não é possível excluir este funcionário porque existem tarefas vinculadas a ele.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        var possuiSubordinados = await _context.Funcionarios.AnyAsync(f => f.CodigoGerente == id);
        if (possuiSubordinados)
        {
            TempData["Erro"] = "Não é possível excluir este funcionário porque ele é gerente de outros funcionários.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _context.Funcionarios.Remove(funcionario);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // NOVO: endpoint AJAX usado pelo JS das views para recarregar o select de Gerente
    // quando o usuário troca o Departamento no formulário, sem precisar submeter a página.
    [HttpGet]
    public async Task<IActionResult> ObterGerentesPorDepartamento(int departamentoId, int? excluirCodigo = null)
    {
        var query = _context.Funcionarios.Where(f => f.EhGerente && f.DepartamentoId == departamentoId);

        if (excluirCodigo.HasValue)
            query = query.Where(f => f.Codigo != excluirCodigo.Value);

        var gerentes = await query
            .OrderBy(f => f.Nome)
            .Select(f => new { codigo = f.Codigo, nome = f.Nome })
            .ToListAsync();

        return Json(gerentes);
    }

    private async Task CarregarDepartamentos(int? departamentoSelecionado = null)
    {
        ViewBag.DepartamentoId = new SelectList(
            await _context.Departamentos.OrderBy(d => d.Nome).ToListAsync(),
            "Codigo", "Nome", departamentoSelecionado);
    }

    private async Task CarregarGerentes(int? gerenteSelecionado = null, int? excluirCodigo = null, int? departamentoId = null)
    {
        var query = _context.Funcionarios.Where(f => f.EhGerente);

        if (departamentoId.HasValue)
            query = query.Where(f => f.DepartamentoId == departamentoId.Value);

        if (excluirCodigo.HasValue)
            query = query.Where(f => f.Codigo != excluirCodigo.Value);

        ViewBag.CodigoGerente = new SelectList(
            await query.OrderBy(f => f.Nome).ToListAsync(),
            "Codigo", "Nome", gerenteSelecionado);
    }
}