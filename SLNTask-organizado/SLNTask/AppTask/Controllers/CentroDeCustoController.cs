using AppTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppTask.Controllers;

public class CentroDeCustoController : Controller
{
    private readonly DbTasksContext _context;

    public CentroDeCustoController(DbTasksContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var centros = await _context.CentrosDeCusto
            .Include(c => c.Departamento)
            .Include(c => c.Responsavel)
            .OrderBy(c => c.Ativo ? 0 : 1)
            .ThenBy(c => c.Nome)
            .ToListAsync();
        return View(centros);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var centro = await _context.CentrosDeCusto
            .Include(c => c.Departamento)
            .Include(c => c.Responsavel)
            .FirstOrDefaultAsync(c => c.Codigo == id);
        return centro == null ? NotFound() : View(centro);
    }

    public async Task<IActionResult> Create()
    {
        await CarregarOpcoes();
        return View(new CentroDeCusto { DataCriacao = DateTime.Today, Ativo = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CentroDeCusto centro)
    {
        if (await _context.CentrosDeCusto.AnyAsync(c => c.CodigoCentro == centro.CodigoCentro))
            ModelState.AddModelError(nameof(centro.CodigoCentro), "Já existe um centro com este código.");

        if (ModelState.IsValid)
        {
            _context.CentrosDeCusto.Add(centro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await CarregarOpcoes(centro.DepartamentoId, centro.ResponsavelId);
        return View(centro);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var centro = await _context.CentrosDeCusto.FindAsync(id);
        if (centro == null) return NotFound();
        await CarregarOpcoes(centro.DepartamentoId, centro.ResponsavelId);
        return View(centro);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CentroDeCusto centro)
    {
        if (id != centro.Codigo) return NotFound();

        if (await _context.CentrosDeCusto.AnyAsync(c => c.Codigo != id && c.CodigoCentro == centro.CodigoCentro))
            ModelState.AddModelError(nameof(centro.CodigoCentro), "Já existe outro centro com este código.");

        if (ModelState.IsValid)
        {
            _context.Update(centro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await CarregarOpcoes(centro.DepartamentoId, centro.ResponsavelId);
        return View(centro);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var centro = await _context.CentrosDeCusto
            .Include(c => c.Departamento)
            .Include(c => c.Responsavel)
            .FirstOrDefaultAsync(c => c.Codigo == id);
        return centro == null ? NotFound() : View(centro);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var centro = await _context.CentrosDeCusto.FindAsync(id);
        if (centro == null) return NotFound();
        _context.CentrosDeCusto.Remove(centro);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarOpcoes(int? departamentoId = null, int? responsavelId = null)
    {
        ViewBag.DepartamentoId = new SelectList(
            await _context.Departamentos.OrderBy(d => d.Nome).ToListAsync(), "Codigo", "Nome", departamentoId);
        ViewBag.ResponsavelId = new SelectList(
            await _context.Funcionarios.OrderBy(f => f.Nome).ToListAsync(), "Codigo", "Nome", responsavelId);
    }
}
