var examData = {};
function examValidtion(type) {
    inputValidation = [];
    inputValidation.push(validateField('', "exam_title" + type));
    inputValidation.push(validateNumericSelect2Field('', "target_class" + type));
    inputValidation.push(validateStringSelect2Field('', "subject" + type));
    inputValidation.push(validateField('', "exam_date" + type));
    inputValidation.push(validateField('', "exam_stime" + type));
    inputValidation.push(validateNumericField('', "exam_duration" + type));
    inputValidation.push(validateNumericField('', "exam_count" + type));
    if (inputValidation.includes(false)) {
        return false;
    }

    examData.Title = $('#exam_title' + type).val();
    examData.ClassX = $('#target_class' + type).val();
    examData.SubjectId = $('#subject' + type).val();
    examData.Duration = $('#exam_duration' + type).val();
    examData.NumberOfQuestions = $('#exam_count' + type).val();
    examData.ShouldRandomize = $("#randomizeQuestions").prop("checked");
    examData.StartDate = new Date($('#exam_date' + type).val() + 'T' + $('#exam_stime' + type).val() + 'Z');
    if (examData.StartDate <= new Date()) {
        errorAlert('Start datetime must be in the future');
        return false;
    }
    if (type === '_edit') {
        examData.Id = $('#edit_Id').val();
    }
    return true;
}

function CreateExam() {
    if (examValidtion('')) {
        processingBtn('add_Exam_btn');
        let ExamDetails = JSON.stringify(examData);
        $.ajax({
            type: 'Post',
            url: "/Exam/CreateExamination",
            dataType: 'json',
            data: { details: ExamDetails },
            success: function (result) {

                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('add_Exam_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('add_Exam_btn')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    }
}

function UpdateExam() {
    if (examValidtion('_edit')) {
        processingBtn('Exam_bulk_upload_btn');
        let ExamDetails = JSON.stringify(examData);
        $.ajax({
            type: 'Post',
            url: "/Exam/UpdateExamination",
            dataType: 'json',
            data: { details: ExamDetails },
            success: function (result) {
                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('Exam_bulk_upload_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('Exam_bulk_upload_btn')
                errorAlert("Network failure, please try again");
            }
        });
    }
}

function DeleteExam() {
    var id = $('#delete_Id').val();
    if (id != "") {
        processingBtn('del_Exam_btn');
        $.ajax({
            type: 'Post',
            url: '/Exam/DeleteExamination',
            dataType: 'json',
            data: { examId: id },
            success: function (result) {
                if (!result.isError) {
                    var url = window.location.pathname;
                    successAlertWithRedirect(result.message, url);
                }
                else {
                    returnDefaultBtn('del_Exam_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('del_Exam_btn');
                errorAlert("Network failure, please try again");
            }
        });
    } else {
        errorAlert("Network failure, please try again");
    }
}

function GetExamDetailForEdit(id) {
    if (id != "") {
        $.ajax({
            type: 'Get',
            url: '/Exam/GetExaminationById',
            dataType: 'json',
            data: { examId: id },
            success: function (result) {
                if (!result.isError) {
                    var data = result.data;
                    $('#edit_Id').val(data.id);
                    $('#exam_title_edit').val(data.title);
                    $('#subject_edit').val(data.subjectId).trigger('change');
                    $('#target_class_edit').val(data.classX).trigger('change');
                    $('#exam_date_edit').val(data.startDateString);
                    $('#exam_stime_edit').val(data.startTimeStringJs);
                    $('#exam_duration_edit').val(data.duration);
                    $('#exam_count_edit').val(data.numberOfQuestions);
                    $('#randomizeQuestions_edit').prop('checked', data.shouldRandomize);
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


function getResultDetailsForPreview(id) {
    if (id != "") {
        $.ajax({
            type: 'GET',
            url: '/Admin/ResultPartialView',
            data: { resultId : id },
            success: function (result) {
                if (result != "") {
                    $('#profileTemplate').html(result);
                    $('#viewModal').modal('show');
                } else {
                    errorAlert("No Record Found")
                }
            },
        });
    } else {
        errorAlert("Network Failure")
    }
}

function studentResultFilter() {
    var f = {
        Id: $('#fstudentId').val(),
        Clas: $('#fclass').val(),
    };
    if (f.Id == "") {
        errorAlert("Select Student to proceed")
        return
    }
    $.ajax({
        type: 'GET',
        url: '/Admin/StudentResultPartialView',
        data: { studentId: f.Id, classX : f.Clas },
        success: function (result) {
            if (result != "") {
                $("#preview_content").html(result);
                //reportDataTableFilter();
            } else {
                errorAlert("No Record Found")
            }
        },
    });
}


function addQuestionToExam(questionId, examId) {
    if (questionId != "" && examId != "") {
        processingBtn(questionId);
        $.ajax({
            type: 'Post',
            url: '/Exam/AddQuestionToExamination',
            dataType: 'json',
            data: { questionId: questionId, examId: examId },
            success: function (result) {
                if (!result.isError) {
                    $("#preview_content").html(result.data);
                    dataTableTrigger();
                }
                else {
                    returnDefaultBtn(questionId);
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn(questionId);
                errorAlert("Network failure, please try again");
            }
        });
    } else {
        errorAlert("Network failure, please try again");
    }
}

function removeQuestionFromExam(questionId, examId) {
    if (questionId != "" && examId != "") {
        processingBtn(questionId);
        $.ajax({
            type: 'Post',
            url: '/Exam/RemoveQuestionFromExamination',
            dataType: 'json',
            data: { questionId: questionId, examId: examId },
            success: function (result) {
                if (!result.isError) {
                    $("#preview_content").html(result.data);
                    dataTableTrigger();
                }
                else {
                    returnDefaultBtn(questionId);
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn(questionId);
                errorAlert("Network failure, please try again");
            }
        });
    } else {
        errorAlert("Network failure, please try again");
    }
}

