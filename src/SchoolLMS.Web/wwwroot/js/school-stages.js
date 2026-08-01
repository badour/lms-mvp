(() => {
  const body = document.getElementById('stagesBody');
  const addBtn = document.getElementById('addStageRow');
  if (!body || !addBtn) return;

  function reindex() {
    [...body.querySelectorAll('tr.stage-row')].forEach((row, i) => {
      row.querySelectorAll('input, select').forEach((el) => {
        if (!el.name) return;
        el.name = el.name.replace(/Stages\[\d+]/, `Stages[${i}]`);
      });
    });
  }

  function addRow() {
    const i = body.querySelectorAll('tr.stage-row').length;
    const year = document.querySelector('[name="YearName"]')?.value || '';
    const tr = document.createElement('tr');
    tr.className = 'stage-row';
    tr.innerHTML = `
      <td>
        <input type="hidden" name="Stages[${i}].Id" value="" />
        <input type="hidden" name="Stages[${i}].GradeLevelId" value="" />
        <input type="hidden" name="Stages[${i}].ClassSectionId" value="" />
        <input class="form-control" name="Stages[${i}].StageName" required />
      </td>
      <td><input class="form-control" name="Stages[${i}].ClassName" required /></td>
      <td><input class="form-control" name="Stages[${i}].YearName" value="${year}" /></td>
      <td>
        <select class="form-select" name="Stages[${i}].IsActive">
          <option value="true" selected>نشطة</option>
          <option value="false">غير نشطة</option>
        </select>
      </td>
      <td><button type="button" class="btn btn-sm btn-outline-danger remove-stage"><i class="bi bi-trash"></i></button></td>`;
    body.appendChild(tr);
  }

  addBtn.addEventListener('click', addRow);
  body.addEventListener('click', (e) => {
    const btn = e.target.closest('.remove-stage');
    if (!btn) return;
    const rows = body.querySelectorAll('tr.stage-row');
    if (rows.length <= 1) return;
    btn.closest('tr')?.remove();
    reindex();
  });
})();
