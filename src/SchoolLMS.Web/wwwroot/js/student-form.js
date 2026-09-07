(function () {
  function reindex(container, fieldName) {
    container.querySelectorAll('.dynamic-item').forEach(function (item, index) {
      var input = item.querySelector('input');
      if (input) input.name = fieldName + '[' + index + ']';
    });
  }

  function addItem(containerId, fieldName, placeholder) {
    var container = document.getElementById(containerId);
    if (!container) return;
    var wrap = document.createElement('div');
    wrap.className = 'dynamic-item input-group mb-2';
    wrap.innerHTML = '<input name="' + fieldName + '[0]" class="form-control" placeholder="' + placeholder + '" />' +
      '<button type="button" class="btn btn-outline-danger remove-item">حذف</button>';
    container.appendChild(wrap);
    reindex(container, fieldName);
  }

  var addHobbyBtn = document.getElementById('addHobbyBtn');
  var addNoteBtn = document.getElementById('addNoteBtn');
  if (addHobbyBtn) {
    addHobbyBtn.addEventListener('click', function () {
      addItem('hobbiesList', 'Hobbies', 'اسم الهواية');
    });
  }
  if (addNoteBtn) {
    addNoteBtn.addEventListener('click', function () {
      addItem('notesList', 'NotesList', 'نص الملاحظة');
    });
  }

  document.addEventListener('click', function (e) {
    if (!e.target.classList.contains('remove-item')) return;
    var item = e.target.closest('.dynamic-item');
    if (!item) return;
    var container = item.parentElement;
    var fieldName = container.id === 'hobbiesList' ? 'Hobbies' : 'NotesList';
    item.remove();
    reindex(container, fieldName);
  });

  if (document.getElementById('hobbiesList') && !document.querySelector('#hobbiesList .dynamic-item')) {
    addItem('hobbiesList', 'Hobbies', 'اسم الهواية');
  }
  if (document.getElementById('notesList') && !document.querySelector('#notesList .dynamic-item')) {
    addItem('notesList', 'NotesList', 'نص الملاحظة');
  }
})();
