function CreatePartner() {
    // Validate required fields
    var name = $('#prt_name').val();
    var logoInput = $('#prt_logo')[0];
    var logo = logoInput && logoInput.files.length > 0 ? logoInput.files[0] : null;

    if (!name || name.trim() === '') {
        errorAlert('Partner Name is required');
        return;
    }

    if (!logo) {
        errorAlert('Partner Logo is required');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Name', name);
    formData.append('Address', $('#prt_address').val() || '');
    formData.append('ContactEmail', $('#prt_email').val() || '');
    formData.append('ContactPhone', $('#prt_phone').val() || '');
    formData.append('Logo', logo);

    // Show processing state
    processingBtn('prt_add_partner_btn');

    $.ajax({
        type: 'POST',
        url: '/Partner/CreatePartner',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('prt_add_partner_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('prt_add_partner_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function GetPartnerDetailForEdit(id) {
    if (!id || id === '') {
        errorAlert('Invalid partner ID');
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/Partner/GetPartnerById',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                var data = result.data;
                $('#prt_edit_Id').val(data.id);
                $('#prt_name_edit').val(data.name);
                $('#prt_address_edit').val(data.address);
                $('#prt_email_edit').val(data.contactEmail);
                $('#prt_phone_edit').val(data.contactPhone);
                $('#prt_display_order_edit').val(data.displayOrder);
                $('#prt_is_active_edit').prop('checked', data.isActive);

                // Display current logo
                if (data.logoUrl) {
                    $('#prt_current_logo').html(`
                        <p class="text-muted mb-2">Current Logo:</p>
                        <img src="/${data.logoUrl}" alt="Partner Logo" style="max-width: 200px; max-height: 100px; object-fit: contain; border: 1px solid #ddd; padding: 5px; border-radius: 4px;">
                    `);
                }

                $('#prt_editModal').modal('show');
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function UpdatePartner() {
    // Validate required fields
    var id = $('#prt_edit_Id').val();
    var name = $('#prt_name_edit').val();

    if (!id || id.trim() === '') {
        errorAlert('Invalid partner ID');
        return;
    }

    if (!name || name.trim() === '') {
        errorAlert('Partner Name is required');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Id', id);
    formData.append('Name', name);
    formData.append('Address', $('#prt_address_edit').val() || '');
    formData.append('ContactEmail', $('#prt_email_edit').val() || '');
    formData.append('ContactPhone', $('#prt_phone_edit').val() || '');
    formData.append('DisplayOrder', $('#prt_display_order_edit').val() || '0');
    formData.append('IsActive', $('#prt_is_active_edit').is(':checked'));

    // Add logo if new one is selected
    var logoInput = $('#prt_logo_edit')[0];
    if (logoInput && logoInput.files.length > 0) {
        formData.append('Logo', logoInput.files[0]);
    }

    // Show processing state
    processingBtn('prt_edit_partner_btn');

    $.ajax({
        type: 'POST',
        url: '/Partner/UpdatePartner',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('prt_edit_partner_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('prt_edit_partner_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function getDataForDelete(id) {
    $('#prt_delete_Id').val(id);
    $('#prt_deleteModal').modal('show');
}

function DeletePartner() {
    var id = $('#prt_delete_Id').val();

    if (!id || id === '') {
        errorAlert('Invalid partner ID');
        return;
    }

    processingBtn('prt_del_partner_btn');

    $.ajax({
        type: 'POST',
        url: '/Partner/DeletePartner',
        data: { id: id },
        success: function (result) {
            returnDefaultBtn('prt_del_partner_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('prt_del_partner_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function TogglePartnerStatus(id) {
    if (!id || id === '') {
        errorAlert('Invalid partner ID');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Partner/TogglePartnerStatus',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
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
            'prt_add_partner_btn': 'Add Partner',
            'prt_edit_partner_btn': 'Update Record',
            'prt_del_partner_btn': 'Delete'
        };
        originalText = defaultTexts[btnId] || 'Submit';
    }

    btn.prop('disabled', false);
    btn.html(originalText);
}

function successAlertWithRedirect(message, url) {
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
        alert(message);
        window.location.href = url;
    }
}

function errorAlert(message) {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: message
        });
    } else {
        alert('Error: ' + message);
    }
}