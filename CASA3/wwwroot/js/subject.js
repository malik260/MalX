var subjectData = {};
function subjectValidtion(type) {
    inputValidation = [];
    inputValidation.push(validateField('', "subject_title" + type));
    if (inputValidation.includes(false)) {
        return false;
    }
    subjectData.Name = $('#subject_title' + type).val();
    if (type === '_edit') {
        subjectData.Id = $('#edit_Id').val();
    };
    return true;
}

function CreateSubject() {
    if (subjectValidtion('')) {
        processingBtn('add_subject_btn');
        $.ajax({
            type: 'Post',
            url: "/Subject/CreateSubject",
            dataType: 'json',
            data: { subjectName: subjectData.Name },
            success: function (result) {

                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('add_subject_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('add_subject_btn')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    }
}

function UpdateSubject() {
    if (subjectValidtion('_edit')) {
        processingBtn('subject_title_edit');
        let subjectDetails = JSON.stringify(subjectData);
        $.ajax({
            type: 'Post',
            url: "/Subject/UpdateSubject",
            dataType: 'json',
            data: { details: subjectDetails },
            success: function (result) {
                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('subject_title_edit');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('subject_title_edit')
                errorAlert("Network failure, please try again");
            }
        });
    }
}

function DeleteSubject() {
    var id = $('#delete_Id').val();
    if (id != "") {
        processingBtn('del_subject_btn');
        $.ajax({
            type: 'Post',
            url: '/Subject/DeleteSubject',
            dataType: 'json',
            data: { subjectId: id },
            success: function (result) {
                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('del_subject_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('del_subject_btn');
                errorAlert("Network failure, please try again");
            }
        });
    } else {
        errorAlert("Network failure, please try again");
    }
}

function GetSubjectDetailForEdit(id, title) {
    if (id != "" && title != "") {
        $('#edit_Id').val(id);
        $('#subject_title_edit').val(title);
        $('#editModal').modal('show');
    }
}
