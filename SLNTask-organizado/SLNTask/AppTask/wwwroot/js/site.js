document.addEventListener('DOMContentLoaded', () => {
    requestAnimationFrame(() => document.body.classList.add('page-ready'));
    const focusMode = localStorage.getItem('apptask-focus-mode') === 'on';
    if (focusMode) document.body.classList.add('high-contrast');

    const navToggle = document.querySelector('[data-nav-toggle]');
    const nav = document.querySelector('.primary-nav');
    navToggle?.addEventListener('click', () => nav?.classList.toggle('is-open'));

    document.querySelector('[data-theme-toggle]')?.addEventListener('click', () => {
        document.body.classList.toggle('high-contrast');
        localStorage.setItem('apptask-focus-mode', document.body.classList.contains('high-contrast') ? 'on' : 'off');
    });

    document.querySelectorAll('[data-grid-search]').forEach((input) => {
        const target = document.querySelector(input.dataset.gridSearch);
        if (!target) return;
        input.addEventListener('input', () => {
            const term = input.value.toLocaleLowerCase('pt-BR').trim();
            target.querySelectorAll('tbody tr').forEach((row) => {
                row.hidden = term.length > 0 && !row.textContent.toLocaleLowerCase('pt-BR').includes(term);
            });
        });
    });
});
