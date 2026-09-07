(function () {
  var toggle = document.getElementById('sidebarToggle');
  var sidebar = document.getElementById('sidebar');
  if (toggle && sidebar) {
    toggle.addEventListener('click', function () {
      sidebar.classList.toggle('open');
    });
  }

  document.querySelectorAll('[data-submenu-toggle]').forEach(function (button) {
    button.addEventListener('click', function () {
      var targetId = button.getAttribute('data-submenu-toggle');
      var menu = document.getElementById(targetId);
      if (!menu) {
        return;
      }

      var isOpen = menu.classList.toggle('show');
      button.classList.toggle('open', isOpen);
      button.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
    });
  });
})();
