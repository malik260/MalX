var questionData = {};
function questionValidtion(type) {
    inputValidation = [];
    inputValidation.push(validateField('', "question_title" + type));
    inputValidation.push(validateNumericSelect2Field('', "correct_opt" + type));
    inputValidation.push(validateNumericSelect2Field('', "target_class" + type));
    inputValidation.push(validateStringSelect2Field('', "subject" + type));
    inputValidation.push(validateField('', "question_optA" + type));
    inputValidation.push(validateField('', "question_optB" + type));
    inputValidation.push(validateField('', "question_optC" + type));
    inputValidation.push(validateField('', "question_optD" + type));
    if (inputValidation.includes(false)) {
        return false;
    }

    questionData = {
        Title: $('#question_title' + type).val(),
        CorrectOption: $('#correct_opt' + type).val(),
        SubjectId: $('#subject' + type).val(),
        ClassX: $('#target_class' + type).val(),
        Options: [
            { OptionTag: 1, OptionValue: $('#question_optA' + type).val(), },
            { OptionTag: 2, OptionValue: $('#question_optB' + type).val(), },
            { OptionTag: 3, OptionValue: $('#question_optC' + type).val(), },
            { OptionTag: 4, OptionValue: $('#question_optD' + type).val(), }
        ]
    };

    if (type === '_edit') {
        questionData.Id = $('#edit_Id').val();
    }
    return true;
}

function CreateQuestion() {
    if (questionValidtion('')) {
        processingBtn('add_Question_btn');
        let QuestionDetails = JSON.stringify(questionData);
        $.ajax({
            type: 'Post',
            url: "/Question/CreateQuestion",
            dataType: 'json',
            data: { details: QuestionDetails },
            success: function (result) {

                if (!result.isError) {
                    QuestionSearch();
                    returnDefaultBtn('add_Question_btn');
                    $('#addModal').modal('hide');
                    $('#question_title').val('');
                    $('#correct_opt').val("").trigger('change');
                    $('#target_class').val(0).trigger('change');
                    $('#subject').val("").trigger('change');
                    $('#question_optA').val(0);
                    $('#question_optB').val(0);
                    $('#question_optC').val(0);
                    $('#question_optD').val(0);
                    successAlert(result.message);
                }
                else {
                    returnDefaultBtn('add_Question_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('add_Question_btn')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    }
}

function UpdateQuestion() {
    if (questionValidtion('_edit')) {
        processingBtn('edit_Question_btn');
        let questionDetails = JSON.stringify(questionData);
        $.ajax({
            type: 'Post',
            url: "/Question/UpdateQuestion",
            dataType: 'json',
            data: { details: questionDetails },
            success: function (result) {
                if (!result.isError) {
                    QuestionSearch();
                    returnDefaultBtn('edit_Question_btn');
                    $('#editModal').modal('hide');
                    successAlert(result.message);
                }
                else {
                    returnDefaultBtn('edit_Question_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('edit_Question_btn')
                errorAlert("Network failure, please try again");
            }
        });
    }
}

function DeleteQuestion() {
    var id = $('#delete_Id').val();
    if (id != "") {
        processingBtn('del_Question_btn');
        $.ajax({
            type: 'Post',
            url: '/Question/DeleteQuestion',
            dataType: 'json',
            data: { questionId: id },
            success: function (result) {
                if (!result.isError) {
                    QuestionSearch();
                    returnDefaultBtn('del_Question_btn');
                    $('#deleteModal').modal('hide');
                    successAlert(result.message);
                }
                else {
                    returnDefaultBtn('del_Question_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('del_Question_btn');
                errorAlert("Network failure, please try again");
            }
        });
    }
    else {
        errorAlert("Network failure, please try again");
    }
}

function GetQuestionDetailForEdit(id) {
    if (id != "") {
        $.ajax({
            type: 'Get',
            url: '/Question/GetQuestionById',
            dataType: 'json',
            data: { questionId: id },
            success: function (result) {
                if (!result.isError) {
                    debugger
                    var res = result.data;
                    var opts = convertToOptionSingleDTO(res.optionsObj);
                    $('#edit_Id').val(res.id);
                    $('#question_title_edit').val(res.title);
                    $('#correct_opt_edit').val(res.correctOption).trigger('change');
                    $('#target_class_edit').val(res.classX).trigger('change');
                    $('#subject_edit').val(res.subjectId).trigger('change');
                    $('#question_optA_edit').val(opts.OptionA);
                    $('#question_optB_edit').val(opts.OptionB);
                    $('#question_optC_edit').val(opts.OptionC);
                    $('#question_optD_edit').val(opts.OptionD);
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

function convertToOptionSingleDTO(options) {
    const dto = {
        OptionA: null,
        OptionB: null,
        OptionC: null,
        OptionD: null
    };

    options.forEach(option => {
        switch (option.optionTag) {
            case 1:
                dto.OptionA = option.optionValue;
                break;
            case 2:
                dto.OptionB = option.optionValue;
                break;
            case 3:
                dto.OptionC = option.optionValue;
                break;
            case 4:
                dto.OptionD = option.optionValue;
                break;
        }
    });

    return dto;
}

function searchParam() {
    return {
        SubjectId: $('#fsubjectId').val(),
        ClassX: $('#fclass').val(),
    };
}

function QuestionSearch() {
    var f = searchParam();
    $.ajax({
        type: 'GET',
        url: '/Question/QuestionPV',
        data: f, 
        success: function (result) {
            if (result != "") {
                $("#preview_content").html(result);
            } else {
                errorAlert("No Record Found")
            }
        },
    });
}
function QuestionFilter() {
    validateStringSelect2Field('', "fsubjectId");
    validateNumericSelect2Field('', "fclass");
    if ($('#fsubjectId').val() == null || $('#fsubjectId').val() == "") {
        return
    }
    if ($('#fclass').val() == 0) {
        return
    }
    QuestionSearch();
}

