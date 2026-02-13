// newsletter.js

function CreateNewsLetter() {
    // Validate required fields
    var title = $('#nl_title').val();
    var author = $('#nl_author').val();
    var coverImageInput = $('#nl_coverImage')[0];
    var documentInput = $('#nl_document')[0];

    var coverImage = coverImageInput && coverImageInput.files.length > 0 ? coverImageInput.files[0] : null;
    var documentFile = documentInput && documentInput.files.length > 0 ? documentInput.files[0] : null;

    if (!title || title.trim() === '') {
        errorAlert('Title is required');
        return;
    }

    if (!author || author.trim() === '') {
        errorAlert('Author is required');
        return;
    }

    if (!coverImage) {
        errorAlert('Cover Image is required');
        return;
    }

    if (!documentFile) {
        errorAlert('Document is required');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Title', title);
    formData.append('Author', author);
    formData.append('Description', $('#nl_description').val() || '');
    formData.append('CoverImage', coverImage);
    formData.append('Document', documentFile);

    // Show processing state
    processingBtn('nl_add_newsletter_btn');

    $.ajax({
        type: 'POST',
        url: '/NewsLetter/CreateNewsLetter',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('nl_add_newsletter_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('nl_add_newsletter_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function GetNewsLetterDetailForEdit(id) {
    if (!id || id === '') {
        errorAlert('Invalid newsletter ID');
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/NewsLetter/GetNewsLetterById',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                var data = result.data;
                $('#nl_edit_Id').val(data.id);
                $('#nl_title_edit').val(data.title);
                $('#nl_author_edit').val(data.author);
                $('#nl_description_edit').val(data.description);

                // Display current files
                if (data.coverImageUrl) {
                    $('#nl_current_cover_image').html(`
                        <p class="text-muted">Current: <a href="/${data.coverImageUrl}" target="_blank">View Image</a></p>
                    `);
                }
                if (data.documentUrl) {
                    $('#nl_current_document').html(`
                        <p class="text-muted">Current: <a href="/${data.documentUrl}" target="_blank">View Document</a></p>
                    `);
                }

                $('#nl_editModal').modal('show');
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function UpdateNewsLetter() {
    // Validate required fields
    var id = $('#nl_edit_Id').val();
    var title = $('#nl_title_edit').val();
    var author = $('#nl_author_edit').val();

    if (!id || id.trim() === '') {
        errorAlert('Invalid newsletter ID');
        return;
    }

    if (!title || title.trim() === '') {
        errorAlert('Title is required');
        return;
    }

    if (!author || author.trim() === '') {
        errorAlert('Author is required');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Id', id);
    formData.append('Title', title);
    formData.append('Author', author);
    formData.append('Description', $('#nl_description_edit').val() || '');

    // Add cover image if new one is selected
    var coverImageInput = $('#nl_coverImage_edit')[0];
    if (coverImageInput && coverImageInput.files.length > 0) {
        formData.append('CoverImage', coverImageInput.files[0]);
    }

    // Add document if new one is selected
    var documentInput = $('#nl_document_edit')[0];
    if (documentInput && documentInput.files.length > 0) {
        formData.append('Document', documentInput.files[0]);
    }

    // Show processing state
    processingBtn('nl_edit_newsletter_btn');

    $.ajax({
        type: 'POST',
        url: '/NewsLetter/UpdateNewsLetter',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('nl_edit_newsletter_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('nl_edit_newsletter_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function getDataForDelete(id) {
    $('#nl_delete_Id').val(id);
    $('#nl_deleteModal').modal('show');
}

function DeleteNewsLetter() {
    var id = $('#nl_delete_Id').val();

    if (!id || id === '') {
        errorAlert('Invalid newsletter ID');
        return;
    }

    processingBtn('nl_del_newsletter_btn');

    $.ajax({
        type: 'POST',
        url: '/NewsLetter/DeleteNewsLetter',
        data: { id: id },
        success: function (result) {
            returnDefaultBtn('nl_del_newsletter_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('nl_del_newsletter_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

// Helper functions (if not already defined globally)
function processingBtn(btnId) {
    var btn = $('#' + btnId);
    btn.prop('disabled', true);
    btn.data('original-text', btn.html());
    btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
}

function returnDefaultBtn(btnId) {
    var btn = $('#' + btnId);
    var originalText = btn.data('original-text');

    if (!originalText) {
        var defaultTexts = {
            'nl_add_newsletter_btn': 'Add NewsLetter',
            'nl_edit_newsletter_btn': 'Update Record',
            'nl_del_newsletter_btn': 'Delete'
        };
        originalText = defaultTexts[btnId] || 'Submit';
    }

    btn.prop('disabled', false);
    btn.html(originalText);
}

function successAlertWithRedirect(message, url) {
    // Check if Swal (SweetAlert2) is available
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'success',
            title: 'Success!',
            text: message,
            showConfirmButton: false,
            timer: 1500
        }).then(function () {
            window.location.href = url;
        });
    } else {
        // Fallback to standard alert
        alert(message);
        window.location.href = url;
    }
}

function errorAlert(message) {
    // Check if Swal (SweetAlert2) is available
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: message
        });
    } else {
        // Fallback to standard alert
        alert('Error: ' + message);
    }
}