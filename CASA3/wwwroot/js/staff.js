var staffData = {};
function staffValidtion(type) {
    inputValidation = [];
    inputValidation.push(validateField('', "fName" + type));
    inputValidation.push(validateField('', "lName" + type));
    inputValidation.push(validateEmail('email' + type));
    inputValidation.push(validateNumericField('', "cpa" + type));
    if (inputValidation.includes(false)) {
        return false;
    }
    staffData.FirstName = $('#fName' + type).val();
    staffData.LastName = $('#lName' + type).val();
    staffData.Email = $('#email' + type).val();
    staffData.CPA = $('#cpa' + type).val();
    if (type === '_edit') {
        staffData.Id = $('#edit_Id').val();
    };
    return true;
}

function CreateStaff() {
    if (staffValidtion('')) {
        processingBtn('add_staff_btn');
        let staffDetails = JSON.stringify(staffData);
        $.ajax({
            type: 'Post',
            url: "/Admin/CreateStaff",
            dataType: 'json',
            data: { details: staffDetails },
            success: function (result) {

                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('add_staff_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('add_staff_btn')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    }
}

function UpdateStaff() {
    if (staffValidtion('_edit')) {
        processingBtn('staff_bulk_upload_btn');
        let staffDetails = JSON.stringify(staffData);
        $.ajax({
            type: 'Post',
            url: "/Admin/UpdateStaff",
            dataType: 'json',
            data: { details: staffDetails },
            success: function (result) {
                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('staff_bulk_upload_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('staff_bulk_upload_btn')
                errorAlert("Network failure, please try again");
            }
        });
    }
}

function DeleteStaff() {
    var id = $('#delete_Id').val();
    if (id != "") {
        processingBtn('del_staff_btn');
        $.ajax({
            type: 'Post',
            url: '/Admin/DeleteStaff',
            dataType: 'json',
            data: { staffId: id },
            success: function (result) {
                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('del_staff_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('del_staff_btn');
                errorAlert("Network failure, please try again");
            }
        });
    } else {
        errorAlert("Network failure, please try again");
    }
}

function GetStaffDetailForEdit(id) {
    if (id != "") {
        $.ajax({
            type: 'Get',
            url: '/Admin/GetStaffById',
            dataType: 'json',
            data: { staffId: id, },
            success: function (result) {
                if (!result.isError) {
                    var data = result.data;
                    $('#edit_Id').val(data.id);
                    $('#fName_edit').val(data.firstName);
                    $('#lName_edit').val(data.lastName);
                    $('#email_edit').val(data.email);
                    $('#cpa_edit').val(data.cpa).trigger('change');
                    $('#editModal').modal('show');
                }
                else { errorAlert(result.message); }
            },
            error: function (ex) {
                return ex;
            }
        });
    }
}

function UploadBulkStaff(id) {
    var file_desc = $('#bulk_staff').val();
    if (file_desc != "") {
        processingBtn('upload_staff');
        var myFile = document.getElementById("bulk_staff").files[0];

        var formData = new FormData();
        formData.append("dulkDetails", myFile);
        $.ajax({
            type: "POST",
            dataType: "Json",
            url: "/Admin/StaffBulkUpload",
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {

                if (!result.isError) {
                    returnDefaultBtn('upload_staff');
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('upload_staff');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('upload_staff')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    } else {

        errorAlert("Upload document");
    }
}