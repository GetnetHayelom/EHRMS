window.initQuillEditor = function (options) {

    const editor = new Quill('#' + options.editorId, {
        theme: 'snow',
        modules: {
            toolbar: '#editor-toolbar',
            table: {}   // 🔥 THIS is what was missing
        }
    });

    // Set initial content
    if (options.initialContent) {
        editor.root.innerHTML = options.initialContent;
    }

    // Sync hidden input
    if (options.hiddenInputId) {
        const hiddenInput = document.getElementById(options.hiddenInputId);
        editor.on('text-change', () => {
            hiddenInput.value = editor.root.innerHTML;
        });
        hiddenInput.value = editor.root.innerHTML;
    }

    const tableModule = editor.getModule('table');

    if (!tableModule) {
        console.error("Table module not loaded");
        return editor;
    }

    // Insert table
    document.querySelectorAll('.insert-table').forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();

            const rows = parseInt(this.dataset.rows);
            const cols = parseInt(this.dataset.cols);

            editor.focus();   // 🔥 REQUIRED

            const tableModule = editor.getModule('table');

            tableModule.insertTable(rows, cols);
        });
    });


    // Table buttons
    document.getElementById('addRowBtn')?.addEventListener('click', () => tableModule.insertRowBelow());
    document.getElementById('addColBtn')?.addEventListener('click', () => tableModule.insertColumnRight());
    document.getElementById('delRowBtn')?.addEventListener('click', () => tableModule.deleteRow());
    document.getElementById('delColBtn')?.addEventListener('click', () => tableModule.deleteColumn());
    document.getElementById('delTableBtn')?.addEventListener('click', () => tableModule.deleteTable());

    return editor;
};
