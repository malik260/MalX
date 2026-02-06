var studentData = {};
function StudentValidtion(type) {
    inputValidation = [];
    inputValidation.push(validateField('', "fName" + type));
    inputValidation.push(validateField('', "lName" + type));
    inputValidation.push(validateField('', "regNo" + type));
    inputValidation.push(validateStringSelect2Field('', "cos" + type));
    if (inputValidation.includes(false)) {
        return false;
    }
    studentData.FirstName = $('#fName' + type).val();
    studentData.LastName = $('#lName' + type).val();
    studentData.RegNumber = $('#regNo' + type).val();
    studentData.CurrentClass = $('#cos' + type).val();
    if (type === '_edit') {
        studentData.Id = $('#edit_Id').val();
    }
    return true;
}

function CreateStudent() {
    if (StudentValidtion('')) {
        processingBtn('add_Student_btn');
        let StudentDetails = JSON.stringify(studentData);
        $.ajax({
            type: 'Post',
            url: "/Admin/CreateStudent",
            dataType: 'json',
            data: { details: StudentDetails },
            success: function (result) {

                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('add_Student_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('add_Student_btn')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    }
}

function UpdateStudent() {
    if (StudentValidtion('_edit')) {
        processingBtn('Student_bulk_upload_btn');
        let StudentDetails = JSON.stringify(studentData);
        $.ajax({
            type: 'Post',
            url: "/Admin/UpdateStudent",
            dataType: 'json',
            data: { details: StudentDetails },
            success: function (result) {
                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('Student_bulk_upload_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('Student_bulk_upload_btn')
                errorAlert("Network failure, please try again");
            }
        });
    }
}

function DeleteStudent() {
    var id = $('#delete_Id').val();
    if (id != "") {
        processingBtn('del_Student_btn');
        $.ajax({
            type: 'Post',
            url: '/Admin/DeleteStudent',
            dataType: 'json',
            data: { StudentId: id },
            success: function (result) {
                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('del_Student_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('del_Student_btn');
                errorAlert("Network failure, please try again");
            }
        });
    } else {
        errorAlert("Network failure, please try again");
    }
}

function GetStudentDetailForEdit(id) {
    if (id != "") {
        $.ajax({
            type: 'Get',
            url: '/Admin/GetStudentById',
            dataType: 'json',
            data: { StudentId: id, },
            success: function (result) {
                if (!result.isError) {
                    data = result.data;
                    $('#edit_Id').val(data.id);
                    $('#fName_edit').val(data.firstName);
                    $('#lName_edit').val(data.lastName);
                    $('#regNo_edit').val(data.regNumber);
                    $('#cos_edit').val(data.currentClass).trigger('change');
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

function UploadBulkStudent(id) {
    var file_desc = $('#bulk_Student').val();
    if (file_desc != "") {
        processingBtn('upload_Student');
        var myFile = document.getElementById("bulk_Student").files[0];

        var formData = new FormData();
        formData.append("dulkDetails", myFile);
        $.ajax({
            type: "POST",
            dataType: "Json",
            url: "/Admin/StudentBulkUpload",
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {

                if (!result.isError) {
                    returnDefaultBtn('upload_Student');
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('upload_Student');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('upload_Student')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    } else {

        errorAlert("Upload document");
    }
}

function submitExamination(obj) {
    if (obj != null) {
        debugger
        processingBtn('submitButton');
        let StudentDetails = JSON.stringify(obj);
        $.ajax({
            type: 'Post',
            url: "/Student/SubmitExam",
            dataType: 'json',
            data: { details: StudentDetails },
            success: function (result) {

                if (!result.isError) {
                    var url = "/Student?access_code=" + encodeURIComponent(obj.AccessCode);
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('submitButton');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('submitButton')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    }
}
