/**
 * Reusable function to initialize TinyMCE editor instances.
 * * @param {string} selector - The CSS selector (e.g., '#myEditorId' or '.rich-editor') 
 * to target the textarea(s) to be converted to rich text editors.
 * @param {number} height - The height of the editor in pixels (default: 400).
 */
// wwwroot/js/tinymce-initializer.js
function initializeTinyMCE(selector, height) {
    tinymce.init({
        selector: selector,
        height: height || 400,
        menubar: false,
        plugins: [
            'lists', 'link', 'image', 'table', 'code'
        ],
        toolbar:
            'undo redo | formatselect | bold italic underline | ' +
            'alignleft aligncenter alignright alignjustify | ' +
            'bullist numlist outdent indent | link image | code',
        skin: 'oxide',
        content_css: 'default',
        branding: false,
        license_key: 'gpl', // 👈 prevents license warning
        setup: function (editor) {
            editor.on('change', function () {
                editor.save(); // ensures textarea updates
            });
        }
    });
}


// NOTE: In a real environment, you would place the downloaded 'tinymce.min.js'
// file locally (e.g., at 'wwwroot/lib/tinymce/tinymce.min.js') and reference it
// using a <script> tag BEFORE this initializer script.
