(() => {
  const body = document.getElementById('stagesBody');
  const addBtn = document.getElementById('addStageRow');
  if (!body || !addBtn) return;

  let schoolOptions = [];
  try {
    const raw = document.getElementById('schoolOptionsData')?.textContent || '[]';
    schoolOptions = JSON.parse(raw);
  } catch {
    schoolOptions = [];
  }

  function schoolOptionsHtml(selectedId) {
    const selected = selectedId != null ? String(selectedId) : '';
    const options = ['<option value="">اختر المدرسة</option>']
      .concat(
        (schoolOptions || []).map((item) => {
          const id = String(item.id);
          const sel = selected && id === selected ? ' selected' : '';
          return `<option value="${id}"${sel}>${escapeHtml(item.name || '')}</option>`;
        })
      );
    return options.join('');
  }

  function escapeHtml(value) {
    return String(value)
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;');
  }

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
    const previousSchool = body.querySelector('tr.stage-row:last-child .stage-school-select')?.value || '';
    const tr = document.createElement('tr');
    tr.className = 'stage-row';
    tr.innerHTML = `
      <td>
        <input type="hidden" name="Stages[${i}].Id" value="" />
        <input type="hidden" name="Stages[${i}].GradeLevelId" value="" />
        <input type="hidden" name="Stages[${i}].ClassSectionId" value="" />
        <input class="form-control" name="Stages[${i}].StageName" placeholder="ابتدائية / متوسطة / ثانوية" required />
      </td>
      <td><input class="form-control" name="Stages[${i}].ClassName" placeholder="الأول ابتدائي" required /></td>
      <td><input class="form-control" name="Stages[${i}].SectionName" value="أ" placeholder="أ / ب / ت" required /></td>
      <td>
        <select class="form-select stage-school-select" name="Stages[${i}].SchoolId">
          ${schoolOptionsHtml(previousSchool)}
        </select>
      </td>
      <td><input class="form-control" name="Stages[${i}].YearName" value="${escapeHtml(year)}" /></td>
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
